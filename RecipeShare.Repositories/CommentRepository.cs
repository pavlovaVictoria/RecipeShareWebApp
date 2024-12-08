using Microsoft.EntityFrameworkCore;
using RecipeShare.Data;
using RecipeShare.Data.Models;
using RecipeShare.Repositories.Interfaces;
using System.ComponentModel.Design;

namespace RecipeShare.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly RecipeShareDbContext context;
        public CommentRepository(RecipeShareDbContext _context)
        {
            context = _context;
        }
        
        public async Task<Recipe?> FindRecipeAsync(Guid recipeId)
        {
            Recipe? recipe = await context.Recipes
                .Where(r => r.Id == recipeId && r.IsDeleted == false && r.IsArchived == false && r.IsApproved)
                .FirstOrDefaultAsync();
            return recipe;
        }
        public async Task AddCommentAsync(Comment comment)
        {
            await context.Comments.AddAsync(comment);
        }
        public async Task<Comment?> FindCommentForDeletingAsync(Guid commentId, Guid currentUserId)
        {
            Comment? comment = await context.Comments
                .Where(c => c.IsDeleted == false && c.Id == commentId && (c.UserId == currentUserId || c.Recipe.UserId == currentUserId))
                .FirstOrDefaultAsync();
            return comment;
        }
        public async Task<bool> IfCommentAnyAsync(Guid commentId)
        {
            return await context.Comments
                .AnyAsync(c => c.IsDeleted == false && c.Id == commentId);
        }
        public async Task<Comment?> FindCommentAsync(Guid commentId)
        {
            Comment? comment = await context.Comments
                .Where(c => c.Id == commentId && c.IsDeleted == false)
                .FirstOrDefaultAsync();
            return comment;
        }
        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}
