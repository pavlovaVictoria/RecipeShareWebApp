using RecipeShare.Data.Models;

namespace RecipeShare.Repositories.Interfaces
{
    public interface ICommentRepository
    {
        Task SaveChangesAsync();
        Task<Recipe?> FindRecipeAsync(Guid recipeId);
        Task AddCommentAsync(Comment comment);
        Task<Comment?> FindCommentForDeletingAsync(Guid commentId, Guid currentUserId);
        Task<bool> IfCommentAnyAsync(Guid commentId);
        Task<Comment?> FindCommentAsync(Guid commentId);
    }
}
