using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using static RecipeShare.Common.ApplicationConstants;
using static RecipeShare.Common.EntityValidationMessages;

namespace RecipeShare.Data.Models
{
    public class Allergen
    {
        public Allergen()
        {
            Id = Guid.NewGuid();
        }

        [Key]
        [Comment("The Id of the Allergen")]
        public Guid Id { get; set; }

        [Required]
        [Comment("The Name of the Allergen")]
        [StringLength(AllergenNameMaxLen, MinimumLength = AllergenNameMinLen, ErrorMessage = ErrorMessageAllergenName)]
        public string AllergenName { get; set; } = null!;

        [Required]
        [Comment("The Url of the Image of the Allergen")]
        public string AllergenImage { get; set; } = null!;

        [Required]
        [Comment("Shows if the Allergen is deleted or not -> Soft Deleting")]
        public bool IsDeleted { get; set; } = false;

        [Comment("A collection of the Recipes with the given Allergen")]
        public virtual ICollection<RecipeAllergen> AllergensRecipes { get; set; } = new List<RecipeAllergen>();
    }
}
