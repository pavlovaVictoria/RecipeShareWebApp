using Azure;
using Microsoft.EntityFrameworkCore;
using RecipeShare.Common.Exceptions;
using RecipeShare.Data;
using RecipeShare.Data.Models;
using RecipeShare.Repositories.Interfaces;
using RecipeShare.Services.Data.Interfaces;
using RecipeShare.Web.ViewModels.CommentViewModels;
using RecipeShare.Web.ViewModels.PaginationViewModels;
using RecipeShare.Web.ViewModels.RecipeViewModels;
using System.Drawing.Printing;
using static RecipeShare.Common.ApplicationConstants;

namespace RecipeShare.Services.Data
{
    public class ModeratorService : IModeratorService
    {
        private readonly IModeratorRepository moderatorRepository;
        public ModeratorService(IModeratorRepository _moderatorRepository)
        {
            moderatorRepository = _moderatorRepository;
        }
        public async Task<PaginatedList<InfoRecipeViewModel>> ViewAllUnapprovedRecipesAsync(int page, int pageSize)
        {
            List<InfoRecipeViewModel> model = await moderatorRepository.ViewAllUnapprovedRecipesAsync();
            IEnumerable<InfoRecipeViewModel> paginatedRecipes = model
            .Skip((page - 1) * pageSize).Take(pageSize);

            PaginatedList<InfoRecipeViewModel> recipes = new PaginatedList<InfoRecipeViewModel>(
                paginatedRecipes,
                model.Count(),
                page,
                pageSize
            );
            return recipes;
        }
        public async Task<RecipeDetailsViewModel?> RecipeDetailsAsync(Guid recipeId, Guid userId)
        {
            return await moderatorRepository.ModelForDetailsAsync(recipeId, userId);
        }
        public async Task UnapproveRecipeAsync(Guid recipeId)
        {
            Recipe? recipe = await moderatorRepository.FindRecipeAsync(recipeId);
            if (recipe == null)
            {
                throw new HttpStatusException(404);
            }
            recipe.IsDeleted = true;
            await moderatorRepository.SaveChangesAsync();
        }
        public async Task ApproveRecipeAsync(Guid recipeId)
        {
            Recipe? recipe = await moderatorRepository.FindRecipeAsync(recipeId);
            if (recipe == null)
            {
                throw new HttpStatusException(404);
            }
            recipe.IsApproved = true;
            await moderatorRepository.SaveChangesAsync();
        }
        public async Task<PaginatedList<InfoRecipeViewModel>> ViewAllRecipesAsync(int page, int pageSize)
        {
            List<InfoRecipeViewModel> model = await moderatorRepository.ViewAllRecipesAsync();
            IEnumerable<InfoRecipeViewModel> paginatedRecipes = model
            .Skip((page - 1) * pageSize).Take(pageSize);

            PaginatedList<InfoRecipeViewModel> recipes = new PaginatedList<InfoRecipeViewModel>(
                paginatedRecipes,
                model.Count(),
                page,
                pageSize
            );
            return recipes;
        }
        public async Task DeleteCommentAsync(Guid commentId)
        {
			Comment? comment = await moderatorRepository.FindCommentAsync(commentId);
			if (comment == null)
			{
				throw new HttpStatusException(404);
			}
			comment.IsDeleted = true;
			await moderatorRepository.SaveChangesAsync();
		}

	}
}
