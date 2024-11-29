using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeShare.Common.Exceptions;
using RecipeShare.Data.Models;
using RecipeShare.Services.Data.Interfaces;
using System.Security.Claims;

namespace RecipeShare.Web.Controllers
{
    [Authorize]
    public class CommentController : Controller
    {
        private readonly ICommentService commentService;

        public CommentController(ICommentService _commentService)
        {
            commentService = _commentService;
        }

        [HttpPost]
        public async Task<IActionResult> Add(Guid recipeId, string text)
        {
            Guid currentUserId = GetCurrentUserId();
            if (currentUserId == Guid.Empty)
            {
                return View($"Error/{403}");
            }
            try
            {
                await commentService.AddCommentAsync(text, recipeId, currentUserId);
                return RedirectToAction("Details", "Recipe", new { recipeId = recipeId });
            }
            catch (HttpStatusException statusCode)
            {
                return View($"Error/{statusCode}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid recipeId, Guid commentId)
        {
            Guid currentUserId = GetCurrentUserId();
            if (currentUserId == Guid.Empty)
            {
                return View($"Error/{403}");
            }
            try
            {
                await commentService.DeleteCommentAsync(commentId, currentUserId);
                return RedirectToAction("Details", "Recipe", new { recipeId = recipeId });
            }
            catch (HttpStatusException statusCode)
            {
                return View($"Error/{statusCode}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddResponse(string text, Guid recipeId, Guid commentId)
        {
            Guid currentUserId = GetCurrentUserId();
            if (currentUserId == Guid.Empty)
            {
                return View($"Error/{403}");
            }
            try
            {
                await commentService.AddResponseAsync(text, recipeId, currentUserId, commentId);
                return RedirectToAction("Details", "Recipe", new { recipeId = recipeId });
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
