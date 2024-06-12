using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace InternetShop.Models
{
    public class CartItem
    {
        public string ProductId { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
    }
}
