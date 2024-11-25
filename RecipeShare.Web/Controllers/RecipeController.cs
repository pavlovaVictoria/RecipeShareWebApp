using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeShare.Data.Models;
using RecipeShare.Services.Data.Interfaces;
using RecipeShare.Web.ViewModels.RecipeViewModels;
using System.Security.Claims;

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
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid recipeId)
        {
            Guid currentUserId = GetCurrentUserId();
            RecipeDetailsViewModel? recipe = await recipeService.RecipeDetailsAsync(recipeId, currentUserId);
            if (recipe == null)
            {
                return RedirectToAction("HttpStatusCodeHandler", "Error", new { statusCade = 404 });
            }
            return View(recipe);
        }

        [HttpPost]
        public async Task<IActionResult> LikeRecipe(Guid recipeId)
        {
            Guid currenrUserId = GetCurrentUserId();
            if (currenrUserId == Guid.Empty)
            {
                return RedirectToAction("HttpStatusCodeHandler", "Error", new { statusCade = 403 });
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
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message,
                    redirectUrl = Url.Action("Error", "Home", new { area = "", errorCode = 404 })
                });
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
