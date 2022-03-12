using EcoSphere_Test.Models;
using System;
using System.Collections.Generic;

namespace EcoSphere_Test.Utils
{
	public class QuotesComparer : IComparer<Quote>
	{
		public int Compare(Quote x, Quote y)
		{
			int dateCompare = DateTime.Compare(x.Date, y.Date);
			int timeCompare = TimeSpan.Compare(x.Time, y.Time);

			if (dateCompare < 0)
				return -1;
			else if (dateCompare > 0)
				return 1;
			else
				return timeCompare;
		}
	}
}
