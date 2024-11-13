using Microsoft.EntityFrameworkCore;
using RecipeShare.Common.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using static RecipeShare.Common.ApplicationConstants;

namespace RecipeShare.Data.Models
{
    public class RecipeProductDetails
    {
        [Required]
        [Comment("The Id of the Recipe")]
        public Guid RecipeId { get; set; }

        [Required]
        [Comment("The Recipe")]
        [ForeignKey(nameof(RecipeId))]
        public virtual Recipe Recipe { get; set; } = null!;

        [Required]
        [Comment("The Id of the Product")]
        public Guid ProductId { get; set; }

        [Required]
        [Comment("The Product")]
        [ForeignKey(nameof(ProductId))]
        public virtual Product Product { get; set; } = null!;

        [Required]
        [Comment("Grams, Milliliters, Cups")]
        public UnitType UnitType { get; set; }

        [Required]
        [Comment("The Quantity of the Product needed for the Recipe")]
        [Precision(NumberOfDigits, NumbersAfter)]
        public decimal Quantity { get; set; }
    }
}
