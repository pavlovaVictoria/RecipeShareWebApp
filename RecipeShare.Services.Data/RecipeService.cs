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
using RecipeShare.Repositories.Interfaces;
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
        private readonly IRecipeRepository recipeRepository;

        public RecipeService(IRecipeRepository _recipeRepository)
        {
            recipeRepository = _recipeRepository;
        }

        public async Task<RecipesIndexViewModel> IndexPageOfRecipesAsync()
        {
            List<InfoRecipeViewModel> recipes = await recipeRepository.GetTop3RecipesAsync();

            List<CategoryViewModel> categories = await recipeRepository.GetAllCategoriesAsync();
            RecipesIndexViewModel model = new RecipesIndexViewModel()
            {
                Recipes = recipes,
                Categories = categories
            };
            return model;
        }

        public async Task<RecipeDetailsViewModel?> RecipeDetailsAsync(Guid recipeId, Guid userId)
        {
            RecipeDetailsViewModel? model = await recipeRepository.RecipeDetailsAsync(recipeId, userId);
            return model;
        }

        public async Task<(bool isLiked, int likes)> LikeRecipeAsync(Guid recipeId, Guid userId)
        {
            return await recipeRepository.LikeRecipeAsync(recipeId, userId);
        }

        public async Task<RecipeByCategoryViewModel> RcipesByCategoryAsync(Guid categoryId)
        {
            bool validCategory = await recipeRepository.IsCategoryValidAsync(categoryId);
            if (!validCategory)
            {
				throw new HttpStatusException(404);
			}
            return await recipeRepository.RcipesByCategoryAsync(categoryId);
        }

        public async Task<AddRecipeViewModel> ModelForAddAsync()
        {
            return await recipeRepository.ModelForAddAsync();
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
                Allergen? allergen = await recipeRepository.FindAllergenAsync(allergenId);
                if(allergen != null && !(await recipeRepository.RecipeAllergensAnyAsync(recipe.Id, allergenId)))
                {
                    RecipeAllergen recipeAllergen = new RecipeAllergen
                    {
                        RecipeId = recipe.Id,
                        AllergenId = allergenId
                    };
                    recipe.AllergensRecipes.Add(recipeAllergen);
                }
            }
            await recipeRepository.AddRecipeAsync(recipe);
        }

        public async Task<PaginatedList<InfoRecipeViewModel>> ViewCreatedRecipesAsync(Guid userId, int page, int pageSize)
        {
            List<InfoRecipeViewModel> model = await recipeRepository.ViewCreatedRecipesAsync(userId);
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
            List<InfoRecipeViewModel> model = await recipeRepository.ViewLikedRecipesAsync(userId);
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
            Recipe? recipe = await recipeRepository.FindRecipeAsync(recipeId, currentUserId);
            if (recipe == null)
            {
                if (await recipeRepository.IfRecipesAnyAsync(recipeId))
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
                Categories = await recipeRepository.GetAllCategoriesAsync(),
                Img = recipe.Img,
                Allergens = await recipeRepository.GetAllAllergensAsync(),
                SelectedAllergenIds = await recipeRepository.AllergensOfResipeAsync(recipeId),
                ProductsDetails = await recipeRepository.AlreadySelectedProductsDetailsAsync(recipeId),
                Products = await recipeRepository.GetProductsAsync()
            };
            return model;
        }

        public async Task EditRecipeAsync(EditRecipeViewModel model, Guid recipeId, Guid currentUserId)
        {
            Recipe? recipe = await recipeRepository.FindRecipeAsync(recipeId, currentUserId);
            if (recipe == null)
            {
                if(await recipeRepository.IfRecipesAnyAsync(recipeId))
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
            
            await recipeRepository.RemoveCurrentRecipeAllergensAsync(recipe.Id);

            foreach (Guid allergenId in model.SelectedAllergenIds)
            {
                Allergen? allergen = await recipeRepository.FindAllergenAsync(allergenId);
                if (allergen != null && !(await recipeRepository.RecipeAllergensAnyAsync(recipeId, allergenId)))
                {
                    RecipeAllergen recipeAllergen = new RecipeAllergen
                    {
                        RecipeId = recipe.Id,
                        AllergenId = allergenId
                    };
                    recipe.AllergensRecipes.Add(recipeAllergen);
                }
            }

            await recipeRepository.RemoveCurrentProductDetailsAsync(recipeId);

            recipe.RecipesProductsDetails = model.ProductsDetails.Select(pd => new RecipeProductDetails
            {
                ProductId = pd.ProductId,
                Quantity = pd.Quantity,
                UnitType = (UnitType)pd.UnitType
            }).ToList();

            await recipeRepository.SaveChangesAsync();
        }

        public async Task<DeleteRecipeViewModel> ModelForDeleteAsync(Guid recipeId, Guid currentUserId)
        {
            DeleteRecipeViewModel? model = await recipeRepository.ModelForDeleteAsync(recipeId, currentUserId);
            if (model == null)
            {
                if(await recipeRepository.IfRecipeForDeleteAnyAsync(recipeId))
                {
                    throw new HttpStatusException(403);
                }
                throw new HttpStatusException(404);
            }
            return model;
        }

        public async Task DeleteOrArchiveAsync(Guid recipeId, Guid currentUserId, string action)
        {
            Recipe? recipe = await recipeRepository.FindRecipeAsync(recipeId, currentUserId);
            if (recipe == null)
            {
                if (await recipeRepository.IfRecipeForDeleteAnyAsync(recipeId))
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
            await recipeRepository.SaveChangesAsync();
        }

        public async Task UnarchiveRecipeAsync(Guid recipeId, Guid currentUserId)
        {
            Recipe? recipe = await recipeRepository.FindArchivedRecipeAsync(recipeId, currentUserId);
            if (recipe == null)
            {
                if (await recipeRepository.IfRecipeForDeleteAnyAsync(recipeId))
                {
                    throw new HttpStatusException(403);
                }
                throw new HttpStatusException(404);
            }
            recipe.IsArchived = false;
            await recipeRepository.SaveChangesAsync();
        }

        public async Task<PaginatedList<InfoRecipeViewModel>> ViewArchivedRecipesAsync(Guid currentUserId, int page, int pageSize)
        {
            List<InfoRecipeViewModel> model = await recipeRepository.ViewArchivedRecipesAsync(currentUserId);
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
