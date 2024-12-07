using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RecipeShare.Common.Exceptions;
using RecipeShare.Data;
using RecipeShare.Data.Models;
using RecipeShare.Services.Data.Interfaces;
using RecipeShare.Web.ViewModels.ApplicationUserViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RecipeShare.Services.Data
{
    public class AdministratorService : IAdministratorService
    {
        private readonly RecipeShareDbContext context;
        private readonly UserManager<ApplicationUser> userManager;
        public AdministratorService(RecipeShareDbContext _context, UserManager<ApplicationUser> _userManager)
        {
            context = _context;
            userManager = _userManager;
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

        public async Task DeleteUserAsync(Guid userId, Guid currentUserId)
        {
            ApplicationUser? user = await context.Users
                .Where(u => u.Id == userId && u.Id != currentUserId && u.IsDeleted == false)
                .FirstOrDefaultAsync();
            if (user == null)
            {
                throw new HttpStatusException(404);
            }
            user.IsDeleted = true;
            await context.SaveChangesAsync();
        }

        public async Task<DeleteUserViewModel> ModelForDeleteAsync(Guid userId, Guid currentUserId)
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
            if (model == null)
            {
                throw new HttpStatusException(404);
            }
            return model;
        }

        public async Task<ChangeRoleViewModel> ModelForChangingRoleAsync(Guid userId, Guid currentUserId)
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
            if (model == null)
            {
                throw new HttpStatusException(404);
            }
            model.Roles = await context.Roles
                .Where(r => r.Name != model.RoleName)
                .Select(r => new RoleViewModel
                { 
                    RoleId = r.Id,
                    RoleName = r.Name ?? "none"
                })
                .ToListAsync();
            return model;
        }

        public async Task ChangeRoleAsync(Guid userId, Guid roleId, Guid currentUserId, string roleName)
        {
            ApplicationUser? user = await context.Users
                .Where(u => u.Id == userId && u.Id != currentUserId && u.IsDeleted == false)
                .FirstOrDefaultAsync();
            if (user == null)
            {
                throw new HttpStatusException(404);
            }
            string? newRoleName = await context.Roles
                .Where(r => r.Id == roleId)
                .Select(r => r.Name)
                .FirstOrDefaultAsync();
            if (newRoleName == null)
            {
                throw new HttpStatusException(404);
            }
            await userManager.RemoveFromRoleAsync(user, roleName);
            await userManager.AddToRoleAsync(user, newRoleName);
            await context.SaveChangesAsync();
        }
    }
}
