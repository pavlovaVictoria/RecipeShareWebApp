using Microsoft.EntityFrameworkCore;
using RecipeShare.Common.Exceptions;
using RecipeShare.Data;
using RecipeShare.Data.Models;
using RecipeShare.Services.Data.Interfaces;
using System.Data;

namespace RecipeShare.Services.Data
{
    public class CommentService : ICommentService
    {
        private readonly RecipeShareDbContext context;

        public CommentService(RecipeShareDbContext _context)
        {
            context = _context;
        }

        public async Task AddCommentAsync(string text, Guid recipeId, Guid currentUserId)
        {
            Recipe? recipe = await context.Recipes
                .Where(r => r.Id == recipeId && r.IsDeleted == false && r.IsArchived == false && r.IsApproved)
                .FirstOrDefaultAsync();
            if (recipe == null)
            {
                throw new HttpStatusException(404);
            }
            Comment comment = new Comment()
            {
                Text = text,
                DateOfRelease = DateTime.UtcNow,
                RecipeId = recipeId,
                UserId = currentUserId
            };
            await context.Comments.AddAsync(comment);
            await context.SaveChangesAsync();
        }
    }
}
