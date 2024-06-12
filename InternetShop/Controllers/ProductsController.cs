using Microsoft.AspNetCore.Mvc;
using InternetShop.Models;
using InternetShop.Services;
using X.PagedList;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Google.Api;

namespace InternetShop.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ProductService _productService;
        private readonly List<string> _categories = new List<string> { "Электроника", "Компьютеры", "Аксессуары", "Бытовая техника", "Кухонная техника", "Транспорт", "Спорт и отдых", "Игры и игрушки" };

        public ProductsController(ProductService productService)
        {
            _productService = productService;
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

            return View(pagedProducts);
        }

        // GET: Products/Create
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Categories = _categories;
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

            ViewBag.Categories = _categories;
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

            ViewBag.Categories = _categories;
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

            ViewBag.Categories = _categories;
            return View(product);
        }

        private async Task<bool> ProductExists(string id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            return product != null;
        }

        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            product.ViewCount += 1;

            await _productService.UpdateProductAsync(product);

            return View(product);
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
    }
}