using Microsoft.AspNetCore.Mvc;
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
        Task<List<InfoRecipeViewModel>> ViewCreatedRecipesAsync(Guid userId);
        Task<List<InfoRecipeViewModel>> ViewLikedRecipesAsync(Guid userId);
        Task<EditRecipeViewModel> ModelForEdidAsync(Guid recipeId, Guid currentUserId);
        Task EditRecipeAsync(EditRecipeViewModel model, Guid recipeId, Guid currentUserId);
        Task<DeleteRecipeViewModel> ModelForDeleteAsync(Guid recipeId, Guid currentUserId);
        Task DeleteRecipeAsync(Guid recipeId, Guid currentUserId);
        Task ArchiveRecipeAsync(Guid recipeId, Guid currentUserId);
    }
}
