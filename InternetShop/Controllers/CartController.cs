using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using InternetShop.Services;

namespace InternetShop.Controllers
{
    public class CartController : Controller
    {
        private readonly CartService _cartService;

        public CartController(CartService cartService)
        {
            _cartService = cartService;
        }

        public IActionResult Index()
        {
            var cartItems = _cartService.GetCartItems();
            return View(cartItems);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(string productId)
        {
            await _cartService.AddToCart(productId);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult RemoveFromCart(string productId)
        {
            _cartService.RemoveFromCart(productId);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult UpdateCartQuantity(string productId, int quantity)
        {
            _cartService.UpdateCartQuantity(productId, quantity);
            return RedirectToAction("Index");
        }
    }
}
