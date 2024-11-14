using Microsoft.AspNetCore.Mvc;
using RecipeShare.Services.Data.Interfaces;
using RecipeShare.Web.ViewModels.ApplicationUserViewModels;

namespace RecipeShare.Services.Data
{
	public class AccountService : IAccountService
	{
		public IActionResult Login()
		{
			throw new NotImplementedException();
		}

		public async Task<IActionResult> Login(LoginViewModel model)
		{
			throw new NotImplementedException();
		}

		public async Task<IActionResult> Logout(Guid userId)
		{
			throw new NotImplementedException();
		}

		public IActionResult Register()
		{
			throw new NotImplementedException();
		}

		public async Task<IActionResult> Register(RegisterViewModel model)
		{
			throw new NotImplementedException();
		}
	}
}
