using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using static RecipeShare.Common.ApplicationConstants;
using static RecipeShare.Common.EntityValidationMessages;

namespace RecipeShare.Data.Models
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public ApplicationUser()
        {
            this.Id = Guid.NewGuid();
        }
        [Required]
        [Comment("Shows if the user is male -> true or female -> false")]
        public bool IsMale { get; set; }

        [AllowNull]
        [Comment("The user's short bio")]
        [StringLength(UserBioMax, ErrorMessage = ErrorMessageUserBio)]
        public string AccountBio { get; set; }

        [Comment("A collection of Recipes made by the given User")]
        public virtual ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();

        [Comment("A collection of the Liked Recipes of the given User")]
        public virtual ICollection<LikedRecipe> LikedRecipes { get; set; } = new List<LikedRecipe>();

        [Comment("A collection of the Friend of the given User")]
        public virtual ICollection<IdentityUser> Friends { get; set; } = new List<IdentityUser>();

        [Comment("A collection of the Coments created by the given User")]
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

        [Comment("A collection of the Archived Recipes of the given User")]
        public virtual ICollection<Recipe> ArchivedRecipes { get; set; } = new List<Recipe>();
    }
}
