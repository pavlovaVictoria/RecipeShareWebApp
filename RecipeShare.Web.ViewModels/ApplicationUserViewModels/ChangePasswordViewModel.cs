using System.ComponentModel.DataAnnotations;
using static RecipeShare.Common.ApplicationConstants;
using static RecipeShare.Common.EntityValidationMessages;

namespace RecipeShare.Web.ViewModels.ApplicationUserViewModels
{
    public class ChangePasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [RegularExpression(RegexForPassword, ErrorMessage = ErrorMessagePassword)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
		[Compare("NewPassword", ErrorMessage = ErrorMessageConfirmedPassword)]
		public string ConfirmedNewPasswored { get; set; } = null!;
    }
}
