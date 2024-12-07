using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecipeShare.Common.Exceptions;
using RecipeShare.Services.Data.Interfaces;
using RecipeShare.Web.ViewModels.ApplicationUserViewModels;
using System.Security.Claims;

namespace RecipeShare.Web.Areas.Administrator.Controllers
{
    [Area("Administrator")]
    [Authorize(Policy = "CanManageEverything")]
    public class AdministratorController : Controller
    {
        private readonly IAdministratorService administratorService;
        public AdministratorController(IAdministratorService _administratorService)
        {
            administratorService = _administratorService;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> All()
        {
            Guid currentUserId = GetCurrentUserId();
            if (currentUserId == Guid.Empty)
            {
                return View($"Error/{403}");
            }
            List<ViewUserViewModel> users = await administratorService.GetUsersAsync(currentUserId);
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteUser(Guid userId)
        {
            Guid currentUserId = GetCurrentUserId();
            if (currentUserId == Guid.Empty)
            {
                return View($"Error/{403}");
            }
            try
            {
                DeleteUserViewModel model = await administratorService.ModelForDeleteAsync(userId, currentUserId);
                return View(model);
            }
            catch (HttpStatusException statusCode)
            {
                return View($"Error/{statusCode}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid userId)
        {
            Guid currentUserId = GetCurrentUserId();
            if (currentUserId == Guid.Empty)
            {
                return View($"Error/{403}");
            }
            try
            {
                await administratorService.DeleteUserAsync(userId, currentUserId);
                return RedirectToAction("All");
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
