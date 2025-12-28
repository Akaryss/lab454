using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AdvertisementServiceMVC2.Models;
using AdvertisementServiceMVC2.Data;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// 1. Настройка MVC + Именованный профиль кэширования (Пункт 1.7)
builder.Services.AddControllersWithViews(options =>
{
    options.CacheProfiles.Add("LabCacheProfile", new CacheProfile
    {
        Duration = 290, // Расчет для N=25
        Location = ResponseCacheLocation.Any
    });
});

builder.Services.AddDbContext<AdvertisementServiceContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequiredLength = 4;
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
.AddEntityFrameworkStores<AdvertisementServiceContext>()
.AddDefaultTokenProviders()
.AddDefaultUI();

var app = builder.Build();

// 2. Инициализация БД через Middleware (Пункт 1.4)
app.UseMiddleware<DatabaseInitializerMiddleware>();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Advertisements}/{action=Index}/{id?}");

app.MapRazorPages();
app.Run();