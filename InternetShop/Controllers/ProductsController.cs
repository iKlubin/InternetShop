using Microsoft.AspNetCore.Mvc;
using InternetShop.Models;
using InternetShop.Services;
using X.PagedList;
using Microsoft.EntityFrameworkCore;

namespace InternetShop.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ProductService _productService;
        private readonly ReviewService _reviewService;

        public ProductsController(ProductService productService, ReviewService reviewService)
        {
            _productService = productService;
            _reviewService = reviewService;
        }

        // GET: Products
        [HttpGet]
        public async Task<IActionResult> Index(string searchString, int pageNumber = 1, int pageSize = 15)
        {
            ViewData["SearchString"] = searchString;

            var products = await _productService.GetProductsAsync();

            if (!string.IsNullOrEmpty(searchString))
            {
                products = products.Where(p => p.Name != null && p.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                                               p.Description != null && p.Description.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                                               p.Category != null && p.Category.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                                               p.Tags != null && p.Tags.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            var pagedProducts = products.ToPagedList(pageNumber, pageSize);

            var mostViewedProducts = products.OrderByDescending(p => p.ViewCount).Take(10).ToList();

            var model = new ListProducts
            {
                PagedProducts = pagedProducts,
                MostViewedProducts = mostViewedProducts
            };

            return View(model);
        }

        // GET: Products/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _productService.GetCategoriesAsync();
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,ExtendedDescription,Price,Category,Tags")] Product product, IFormFile? imageFile)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                var imageName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                var imageUrl = await _productService.UploadImageAsync(imageFile.OpenReadStream(), imageName);
                product.ImageUrl = imageUrl;
            }

            ModelState.Remove("Id");
            ModelState.Remove("ImageUrl");

            if (ModelState.IsValid)
            {
                await _productService.AddProductAsync(product);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = await _productService.GetCategoriesAsync();
            return View(product);
        }


        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            Console.WriteLine(id);
            if (id == null)
            {
                return NotFound();
            }

            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            ViewBag.Categories = await _productService.GetCategoriesAsync();
            return View(product);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name,Description,ExtendedDescription,Price,Category,Tags")] Product product, IFormFile? imageFile)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            ModelState.Remove("ImageUrl");

            if (ModelState.IsValid)
            {
                try
                {
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        var imageName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                        var imageUrl = await _productService.UploadImageAsync(imageFile.OpenReadStream(), imageName);
                        product.ImageUrl = imageUrl;
                    }

                    ModelState.Remove("ImageUrl");

                    await _productService.UpdateProductAsync(product);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = await _productService.GetCategoriesAsync();
            return View(product);
        }

        private async Task<bool> ProductExists(string id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            return product != null;
        }

        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            // Обновление счетчика просмотров
            product.ViewCount += 1;
            await _productService.UpdateProductAsync(product);

            var reviews = await _reviewService.GetReviewsByProductIdAsync(id);
            if (reviews == null)
            {
                reviews = new List<Review>();
            }
            ViewBag.Reviews = reviews;

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReview(string productId, int rating, string comment)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            var review = new Review
            {
                ProductId = productId,
                UserEmail = User.Identity.Name,
                Rating = rating,
                Comment = comment,
                Date = DateTime.UtcNow
            };

            await _reviewService.AddReviewAsync(review);
            return RedirectToAction("Details", new { id = productId });
        }

        // POST: Products/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            await _productService.DeleteProductAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // GET: Products by Category
        [HttpGet]
        public async Task<IActionResult> ByCategory(string category, int pageNumber = 1, int pageSize = 15)
        {
            var products = await _productService.GetProductsAsync();
            var filteredProducts = products.Where(p => p.Category != null && p.Category.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList();

            var pagedProducts = filteredProducts.ToPagedList(pageNumber, pageSize);

            var mostViewedProducts = products.OrderByDescending(p => p.ViewCount).Take(10).ToList();

            var model = new ListProducts
            {
                PagedProducts = pagedProducts,
                MostViewedProducts = mostViewedProducts
            };

            ViewData["Category"] = category;

            return View("Index", model);
        }
    }
}