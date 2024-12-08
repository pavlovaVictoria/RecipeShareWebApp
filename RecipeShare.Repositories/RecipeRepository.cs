using Microsoft.EntityFrameworkCore;
using RecipeShare.Common.Exceptions;
using RecipeShare.Data;
using RecipeShare.Data.Models;
using RecipeShare.Repositories.Interfaces;
using RecipeShare.Web.ViewModels.CommentViewModels;
using RecipeShare.Web.ViewModels.RecipeViewModels;
using static RecipeShare.Common.ApplicationConstants;

namespace RecipeShare.Repositories
{
    public class RecipeRepository : IRecipeRepository
    {
        private readonly RecipeShareDbContext context;

        public RecipeRepository(RecipeShareDbContext _context)
        {
            context = _context;
        }

        public async Task<List<InfoRecipeViewModel>> GetTop3RecipesAsync()
        {
            return await context.Recipes
                .Where(r => r.IsApproved && r.IsDeleted == false && r.IsArchived == false && !r.User.IsDeleted)
                .OrderByDescending(r => r.LikedRecipes.Count)
                .ThenBy(r => r.RecipeTitle)
                .AsNoTracking()
                .Select(r => new InfoRecipeViewModel
                {
                    Id = r.Id,
                    RecipeTitle = r.RecipeTitle,
                    Description = r.Description,
                    DateOfRelease = r.DateOfRelease.ToString(RecipeReleaseDatePattern),
                    ImageUrl = r.Img ?? "~/images/recipes/Recipe.png"
                })
                .Take(3)
                .ToListAsync();
        }

        public async Task<List<InfoRecipeViewModel>> SearchRecipesAsync(string inputText)
        {
            string text = inputText.ToUpper();
            return await context.Recipes
                .Where(r => r.IsDeleted == false && r.IsArchived == false && !r.User.IsDeleted && r.IsApproved && (r.NormalizedRecipeTitle.Contains(inputText)))
                .Select(r => new InfoRecipeViewModel
                {
                    Id = r.Id,
                    RecipeTitle = r.RecipeTitle,
                    Description = r.Description,
                    DateOfRelease = r.DateOfRelease.ToString(RecipeReleaseDatePattern),
                    ImageUrl = r.Img ?? "~/images/recipes/Recipe.png"
                })
                .ToListAsync();
        }
        public async Task<List<CategoryViewModel>> GetAllCategoriesAsync()
        {
            List<CategoryViewModel> categories = await context.Categories
                .Where(c => c.IsDeleted == false)
                .AsNoTracking()
                .Select(c => new CategoryViewModel
                {
                    Id = c.Id,
                    CategoryName = c.CategoryName
                })
                .ToListAsync();
            return categories;
        }
        public async Task<RecipeDetailsViewModel?> RecipeDetailsAsync(Guid recipeId, Guid userId)
        {
            RecipeDetailsViewModel? model = await context.Recipes
                .Where(r => r.Id == recipeId && r.IsDeleted == false && r.IsApproved && r.IsArchived == false && !r.User.IsDeleted)
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
                    .Where(c => c.IsDeleted == false && c.IsResponse == false && !c.User.IsDeleted)
                    .Select(c => new CommentViewModel
                    {
                        Id = c.Id,
                        UserName = c.User.UserName ?? "Unknown User",
                        DateOfRelease = c.DateOfRelease.ToString(RecipeReleaseDatePattern),
                        Text = c.Text,
                        Responses = c.Responses
                        .Where(cr => cr.IsDeleted == false && cr.IsResponse == true && cr.ParentCommentId == c.Id && !c.User.IsDeleted)
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
        public async Task<(bool isLiked, int likes)> LikeRecipeAsync(Guid recipeId, Guid userId)
        {
            Recipe? recipe = await context.Recipes.Where(r => r.Id == recipeId && r.IsDeleted == false && r.IsApproved && r.IsArchived == false && !r.User.IsDeleted).Include(r => r.LikedRecipes).FirstOrDefaultAsync();
            if (recipe == null)
            {
                throw new HttpStatusException(404);
            }
            bool isLikedNow = await context.LikedRecipes.AnyAsync(lr => lr.UserId == userId && lr.RecipeId == recipeId);
            if (isLikedNow)
            {
                LikedRecipe? likedRecipe = await context.LikedRecipes.Where(lr => lr.UserId == userId && lr.RecipeId == recipeId).FirstOrDefaultAsync();
                if (likedRecipe == null)
                {
                    throw new HttpStatusException(404);
                }
                recipe.LikedRecipes.Remove(likedRecipe);
                context.LikedRecipes.Remove(likedRecipe);
            }
            else
            {
                LikedRecipe likedRecipe = new LikedRecipe()
                {
                    UserId = userId,
                    RecipeId = recipeId
                };
                recipe.LikedRecipes.Add(likedRecipe);
                await context.LikedRecipes.AddAsync(likedRecipe);
            }
            await context.SaveChangesAsync();
            int count = await context.LikedRecipes.Where(lr => lr.RecipeId == recipeId).CountAsync();
            return (!isLikedNow, count);
        }
        public async Task<bool> IsCategoryValidAsync(Guid categoryId)
        {
            bool validCategory = await context.Categories.AnyAsync(c => c.Id == categoryId && c.IsDeleted == false);
            return validCategory;
        }
        public async Task<RecipeByCategoryViewModel> RcipesByCategoryAsync(Guid categoryId)
        {
            RecipeByCategoryViewModel model = await context.Categories
                .Where(c => c.Id == categoryId)
                .AsNoTracking()
                .Select(c => new RecipeByCategoryViewModel
                {
                    CategoryId = c.Id,
                    CategoryName = c.CategoryName,
                    Recipes = context.Recipes.Where(r => r.CategoryId == c.Id && r.IsDeleted == false && r.IsApproved && r.IsArchived == false && !r.User.IsDeleted)
                    .Select(r => new InfoRecipeViewModel
                    {
                        Id = r.Id,
                        RecipeTitle = r.RecipeTitle,
                        Description = r.Description,
                        DateOfRelease = r.DateOfRelease.ToString(RecipeReleaseDatePattern),
                        ImageUrl = r.Img ?? "~/images/recipes/Recipe.png"
                    }).ToList()
                }).FirstAsync();
            return model;
        }
        public async Task<AddRecipeViewModel> ModelForAddAsync()
        {
            AddRecipeViewModel model = new AddRecipeViewModel();
            model.Categories = await context.Categories
                .Where(c => c.IsDeleted == false)
                .AsNoTracking()
                .Select(c => new CategoryViewModel
                {
                    Id = c.Id,
                    CategoryName = c.CategoryName,
                })
                .ToListAsync();

            model.Allergens = await context.Allergens
                .Where(a => a.IsDeleted == false)
                .AsNoTracking()
                .Select(a => new AllergenForAddAndEditRecipeViewModel
                {
                    AllergenId = a.Id,
                    AllergenName = a.AllergenName
                })
                .ToListAsync();

            model.Products = await context.Products
                .Where(p => p.IsDeleted == false)
                .AsNoTracking()
                .Select(p => new ProductsViewModel
                {
                    ProductId = p.Id,
                    ProductName = p.ProductName,
                    ProductType = (int)p.ProductType
                })
                .OrderBy(p => p.ProductType)
                .ThenBy(p => p.ProductName)
                .ToListAsync();
            return model;
        }
        public async Task<Allergen?> FindAllergenAsync(Guid allergenId)
        {
            Allergen? allergen = await context.Allergens
                .Where(a => a.Id == allergenId && a.IsDeleted == false)
                .FirstOrDefaultAsync();
            return allergen;
        }
        public async Task<bool> RecipeAllergensAnyAsync(Guid recipeId, Guid allergenId)
        {
            return await context.RecipesAllergens
                .AnyAsync(ra => ra.AllergenId == allergenId && ra.RecipeId == recipeId);
        }
        public async Task AddRecipeAsync(Recipe recipe)
        {
            await context.Recipes.AddAsync(recipe);
            await context.SaveChangesAsync();
        }
        public async Task<List<InfoRecipeViewModel>> ViewCreatedRecipesAsync(Guid userId)
        {
            List<InfoRecipeViewModel> model = await context.Recipes
                .Where(r => r.UserId == userId && r.IsDeleted == false && r.IsArchived == false && r.IsApproved && !r.User.IsDeleted)
                .Select(r => new InfoRecipeViewModel
                {
                    Id = r.Id,
                    RecipeTitle = r.RecipeTitle,
                    ImageUrl = r.Img ?? "~/images/recipes/Recipe.png",
                    Description = r.Description,
                    DateOfRelease = r.DateOfRelease.ToString(RecipeReleaseDatePattern)
                })
                .ToListAsync();
            return model;
        }
        public async Task<List<InfoRecipeViewModel>> ViewLikedRecipesAsync(Guid userId)
        {
            List<InfoRecipeViewModel> model = await context.LikedRecipes
                .Where(lr => lr.UserId == userId)
                .Select(lr => lr.Recipe)
                .Where(r => r.IsDeleted == false && r.IsApproved && r.IsArchived == false && !r.User.IsDeleted)
                .Select(r => new InfoRecipeViewModel
                {
                    Id = r.Id,
                    RecipeTitle = r.RecipeTitle,
                    DateOfRelease = r.DateOfRelease.ToString(RecipeReleaseDatePattern),
                    Description = r.Description,
                    ImageUrl = r.Img ?? "~/images/recipes/Recipe.png"
                }).ToListAsync();
            return model;
        }
        public async Task<Recipe?> FindRecipeAsync(Guid recipeId, Guid currentUserId)
        {
            Recipe? recipe = await context.Recipes
                .Where(r => r.Id == recipeId && r.UserId == currentUserId && r.IsDeleted == false && r.IsApproved && r.IsArchived == false)
                .FirstOrDefaultAsync();
            return recipe;
        }
        public async Task<bool> IfRecipesAnyAsync(Guid recipeId)
        {
            return await context.Recipes
                .AnyAsync(r => r.Id == recipeId && r.IsDeleted == false && r.IsApproved && r.IsArchived == false);
        }
        public async Task<List<AllergenForAddAndEditRecipeViewModel>> GetAllAllergensAsync()
        {
            List<AllergenForAddAndEditRecipeViewModel> model = await context.Allergens
                .Where(a => a.IsDeleted == false)
                .AsNoTracking()
                .Select(a => new AllergenForAddAndEditRecipeViewModel
                {
                    AllergenId = a.Id,
                    AllergenName = a.AllergenName
                }).ToListAsync();
            return model;
        }
        public async Task<List<Guid>> AllergensOfResipeAsync(Guid recipeId)
        {
            List<Guid> allergens = await context.RecipesAllergens
                .Where(ra => ra.RecipeId == recipeId)
                .Select(ra => ra.AllergenId).ToListAsync();
            return allergens;
        }
        public async Task<List<ProductDetailsViewModel>> AlreadySelectedProductsDetailsAsync(Guid recipeId)
        {
            List<ProductDetailsViewModel> model = await context.RecipesProductsDetails
                .Where(pd => pd.RecipeId == recipeId)
                .Select(pd => new ProductDetailsViewModel
                {
                    ProductId = pd.ProductId,
                    Quantity = pd.Quantity,
                    UnitType = (int)pd.UnitType
                }).ToListAsync();
            return model;
        }
        public async Task<List<ProductsViewModel>> GetProductsAsync()
        {
            List<ProductsViewModel> model = await context.Products
                .Where(p => p.IsDeleted == false)
                .AsNoTracking()
                .Select(p => new ProductsViewModel
                {
                    ProductId = p.Id,
                    ProductName = p.ProductName,
                    ProductType = (int)p.ProductType
                })
                .OrderBy(p => p.ProductType)
                .ThenBy(p => p.ProductName)
                .ToListAsync();
            return model;
        }
        public async Task RemoveCurrentRecipeAllergensAsync(Guid recipeId)
        {
            List<RecipeAllergen> currentRecipeAllergens = await context.RecipesAllergens
                .Where(ra => ra.RecipeId == recipeId)
                .ToListAsync();
            context.RecipesAllergens.RemoveRange(currentRecipeAllergens);
        }
        public async Task RemoveCurrentProductDetailsAsync(Guid recipeId)
        {
            List<RecipeProductDetails> currentRecipeProductDetails = await context.RecipesProductsDetails
                .Where(rpd => rpd.RecipeId == recipeId)
                .ToListAsync();
            context.RecipesProductsDetails.RemoveRange(currentRecipeProductDetails);
        }
        public async Task<DeleteRecipeViewModel?> ModelForDeleteAsync(Guid recipeId, Guid currentUserId)
        {
            DeleteRecipeViewModel? model = await context.Recipes
                .Where(r => r.Id == recipeId && r.IsDeleted == false && r.IsApproved && r.UserId == currentUserId)
                .Select(r => new DeleteRecipeViewModel
                {
                    Id = r.Id,
                    RecipeTitle = r.RecipeTitle
                })
                .FirstOrDefaultAsync();
            return model;
        }
        public async Task<bool> IfRecipeForDeleteAnyAsync(Guid recipeId)
        {
            return await context.Recipes
                .AnyAsync(r => r.Id == recipeId && r.IsDeleted == false && r.IsApproved);
        }
        public async Task<Recipe?> FindArchivedRecipeAsync(Guid recipeId, Guid currentUserId)
        {
            Recipe? recipe = await context.Recipes
                .Where(r => r.Id == recipeId && r.IsDeleted == false && r.IsApproved && r.UserId == currentUserId && r.IsArchived == true)
                .FirstOrDefaultAsync();
            return recipe;
        }
        public async Task<List<InfoRecipeViewModel>> ViewArchivedRecipesAsync(Guid currentUserId)
        {
            List<InfoRecipeViewModel> model = await context.Recipes
                .Where(r => r.IsDeleted == false && r.IsApproved && r.IsArchived == true && r.UserId == currentUserId && !r.User.IsDeleted)
                .Select(r => new InfoRecipeViewModel
                {
                    Id = r.Id,
                    RecipeTitle = r.RecipeTitle,
                    DateOfRelease = r.DateOfRelease.ToString(RecipeReleaseDatePattern),
                    Description = r.Description,
                    ImageUrl = r.Img ?? "~/images/recipes/Recipe.png"
                }).ToListAsync();
            return model;
        }
        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}
