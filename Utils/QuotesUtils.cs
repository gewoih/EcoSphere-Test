using EcoSphere_Test.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace EcoSphere_Test.Utils
{
	public static class QuotesUtils
	{
		public static IEnumerable<Quote> LoadQuotesFromFile()
		{
			//Коллекция котировок, полученных из текстового файла
			ICollection<Quote> parsedQuotes = new List<Quote>();

			//Создаем диалоговое окно для выбора текстового файла с котировками
			OpenFileDialog fileDialog = new OpenFileDialog();
			fileDialog.Title = "Обзор файла с котировками";
			fileDialog.Filter = "Текстовые файлы (.txt) | *.txt";

			//Если файл успешно выбран
			if ((bool)fileDialog.ShowDialog())
			{
				//Создаем поток для чтения текстового файла
				using (StreamReader reader = new(fileDialog.FileName))
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
							Debug.WriteLine($"Не удалось считать строку '{line}'");
							continue;
						}
					}
				}
			}
			return parsedQuotes;
		}

		public static bool SaveQuotesToFile(IEnumerable<Quote> quotes)
		{
			//Сохраняем получившийся список котировок
			SaveFileDialog fileDialog = new SaveFileDialog();
			fileDialog.Title = "Сохранение файла с котировками";
			fileDialog.Filter = "Текстовые файлы (.txt) | *.txt";

			try
			{
				if ((bool)fileDialog.ShowDialog())
				{
					using (StreamWriter writer = new StreamWriter(fileDialog.FileName))
					{
						foreach (Quote quote in quotes)
							writer.WriteLine(quote);
					}
				}
			}
			catch
			{
				Debug.WriteLine($"Произошла ошибка при записи данных в файл '{fileDialog.FileName}'");
				return false;
			}

			return true;
		}

		//Нахождение максимальной котировки
		public static Quote GetMaxQuote(IList<Quote> quotes)
		{
			if (quotes == null || quotes.Count == 0)
				return null;

			Quote maxQuote = quotes[0];
			foreach (Quote quote in quotes)
			{
				if (quote.High > maxQuote.High)
					maxQuote = quote;
			}
			return maxQuote;
		}

		//Нахождение минимальной котировки
		public static Quote GetMinQuote(IList<Quote> quotes)
		{
			if (quotes == null || quotes.Count == 0)
				return null;

			Quote minQuote = quotes[0];
			foreach (Quote quote in quotes)
			{
				if (quote.Low < minQuote.Low)
					minQuote = quote;
			}
			return minQuote;
		}

		//Общий объем всех котировок
		public static int GetTotalVolume(IEnumerable<Quote> quotes)
		{
			int totalVolume = 0;
			foreach(Quote quote in quotes)
			{
				totalVolume += quote.TotalVolume;
			}
			return totalVolume;
		}

		public static int FindQuote(List<Quote> quotes, Quote targetQuote)
		{
			//Так как все котировки в списке гарантированно отсортированы, можем использовать бинарный поиск
			return quotes.BinarySearch(targetQuote, new QuotesComparer());
		}
	}
}
