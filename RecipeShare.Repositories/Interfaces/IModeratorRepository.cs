using RecipeShare.Data.Models;
using RecipeShare.Web.ViewModels.PaginationViewModels;
using RecipeShare.Web.ViewModels.RecipeViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeShare.Repositories.Interfaces
{
    public interface IModeratorRepository
    {
        Task SaveChangesAsync();
        Task<List<InfoRecipeViewModel>> ViewAllUnapprovedRecipesAsync();
        Task<RecipeDetailsViewModel?> ModelForDetailsAsync(Guid recipeId, Guid userId);
        Task<Recipe?> FindRecipeAsync(Guid recipeId);
        Task<List<InfoRecipeViewModel>> ViewAllRecipesAsync();
        Task<Comment?> FindCommentAsync(Guid commentId);
    }
}
