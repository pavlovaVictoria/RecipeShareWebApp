using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RecipeShare.Web.ViewModels.ApplicationUserViewModels;

namespace RecipeShare.Services.Data.Interfaces
{
	public interface IAccountService
	{
		Task<IdentityResult> RegisterAsync(RegisterViewModel model);
		Task<SignInResult> LoginAsync(LoginViewModel model);

		Task LogoutAsync();
	}
}
