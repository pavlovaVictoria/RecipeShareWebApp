using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RecipeShare.Data.Models
{
    public class LikedRecipe
    {
        [Required]
        [Comment("The Id of the User")]
        public Guid UserId { get; set; }

        [Required]
        [Comment("The User")]
        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser User { get; set; } = null!;

        [Required]
        [Comment("The Id of the Recipe")]
        public Guid RecipeId { get; set; }

        [Required]
        [Comment("The Recipe")]
        [ForeignKey(nameof(RecipeId))]
        public virtual Recipe Recipe { get; set; } = null!;
    }
}
