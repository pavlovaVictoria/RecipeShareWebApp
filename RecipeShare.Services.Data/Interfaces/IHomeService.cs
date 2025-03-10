﻿using RecipeShare.Web.ViewModels.RecipeViewModels;

namespace RecipeShare.Services.Data.Interfaces
{
    public interface IHomeService
    {
        Task<List<InfoRecipeViewModel>> Top3RecipesAsync();
        Task<List<InfoRecipeViewModel>> SearchForRecipesAsync(string inputText);
    }
}
