using Azure;
using RecipeShare.Web.ViewModels.PaginationViewModels;
using RecipeShare.Web.ViewModels.RecipeViewModels;
using System.Drawing.Printing;


namespace RecipeShare.Services.Data.Interfaces
{
    public interface IModeratorService
    {
        Task<PaginatedList<InfoRecipeViewModel>> ViewAllUnapprovedRecipes(int page, int pageSize);
    }
}
