using Microsoft.EntityFrameworkCore;
using RecipeShare.Data;
using RecipeShare.Data.Models;
using RecipeShare.Repositories.Interfaces;
using RecipeShare.Web.ViewModels.AccountSettingsViewModels;
using RecipeShare.Web.ViewModels.ApplicationUserViewModels;

namespace RecipeShare.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly RecipeShareDbContext context;
        public AccountRepository(RecipeShareDbContext _context)
        {
            context = _context;
        }
        
        public async Task<AccountInfoViewModel?> AccountInfoModelAsync(Guid accountId)
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
            return model;
        }
        public async Task<bool> IfAccountAnyAsync(Guid accountId)
        {
            return await context.Users.AnyAsync(u => u.Id == accountId);
        }
        public async Task<ApplicationUser?> FindUserAsync(Guid userId)
        {
            ApplicationUser? user = await context.Users
                .Where(u => u.Id == userId && u.IsDeleted == false)
                .FirstOrDefaultAsync();
            return user;
        }
        public async Task<DeleteUserViewModel?> ModelForDeleteUserAsunc(Guid accountId)
        {
            DeleteUserViewModel? model = await context.Users
                .Where(u => u.Id == accountId && u.IsDeleted == false)
                .Select(u => new DeleteUserViewModel
                {
                    Id = accountId,
                    UserName = u.UserName ?? ""
                })
                .FirstOrDefaultAsync();
            return model;
        }
        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}
