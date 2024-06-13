using InternetShop.Models;
using InternetShop.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[Authorize]
public class ReviewsController : Controller
{
    private readonly ReviewService _reviewService;
    private readonly UserManager<User> _userManager;

    public ReviewsController(ReviewService reviewService, UserManager<User> userManager)
    {
        _reviewService = reviewService;
        _userManager = userManager;
    }

    [HttpPost]
    public async Task<IActionResult> Create(int productId, int rating, string comment)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        var review = new Review
        {
            ProductId = productId.ToString(),
            UserEmail = user.Email,
            Rating = rating,
            Comment = comment
        };

        await _reviewService.AddReviewAsync(review);

        return RedirectToAction("Details", "Products", new { id = productId });
    }
}
