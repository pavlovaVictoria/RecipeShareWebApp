using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RecipeShare.Web.ViewModels.ApplicationUserViewModels
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [DisplayName("Remember me?")]
        public bool RememberMe { get; set; }
    }
}
