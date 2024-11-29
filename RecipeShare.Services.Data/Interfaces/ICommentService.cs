namespace RecipeShare.Services.Data.Interfaces
{
    public interface ICommentService
    {
        Task AddCommentAsync(string text, Guid recipeId, Guid currentUserId);
    }
}
