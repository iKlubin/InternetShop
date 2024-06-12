using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using InternetShop.Models;

namespace InternetShop.Services
{
    public class CartService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ProductService _productService;

        public CartService(IHttpContextAccessor httpContextAccessor, ProductService productService)
        {
            _httpContextAccessor = httpContextAccessor;
            _productService = productService;
        }

        private ISession Session => _httpContextAccessor.HttpContext.Session;

        public List<CartItem> GetCartItems()
        {
            var cart = Session.GetString("Cart");
            return string.IsNullOrEmpty(cart) ? new List<CartItem>() : JsonConvert.DeserializeObject<List<CartItem>>(cart);
        }

        public async Task AddToCart(string productId)
        {
            var cart = GetCartItems();
            var cartItem = cart.SingleOrDefault(c => c.ProductId == productId);

            if (cartItem == null)
            {
                var product = await _productService.GetProductByIdAsync(productId);
                if (product != null)
                {
                    cartItem = new CartItem
                    {
                        ProductId = productId,
                        Product = product,
                        Quantity = 1
                    };
                    cart.Add(cartItem);
                }
            }
            else
            {
                cartItem.Quantity++;
            }

            SaveCart(cart);
        }

        public void RemoveFromCart(string productId)
        {
            var cart = GetCartItems();
            var cartItem = cart.SingleOrDefault(c => c.ProductId == productId);

            if (cartItem != null)
            {
                cart.Remove(cartItem);
                SaveCart(cart);
            }
        }

        public void UpdateCartQuantity(string productId, int quantity)
        {
            var cart = GetCartItems();
            var cartItem = cart.SingleOrDefault(c => c.ProductId == productId);

            if (cartItem != null)
            {
                cartItem.Quantity = quantity;
                SaveCart(cart);
            }
        }

        private void SaveCart(List<CartItem> cart)
        {
            Session.SetString("Cart", JsonConvert.SerializeObject(cart));
        }
    }
}
