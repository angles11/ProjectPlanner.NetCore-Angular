using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectPlanner.API.Helpers
{
    public class UserParams
    {
		private const int MaxPageSize = 100;
		public int PageIndex { get; set; } = 0;

		private int pageSize = 10;

		public int PageSize
		{
			get { return pageSize; }
			set { pageSize = (value > MaxPageSize) ? MaxPageSize : value; }
		}

		public string SearchTerm { get; set; }
		public string OrderBy { get; set; }
		public string Gender { get; set; }
	}
}
