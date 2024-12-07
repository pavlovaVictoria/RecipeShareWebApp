using Microsoft.AspNetCore.Identity;
using RecipeShare.Data.Models;
using RecipeShare.Data;
using RecipeShare.Services.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


    }
}
