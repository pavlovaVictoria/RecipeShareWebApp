using Microsoft.AspNetCore.Mvc;
using RecipeShare.Web.ViewModels.ApplicationUserViewModels;

namespace RecipeShare.Services.Data.Interfaces
{
	public interface IAccountService
	{
		IActionResult Register();
		Task<IActionResult> Register(RegisterViewModel model);

		IActionResult Login();
		Task<IActionResult> Login(LoginViewModel model);

		Task<IActionResult> Logout(Guid userId);
	}
}
