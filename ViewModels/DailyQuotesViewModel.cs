using EcoSphere_Test.Commands;
using EcoSphere_Test.Models;
using EcoSphere_Test.ViewModels.Base;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows.Input;

namespace EcoSphere_Test.ViewModels
{
	public class DailyQuotesViewModel : BaseViewModel
	{
		#region Constructor
		public DailyQuotesViewModel()
		{
			this.LoadedQuotes = new();
			this.DailyMinMaxQuotes = new();

			this.LoadQuotesCommand = new RelayCommand(OnLoadQuotesCommandExecuted, CanLoadQuotesCommandExecute);
		}
		#endregion

		#region Properties
		private ObservableCollection<Quote> _LoadedQuotes;
		public ObservableCollection<Quote> LoadedQuotes
		{
			get => _LoadedQuotes;
			set => Set(ref _LoadedQuotes, value);
		}

		private ObservableCollection<Quote> _DailyMinMaxQuotes;
		public ObservableCollection<Quote> DailyMinMaxQuotes
		{
			get => _DailyMinMaxQuotes;
			set => Set(ref _DailyMinMaxQuotes, value);
		}
		#endregion

		#region Commands
		public ICommand LoadQuotesCommand { get; }
		private bool CanLoadQuotesCommandExecute(object p) => true;
		private void OnLoadQuotesCommandExecuted(object p)
		{
			//Создаем диалоговое окно для выбора текстового файла с котировками
			OpenFileDialog fileDialog = new OpenFileDialog();
			fileDialog.Title = "Обзор файла с котировками";
			fileDialog.Filter = "Текстовые файлы (.txt) | *.txt";

			//Если файл успешно выбран
			if ((bool)fileDialog.ShowDialog())
			{
				//Вызываем метод для парсинга котировок в оригинальном виде
				this.LoadedQuotes = new ObservableCollection<Quote>(this.ParseQuotes(fileDialog.FileName));

				//Формируем минимум и максимум по котировкам за каждый день
				this.DailyMinMaxQuotes = new ObservableCollection<Quote>(this.AggregateMinMaxDailyQuotes(this.LoadedQuotes));

				//Сохраняем получившийся список котировок
				this.SaveQuotesToFile(this.DailyMinMaxQuotes);
			}
		}
		#endregion

		#region Methods
		private IEnumerable<Quote> AggregateMinMaxDailyQuotes(ICollection<Quote> quotes)
		{
			Stopwatch sw = new Stopwatch();
			sw.Start();

			//Словарь для разбиения котировок по дням.
			//Дата - ключ, List - список котировок за дату. Т.е. на одну дату может приходиться несколько котировок
			Dictionary<DateTime, List<Quote>> aggregatedDailyQuotes = new();
			foreach(Quote quote in quotes)
			{
				//Если в словаре уже есть такая дата (т.е. определенный день), то добавляем котировку к этой дате. Иначе - создаем такой ключ (дату)
				if (aggregatedDailyQuotes.ContainsKey(quote.Date))
					aggregatedDailyQuotes[quote.Date].Add(quote);
				else
					aggregatedDailyQuotes.Add(quote.Date, new List<Quote> { quote });
			}

			//Создаем коллекцию максимальных и минимальных котировок размером в 2 раза больше коллекции, аггрегированной по дням (1 день - 2 котировки)
			ICollection<Quote> maxMinQuotes = new List<Quote>(aggregatedDailyQuotes.Count * 2);
			foreach (var dailyQuotes in aggregatedDailyQuotes)
			{
				//Находим пару максимальная-минимальная котировка и добавляем их в итоговый список
				Tuple<Quote, Quote> maxMinQuotesPair = this.FindMaxMinQuote(dailyQuotes.Value);
				maxMinQuotes.Add(maxMinQuotesPair.Item1);
				maxMinQuotes.Add(maxMinQuotesPair.Item2);
			}

			sw.Stop();
			Debug.WriteLine($"{MethodBase.GetCurrentMethod().Name} " +
				$"(Вход: {quotes.Count}; Выход: {aggregatedDailyQuotes.Count}): {sw.ElapsedMilliseconds}мс.");

			return maxMinQuotes;
		}

		private IEnumerable<Quote> ParseQuotes(string path)
		{
			Stopwatch sw = new Stopwatch();
			sw.Start();

			//Коллекция котировок, полученных из текстового файла
			ICollection<Quote> parsedQuotes = new List<Quote>();

			//Создаем поток для чтения текстового файла
			using (StreamReader reader = new(path))
			{
				for (string? line = reader.ReadLine(); line != null; line = reader.ReadLine())
				{
					//Разделяем текущую строку на массив строк (параметров)
					string[] quoteParameters = line.Split(',');

					try
					{
						//Заполняем все необходимые переменные
						string symbol = Convert.ToString(quoteParameters[0], CultureInfo.InvariantCulture);
						string description = Convert.ToString(quoteParameters[1], CultureInfo.InvariantCulture);
						DateTime date = DateTime.ParseExact(quoteParameters[2], "dd.MM.yyyy", CultureInfo.InvariantCulture);
						TimeSpan time = TimeSpan.Parse(quoteParameters[3], CultureInfo.InvariantCulture);
						decimal open = Convert.ToDecimal(quoteParameters[4], CultureInfo.InvariantCulture);
						decimal high = Convert.ToDecimal(quoteParameters[5], CultureInfo.InvariantCulture);
						decimal low = Convert.ToDecimal(quoteParameters[6], CultureInfo.InvariantCulture);
						decimal close = Convert.ToDecimal(quoteParameters[7], CultureInfo.InvariantCulture);
						int totalVolume = Convert.ToInt32(quoteParameters[8], CultureInfo.InvariantCulture);

						//Добавляем новую котировку в список
						parsedQuotes.Add(new Quote(symbol, description, date, time, open, high, low, close, totalVolume));
					}
					catch
					{
						continue;
					}
				}
			}

			sw.Stop();
			Debug.WriteLine($"{MethodBase.GetCurrentMethod().Name} ({parsedQuotes.Count} элементов): {sw.ElapsedMilliseconds}мс.");

			return parsedQuotes;
		}

		private Tuple<Quote, Quote> FindMaxMinQuote(IList<Quote> quotes)
		{
			Stopwatch sw = new Stopwatch();
			sw.Start();

			if (quotes == null || quotes.Count == 0)
				return null;

			Quote maxQuote = quotes[0];
			Quote minQuote = quotes[0];
			foreach (Quote quote in quotes)
			{
				if (quote.High > maxQuote.High)
					maxQuote = quote;

				if (quote.Low < minQuote.Low)
					minQuote = quote;
			}

			sw.Stop();
			Debug.WriteLine($"{MethodBase.GetCurrentMethod().Name} ({quotes.Count} элементов): {sw.ElapsedMilliseconds}мс.");

			return new Tuple<Quote, Quote>(maxQuote, minQuote);
		}

		private void SaveQuotesToFile(ICollection<Quote> quotes)
		{
			Stopwatch sw = new Stopwatch();
			sw.Start();

			//Создаем диалоговое окно для сохранения файла
			SaveFileDialog fileDialog = new SaveFileDialog();
			fileDialog.Title = "Сохранение файла с котировками";
			fileDialog.Filter = "Текстовые файлы (.txt) | *.txt";

			if ((bool)fileDialog.ShowDialog())
			{
				using (StreamWriter writer = new StreamWriter(fileDialog.FileName))
				{
					//Спокойно записываем котировку в виде строки, т.к. у нее перегружен метод ToString()
					foreach (Quote quote in quotes)
						writer.WriteLine(quote);
				}
			}

			sw.Stop();
			Debug.WriteLine($"{MethodBase.GetCurrentMethod().Name} ({quotes.Count} элементов): {sw.ElapsedMilliseconds}мс.");
		}
		#endregion
	}
}
