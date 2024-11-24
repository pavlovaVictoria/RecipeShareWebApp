namespace RecipeShare.Web.ViewModels.RecipeViewModels
{
    public class InfoRecipeViewModel
    {
        public required Guid Id { get; set; }
        public required string RecipeTitle { get; set; }
        public string? Description { get; set; }
        public required string DateOfRelease { get; set; }
        public required string ImageUrl { get; set; }
    }
}
