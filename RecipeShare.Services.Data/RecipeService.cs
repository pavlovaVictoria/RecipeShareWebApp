using Microsoft.EntityFrameworkCore;
using RecipeShare.Data;
using RecipeShare.Services.Data.Interfaces;
using RecipeShare.Web.ViewModels.RecipeViewModels;
using static RecipeShare.Common.ApplicationConstants;

namespace RecipeShare.Services.Data
{
    public class RecipeService : IRecipeService
    {
        private readonly RecipeShareDbContext context;

        public RecipeService(RecipeShareDbContext _context)
        {
            context = _context;
        }

        public async Task<RecipesIndexViewModel> IndexPageOfRecipes()
        {
            List<InfoRecipeViewModel> recipes = await context.Recipes
                .Where(r => r.IsApproved)
                .OrderByDescending(r => r.LikedRecipes.Count)
                .OrderBy(r => r.RecipeTitle)
                .AsNoTracking()
                .Select(r => new InfoRecipeViewModel
                { 
                    Id = r.Id,
                    RecipeTitle = r.RecipeTitle,
                    Description = r.Description,
                    DateOfRelease = r.DateOfRelease.ToString(RecipeReleaseDatePattern),
                    ImageUrl = r.Img ?? "~/images/recipes/Recipe.png"
                })
                .Take(3)
                .ToListAsync();

            List<CategoryViewModel> categories = await context.Categories
                .AsNoTracking()
                .Select(c => new CategoryViewModel
                {
                    Id = c.Id,
                    CategoryName = c.CategoryName
                })
                .ToListAsync();
            RecipesIndexViewModel model = new RecipesIndexViewModel()
            {
                Recipes = recipes,
                Categories = categories
            };
            return model;
        }
    }
}
