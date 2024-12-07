using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeShare.Web.ViewModels.ApplicationUserViewModels
{
    public class RoleViewModel
    {
        public required Guid RoleId { get; set; }
        public required string RoleName { get; set; }
    }
}
