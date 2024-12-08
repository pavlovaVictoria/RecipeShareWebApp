using RecipeShare.Web.ViewModels.AccountSettingsViewModels;
using RecipeShare.Web.ViewModels.ApplicationUserViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeShare.Services.Data.Interfaces
{
    public interface IAccountSettingsService
    {
        Task<AccountInfoViewModel> AccountInfoModelAsync(Guid accountId);
        Task SaveAccountInfoAsync(AccountInfoViewModel model);
        Task<DeleteUserViewModel> ModelForDeleteUserAsunc(Guid accountId);
        Task DeleteUserAsync(Guid userId);
    }
}
