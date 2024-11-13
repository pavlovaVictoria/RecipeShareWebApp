using Microsoft.EntityFrameworkCore;
using RecipeShare.Common.Enums;
using System.ComponentModel.DataAnnotations;
using static RecipeShare.Common.ApplicationConstants;
using static RecipeShare.Common.EntityValidationMessages;

namespace RecipeShare.Data.Models
{
    public class Product
    {
        public Product()
        {
            Id = Guid.NewGuid();
        }

        [Key]
        [Comment("The Id of the Product")]
        public Guid Id { get; set; }

        [Required]
        [Comment("The Name of the Product")]
        [StringLength(ProductNameMaxLen, MinimumLength = ProductNameMinLen, ErrorMessage = ErrorMessageProductName)]
        public string ProductName { get; set; } = null!;

        [Required]
        [Comment("The Type of the Product (is it vegetable, fruit, dairy...etc)")]
        public ProductType ProductType { get; set; }

        [Required]
        [Comment("The Calorie content of one Product")]
        [Precision(NumberOfDigits, NumbersAfter)]
        public decimal Calories { get; set; }

        [Required]
        [Comment("Shows if the Product is deleted or not -> Soft Deleting")]
        public bool IsDeleted { get; set; } = false;

        [Comment("A collection of the Recipes with the given Product")]
        public virtual ICollection<RecipeProductDetails> RecipesProductsDetails { get; set; } = new List<RecipeProductDetails>();
    }
}
