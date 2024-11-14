using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using static RecipeShare.Common.ApplicationConstants;
using static RecipeShare.Common.EntityValidationMessages;

namespace RecipeShare.Web.ViewModels.ApplicationUserViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [StringLength(UserNameMaxLen, MinimumLength = UserNameMinLen, ErrorMessage = ErrorMessageUserName)]
        public string UserName { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [RegularExpression(RegexForPassword, ErrorMessage = ErrorMessagePassword)]
        [DataType(DataType.Password)]
        [Compare("ConfirmedPasswored", ErrorMessage = ErrorMessageConfirmedPassword)]
        public string Password { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        public string ConfirmedPasswored { get; set; } = null!;

        [Required]
        public bool IsMale { get; set; }

        [AllowNull]
        [StringLength(UserBioMax, ErrorMessage = ErrorMessageUserBio)]
        public string AccountBio { get; set; }
    }
}
