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
            if (context.Categories.Any())
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

        private static string GetJsonFilePath(string jsonName)
        {
            string solutionDir = Directory.GetCurrentDirectory();
            string wwwrootPath = Path.Combine(solutionDir, "..", "RecipeShare.Web", "wwwroot", "data", jsonName);
            string jsonFilePath = Path.GetFullPath(wwwrootPath);
            return jsonFilePath;
        }
    }
}
