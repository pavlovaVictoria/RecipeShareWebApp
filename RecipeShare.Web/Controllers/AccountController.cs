using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using RecipeShare.Services.Data.Interfaces;
using RecipeShare.Web.ViewModels.ApplicationUserViewModels;

namespace RecipeShare.Web.Controllers
{
	public class AccountController : Controller
	{
        private readonly IAccountService accountService;

        public AccountController(IAccountService _accountService)
        {
            accountService = _accountService;
        }

		[HttpGet]
		public IActionResult Login()
		{
			LoginViewModel loginViewModel = new LoginViewModel();
			return View(loginViewModel);
		}

		[HttpPost]
		public async Task<IActionResult> Login(LoginViewModel loginViewModel)
		{
			if (!ModelState.IsValid)
			{
				return View(loginViewModel);
			}
            bool result = await accountService.LoginAsync(loginViewModel);
            if (result)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
				ModelState.AddModelError("", "Email or password is incorrect!");
                return View(loginViewModel);
            }
        }

		[HttpGet]
		public IActionResult Register()
		{
			RegisterViewModel registerViewModel = new RegisterViewModel();
			return View(registerViewModel);
		}

		[HttpPost]
		public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
		{
			if (!ModelState.IsValid)
			{
				return View(registerViewModel);
			}

			IdentityResult result = await accountService.RegisterAsync(registerViewModel);

			if (result.Succeeded)
			{
				return RedirectToAction("Login");
			}
			else
			{
				return View(registerViewModel);
			}
		}

		public async Task<IActionResult> Logout()
		{
			await accountService.LogoutAsync();
			return RedirectToAction("Index", "Home");
		}

		[HttpGet]
		public IActionResult ForgotPassword()
		{
			ChangePasswordViewModel changePasswordViewModel = new ChangePasswordViewModel();
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> ForgotPassword(ChangePasswordViewModel changePasswordViewModel)
		{
			if (!ModelState.IsValid)
			{
				return View(changePasswordViewModel);
			}
			bool result = await accountService.ForgotPasswordAsync(changePasswordViewModel);
			if (result)
			{
				return RedirectToAction("Login");
			}
			else
			{
				return View(changePasswordViewModel);
			}
		}
    }
}
