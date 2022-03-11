using EcoSphere_Test.Models;
using EcoSphere_Test.ViewModels.Base;
using System.Collections.ObjectModel;

namespace EcoSphere_Test.ViewModels
{
	public class HourlyQuotesViewModel : BaseViewModel
	{
		#region Constructor
		public HourlyQuotesViewModel()
		{
			this.LoadedQuotes = new();
			this.HourlyAggregatedQuotes = new();
		}
		#endregion

		#region Properties
		private ObservableCollection<Quote> _LoadedQuotes;
		public ObservableCollection<Quote> LoadedQuotes
		{
			get => _LoadedQuotes;
			set => Set(ref _LoadedQuotes, value);
		}

		private ObservableCollection<Quote> _HourlyAggregatedQuotes;
		public ObservableCollection<Quote> HourlyAggregatedQuotes
		{
			get => _HourlyAggregatedQuotes;
			set => Set(ref _HourlyAggregatedQuotes, value);
		}
		#endregion
	}
}
