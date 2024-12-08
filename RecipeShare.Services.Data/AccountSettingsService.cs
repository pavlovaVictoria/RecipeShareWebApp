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

		public async Task<AccountInfoViewModel> AccountInfoModelAsync(Guid accountId)
		{
			AccountInfoViewModel? model = await context.Users
				.Where(u => u.IsDeleted == false && u.Id == accountId)
				.Select(u => new AccountInfoViewModel
				{
					Id = accountId,
					UserName = u.UserName ?? "",
					AccountBio = u.AccountBio,
					IsMale = u.IsMale
				})
				.FirstOrDefaultAsync();
			if (model == null)
			{
                if (await context.Users.AnyAsync(u => u.Id == accountId))
                {
                    throw new HttpStatusException(404);
                }
                throw new HttpStatusException(403);
            }
			return model;
		}

		public async Task DeleteUserAsync(Guid userId)
		{
            ApplicationUser? user = await context.Users
                .Where(u => u.Id == userId && u.IsDeleted == false)
                .FirstOrDefaultAsync();
            if (user == null)
            {
                if (await context.Users.AnyAsync(u => u.Id == userId))
                {
                    throw new HttpStatusException(404);
                }
                throw new HttpStatusException(403);
            }
            user.IsDeleted = true;
            await context.SaveChangesAsync();
        }
		public async Task<DeleteUserViewModel> ModelForDeleteUserAsunc(Guid accountId)
		{
			DeleteUserViewModel? model = await context.Users
				.Where(u => u.Id == accountId && u.IsDeleted == false)
				.Select(u => new DeleteUserViewModel 
				{ 
					Id = accountId,
					UserName = u.UserName ?? ""
				})
				.FirstOrDefaultAsync();
			if (model == null)
			{
                if (await context.Users.AnyAsync(u => u.Id == accountId))
                {
                    throw new HttpStatusException(404);
                }
                throw new HttpStatusException(403);
            }
			return model;
		}

		public async Task SaveAccountInfoAsync(AccountInfoViewModel model)
		{
			ApplicationUser? user = await context.Users
				.Where(u => u.Id == model.Id && u.IsDeleted == false)
				.FirstOrDefaultAsync();
			if (user == null)
			{
                if (await context.Users.AnyAsync(u => u.Id == model.Id))
                {
                    throw new HttpStatusException(404);
                }
                throw new HttpStatusException(403);
            }
			user.UserName = model.UserName;
			user.AccountBio = model.AccountBio;
			user.IsMale = model.IsMale;
			await context.SaveChangesAsync();
		}
	}
}
