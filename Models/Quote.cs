using System;

namespace EcoSphere_Test.Models
{
	public class Quote
	{
		public Quote(string symbol, string description, DateTime date, TimeSpan time,
			decimal open, decimal high, decimal low, decimal close, int totalVolume)
		{
			this.Symbol = symbol;
			this.Description = description;
			this.Date = date;
			this.Time = time;
			this.Open = open;
			this.High = high;
			this.Low = low;
			this.Close = close;
			this.TotalVolume = totalVolume;
		}

		public string Symbol { get; }
		public string Description { get; }
		public DateTime Date { get; }
		public TimeSpan Time { get; }
		public decimal Open { get; }
		public decimal High { get; }
		public decimal Low { get; }
		public decimal Close { get; }
		public int TotalVolume { get; }

		public override string ToString()
		{
			return 
				$"{this.Symbol}," +
				$"{this.Description}," +
				$"{this.Date.ToString("dd.MM.yyyy")}," +
				$"{this.Time}," +
				$"{this.Open}," +
				$"{this.High}," +
				$"{this.Low}," +
				$"{this.Close}," +
				$"{this.TotalVolume}";
		}
	}
}
