using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using static RecipeShare.Common.ApplicationConstants;
using static RecipeShare.Common.EntityValidationMessages;

namespace RecipeShare.Web.ViewModels.AccountSettingsViewModels
{
	public class AccountInfoViewModel
	{
        [Required]
        public Guid Id { get; set; }
        [Required]
		[StringLength(UserNameMaxLen, MinimumLength = UserNameMinLen, ErrorMessage = ErrorMessageUserName)]
		public string UserName { get; set; } = null!;

		[Required]
		public bool IsMale { get; set; }

		[AllowNull]
		[StringLength(UserBioMax, ErrorMessage = ErrorMessageUserBio)]
		public string AccountBio { get; set; }
	}
}
