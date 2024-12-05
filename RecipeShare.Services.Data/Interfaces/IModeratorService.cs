using Azure;
using RecipeShare.Data.Models;
using RecipeShare.Web.ViewModels.PaginationViewModels;
using RecipeShare.Web.ViewModels.RecipeViewModels;
using System.Drawing.Printing;


namespace RecipeShare.Services.Data.Interfaces
{
    public interface IModeratorService
    {
        Task<PaginatedList<InfoRecipeViewModel>> ViewAllUnapprovedRecipesAsync(int page, int pageSize);
        Task<RecipeDetailsViewModel?> RecipeDetailsAsync(Guid recipeId, Guid currentUserId);
        Task UnapproveRecipeAsync(Guid recipeId, Guid currentUserId);
    }
}
