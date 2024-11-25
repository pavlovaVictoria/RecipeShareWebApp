﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeShare.Data;
using RecipeShare.Data.Models;
using RecipeShare.Services.Data.Interfaces;
using RecipeShare.Web.ViewModels.CommentViewModels;
using RecipeShare.Web.ViewModels.RecipeViewModels;
using static RecipeShare.Common.ApplicationConstants;

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
                .Where(r => r.IsApproved)
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
                .Where(r => r.Id == recipeId && r.IsDeleted == false)
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
                    .Where(c => c.IsDeleted == false)
                    .Select(c => new CommentViewModel
                    {
                        UserName = c.User.UserName ?? "Unknown User",
                        DateOfRelease = c.DateOfRelease.ToString(RecipeReleaseDatePattern),
                        Text = c.Text
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
                    IsLikedByCurrentUser = (r.LikedRecipes.Any(lr => lr.User.Id == userId))
                })
                .FirstOrDefaultAsync();
            return model;
        }

        public async Task<(bool isLiked, int likes)> LikeRecipeAsync(Guid recipeId, Guid userId)
        {
            Recipe? recipe = await context.Recipes.Where(r => r.Id == recipeId && r.IsDeleted == false).Include(r => r.LikedRecipes).FirstOrDefaultAsync();
            if (recipe == null)
            {
                throw new ArgumentException("Recipe not found.");
            }
            bool isLikedNow = await context.LikedRecipes.AnyAsync(lr => lr.UserId == userId && lr.RecipeId == recipeId);
            if (isLikedNow)
            {
                LikedRecipe? likedRecipe = await context.LikedRecipes.Where(lr => lr.UserId == userId && lr.RecipeId == recipeId).FirstOrDefaultAsync();
                if (likedRecipe == null)
                {
                    throw new ArgumentException("Recipe not found.");
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
				throw new ArgumentException("Category not found.");
			}
            RecipeByCategoryViewModel model = await context.Categories
                .Where(c => c.Id == categoryId)
                .AsNoTracking()
                .Select(c => new RecipeByCategoryViewModel
                {
                    CategoryId = c.Id,
                    CategoryName = c.CategoryName,
                    Recipes = context.Recipes.Where(r => r.CategoryId == c.Id && r.IsDeleted == false)
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

	}
}
