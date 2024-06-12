using FirebaseAdmin;
using Google.Api;
using Google.Apis.Auth.OAuth2;
using InternetShop.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.Http;

var builder = WebApplication.CreateBuilder(args);

// Настройка сервисов
builder.Services.AddControllersWithViews();

// Регистрация ProductService
builder.Services.AddSingleton<ProductService>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<CartService>();
builder.Services.AddHttpClient();

// Настройка Firebase Admin SDK
FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromFile("credentials.json")
});

// Register ProductService
builder.Services.AddSingleton(new ProductService());

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
    });

builder.Services.AddControllersWithViews();

builder.Services.AddSession();
builder.Services.AddDistributedMemoryCache();

var app = builder.Build();

// Конфигурация middleware
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();
app.UseAuthentication();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Products}/{action=Index}/{id?}");

app.Run();
