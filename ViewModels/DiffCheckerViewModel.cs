using EcoSphere_Test.Commands;
using EcoSphere_Test.Models;
using EcoSphere_Test.Utils;
using EcoSphere_Test.ViewModels.Base;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace EcoSphere_Test.ViewModels
{
	public class DiffCheckerViewModel : BaseViewModel
	{
		#region Constructor
		public DiffCheckerViewModel()
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
			List<Quote> originalLoadedQuotes = new List<Quote>(QuotesUtils.LoadQuotesFromFile());
			List<Quote> comparedLoadedQuotes = new List<Quote>(QuotesUtils.LoadQuotesFromFile());

			//Новые строки во 2 файле
			IEnumerable<Quote> newQuotes = this.GetNewQuotes(originalLoadedQuotes, comparedLoadedQuotes);
			//Потерянные строки во 2 файле
			IEnumerable<Quote> lostQuotes = this.GetLostQuotes(originalLoadedQuotes, comparedLoadedQuotes);
			//Уникальные строки из 1 и 2 файлов
			IEnumerable<Quote> uniqueQuotes = this.GetUniqueQuotes(originalLoadedQuotes, comparedLoadedQuotes);

			if (QuotesUtils.SaveQuotesToFile(newQuotes) && QuotesUtils.SaveQuotesToFile(lostQuotes) && QuotesUtils.SaveQuotesToFile(uniqueQuotes))
				MessageBox.Show("Все файлы успешно сохранены.");
			else
				MessageBox.Show("При сохранении одного или нескольких файлов произошла ошибка!");
		}
		#endregion

		#region Methods
		//Поиск новых котировок из списка comparedQuotes, которых нет в originalQuotes
		private ICollection<Quote> GetNewQuotes(List<Quote> originalQuotes, ICollection<Quote> comparedQuotes)
		{
			Stopwatch sw = new Stopwatch();
			sw.Start();

			ICollection<Quote> newQuotes = new List<Quote>(comparedQuotes.Count);
			foreach(Quote quote in comparedQuotes)
			{
				//Если котировка из сравниваемого списка не найдена в оригинальном списке, то она считается новой
				if (QuotesUtils.FindQuote(originalQuotes, quote) < 0)
					newQuotes.Add(quote);
			}

			sw.Stop();
			Debug.WriteLine($"{MethodBase.GetCurrentMethod().Name} ({originalQuotes.Count}/{comparedQuotes.Count} элементов): {sw.ElapsedMilliseconds}мс.");

			return newQuotes;
		}

		//Поиск утерянных котировок из списка originalQuotes, которых нет в comparedQuotes
		private ICollection<Quote> GetLostQuotes(ICollection<Quote> originalQuotes, List<Quote> comparedQuotes)
		{
			Stopwatch sw = new Stopwatch();
			sw.Start();

			ICollection<Quote> lostQuotes = new List<Quote>(originalQuotes.Count);
			foreach(Quote quote in originalQuotes)
			{
				//Если котировка из оригинального списка не найдена в сравниваемом списке, то она считается утерянной
				if (QuotesUtils.FindQuote(comparedQuotes, quote) < 0)
					lostQuotes.Add(quote);
			}

			sw.Stop();
			Debug.WriteLine($"{MethodBase.GetCurrentMethod().Name} ({originalQuotes.Count}/{comparedQuotes.Count} элементов): {sw.ElapsedMilliseconds}мс.");

			return lostQuotes;
		}

		//Поиск уникальных котировок из 2х списков
		private ICollection<Quote> GetUniqueQuotes(List<Quote> quotes1, List<Quote> quotes2)
		{
			Stopwatch sw = new Stopwatch();
			sw.Start();

			IEnumerable<Quote> newQuotes = this.GetNewQuotes(quotes1, quotes2);
			List<Quote> uniqueQuotes = new List<Quote>(quotes1);
			uniqueQuotes.AddRange(newQuotes);
			uniqueQuotes.Sort(new QuotesComparer());

			sw.Stop();
			Debug.WriteLine($"{MethodBase.GetCurrentMethod().Name} ({quotes1.Count}/{quotes2.Count} элементов): {sw.ElapsedMilliseconds}мс.");

			return uniqueQuotes;
		}
		#endregion
	}
}
