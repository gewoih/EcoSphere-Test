using EcoSphere_Test.Commands;
using EcoSphere_Test.Models;
using EcoSphere_Test.ViewModels.Base;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
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
			}
		}
		#endregion

		#region Methods
		private IEnumerable<Quote> AggregateQuotes(string[] quotes)
		{
			return null;
		}

		private IEnumerable<Quote> ParseQuotes(string path)
		{
			ICollection<Quote> parsedQuotes = new List<Quote>();
			StreamReader reader = new StreamReader(path);

			for (string? line = reader.ReadLine(); line != null; line = reader.ReadLine())
			{
				string[] quoteParameters = line.Split(',');

				string symbol = Convert.ToString(quoteParameters[0], CultureInfo.InvariantCulture);
				string description = Convert.ToString(quoteParameters[1], CultureInfo.InvariantCulture);
				DateTime date = DateTime.ParseExact(quoteParameters[2], "dd.MM.yyyy", CultureInfo.InvariantCulture);
				TimeSpan time = TimeSpan.Parse(quoteParameters[3], CultureInfo.InvariantCulture);
				decimal open = Convert.ToDecimal(quoteParameters[4], CultureInfo.InvariantCulture);
				decimal high = Convert.ToDecimal(quoteParameters[5], CultureInfo.InvariantCulture);
				decimal low = Convert.ToDecimal(quoteParameters[6], CultureInfo.InvariantCulture);
				decimal close = Convert.ToDecimal(quoteParameters[7], CultureInfo.InvariantCulture);
				int totalVolume = Convert.ToInt32(quoteParameters[8], CultureInfo.InvariantCulture);

				parsedQuotes.Add(new Quote(symbol, description, date, time, open, high, low, close, totalVolume));
			}

			reader.Close();
			return parsedQuotes;
		}
		#endregion
	}
}
