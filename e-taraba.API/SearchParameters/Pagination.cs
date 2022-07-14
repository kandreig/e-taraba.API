using Microsoft.AspNetCore.Mvc;

namespace e_taraba.API.SearchParameters
{
    public class Pagination
    {
        [FromQuery(Name = "pageNumber")]
        public int CurrentPageNumber { get; set; }
        [FromQuery(Name = "itemsOnPage")]
        public int ItemsOnPage { get; set; }
        public int TotalItemsNumber { get; set; }
        public int TotalPagesNumber { get
            {
                return (int)Math.Ceiling(TotalItemsNumber / (double)ItemsOnPage);
            }
        }

    }
}
