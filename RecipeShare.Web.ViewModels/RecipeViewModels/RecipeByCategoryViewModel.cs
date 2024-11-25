namespace RecipeShare.Web.ViewModels.RecipeViewModels
{
    public class RecipeByCategoryViewModel
    {
        public required Guid CategoryId { get; set; }
        public required string CategoryName { get; set; }
        public List<InfoRecipeViewModel> Recipes { get; set; } = new List<InfoRecipeViewModel>();
    }
}
