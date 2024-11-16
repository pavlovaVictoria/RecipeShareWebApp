using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RecipeShare.Data.Models;
using RecipeShare.Services.Data.Interfaces;
using RecipeShare.Web.ViewModels.ApplicationUserViewModels;
using System.Transactions;

namespace RecipeShare.Services.Data
{
	public class AccountService : IAccountService
	{
		private readonly SignInManager<ApplicationUser> signInManager;
		private readonly UserManager<ApplicationUser> userManager;

        public AccountService(SignInManager<ApplicationUser> _signInManager, UserManager<ApplicationUser> _userManager)
        {
            signInManager = _signInManager;
			userManager = _userManager;
        }

        public async Task<SignInResult> LoginAsync(LoginViewModel model)
		{
			SignInResult result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
			return result;
		}

		public async Task LogoutAsync()
		{
			await signInManager.SignOutAsync();
		}

		public async Task<IdentityResult> RegisterAsync(RegisterViewModel model)
		{
			ApplicationUser user = new ApplicationUser
			{
				IsMale = model.IsMale,
				Email = model.Email,
				UserName = model.UserName,
				AccountBio = model.AccountBio,
				NormalizedUserName = model.UserName.ToLower()
			};

			IdentityResult result = await userManager.CreateAsync(user, model.Password);

			return result;
		}
	}
}
