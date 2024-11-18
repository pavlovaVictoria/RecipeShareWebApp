using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RecipeShare.Data;
using RecipeShare.Data.Models;

namespace RecipeShare.Services.Data
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using IServiceScope scope = serviceProvider.CreateScope();
            RecipeShareDbContext context = scope.ServiceProvider.GetRequiredService<RecipeShareDbContext>();

            await context.Database.MigrateAsync();

            //Seed all default values
            await SeedAllergensAsync(context);
            await SeedAllCategoriesAsync(context);
            await SeedAllProductsAsync(context);
            await SeedDefaultUserAsync(scope.ServiceProvider);
        }

        private static async Task SeedAllergensAsync(RecipeShareDbContext context)
        {
            if (context.Allergens.Any())
            {
                return;
            }
            string jsonFilePath = GetJsonFilePath("allergen.json");
            List<Allergen>? allergensData = JsonConvert.DeserializeObject<List<Allergen>>(await File.ReadAllTextAsync(jsonFilePath));
            
            if (allergensData != null)
            {
                await context.Allergens.AddRangeAsync(allergensData);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedAllCategoriesAsync(RecipeShareDbContext context)
        {
            if (context.Categories.Any())
            {
                return;
            }
            string jsonFilePath = GetJsonFilePath("category.json");
            List<Category>? categoriesData = JsonConvert.DeserializeObject<List<Category>>(await File.ReadAllTextAsync(jsonFilePath));

            if (categoriesData != null)
            {
                await context.Categories.AddRangeAsync(categoriesData);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedAllProductsAsync(RecipeShareDbContext context)
        {
            if (context.Products.Any())
            {
                return;
            }
            string jsonFilePath = GetJsonFilePath("product.json");
            List<Product>? productsData = JsonConvert.DeserializeObject<List<Product>>(await File.ReadAllTextAsync(jsonFilePath));

            if (productsData != null)
            {
                await context.Products.AddRangeAsync(productsData);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedDefaultUserAsync(IServiceProvider serviceProvider)
        {
            UserManager<ApplicationUser> userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            ApplicationUser? defaultUser = await userManager.FindByEmailAsync("defaultuser@example.com");
            if (defaultUser == null)
            {
                ApplicationUser user = new ApplicationUser
                {
                    UserName = "Default User",
                    Email = "defaultuser@example.com",
                    IsMale = true,
                    AccountBio = "This is a default user made for practice!"
                };

                IdentityResult result = await userManager.CreateAsync(user, "DefaultPassword123");
                if (!result.Succeeded)
                {
                    return;
                }
            }
        }



        private static string GetJsonFilePath(string jsonName)
        {
            string solutionDir = Directory.GetCurrentDirectory();
            string wwwrootPath = Path.Combine(solutionDir, "..", "RecipeShare.Web", "wwwroot", "data", jsonName);
            string jsonFilePath = Path.GetFullPath(wwwrootPath);
            return jsonFilePath;
        }
    }
}
