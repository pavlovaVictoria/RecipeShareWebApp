namespace RecipeShare.Web.ViewModels.RecipeViewModels
{
    public class DeleteRecipeViewModel
    {
        public required Guid Id { get; set; }
        public required string RecipeTitle { get; set; }
    }
}
