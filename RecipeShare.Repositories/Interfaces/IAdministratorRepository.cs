using RecipeShare.Data.Models;
using RecipeShare.Web.ViewModels.ApplicationUserViewModels;

namespace RecipeShare.Repositories.Interfaces
{
    public interface IAdministratorRepository
    {
        Task SaveChangesAsync();
        Task<List<ViewUserViewModel>> GetUsersAsync(Guid adminId);
        Task<DeleteUserViewModel?> ModelForDeleteAsync(Guid userId, Guid currentUserId);
        Task<ApplicationUser?> FindUserAsync(Guid userId, Guid currentUserId);
        Task<ChangeRoleViewModel?> ModelForChangingRoleAsync(Guid userId, Guid currentUserId);
        Task<List<RoleViewModel>> GetRolesAsync(string currentRole);
        Task<string?> GetNewRoleNameAsync(Guid roleId);
    }
}
