﻿using Azure;
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
        Task UnapproveRecipeAsync(Guid recipeId);
        Task ApproveRecipeAsync(Guid recipeId);
        Task<PaginatedList<InfoRecipeViewModel>> ViewAllRecipesAsync(int page, int pageSize);
        Task DeleteCommentAsync(Guid commentId);
    }
}
