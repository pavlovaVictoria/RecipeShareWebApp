using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using RecipeShare.Web.ViewModels.ApplicationUserViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeShare.Services.Data.Interfaces
{
    public interface IAdministratorService
    {
        Task<List<ViewUserViewModel>> GetUsersAsync(Guid adminId);
        Task DeleteUserAsync(Guid userId, Guid currentUserId);
        Task<DeleteUserViewModel> ModelForDeleteAsync(Guid userId, Guid currentUserId);
    }
}
