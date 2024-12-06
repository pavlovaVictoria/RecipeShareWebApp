using Azure;
using Azure.Messaging;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeShare.Common.Enums;
using RecipeShare.Common.Exceptions;
using RecipeShare.Data;
using RecipeShare.Data.Models;
using RecipeShare.Services.Data.Interfaces;
using RecipeShare.Web.ViewModels.CommentViewModels;
using RecipeShare.Web.ViewModels.PaginationViewModels;
using RecipeShare.Web.ViewModels.RecipeViewModels;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Printing;
using System.Globalization;
using System.Net;
using static RecipeShare.Common.ApplicationConstants;
using static System.Formats.Asn1.AsnWriter;

namespace RecipeShare.Services.Data
{
    public class RecipeService : IRecipeService
    {
        private readonly RecipeShareDbContext context;
        private readonly UserManager<ApplicationUser> userManager;

        public RecipeService(RecipeShareDbContext _context, UserManager<ApplicationUser> _userManager)
        {
            context = _context;
            userManager = _userManager;
        }

        public async Task<RecipesIndexViewModel> IndexPageOfRecipesAsync()
        {
            List<InfoRecipeViewModel> recipes = await context.Recipes
                .Where(r => r.IsApproved && r.IsDeleted == false && r.IsArchived == false)
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

            List<CategoryViewModel> categories = await context.Categories
                .Where(c => c.IsDeleted == false)
                .AsNoTracking()
                .Select(c => new CategoryViewModel
                {
                    Id = c.Id,
                    CategoryName = c.CategoryName
                })
                .ToListAsync();
            RecipesIndexViewModel model = new RecipesIndexViewModel()
            {
                Recipes = recipes,
                Categories = categories
            };
            return model;
        }

        public async Task<RecipeDetailsViewModel?> RecipeDetailsAsync(Guid recipeId, Guid userId)
        {
            RecipeDetailsViewModel? model = await context.Recipes
                .Where(r => r.Id == recipeId && r.IsDeleted == false && r.IsApproved && r.IsArchived == false)
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

        public async Task<(bool isLiked, int likes)> LikeRecipeAsync(Guid recipeId, Guid userId)
        {
            Recipe? recipe = await context.Recipes.Where(r => r.Id == recipeId && r.IsDeleted == false && r.IsApproved && r.IsArchived == false).Include(r => r.LikedRecipes).FirstOrDefaultAsync();
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

        public async Task<RecipeByCategoryViewModel> RcipesByCategoryAsync(Guid categoryId)
        {
            bool validCategory = await context.Categories.AnyAsync(c => c.Id == categoryId && c.IsDeleted == false);
            if (!validCategory)
            {
				throw new HttpStatusException(404);
			}
            RecipeByCategoryViewModel model = await context.Categories
                .Where(c => c.Id == categoryId)
                .AsNoTracking()
                .Select(c => new RecipeByCategoryViewModel
                {
                    CategoryId = c.Id,
                    CategoryName = c.CategoryName,
                    Recipes = context.Recipes.Where(r => r.CategoryId == c.Id && r.IsDeleted == false && r.IsApproved && r.IsArchived == false)
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

        public async Task AddRecipeAsync(AddRecipeViewModel model, Guid currentUserId)
        {
            Recipe recipe = new Recipe()
            {
                RecipeTitle = model.RecipeTitle,
                NormalizedRecipeTitle = model.RecipeTitle.ToUpper(),
                UserId = currentUserId,
                Description = model.Description,
                Preparation = model.Preparation,
                MinutesForPrep = model.MinutesForPrep,
                MealType = (MealType)model.MealType,
                CategoryId = model.CategoryId,
                Img = model.Img ?? "~/images/recipes/Recipe.png",
                DateOfRelease = DateTime.UtcNow,
                RecipesProductsDetails = model.ProductsDetails.Select(pd => new RecipeProductDetails
                {
                    ProductId = pd.ProductId,
                    Quantity = pd.Quantity,
                    UnitType = (UnitType)pd.UnitType
                }).ToList(),
            };
            foreach(Guid allergenId in model.SelectedAllergenIds)
            {
                Allergen? allergen = await context.Allergens.Where(a => a.Id == allergenId && a.IsDeleted == false).FirstOrDefaultAsync();
                if(allergen != null && !(await context.RecipesAllergens.AnyAsync(ra => ra.AllergenId == allergenId && ra.RecipeId == recipe.Id)))
                {
                    RecipeAllergen recipeAllergen = new RecipeAllergen
                    {
                        RecipeId = recipe.Id,
                        AllergenId = allergenId
                    };
                    recipe.AllergensRecipes.Add(recipeAllergen);
                }
            }
            await context.Recipes.AddAsync(recipe);
            await context.SaveChangesAsync();
        }

        public async Task<PaginatedList<InfoRecipeViewModel>> ViewCreatedRecipesAsync(Guid userId, int page, int pageSize)
        {
            List<InfoRecipeViewModel> model = await context.Recipes
                .Where(r => r.UserId == userId && r.IsDeleted == false && r.IsArchived == false && r.IsApproved)
                .Select(r => new InfoRecipeViewModel
                {
                    Id = r.Id,
                    RecipeTitle = r.RecipeTitle,
                    ImageUrl = r.Img ?? "~/images/recipes/Recipe.png",
                    Description = r.Description,
                    DateOfRelease = r.DateOfRelease.ToString(RecipeReleaseDatePattern)
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

        public async Task<PaginatedList<InfoRecipeViewModel>> ViewLikedRecipesAsync(Guid userId, int page, int pageSize)
        {
            List<InfoRecipeViewModel> model = await context.LikedRecipes
                .Where(lr => lr.UserId == userId)
                .Select(lr => lr.Recipe)
                .Where(r => r.IsDeleted == false && r.IsApproved && r.IsArchived == false)
                .Select(r => new InfoRecipeViewModel 
                { 
                    Id = r.Id,
                    RecipeTitle = r.RecipeTitle,
                    DateOfRelease = r.DateOfRelease.ToString(RecipeReleaseDatePattern),
                    Description = r.Description,
                    ImageUrl = r.Img ?? "~/images/recipes/Recipe.png"
                }).ToListAsync();
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

        public async Task<EditRecipeViewModel> ModelForEdidAsync(Guid recipeId, Guid currentUserId)
        {
            Recipe? recipe = await context.Recipes
                .Where(r => r.Id == recipeId && r.UserId == currentUserId && r.IsDeleted == false && r.IsApproved && r.IsArchived == false)
                .FirstOrDefaultAsync();
            if (recipe == null)
            {
                if (await context.Recipes.AnyAsync(r => r.Id == recipeId && r.IsDeleted == false && r.IsApproved && r.IsArchived == false))
                {
                    throw new HttpStatusException(403);
                }
                throw new HttpStatusException(404);
            }
            EditRecipeViewModel model = new EditRecipeViewModel
            {
                RecipeId = recipeId,
                RecipeTitle = recipe.RecipeTitle,
                Description = recipe.Description,
                Preparation = recipe.Preparation,
                MinutesForPrep = recipe.MinutesForPrep,
                MealType = (int)recipe.MealType,
                CategoryId = recipe.CategoryId,
                Categories = context.Categories.Where(c => c.IsDeleted == false)
                .Select(c => new CategoryViewModel 
                { 
                    Id = c.Id,
                    CategoryName = c.CategoryName,
                }).ToList(),
                Img = recipe.Img,
                Allergens = context.Allergens
                .Where(a => a.IsDeleted == false)
                .AsNoTracking()
                .Select(a => new AllergenForAddAndEditRecipeViewModel
                {
                    AllergenId = a.Id,
                    AllergenName = a.AllergenName
                }).ToList(),
                SelectedAllergenIds = context.RecipesAllergens.Where(ra => ra.RecipeId == recipeId)
                .Select(ra => ra.AllergenId).ToList(),
                ProductsDetails = context.RecipesProductsDetails.Where(pd => pd.RecipeId == recipeId)
                .Select(pd => new ProductDetailsViewModel 
                { 
                    ProductId = pd.ProductId,
                    Quantity = pd.Quantity,
                    UnitType = (int)pd.UnitType
                }).ToList(),
                Products = context.Products
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
                .ToList()
            };
            return model;
        }

        public async Task EditRecipeAsync(EditRecipeViewModel model, Guid recipeId, Guid currentUserId)
        {
            Recipe? recipe = await context.Recipes
                .Where(r => r.Id == recipeId && r.UserId == currentUserId && r.IsDeleted == false && r.IsApproved && r.IsArchived == false)
                .FirstOrDefaultAsync();
            if (recipe == null)
            {
                if(await context.Recipes.AnyAsync(r => r.Id == recipeId && r.IsDeleted == false && r.IsApproved && r.IsArchived == false))
                {
                    throw new HttpStatusException(403);
                }
                throw new HttpStatusException(404);
            }
            recipe.RecipeTitle = model.RecipeTitle;
            recipe.NormalizedRecipeTitle = model.RecipeTitle.ToUpper();
            recipe.Description = model.Description;
            recipe.Preparation = model.Preparation;
            recipe.MinutesForPrep = model.MinutesForPrep;
            recipe.MealType = (MealType)model.MealType;
            recipe.CategoryId = model.CategoryId;
            recipe.Img = model.Img;
            //recipe.IsApproved = false;
            List<RecipeAllergen> currentRecipeAllergens = await context.RecipesAllergens
                .Where(ra => ra.RecipeId == recipe.Id)
                .ToListAsync();
            context.RecipesAllergens.RemoveRange(currentRecipeAllergens);

            foreach (Guid allergenId in model.SelectedAllergenIds)
            {
                Allergen? allergen = await context.Allergens.Where(a => a.Id == allergenId && a.IsDeleted == false).FirstOrDefaultAsync();
                if (allergen != null && !(await context.RecipesAllergens.AnyAsync(ra => ra.AllergenId == allergenId && ra.RecipeId == recipe.Id)))
                {
                    RecipeAllergen recipeAllergen = new RecipeAllergen
                    {
                        RecipeId = recipe.Id,
                        AllergenId = allergenId
                    };
                    recipe.AllergensRecipes.Add(recipeAllergen);
                }
            }

            List<RecipeProductDetails> currentRecipeProductDetails = await context.RecipesProductsDetails
                .Where(rpd => rpd.RecipeId == recipe.Id)
                .ToListAsync();
            context.RecipesProductsDetails.RemoveRange(currentRecipeProductDetails);

            recipe.RecipesProductsDetails = model.ProductsDetails.Select(pd => new RecipeProductDetails
            {
                ProductId = pd.ProductId,
                Quantity = pd.Quantity,
                UnitType = (UnitType)pd.UnitType
            }).ToList();
            ;
            await context.SaveChangesAsync();
        }

        public async Task<DeleteRecipeViewModel> ModelForDeleteAsync(Guid recipeId, Guid currentUserId)
        {
            DeleteRecipeViewModel? model = await context.Recipes
                .Where(r => r.Id == recipeId && r.IsDeleted == false && r.IsApproved && r.UserId == currentUserId)
                .Select(r => new DeleteRecipeViewModel
                {
                    Id = r.Id,
                    RecipeTitle = r.RecipeTitle
                })
                .FirstOrDefaultAsync();
            if (model == null)
            {
                if(await context.Recipes.AnyAsync(r => r.Id == recipeId && r.IsDeleted == false && r.IsApproved))
                {
                    throw new HttpStatusException(403);
                }
                throw new HttpStatusException(404);
            }
            return model;
        }

        public async Task DeleteOrArchiveAsync(Guid recipeId, Guid currentUserId, string action)
        {
            Recipe? recipe = await context.Recipes
                .Where(r => r.Id == recipeId && r.IsDeleted == false && r.IsApproved && r.UserId == currentUserId)
                .FirstOrDefaultAsync();
            if (recipe == null)
            {
                if (await context.Recipes.AnyAsync(r => r.Id == recipeId && r.IsDeleted == false && r.IsApproved))
                {
                    throw new HttpStatusException(403);
                }
                throw new HttpStatusException(404);
            }
            if (action == "delete")
            {
                recipe.IsDeleted = true;
            }
            else if (action == "archive")
            {
                recipe.IsArchived = true;
            }
            await context.SaveChangesAsync();
        }

        public async Task UnarchiveRecipeAsync(Guid recipeId, Guid currentUserId)
        {
            Recipe? recipe = await context.Recipes
                .Where(r => r.Id == recipeId && r.IsDeleted == false && r.IsApproved && r.UserId == currentUserId && r.IsArchived == true)
                .FirstOrDefaultAsync();
            if (recipe == null)
            {
                if (await context.Recipes.AnyAsync(r => r.Id == recipeId && r.IsDeleted == false && r.IsApproved))
                {
                    throw new HttpStatusException(403);
                }
                throw new HttpStatusException(404);
            }
            recipe.IsArchived = false;
            await context.SaveChangesAsync();
        }

        public async Task<PaginatedList<InfoRecipeViewModel>> ViewArchivedRecipesAsync(Guid currentUserId, int page, int pageSize)
        {
            List<InfoRecipeViewModel> model = await context.Recipes
                .Where(r => r.IsDeleted == false && r.IsApproved && r.IsArchived == true && r.UserId == currentUserId)
                .Select(r => new InfoRecipeViewModel
                {
                    Id = r.Id,
                    RecipeTitle = r.RecipeTitle,
                    DateOfRelease = r.DateOfRelease.ToString(RecipeReleaseDatePattern),
                    Description = r.Description,
                    ImageUrl = r.Img ?? "~/images/recipes/Recipe.png"
                }).ToListAsync();
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
    }
}
