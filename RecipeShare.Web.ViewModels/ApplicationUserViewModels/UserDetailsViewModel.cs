using RecipeShare.Web.ViewModels.RecipeViewModels;

namespace RecipeShare.Web.ViewModels.ApplicationUserViewModels
{
    public class UserDetailsViewModel
    {
        public required string Username { get; set; }
        public required bool IsMale { get; set; }
        public required string AccountBio {  get; set; }
        public required int Friends {  get; set; }
        public List<InfoRecipeViewModel> MadeRecipes = new List<InfoRecipeViewModel>();
    }
}
