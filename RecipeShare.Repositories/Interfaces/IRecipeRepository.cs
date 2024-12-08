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
    public interface IRecipeRepository
    {
        Task<List<InfoRecipeViewModel>> GetTop3RecipesAsync();
        Task<List<InfoRecipeViewModel>> SearchRecipesAsync(string inputText);
        Task<List<CategoryViewModel>> GetAllCategoriesAsync();
        Task<RecipeDetailsViewModel?> RecipeDetailsAsync(Guid recipeId, Guid userId);
        Task<(bool isLiked, int likes)> LikeRecipeAsync(Guid recipeId, Guid userId);
        Task<bool> IsCategoryValidAsync(Guid categoryId);
        Task<RecipeByCategoryViewModel> RcipesByCategoryAsync(Guid categoryId);
        Task<AddRecipeViewModel> ModelForAddAsync();
        Task<Allergen?> FindAllergenAsync(Guid allergenId);
        Task<bool> RecipeAllergensAnyAsync(Guid recipeId, Guid allergenId);
        Task AddRecipeAsync(Recipe recipe);
        Task<List<InfoRecipeViewModel>> ViewCreatedRecipesAsync(Guid userId);
        Task<List<InfoRecipeViewModel>> ViewLikedRecipesAsync(Guid userId);
        Task<Recipe?> FindRecipeAsync(Guid recipeId, Guid currentUserId);
        Task<bool> IfRecipesAnyAsync(Guid recipeId);
        Task<List<AllergenForAddAndEditRecipeViewModel>> GetAllAllergensAsync();
        Task<List<Guid>> AllergensOfResipeAsync(Guid recipeId);
        Task<List<ProductDetailsViewModel>> AlreadySelectedProductsDetailsAsync(Guid recipeId);
        Task<List<ProductsViewModel>> GetProductsAsync();
        Task RemoveCurrentRecipeAllergensAsync(Guid recipeId);
        Task RemoveCurrentProductDetailsAsync(Guid recipeId);
        Task SaveChangesAsync();
        Task<DeleteRecipeViewModel?> ModelForDeleteAsync(Guid recipeId, Guid currentUserId);
        Task<bool> IfRecipeForDeleteAnyAsync(Guid recipeId);
        Task<Recipe?> FindArchivedRecipeAsync(Guid recipeId, Guid currentUserId);
        Task<List<InfoRecipeViewModel>> ViewArchivedRecipesAsync(Guid currentUserId);
    }
}
