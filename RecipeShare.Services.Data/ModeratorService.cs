using Azure;
using Microsoft.EntityFrameworkCore;
using RecipeShare.Data;
using RecipeShare.Services.Data.Interfaces;
using RecipeShare.Web.ViewModels.PaginationViewModels;
using RecipeShare.Web.ViewModels.RecipeViewModels;
using System.Drawing.Printing;
using static RecipeShare.Common.ApplicationConstants;

namespace RecipeShare.Services.Data
{
    public class ModeratorService : IModeratorService
    {
        private readonly RecipeShareDbContext context;
        public ModeratorService(RecipeShareDbContext _context)
        {
            context = _context;
        }
        public async Task<PaginatedList<InfoRecipeViewModel>> ViewAllUnapprovedRecipes(int page, int pageSize)
        {
            List<InfoRecipeViewModel> model = await context.Recipes
                .Where(r => r.IsApproved == false && r.IsDeleted == false && r.IsArchived == false)
                .AsNoTracking()
                .Select(r => new InfoRecipeViewModel
                {
                    Id = r.Id,
                    RecipeTitle = r.RecipeTitle,
                    DateOfRelease = r.DateOfRelease.ToString(RecipeReleaseDatePattern),
                    ImageUrl = r.Img ?? "~/images/recipes/Recipe.png",
                })
            .ToListAsync();
            IEnumerable<InfoRecipeViewModel> paginatedRecipes = model
            .Skip((page - 1) * pageSize).Take(pageSize);

            PaginatedList<InfoRecipeViewModel> recipes = new PaginatedList<InfoRecipeViewModel>(
                paginatedRecipes,
                model.Count(),
                page,
                pageSize
            );
            return recipes;
        }
    }
}
