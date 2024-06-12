using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using InternetShop.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Threading.Tasks;
using System.Security.Claims;
using Newtonsoft.Json.Linq;


namespace InternetShop.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AccountController> _logger;

        public AccountController(HttpClient httpClient, ILogger<AccountController> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var response = await _httpClient.PostAsJsonAsync("https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=AIzaSyCpt_ja3lJik44JjRchNNGdlpN6EOAwPM8", new
                {
                    email = model.Email,
                    password = model.Password,
                    returnSecureToken = true
                });

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<FirebaseAuthResponse>(jsonResponse);

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, model.Email)
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
            }

            return View(model);
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(User model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var content = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("email", model.Email),
                        new KeyValuePair<string, string>("password", model.Password),
                        new KeyValuePair<string, string>("returnSecureToken", "true")
                    });

                    var response = await _httpClient.PostAsync("https://identitytoolkit.googleapis.com/v1/accounts:signUp?key=AIzaSyCpt_ja3lJik44JjRchNNGdlpN6EOAwPM8", content);
                    response.EnsureSuccessStatusCode();

                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var authResponse = JsonSerializer.Deserialize<FirebaseAuthResponse>(jsonResponse);

                    // Log the success to console
                    Console.WriteLine($"Registration successful for user: {model.Email}");
                    Console.WriteLine($"Token: {authResponse.IdToken}");

                    // Save token or perform other registration actions

                    return RedirectToAction("Index", "Products");
                }
                catch (HttpRequestException ex)
                {
                    // Log the error to console
                    Console.WriteLine($"Registration failed for user: {model.Email}. HTTP Error: {ex.Message}");
                    ModelState.AddModelError(string.Empty, "Registration attempt failed.");
                }
                catch (JsonException ex)
                {
                    // Log the error to console
                    Console.WriteLine($"Registration failed for user: {model.Email}. JSON Error: {ex.Message}");
                    ModelState.AddModelError(string.Empty, "Registration attempt failed.");
                }
                catch (Exception ex)
                {
                    // Log the error to console
                    Console.WriteLine($"Unexpected error: {ex.Message}");
                    ModelState.AddModelError(string.Empty, "An unexpected error occurred.");
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Profile()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Profile(User model)
        {
            if (ModelState.IsValid)
            {
                // Получите токен пользователя из аутентификационных куки
                var token = User.FindFirst("Token")?.Value;

                if (token != null)
                {
                    // Подготовьте данные для обновления профиля
                    var updateData = new
                    {
                        idToken = token,
                        returnSecureToken = true
                    };

                    // Отправьте запрос на обновление профиля в Firebase
                    var response = await _httpClient.PostAsJsonAsync("https://identitytoolkit.googleapis.com/v1/accounts:update?key=AIzaSyCpt_ja3lJik44JjRchNNGdlpN6EOAwPM8", updateData);

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonResponse = await response.Content.ReadAsStringAsync();
                        var result = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonResponse);

                        // Логика после успешного обновления профиля (например, вывод сообщения об успехе)
                        ViewBag.Message = "Profile updated successfully.";
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Failed to update profile.");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "User is not authenticated.");
                }
            }

            return View(model);
        }

    }
}
