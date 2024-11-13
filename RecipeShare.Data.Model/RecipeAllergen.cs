using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RecipeShare.Data.Models
{
    public class RecipeAllergen
    {
        [Required]
        [Comment("The Id of the Allergen")]
        public Guid AllergenId { get; set; }

        [Required]
        [Comment("The Allergen")]
        [ForeignKey(nameof(AllergenId))]
        public virtual Allergen Allergen { get; set; } = null!;

        [Required]
        [Comment("The Id of the Recipe")]
        public Guid RecipeId { get; set; }

        [Required]
        [Comment("The Recipe")]
        [ForeignKey(nameof(RecipeId))]
        public virtual Recipe Recipe { get; set; } = null!;
    }
}
