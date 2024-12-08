using Microsoft.EntityFrameworkCore;
using RecipeShare.Data;
using RecipeShare.Repositories.Interfaces;
using RecipeShare.Services.Data.Interfaces;
using RecipeShare.Web.ViewModels.RecipeViewModels;
using static RecipeShare.Common.ApplicationConstants;

namespace RecipeShare.Services.Data
{
    public class HomeService : IHomeService
    {
        private readonly IRecipeRepository recipeRepository;

        public HomeService(IRecipeRepository _recipeRepository)
        {
            recipeRepository = _recipeRepository;
        }

        public async Task<List<InfoRecipeViewModel>> Top3RecipesAsync()
        {
            return await recipeRepository.GetTop3RecipesAsync();
        }

        public async Task<List<InfoRecipeViewModel>> SearchForRecipesAsync(string inputText)
        {
            return await recipeRepository.SearchRecipesAsync(inputText);
        }
    }
}
