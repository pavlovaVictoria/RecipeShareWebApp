using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RecipeShare.Common.Exceptions;
using RecipeShare.Data;
using RecipeShare.Data.Models;
using RecipeShare.Repositories.Interfaces;
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
        private readonly IAdministratorRepository adminRepository;
        public AdministratorService(RecipeShareDbContext _context, UserManager<ApplicationUser> _userManager, IAdministratorRepository _adminRepository)
        {
            context = _context;
            userManager = _userManager;
            adminRepository = _adminRepository;
        }
        
        public async Task<List<ViewUserViewModel>> GetUsersAsync(Guid adminId)
        {
            return await adminRepository.GetUsersAsync(adminId);
        }

        public async Task DeleteUserAsync(Guid userId, Guid currentUserId)
        {
            ApplicationUser? user = await adminRepository.FindUserAsync(userId, currentUserId);
            if (user == null)
            {
                throw new HttpStatusException(404);
            }
            user.IsDeleted = true;
            await adminRepository.SaveChangesAsync();
        }

        public async Task<DeleteUserViewModel> ModelForDeleteAsync(Guid userId, Guid currentUserId)
        {
            DeleteUserViewModel? model = await adminRepository.ModelForDeleteAsync(userId, currentUserId);
            if (model == null)
            {
                throw new HttpStatusException(404);
            }
            return model;
        }

        public async Task<ChangeRoleViewModel> ModelForChangingRoleAsync(Guid userId, Guid currentUserId)
        {
            ChangeRoleViewModel? model = await adminRepository.ModelForChangingRoleAsync(userId, currentUserId);
            if (model == null)
            {
                throw new HttpStatusException(404);
            }
            model.Roles = await adminRepository.GetRolesAsync(model.RoleName);
            return model;
        }

        public async Task ChangeRoleAsync(Guid userId, Guid roleId, Guid currentUserId, string roleName)
        {
            ApplicationUser? user = await adminRepository.FindUserAsync(userId, currentUserId);
            if (user == null)
            {
                throw new HttpStatusException(404);
            }
            string? newRoleName = await adminRepository.GetNewRoleNameAsync(roleId);
            if (newRoleName == null)
            {
                throw new HttpStatusException(404);
            }
            await userManager.RemoveFromRoleAsync(user, roleName);
            await userManager.AddToRoleAsync(user, newRoleName);
            await adminRepository.SaveChangesAsync();
        }
    }
}
