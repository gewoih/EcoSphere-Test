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
	public class HourlyQuotesViewModel : BaseViewModel
	{
		#region Constructor
		public HourlyQuotesViewModel()
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

			//Формируем часовые котировки из исходных
			List<Quote> hourlyAggregatedQuotes = new List<Quote>(this.AggregateHourlyQuotes(loadedQuotes));

			if (QuotesUtils.SaveQuotesToFile(hourlyAggregatedQuotes))
				MessageBox.Show("Файл успешно сохранен.");
			else
				MessageBox.Show("При сохранении файла произошла ошибка!");
		}
		#endregion

		#region Methods
		private IEnumerable<Quote> AggregateHourlyQuotes(ICollection<Quote> quotes)
		{
			Stopwatch sw = new Stopwatch();
			sw.Start();

			//Словарь для разбиения котировок по часам
			//Ключ - дата с часом, List - список котировок за этот час
			Dictionary<DateTime, List<Quote>> aggregatedHourlyQuotes = new();
			foreach (Quote quote in quotes)
			{
				//Если в словаре уже есть такая дата, то добавляем котировку к этой дате. Иначе - создаем такой ключ (дату с часом)
				DateTime dateKey = new DateTime(quote.Date.Year, quote.Date.Month, quote.Date.Day, quote.Time.Hours, 0, 0);
				if (aggregatedHourlyQuotes.ContainsKey(dateKey))
					aggregatedHourlyQuotes[dateKey].Add(quote);
				else
					aggregatedHourlyQuotes.Add(dateKey, new List<Quote> { quote });
			}

			//Создаем и заполняем коллекцию часовых котировок
			ICollection<Quote> resultHourlyQuotes = new List<Quote>(aggregatedHourlyQuotes.Count);
			foreach (var hourlyQuotes in aggregatedHourlyQuotes)
			{
				//Символ первой свечи
				string symbol = hourlyQuotes.Value[0].Symbol;
				//Описание первой свечи
				string description = hourlyQuotes.Value[0].Description;
				//Дата
				DateTime date = hourlyQuotes.Key.Date;
				//Время 
				TimeSpan time = hourlyQuotes.Key.TimeOfDay;
				//Открытие первой свечи
				decimal open = hourlyQuotes.Value[0].Open;
				//Максимум - максимум свечей за этот час
				decimal high = QuotesUtils.GetMaxQuote(hourlyQuotes.Value).High;
				//Минимум - минимум свечей за этот час
				decimal low = QuotesUtils.GetMinQuote(hourlyQuotes.Value).Low;
				//Закрытие последней свечи
				decimal close = hourlyQuotes.Value[hourlyQuotes.Value.Count - 1].Close;
				//Объем - сумма объемов всех свечей за этот час
				int totalVolume = QuotesUtils.GetTotalVolume(hourlyQuotes.Value);

				resultHourlyQuotes.Add(new Quote(symbol, description, date, time, open, high, low, close, totalVolume));
			}

			sw.Stop();
			Debug.WriteLine($"{MethodBase.GetCurrentMethod().Name} " +
				$"(Вход: {quotes.Count}; Выход: {aggregatedHourlyQuotes.Count}): {sw.ElapsedMilliseconds}мс.");

			return resultHourlyQuotes;
		}
		#endregion
	}
}
