using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecipeShare.Common.Exceptions;
using RecipeShare.Services.Data.Interfaces;
using RecipeShare.Web.ViewModels.PaginationViewModels;
using RecipeShare.Web.ViewModels.RecipeViewModels;
using System.Globalization;
using System.Security.Claims;
using static RecipeShare.Common.ApplicationConstants;

namespace RecipeShare.Web.Controllers
{
    [Authorize]
    public class RecipeController : Controller
    {
        private readonly IRecipeService recipeService;

        public RecipeController(IRecipeService _recipeService)
        {
            recipeService = _recipeService;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            RecipesIndexViewModel model = await recipeService.IndexPageOfRecipesAsync();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> AllRecipesByCategory(Guid categoryId)
        {
			RecipeByCategoryViewModel recipes;

			try
            {
                recipes = await recipeService.RcipesByCategoryAsync(categoryId);
            }
            catch (HttpStatusException statusCode)
            {
                return View($"Error/{statusCode}");
            }
            return View(recipes);
        }

        [HttpGet]
        [Route("Recipe/Details/{recipeId:guid}")]
        public async Task<IActionResult> Details(Guid recipeId)
        {
            Guid currentUserId = GetCurrentUserId();
            RecipeDetailsViewModel? recipe = await recipeService.RecipeDetailsAsync(recipeId, currentUserId);
            if (recipe == null)
            {
                return View($"Error/{404}");
            }
            return View(recipe);
        }

        [HttpPost]
        public async Task<IActionResult> LikeRecipe(Guid recipeId)
        {
            Guid currenrUserId = GetCurrentUserId();
            if (currenrUserId == Guid.Empty)
            {
                return View($"Error/{403}");
            }

            try
            {
                var (isLiked, likes) = await recipeService.LikeRecipeAsync(recipeId, currenrUserId);
                
                return Json(new
                {
                    success = true,
                    isLiked,
                    likes
                });
            }
            catch (HttpStatusException ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message,
                    redirectUrl = Url.Action("Error", "Home", new { area = "", errorCode = 404 })
                });
            }
        }


        [HttpGet]
        public async Task<IActionResult> Add()
        {
            AddRecipeViewModel model = await recipeService.ModelForAddAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddRecipeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            Guid currentUserId = GetCurrentUserId();
            if (currentUserId == Guid.Empty)
            {
                return View($"Error/{403}");
            }
            try
            {
                await recipeService.AddRecipeAsync(model, currentUserId);
                TempData["SuccessMessage"] = "Your recipe was successfully added!";
                return RedirectToAction("Index");
            }
            catch (HttpStatusException statusCode)
            {
                return View($"Error/{statusCode}");
            }
        }

        [HttpGet]
        [Route("Recipe/Edit/{recipeId:guid}")]
        public async Task<IActionResult> Edit(Guid recipeId)
        {
            Guid currentUserId = GetCurrentUserId();
            if (currentUserId == Guid.Empty)
            {
                return View($"Error/{403}");
            }
            try
            {
                EditRecipeViewModel model = await recipeService.ModelForEdidAsync(recipeId, currentUserId);
                return View(model);
            }
            catch (HttpStatusException statusCode)
            {
                return View($"Error/{statusCode}");
            }
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditRecipeViewModel model, Guid recipeId)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            Guid currentUserId = GetCurrentUserId();
            if (currentUserId == Guid.Empty)
            {
                return View($"Error/{403}");
            }
            try
            {
                await recipeService.EditRecipeAsync(model, recipeId, currentUserId);
                return RedirectToAction("Index", "Recipe");
            }
            catch (HttpStatusException statusCode)
            {
                return View($"Error/{statusCode}");
            }
        }


        [HttpGet]
        public async Task<IActionResult> MyCollection(int page = 1, int pageSize = 4)
        {
            Guid currentUserId = GetCurrentUserId();
            if (currentUserId == Guid.Empty)
            {
                return View($"Error/{403}");
            }
            PaginatedList<InfoRecipeViewModel> model = await recipeService.ViewCreatedRecipesAsync(currentUserId, page, pageSize);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> LikedRecipes(int page = 1, int pageSize = 4)
        {
            Guid currentUserId = GetCurrentUserId();
            if (currentUserId == Guid.Empty)
            {
                return View($"Error/{403}");
            }
            PaginatedList<InfoRecipeViewModel> model = await recipeService.ViewLikedRecipesAsync(currentUserId, page, pageSize);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid recipeId)
        {
            Guid currentUserId = GetCurrentUserId();
            if (currentUserId == Guid.Empty)
            {
                return View($"Error/{403}");
            }
            try
            {
                DeleteRecipeViewModel model = await recipeService.ModelForDeleteAsync(recipeId, currentUserId);
                return View(model);
            }
            catch (HttpStatusException statusCode)
            {
                return View($"Error/{statusCode}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(DeleteRecipeViewModel model)
        {
            Guid currentUserId = GetCurrentUserId();
            if (currentUserId == Guid.Empty)
            {
                return View($"Error/{403}");
            }
            try
            {
                await recipeService.DeleteOrArchiveAsync(model.Id, currentUserId, "delete");
                return RedirectToAction("MyCollection", "Recipe");
            }
            catch (HttpStatusException statusCode)
            {
                return View($"Error/{statusCode}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Archive(DeleteRecipeViewModel model)
        {
            Guid currentUserId = GetCurrentUserId();
            if (currentUserId == Guid.Empty)
            {
                return View($"Error/{403}");
            }
            try
            {
                await recipeService.DeleteOrArchiveAsync(model.Id, currentUserId, "archive");
                return RedirectToAction("MyCollection", "Recipe");
            }
            catch (HttpStatusException statusCode)
            {
                return View($"Error/{statusCode}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ArchivedRecipes(int page = 1, int pageSize = 4)
        {
            Guid currentUserId = GetCurrentUserId();
            if (currentUserId == Guid.Empty)
            {
                return View($"Error/{403}");
            }
            PaginatedList<InfoRecipeViewModel> model = await recipeService.ViewArchivedRecipesAsync(currentUserId, page, pageSize);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unarchive(Guid recipeId)
        {
            Guid currentUserId = GetCurrentUserId();
            if (currentUserId == Guid.Empty)
            {
                return View($"Error/{403}");
            }
            try
            {
                await recipeService.UnarchiveRecipeAsync(recipeId, currentUserId);
                return RedirectToAction("MyCollection", "Recipe");
            }
            catch (HttpStatusException statusCode)
            {
                return View($"Error/{statusCode}");
            }
        }

        private Guid GetCurrentUserId()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Guid.Empty;
            }
            if (Guid.TryParse(userId, out Guid userIdGuid))
            {
                return userIdGuid;
            }
            return Guid.Empty;
        }
    }
}
