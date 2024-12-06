using Azure;
using Microsoft.EntityFrameworkCore;
using RecipeShare.Common.Exceptions;
using RecipeShare.Data;
using RecipeShare.Data.Models;
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
        private readonly RecipeShareDbContext context;
        public ModeratorService(RecipeShareDbContext _context)
        {
            context = _context;
        }
        public async Task<PaginatedList<InfoRecipeViewModel>> ViewAllUnapprovedRecipesAsync(int page, int pageSize)
        {
            List<InfoRecipeViewModel> model = await context.Recipes
                .Where(r => r.IsApproved == false && r.IsDeleted == false && r.IsArchived == false)
                .AsNoTracking()
                .Select(r => new InfoRecipeViewModel
                {
                    Id = r.Id,
                    RecipeTitle = r.RecipeTitle,
                    DateOfRelease = r.DateOfRelease.ToString(RecipeReleaseDatePattern),
                    ImageUrl = r.Img ?? "~/images/recipes/Recipe.png",
                })
            .ToListAsync();
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
            RecipeDetailsViewModel? model = await context.Recipes
                .Where(r => r.Id == recipeId && r.IsDeleted == false && r.IsApproved == true && r.IsArchived == false)
                .AsNoTracking()
                .Select(r => new RecipeDetailsViewModel
                {
                    Id = r.Id,
                    RecipeTitle = r.RecipeTitle,
                    Description = r.Description,
                    UserName = r.User.UserName ?? "Unknown User",
                    Preparation = r.Preparation,
                    MinutesForPrep = r.MinutesForPrep,
                    MealType = r.MealType.ToString(),
                    Category = r.Category.CategoryName,
                    DateOfRelease = r.DateOfRelease.ToString(RecipeReleaseDatePattern),
                    Comments = r.Comments
                    .Where(c => c.IsDeleted == false && c.IsResponse == false)
                    .Select(c => new CommentViewModel
                    {
                        Id = c.Id,
                        UserName = c.User.UserName ?? "Unknown User",
                        DateOfRelease = c.DateOfRelease.ToString(RecipeReleaseDatePattern),
                        Text = c.Text,
                        Responses = c.Responses
                        .Where(cr => cr.IsDeleted == false && cr.IsResponse == true && cr.ParentCommentId == c.Id)
                        .Select(cr => new CommentViewModel
                        {
                            Id = cr.Id,
                            DateOfRelease = cr.DateOfRelease.ToString(RecipeReleaseDatePattern),
                            Text = cr.Text,
                            UserName = cr.User.UserName ?? "Unknown User",
                            IsResponse = c.IsResponse
                        })
                        .ToList(),
                        IsResponse = c.IsResponse
                    }).ToList(),
                    Allergens = r.AllergensRecipes.Select(ar => ar.Allergen)
                    .Where(a => a.IsDeleted == false)
                    .Select(a => new AllergenViewModel
                    {
                        AllergenImage = a.AllergenImage,
                        AllergenName = a.AllergenName
                    })
                    .ToList(),
                    Likes = r.LikedRecipes.Count(),
                    IsLikedByCurrentUser = (r.LikedRecipes.Any(lr => lr.User.Id == userId)),
                    ProductDetails = context.RecipesProductsDetails
                    .Where(rp => rp.RecipeId == recipeId)
                    .Select(rp => new RecipeProductDetailsViewModel
                    {
                        ProductName = rp.Product.ProductName,
                        Quantity = rp.Quantity,
                        UnitType = rp.UnitType.ToString()
                    })
                    .ToList(),
                })
                .FirstOrDefaultAsync();
            return model;
        }
        public async Task UnapproveRecipeAsync(Guid recipeId)
        {
            Recipe? recipe = await context.Recipes
                .Where(r => r.Id == recipeId && r.IsDeleted == false && r.IsApproved == false)
                .FirstOrDefaultAsync();
            if (recipe == null)
            {
                throw new HttpStatusException(404);
            }
            recipe.IsDeleted = true;
            await context.SaveChangesAsync();
        }
        public async Task ApproveRecipeAsync(Guid recipeId)
        {
            Recipe? recipe = await context.Recipes
                .Where(r => r.Id == recipeId && r.IsDeleted == false && r.IsApproved == false)
                .FirstOrDefaultAsync();
            if (recipe == null)
            {
                throw new HttpStatusException(404);
            }
            recipe.IsApproved = true;
            await context.SaveChangesAsync();
        }
        public async Task<PaginatedList<InfoRecipeViewModel>> ViewAllRecipesAsync(int page, int pageSize)
        {
            List<InfoRecipeViewModel> model = await context.Recipes
                .Where(r => r.IsApproved == true && r.IsDeleted == false && r.IsArchived == false)
                .AsNoTracking()
                .Select(r => new InfoRecipeViewModel
                {
                    Id = r.Id,
                    RecipeTitle = r.RecipeTitle,
                    DateOfRelease = r.DateOfRelease.ToString(RecipeReleaseDatePattern),
                    ImageUrl = r.Img ?? "~/images/recipes/Recipe.png",
                })
            .ToListAsync();
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
			Comment? comment = await context.Comments
				.Where(c => c.IsDeleted == false && c.Id == commentId)
				.FirstOrDefaultAsync();
			if (comment == null)
			{
				throw new HttpStatusException(404);
			}
			comment.IsDeleted = true;
			await context.SaveChangesAsync();
		}

	}
}
