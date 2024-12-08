using RecipeShare.Data.Models;
using RecipeShare.Web.ViewModels.AccountSettingsViewModels;
using RecipeShare.Web.ViewModels.ApplicationUserViewModels;

namespace RecipeShare.Repositories.Interfaces
{
    public interface IAccountRepository
    {
        Task SaveChangesAsync();
        Task<AccountInfoViewModel?> AccountInfoModelAsync(Guid accountId);
        Task<bool> IfAccountAnyAsync(Guid accountId);
        Task<ApplicationUser?> FindUserAsync(Guid userId);
        Task<DeleteUserViewModel?> ModelForDeleteUserAsunc(Guid accountId);
    }
}
