using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectPlanner.API.Helpers
{
    public class PaginationHeader
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }

        public PaginationHeader(int pageIndex, int pageSize, int totalItems, int totalPages)
        {
            this.PageIndex = pageIndex;
            this.TotalItems = totalItems;
            this.PageSize = pageSize;
            this.TotalPages = totalPages;
        }
    }
}
