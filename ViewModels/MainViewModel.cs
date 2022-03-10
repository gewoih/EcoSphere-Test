using EcoSphere_Test.ViewModels.Base;
using EcoSphere_Test.Views;

namespace EcoSphere_Test.ViewModels
{
	public class MainViewModel : BaseViewModel
	{
		#region Constructor
		public MainViewModel()
		{
			this.DailyQuotesView = new();
			this.HourlyQuotesView = new();
			this.DiffCheckerView = new();
		}
		#endregion

		#region Properties
		private DailyQuotesView _DailyQuotesView;
		public DailyQuotesView DailyQuotesView
		{
			get => _DailyQuotesView;
			set => Set(ref _DailyQuotesView, value);
		}

		private HourlyQuotesView _HourlyQuotesView;
		public HourlyQuotesView HourlyQuotesView
		{
			get => _HourlyQuotesView;
			set => Set(ref _HourlyQuotesView, value);
		}

		private DiffCheckerView _DiffCheckerView;
		public DiffCheckerView DiffCheckerView
		{
			get => _DiffCheckerView;
			set => Set(ref _DiffCheckerView, value);
		}
		#endregion
	}
}
