using Microsoft.EntityFrameworkCore;
using RecipeShare.Data;
using RecipeShare.Data.Models;
using RecipeShare.Repositories.Interfaces;
using RecipeShare.Web.ViewModels.ApplicationUserViewModels;

namespace RecipeShare.Repositories
{
    public class AdministratorRepository : IAdministratorRepository
    {
        private readonly RecipeShareDbContext context;
        public AdministratorRepository(RecipeShareDbContext _context)
        {
            context = _context;
        }

        public async Task<List<ViewUserViewModel>> GetUsersAsync(Guid adminId)
        {
            List<ViewUserViewModel> users = await context.Users
                .Where(u => u.IsDeleted == false && u.Id != adminId)
                .AsNoTracking()
                .Select(u => new ViewUserViewModel
                {
                    Id = u.Id,
                    Email = u.Email ?? "without",
                    Username = u.UserName ?? "without",
                    RoleName = context.UserRoles
                     .Where(ur => ur.UserId == u.Id)
                     .Join(context.Roles,
                         ur => ur.RoleId,
                         r => r.Id,
                         (ur, r) => r.Name)
                     .FirstOrDefault() ?? "none"
                })
                .ToListAsync();
            return users;
        }
        public async Task<DeleteUserViewModel?> ModelForDeleteAsync(Guid userId, Guid currentUserId)
        {
            DeleteUserViewModel? model = await context.Users
                .Where(u => u.Id == userId && u.Id != currentUserId && u.IsDeleted == false)
                .AsNoTracking()
                .Select(u => new DeleteUserViewModel
                {
                    Id = u.Id,
                    UserName = u.UserName ?? "username"
                })
                .FirstOrDefaultAsync();
            return model;
        }
        public async Task<ApplicationUser?> FindUserAsync(Guid userId, Guid currentUserId)
        {
            ApplicationUser? user = await context.Users
                .Where(u => u.Id == userId && u.Id != currentUserId && u.IsDeleted == false)
                .FirstOrDefaultAsync();
            return user;
        }
        public async Task<ChangeRoleViewModel?> ModelForChangingRoleAsync(Guid userId, Guid currentUserId)
        {
            ChangeRoleViewModel? model = await context.Users
                .Where(u => u.Id == userId && u.Id != currentUserId && u.IsDeleted == false)
                .Select(u => new ChangeRoleViewModel
                {
                    UserId = u.Id,
                    Username = u.UserName ?? "username",
                    RoleName = context.UserRoles
                     .Where(ur => ur.UserId == u.Id)
                     .Join(context.Roles,
                         ur => ur.RoleId,
                         r => r.Id,
                         (ur, r) => r.Name)
                     .FirstOrDefault() ?? "none",
                })
                .FirstOrDefaultAsync();
            return model;
        }
        public async Task<List<RoleViewModel>> GetRolesAsync(string currentRole)
        {
            List<RoleViewModel> model = await context.Roles
                .Where(r => r.Name != currentRole)
                .Select(r => new RoleViewModel
                {
                    RoleId = r.Id,
                    RoleName = r.Name ?? "none"
                })
                .ToListAsync();
            return model;
        }
        public async Task<string?> GetNewRoleNameAsync(Guid roleId)
        {
            string? newRoleName = await context.Roles
                .Where(r => r.Id == roleId)
                .Select(r => r.Name)
                .FirstOrDefaultAsync();
            return newRoleName;
        }
        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}
