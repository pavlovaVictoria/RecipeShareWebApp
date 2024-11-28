using Microsoft.AspNetCore.Mvc;
using RecipeShare.Web.ViewModels.PaginationViewModels;
using RecipeShare.Web.ViewModels.RecipeViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeShare.Services.Data.Interfaces
{
    public interface IRecipeService
    {
        Task<RecipesIndexViewModel> IndexPageOfRecipesAsync();
        Task<RecipeDetailsViewModel?> RecipeDetailsAsync(Guid recipeId, Guid userId);
        Task<(bool isLiked, int likes)> LikeRecipeAsync(Guid recipeId, Guid userId);
        Task<RecipeByCategoryViewModel> RcipesByCategoryAsync(Guid categoryId);
        Task<AddRecipeViewModel> ModelForAddAsync();
        Task AddRecipeAsync(AddRecipeViewModel model, Guid currentUserId);
        Task<PaginatedList<InfoRecipeViewModel>> ViewCreatedRecipesAsync(Guid userId, int page, int pageSize);
        Task<List<InfoRecipeViewModel>> ViewLikedRecipesAsync(Guid userId);
        Task<EditRecipeViewModel> ModelForEdidAsync(Guid recipeId, Guid currentUserId);
        Task EditRecipeAsync(EditRecipeViewModel model, Guid recipeId, Guid currentUserId);
        Task<DeleteRecipeViewModel> ModelForDeleteAsync(Guid recipeId, Guid currentUserId);
        Task UnarchiveRecipeAsync(Guid recipeId, Guid currentUserId);
        Task DeleteOrArchiveAsync(Guid recipeId, Guid currentUserId, string action);
        Task<List<InfoRecipeViewModel>> ViewArchivedRecipesAsync(Guid currentUserId);
    }
}
