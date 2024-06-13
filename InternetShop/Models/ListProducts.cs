using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using X.PagedList;

namespace InternetShop.Models
{
    public class ListProducts
    {
        public IPagedList<Product> PagedProducts { get; set; }
        public List<Product> MostViewedProducts { get; set; }
    }
}
