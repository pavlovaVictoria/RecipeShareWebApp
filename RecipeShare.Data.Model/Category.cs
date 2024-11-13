using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using static RecipeShare.Common.ApplicationConstants;
using static RecipeShare.Common.EntityValidationMessages;

namespace RecipeShare.Data.Models
{
    public class Category
    {
        public Category()
        {
            Id = Guid.NewGuid();
        }

        [Key]
        [Comment("The Id of the Category")]
        public Guid Id { get; set; }

        [Required]
        [Comment("The Name of the Category")]
        [StringLength(CategoryNameMaxLen, MinimumLength = CategoryNameMinLen, ErrorMessage = ErrorMessageCategoryName)]
        public string CategoryName { get; set; } = null!;

        [Required]
        [Comment("Shows if the Category is deleted or not -> Soft Deleting")]
        public bool IsDeleted { get; set; } = false;

        [Comment("A collection of Recipes with the given Category")]
        public virtual ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
    }
}
