using Microsoft.EntityFrameworkCore;
using RecipeShare.Data;
using RecipeShare.Data.Models;
using RecipeShare.Repositories.Interfaces;
using RecipeShare.Web.ViewModels.CommentViewModels;
using RecipeShare.Web.ViewModels.PaginationViewModels;
using RecipeShare.Web.ViewModels.RecipeViewModels;
using static RecipeShare.Common.ApplicationConstants;

namespace RecipeShare.Repositories
{
    public class ModeratorRepository : IModeratorRepository
    {
        private readonly RecipeShareDbContext context;
        public ModeratorRepository(RecipeShareDbContext _context)
        {
            context = _context;
        }

        public async Task<List<InfoRecipeViewModel>> ViewAllUnapprovedRecipesAsync()
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
            return model;
        }
        public async Task<RecipeDetailsViewModel?> ModelForDetailsAsync(Guid recipeId, Guid userId)
        {
            RecipeDetailsViewModel? model = await context.Recipes
                .Where(r => r.Id == recipeId && r.IsDeleted == false && r.IsApproved == false && r.IsArchived == false)
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
        public async Task<Recipe?> FindRecipeAsync(Guid recipeId)
        {
            Recipe? recipe = await context.Recipes
                .Where(r => r.Id == recipeId && r.IsDeleted == false && r.IsApproved == false && !r.User.IsDeleted)
                .FirstOrDefaultAsync();
            return recipe;
        }
        public async Task<List<InfoRecipeViewModel>> ViewAllRecipesAsync()
        {
            List<InfoRecipeViewModel> model = await context.Recipes
                .Where(r => r.IsApproved == true && r.IsDeleted == false && r.IsArchived == false && !r.User.IsDeleted)
                .AsNoTracking()
                .Select(r => new InfoRecipeViewModel
                {
                    Id = r.Id,
                    RecipeTitle = r.RecipeTitle,
                    DateOfRelease = r.DateOfRelease.ToString(RecipeReleaseDatePattern),
                    ImageUrl = r.Img ?? "~/images/recipes/Recipe.png",
                })
            .ToListAsync();
            return model;
        }
        public async Task<Comment?> FindCommentAsync(Guid commentId)
        {
            Comment? comment = await context.Comments
                .Where(c => c.IsDeleted == false && c.Id == commentId)
                .FirstOrDefaultAsync();
            return comment;
        }
        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}
