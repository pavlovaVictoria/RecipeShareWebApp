namespace RecipeShare.Web.ViewModels.ApplicationUserViewModels
{
    public class ViewUserViewModel
    {
        public required Guid Id { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required string RoleName { get; set; }
    }
}
