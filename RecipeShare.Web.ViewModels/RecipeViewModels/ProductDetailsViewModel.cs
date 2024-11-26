namespace RecipeShare.Web.ViewModels.RecipeViewModels
{
    public class ProductDetailsViewModel
    {
        public required Guid ProductId { get; set; }
        public required int UnitType { get; set; }
        public required decimal Quantity { get; set; }
    }
}
