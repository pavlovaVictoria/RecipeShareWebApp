using Microsoft.AspNetCore.Identity;
using RecipeShare.Data.Models;
using RecipeShare.Data;
using RecipeShare.Services.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecipeShare.Web.ViewModels.AccountSettingsViewModels;
using RecipeShare.Web.ViewModels.ApplicationUserViewModels;

namespace RecipeShare.Services.Data
{
    public class AccountSettingsService : IAccountSettingsService
    {
        private readonly RecipeShareDbContext context;
        private readonly UserManager<ApplicationUser> userManager;
        public AccountSettingsService(RecipeShareDbContext _context, UserManager<ApplicationUser> _userManager)
        {
            context = _context;
            userManager = _userManager;
        }

		public Task<AccountInfoViewModel> AccountInfoModelAsync(Guid accountId)
		{
			throw new NotImplementedException();
		}

		public Task ChangePasswordAsync(ChangePasswordViewModel model, Guid accountId)
		{
			throw new NotImplementedException();
		}

		public Task DeleteUserAsync(DeleteUserViewModel model, Guid userId)
		{
			throw new NotImplementedException();
		}

		public Task<ChangePasswordViewModel> ModelForChangingPassAsync(Guid accountId)
		{
			throw new NotImplementedException();
		}

		public Task<DeleteUserViewModel> ModelForDeleteUserAsunc(Guid accountId)
		{
			throw new NotImplementedException();
		}

		public Task SaveAccountInfoAsync(AccountInfoViewModel model, Guid accountId)
		{
			throw new NotImplementedException();
		}
	}
}
