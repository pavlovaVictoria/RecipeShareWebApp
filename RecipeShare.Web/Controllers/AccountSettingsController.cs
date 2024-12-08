using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeShare.Common.Exceptions;
using RecipeShare.Services.Data;
using RecipeShare.Services.Data.Interfaces;
using RecipeShare.Web.ViewModels.AccountSettingsViewModels;
using RecipeShare.Web.ViewModels.ApplicationUserViewModels;
using System.Security.Claims;

namespace RecipeShare.Web.Controllers
{
    [Authorize(Policy = "AccountSettings")]
    public class AccountSettingsController : Controller
    {
        private readonly IAccountSettingsService accountSettingsService;
        public AccountSettingsController(IAccountSettingsService _accountSettingsService)
        {
            accountSettingsService = _accountSettingsService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ManageInfo()
        {
			Guid currentUserId = GetCurrentUserId();
			if (currentUserId == Guid.Empty)
			{
				return View($"Error/{403}");
			}
			try
			{
				AccountInfoViewModel model = await accountSettingsService.AccountInfoModelAsync(currentUserId);
				return View(model);
			}
			catch (HttpStatusException statusCode)
			{
				return View($"Error/{statusCode}");
			}
		}

		[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveAccountInfo(AccountInfoViewModel model, Guid userId)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}
            Guid currentUserId = GetCurrentUserId();
            if (currentUserId == Guid.Empty || userId != currentUserId)
            {
                return View($"Error/{403}");
            }
			try
			{
				await accountSettingsService.SaveAccountInfoAsync(model, userId);
				return RedirectToAction("Index");
			}
			catch (HttpStatusException statusCode)
			{
                return View($"Error/{statusCode}");
            }
        }

		[HttpGet]
		public IActionResult ChangePassword()
		{
			return RedirectToAction("ForgotPassword", "Account");
		}

		[HttpGet]
		public async Task<IActionResult> DeleteAccount()
		{
            Guid currentUserId = GetCurrentUserId();
            if (currentUserId == Guid.Empty)
            {
                return View($"Error/{403}");
            }
            try
            {
				DeleteUserViewModel model = await accountSettingsService.ModelForDeleteUserAsunc(currentUserId);
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
                await accountSettingsService.DeleteUserAsync(userId);
                return RedirectToAction("Logout", "Account");
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
