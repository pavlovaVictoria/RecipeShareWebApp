using Microsoft.EntityFrameworkCore;
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

            // Seed default user
            //await SeedUsersAsync(scope.ServiceProvider);
            //
            //// Seed recipes
            //await SeedRecipesAsync(context, scope.ServiceProvider);
            await SeedAllergensAsync(context);
        }

        private static async Task SeedAllergensAsync(RecipeShareDbContext context)
        {
            if (context.Allergens.Any())
            {
                return;
            }
            string solutionDir = Directory.GetCurrentDirectory();
            string wwwrootPath = Path.Combine(solutionDir, "..", "RecipeShare.Web", "wwwroot", "data", "allergen.json");
            string jsonFilePath = Path.GetFullPath(wwwrootPath);
            List<Allergen>? allergensData = JsonConvert.DeserializeObject<List<Allergen>>(await File.ReadAllTextAsync(jsonFilePath));
            
            if (allergensData != null)
            {
                await context.Allergens.AddRangeAsync(allergensData);
                await context.SaveChangesAsync();
            }
        }
    }
}
