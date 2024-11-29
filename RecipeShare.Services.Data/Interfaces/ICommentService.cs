namespace RecipeShare.Services.Data.Interfaces
{
    public interface ICommentService
    {
        Task AddCommentAsync(string text, Guid recipeId, Guid currentUserId);
        Task DeleteCommentAsync(Guid commentId, Guid currentUserId);
        Task AddResponseAsync(string text, Guid recipeId, Guid currentUserId, Guid commentId);
    }
}
