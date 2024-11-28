using Microsoft.EntityFrameworkCore;
using RecipeShare.Common.Enums;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using static RecipeShare.Common.ApplicationConstants;
using static RecipeShare.Common.EntityValidationMessages;

namespace RecipeShare.Web.ViewModels.RecipeViewModels
{
    public class EditRecipeViewModel
    {
        [Required]
        public Guid RecipeId { get; set; }
        
        [Required]
        [Comment("The Title of the Recipe")]
        [StringLength(RecipeTitleMaxLen, MinimumLength = RecipeTitleMinLen, ErrorMessage = ErrorMessageRecipeTitle)]
        public string RecipeTitle { get; set; } = string.Empty;

        [AllowNull]
        [Comment("Short Description of the Recipe")]
        [StringLength(RecipeDescriptionMaxLen, MinimumLength = RecipeDescriptionMinLen, ErrorMessage = ErrorMessageRecipeDescription)]
        public string? Description { get; set; }

        [Required]
        [Comment("The way of Preparation")]
        [StringLength(RecipePreparationMaxLen, MinimumLength = RecipePreparationMinLen, ErrorMessage = ErrorMessageRecipePreparation)]
        public string Preparation { get; set; } = string.Empty;

        [Required]
        [Comment("The Minutes that were needed for the Preparation of the Recipe")]
        [Range(MinutesMin, MinutesMax, ErrorMessage = ErrorMessageMinutes)]
        public int MinutesForPrep { get; set; }

        [Required]
        [Comment("If the meal is vegetarian, vegan or with meat")]
        public int MealType { get; set; }

        [Required]
        public Guid CategoryId { get; set; }

        public List<CategoryViewModel> Categories { get; set; } = new List<CategoryViewModel>();

        [AllowNull]
        [Comment("The Url of the Img that shows the prepared meal")]
        public string? Img { get; set; }

        public List<AllergenForAddAndEditRecipeViewModel> Allergens { get; set; } = new List<AllergenForAddAndEditRecipeViewModel>();

        public List<Guid> SelectedAllergenIds { get; set; } = new List<Guid>();

        public List<ProductDetailsViewModel> ProductsDetails { get; set; } = new List<ProductDetailsViewModel>();

        public Array ProductTypes { get; set; } = Enum.GetValues(typeof(ProductType));

        public List<ProductsViewModel> Products { get; set; } = new List<ProductsViewModel>();
    }
}
