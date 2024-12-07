using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeShare.Services.Data.Interfaces;

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
    }
}
