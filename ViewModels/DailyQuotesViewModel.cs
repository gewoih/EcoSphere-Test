using EcoSphere_Test.Commands;
using EcoSphere_Test.Models;
using EcoSphere_Test.Utils;
using EcoSphere_Test.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace EcoSphere_Test.ViewModels
{
	public class DailyQuotesViewModel : BaseViewModel
	{
		#region Constructor
		public DailyQuotesViewModel()
		{
			this.LoadQuotesCommand = new RelayCommand(OnLoadQuotesCommandExecuted, CanLoadQuotesCommandExecute);
		}
		#endregion

		#region Commands
		public ICommand LoadQuotesCommand { get; }
		private bool CanLoadQuotesCommandExecute(object p) => true;
		private void OnLoadQuotesCommandExecuted(object p)
		{
			//Вызываем метод для парсинга котировок из файла
			List<Quote> loadedQuotes = new List<Quote>(QuotesUtils.LoadQuotesFromFile());

			//Формируем минимум и максимум по котировкам за каждый день
			List<Quote> dailyMinMaxQuotes = new List<Quote>(this.AggregateMinMaxDailyQuotes(loadedQuotes));

			if (QuotesUtils.SaveQuotesToFile(dailyMinMaxQuotes))
				MessageBox.Show("Файл успешно сохранен.");
			else
				MessageBox.Show("При сохранении файла произошла ошибка!");
		}
		#endregion

		#region Methods
		//Формирование списка максимальных-минимальных котировок по дням
		private IEnumerable<Quote> AggregateMinMaxDailyQuotes(ICollection<Quote> quotes)
		{
			Stopwatch sw = new Stopwatch();
			sw.Start();

			//Словарь для разбиения котировок по дням
			//Дата - ключ, List - список котировок за дату
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
				Tuple<Quote, Quote> maxMinQuotesPair = 
					new Tuple<Quote, Quote>(
						QuotesUtils.GetMaxQuote(dailyQuotes.Value),
						QuotesUtils.GetMinQuote(dailyQuotes.Value));

				maxMinQuotes.Add(maxMinQuotesPair.Item1);
				maxMinQuotes.Add(maxMinQuotesPair.Item2);
			}

			sw.Stop();
			Debug.WriteLine($"{MethodBase.GetCurrentMethod().Name} " +
				$"(Вход: {quotes.Count}; Выход: {aggregatedDailyQuotes.Count}): {sw.ElapsedMilliseconds}мс.");

			return maxMinQuotes;
		}
		#endregion
	}
}
