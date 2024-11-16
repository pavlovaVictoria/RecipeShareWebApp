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

        public async Task<bool> LoginAsync(LoginViewModel model)
		{
			ApplicationUser? user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return false;
            }
            SignInResult result = await signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, false);
			return true;
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
