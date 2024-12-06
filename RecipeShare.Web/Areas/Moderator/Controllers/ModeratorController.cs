using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeShare.Common.Exceptions;
using RecipeShare.Data.Models;
using RecipeShare.Services.Data;
using RecipeShare.Services.Data.Interfaces;
using RecipeShare.Web.ViewModels.PaginationViewModels;
using RecipeShare.Web.ViewModels.RecipeViewModels;
using System.Security.Claims;

namespace RecipeShare.Web.Areas.Moderator.Controllers
{
    [Area("Moderator")]
    [Authorize(Policy = "CanApproveRecipes")]
    public class ModeratorController : Controller
    {
        private readonly IModeratorService moderatorService;
        public ModeratorController(IModeratorService _moderatorService)
        {
            moderatorService = _moderatorService;
        }
        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 4)
        {
            PaginatedList<InfoRecipeViewModel> recipes = await moderatorService.ViewAllUnapprovedRecipesAsync(page, pageSize);
            return View(recipes);
        }
        [HttpGet]
        public async Task<IActionResult> Details(Guid recipeId)
        {
            if (recipeId == Guid.Empty)
            {
                return RedirectToAction("Index", "Home");
            }
            Guid currentUserId = GetCurrentUserId();
            if (currentUserId == Guid.Empty)
            {
                return View($"Error/{403}");
            }
            RecipeDetailsViewModel? recipe = await moderatorService.RecipeDetailsAsync(recipeId, currentUserId);
            if (recipe == null)
            {
                return View($"Error/{404}");
            }
            return View(recipe);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unapprove(Guid recipeId)
        {
            Guid currentUserId = GetCurrentUserId();
            if (currentUserId == Guid.Empty || !User.IsInRole("Moderator"))
            {
                return View($"Error/{403}");
            }
            try
            {
                await moderatorService.UnapproveRecipeAsync(recipeId);
                return RedirectToAction("Index", "Moderator");
            }
            catch (HttpStatusException statusCode)
            {
                return View($"Error/{statusCode}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(Guid recipeId)
        {
            Guid currentUserId = GetCurrentUserId();
            if (currentUserId == Guid.Empty || !User.IsInRole("Moderator"))
            {
                return View($"Error/{403}");
            }
            try
            {
                await moderatorService.ApproveRecipeAsync(recipeId);
                return RedirectToAction("Index", "Moderator");
            }
            catch (HttpStatusException statusCode)
            {
                return View($"Error/{statusCode}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> All(int page = 1, int pageSize = 4)
        {
            PaginatedList<InfoRecipeViewModel> recipes = await moderatorService.ViewAllRecipesAsync(page, pageSize);
            return View(recipes);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteComment(Guid commentId)
        {
			Guid currentUserId = GetCurrentUserId();
			if (currentUserId == Guid.Empty || !User.IsInRole("Moderator"))
			{
				return View($"Error/{403}");
			}
			try
			{
				await moderatorService.DeleteCommentAsync(commentId);
				return RedirectToAction("Index", "Moderator");
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
