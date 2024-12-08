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
using Microsoft.EntityFrameworkCore;
using RecipeShare.Common.Exceptions;
using Microsoft.Identity.Client;
using RecipeShare.Repositories.Interfaces;

namespace RecipeShare.Services.Data
{
    public class AccountSettingsService : IAccountSettingsService
    {
        private readonly UserManager<ApplicationUser> userManager;
		private readonly IAccountRepository accountRepository;
        public AccountSettingsService(UserManager<ApplicationUser> _userManager, IAccountRepository _accountRepository)
        {
            userManager = _userManager;
			accountRepository = _accountRepository;
        }

		public async Task<AccountInfoViewModel> AccountInfoModelAsync(Guid accountId)
		{
			AccountInfoViewModel? model = await accountRepository.AccountInfoModelAsync(accountId);
			if (model == null)
			{
                if (await accountRepository.IfAccountAnyAsync(accountId))
                {
                    throw new HttpStatusException(404);
                }
                throw new HttpStatusException(403);
            }
			return model;
		}

		public async Task DeleteUserAsync(Guid userId)
		{
            ApplicationUser? user = await accountRepository.FindUserAsync(userId);
            if (user == null)
            {
                if (await accountRepository.IfAccountAnyAsync(userId))
                {
                    throw new HttpStatusException(404);
                }
                throw new HttpStatusException(403);
            }
            user.IsDeleted = true;
            await accountRepository.SaveChangesAsync();
        }
		public async Task<DeleteUserViewModel> ModelForDeleteUserAsunc(Guid accountId)
		{
			DeleteUserViewModel? model = await accountRepository.ModelForDeleteUserAsunc(accountId);
			if (model == null)
			{
                if (await accountRepository.IfAccountAnyAsync(accountId))
                {
                    throw new HttpStatusException(404);
                }
                throw new HttpStatusException(403);
            }
			return model;
		}

		public async Task SaveAccountInfoAsync(AccountInfoViewModel model, Guid userId)
		{
			ApplicationUser? user = await accountRepository.FindUserAsync(userId);
			if (user == null)
			{
                if (await accountRepository.IfAccountAnyAsync(userId))
                {
                    throw new HttpStatusException(404);
                }
                throw new HttpStatusException(403);
            }
			user.UserName = model.UserName;
			user.AccountBio = model.AccountBio;
			user.IsMale = model.IsMale;
			await accountRepository.SaveChangesAsync();
		}
	}
}
