using Microsoft.EntityFrameworkCore;
using RecipeShare.Common.Custom_Validations;
using RecipeShare.Common.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using static RecipeShare.Common.ApplicationConstants;
using static RecipeShare.Common.EntityValidationMessages;

namespace RecipeShare.Data.Models
{
    public class Recipe
    {
        public Recipe()
        {
            Id = Guid.NewGuid();
        }

        [Key]
        [Comment("The Id of the Recipe")]
        public Guid Id { get; set; }

        [Required]
        [Comment("The Title of the Recipe")]
        [StringLength(RecipeTitleMaxLen, MinimumLength = RecipeTitleMinLen, ErrorMessage = ErrorMessageRecipeTitle)]
        public string RecipeTitle { get; set; } = null!;

        [Required]
        [Comment("The Id of the User")]
        public Guid UserId { get; set; }

        [Required]
        [Comment("The User who has created the Recipe")]
        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser User { get; set; } = null!;

        [AllowNull]
        [Comment("Short Description of the Recipe")]
        [StringLength(RecipeDescriptionMaxLen, MinimumLength = RecipeDescriptionMinLen, ErrorMessage = ErrorMessageRecipeDescription)]
        public string? Description { get; set; }

        [Required]
        [Comment("The way of Preparation")]
        [StringLength(RecipePreparationMaxLen, MinimumLength = RecipePreparationMinLen, ErrorMessage = ErrorMessageRecipePreparation)]
        public string Preparation { get; set; } = null!;

        [Required]
        [Comment("The Minutes that were needed for the Preparation of the Recipe")]
        [MinutesForPrepCustomValidation]
        public int MinutesForPrep { get; set; }

        [Required]
        [Comment("If the meal is vegetarian, vegan or with meat")]
        public MealType MealType { get; set; }

        [Required]
        [Comment("The Id of the Category of the Recipe")]
        public Guid CategoryId { get; set; }

        [Required]
        [Comment("The Category of the Recipe")]
        [ForeignKey(nameof(CategoryId))]
        public virtual Category Category { get; set; } = null!;

        [AllowNull]
        [Comment("The Url of the Img that shows the prepared meal")]
        public string? Img { get; set; }

        [Required]
        [Comment("The Date when the Recipe was created")]
        [RegularExpression(RegexDateTimePattern, ErrorMessage = ErrorMessageDate)]
        public DateTime DateOfRelease { get; set; }

        [Required]
        [Comment("Shows if the Recipe is deleted or not -> Soft Deleting")]
        public bool IsDeleted { get; set; } = false;

        [Required]
        [Comment("If the User don't want to delete a recipe -> has the chance of archive it")]
        public bool IsArchived { get; set; } = false;

        [Comment("A collection of the Allergens of the given Recipe")]
        public virtual ICollection<RecipeAllergen> AllergensRecipes { get; set; } = new List<RecipeAllergen>();

        [Comment("A collection of the Products needed for the given Recipe")]
        public virtual ICollection<RecipeProductDetails> RecipesProductsDetails { get; set; } = new List<RecipeProductDetails>();

        [Comment("A collection of the Comments of the given Recipe")]
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

        [Comment("A collection of users who like the recipe")]
        public virtual ICollection<LikedRecipe> LikedRecipes { get; set; } = new List<LikedRecipe>();
    }
}
