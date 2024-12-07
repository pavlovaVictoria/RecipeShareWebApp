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
        public AdministratorService(RecipeShareDbContext _context)
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
                    Username = u.UserName ?? "without"
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
    }
}
