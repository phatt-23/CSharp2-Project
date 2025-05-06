using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoworkingDesktop.Models
{
    public class PaginationRequestDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class PaginationResponseDto
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }
        public int TotalCount { get; set; }
    }

    public class PagedResult<T>
    {
        public required List<T> Items { get; set; }
        public required int TotalCount { get; set; }

        public static PagedResult<T> EmptyResult => new()
        {
            Items = [],
            TotalCount = 0
        };
    }
}
