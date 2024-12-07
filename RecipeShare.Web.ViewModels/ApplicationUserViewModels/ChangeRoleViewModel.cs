using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeShare.Web.ViewModels.ApplicationUserViewModels
{
    public class ChangeRoleViewModel
    {
        public required Guid UserId { get; set; }
        public required string Username { get; set; }
        public required string RoleName { get; set; }
        public List<RoleViewModel> Roles { get; set; } = new List<RoleViewModel>();
    }
}
