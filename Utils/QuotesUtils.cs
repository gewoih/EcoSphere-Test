using EcoSphere_Test.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace EcoSphere_Test.Utils
{
	public static class QuotesUtils
	{
		public static IEnumerable<Quote> LoadQuotesFromFile(string path)
		{
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
			return parsedQuotes;
		}

		public static bool SaveQuotesToFile(ICollection<Quote> quotes, string path)
		{
			if (String.IsNullOrEmpty(path))
				return false;

			using (StreamWriter writer = new StreamWriter(path))
			{
				foreach (Quote quote in quotes)
					writer.WriteLine(quote);
			}

			return true;
		}
	}
}
