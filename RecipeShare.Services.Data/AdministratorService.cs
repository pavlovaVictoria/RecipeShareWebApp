using RecipeShare.Data;
using RecipeShare.Services.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
