using Microsoft.AspNetCore.Mvc;

namespace InternetShop.Controllers
{
    public class ApplicationDbContext : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
