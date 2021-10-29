using System.Collections.Generic;

namespace Bgt.Ocean.Models.BaseModel
{
    public class PagingBase
    {
        public PagingBase()
        {
            NumberPerPage = NumberPerPage == 0 ? int.MaxValue : NumberPerPage;
            PageNumber = PageNumber == 0 ? 1 : PageNumber;
        }

        public int NumberPerPage { get; set; }
        public int PageNumber { get; set; }
        public int MaxRow { get; set; } = 2000;

        public int Skip { get; set; }

        /// <summary>
        /// nameof field of column
        /// </summary>
        public string SortBy { get; set; }

        /// <summary>
        /// "asc" or "desc" order
        /// </summary>
        public string SortWith { get; set; }

        public Filter Filter { get; set; }
    }
    public class Filter
    {
        public string Logic { get; set; }
        public List<Filters> Filters { get; set; }
    }
    public class Filters
    {
        public string Field { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
    }

    public class PagingResponseBase<T>
    {
        public List<T> Data { get; set; }
        public int? Total { get; set; }

    }
}
