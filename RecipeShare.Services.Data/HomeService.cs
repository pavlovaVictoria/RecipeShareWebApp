using Microsoft.EntityFrameworkCore;
using RecipeShare.Data;
using RecipeShare.Services.Data.Interfaces;
using RecipeShare.Web.ViewModels.RecipeViewModels;
using static RecipeShare.Common.ApplicationConstants;

namespace RecipeShare.Services.Data
{
    public class HomeService : IHomeService
    {
        private readonly RecipeShareDbContext context;

        public HomeService(RecipeShareDbContext _context)
        {
            context = _context;
        }

        public async Task<List<InfoRecipeViewModel>> Top3Recipes()
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
            return recipes;
        }
    }
}
