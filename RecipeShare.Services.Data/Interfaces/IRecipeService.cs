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
        Task<AddAndEditViewModel> ModelForAddAsync();
        Task AddRecipeAsync(AddAndEditViewModel model, Guid currentUserId);
    }
}
