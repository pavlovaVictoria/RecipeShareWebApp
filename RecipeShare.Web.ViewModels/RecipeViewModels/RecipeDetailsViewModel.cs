using RecipeShare.Common.Enums;
using RecipeShare.Data.Models;
using RecipeShare.Web.ViewModels.CommentViewModels;

namespace RecipeShare.Web.ViewModels.RecipeViewModels
{
    public class RecipeDetailsViewModel
    {
        public required Guid Id { get; set; }
        public required string RecipeTitle { get; set; }
        public required string UserName { get; set; }
        public string? Description { get; set; }
        public required string Preparation { get; set; }
        public required int MinutesForPrep { get; set; }
        public required string MealType { get; set; }
        public required string Category { get; set; }
        public required string DateOfRelease { get; set; }
        public List<RecipeProductDetailsViewModel> ProductDetails { get; set; } = new List<RecipeProductDetailsViewModel>();
        public List<CommentViewModel> Comments { get; set; } = new List<CommentViewModel>();
        public List<AllergenViewModel> Allergens { get; set; } = new List<AllergenViewModel>();
        public required int Likes { get; set; }
        public required bool IsLikedByCurrentUser { get; set; }
    }
}
