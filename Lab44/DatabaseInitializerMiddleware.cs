using AdvertisementServiceMVC2.Models;
using Microsoft.EntityFrameworkCore;

public class DatabaseInitializerMiddleware
{
    private readonly RequestDelegate _next;
    public DatabaseInitializerMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context, AdvertisementServiceContext db)
    {
        db.Database.SetCommandTimeout(300);

        // Если категорий нет, начинаем заполнение
        if (!await db.Categories.AnyAsync())
        {
            // 1. Регионы (оставляем как было)
            var regions = new List<Region>
            {
                new Region { RegionName = "Москва", CityName = "Москва" },
                new Region { RegionName = "Санкт-Петербург", CityName = "Санкт-Петербург" },
                new Region { RegionName = "Екатеринбург", CityName = "Екатеринбург" }
            };
            db.Regions.AddRange(regions);
            await db.SaveChangesAsync();

            // 2. ГЛАВНЫЕ КАТЕГОРИИ
            var catTransport = new Category { CategoryName = "Транспорт" };
            var catRealEstate = new Category { CategoryName = "Недвижимость" };
            var catElectronics = new Category { CategoryName = "Электроника" };

            db.Categories.AddRange(catTransport, catRealEstate, catElectronics);
            await db.SaveChangesAsync(); // Сохраняем, чтобы получить их ID

            // 3. ПОДКАТЕГОРИИ (Привязываем к главным через ParentCategoryID)
            var subCats = new List<Category>
            {
                // Транспорт
                new Category { CategoryName = "Автомобили", ParentCategoryID = catTransport.CategoryID },
                new Category { CategoryName = "Мотоциклы", ParentCategoryID = catTransport.CategoryID },
                new Category { CategoryName = "Запчасти", ParentCategoryID = catTransport.CategoryID },

                // Недвижимость
                new Category { CategoryName = "Квартиры", ParentCategoryID = catRealEstate.CategoryID },
                new Category { CategoryName = "Дома", ParentCategoryID = catRealEstate.CategoryID },
                new Category { CategoryName = "Гаражи", ParentCategoryID = catRealEstate.CategoryID },

                // Электроника
                new Category { CategoryName = "Телефоны", ParentCategoryID = catElectronics.CategoryID },
                new Category { CategoryName = "Ноутбуки", ParentCategoryID = catElectronics.CategoryID },
                new Category { CategoryName = "Фототехника", ParentCategoryID = catElectronics.CategoryID }
            };
            db.Categories.AddRange(subCats);
            await db.SaveChangesAsync(); // Сохраняем подкатегории

            // 4. Юзеры
            var user1 = new AppUser { UserName = "ivan", Email = "i@test.ru", Name = "Иван", RegionId = regions[0].RegionID };
            var user2 = new AppUser { UserName = "petr", Email = "p@test.ru", Name = "Петр", RegionId = regions[1].RegionID };
            db.Users.AddRange(user1, user2);
            await db.SaveChangesAsync();

            // 5. ГЕНЕРАЦИЯ ОБЪЯВЛЕНИЙ
            // Важно: Объявления мы обычно привязываем к ПОДКАТЕГОРИЯМ (конечным узлам)
            // Берем ID только из subCats
            var subCatIds = subCats.Select(c => c.CategoryID).ToList();
            var regIds = regions.Select(r => r.RegionID).ToList();
            var userIds = new[] { user1.Id, user2.Id };
            var rnd = new Random();
            var adsList = new List<Advertisement>();

            int adsToGenerate = 1000; // Для теста

            for (int i = 1; i <= adsToGenerate; i++)
            {
                adsList.Add(new Advertisement
                {
                    Title = $"Лот №{i}",
                    Description = "Описание...",
                    Price = rnd.Next(10, 10000) * 100m,
                    CategoryId = subCatIds[rnd.Next(subCatIds.Count)], // Случайная подкатегория
                    RegionId = regIds[rnd.Next(regIds.Count)],
                    UserId = userIds[rnd.Next(userIds.Length)],
                    Status = "Active",
                    CreatedAt = DateTime.Now.AddDays(-rnd.Next(30))
                });
            }
            await db.Advertisements.AddRangeAsync(adsList);
            await db.SaveChangesAsync();
        }

        await _next(context);
    }
}