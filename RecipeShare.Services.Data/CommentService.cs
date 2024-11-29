using Microsoft.EntityFrameworkCore;
using RecipeShare.Common.Exceptions;
using RecipeShare.Data;
using RecipeShare.Data.Migrations;
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
                UserId = currentUserId,
                IsResponse = false
            };
            await context.Comments.AddAsync(comment);
            await context.SaveChangesAsync();
        }

        public async Task DeleteCommentAsync(Guid commentId, Guid currentUserId)
        {
            Comment? comment = await context.Comments
                .Where(c => c.IsDeleted == false && c.Id == commentId && (c.UserId == currentUserId || c.Recipe.UserId == currentUserId))
                .FirstOrDefaultAsync();
            if (comment == null)
            {
                if (await context.Comments.AnyAsync(c => c.IsDeleted == false && c.Id == commentId))
                {
                    throw new HttpStatusException(403);
                }
                throw new HttpStatusException(404);
            }
            comment.IsDeleted = true;
            await context.SaveChangesAsync();
        }

        public async Task AddResponseAsync(string text, Guid recipeId, Guid currentUserId, Guid commentId)
        {
            Recipe? recipe = await context.Recipes
                .Where(r => r.Id == recipeId && r.IsDeleted == false && r.IsArchived == false && r.IsApproved)
                .FirstOrDefaultAsync();
            if (recipe == null)
            {
                throw new HttpStatusException(404);
            }
            Comment? comment = await context.Comments
                .Where(c => c.Id == commentId && c.IsDeleted == false)
                .FirstOrDefaultAsync();
            if (comment == null)
            {
                throw new HttpStatusException(404);
            }
            Comment response = new Comment()
            {
                Text = text,
                DateOfRelease = DateTime.UtcNow,
                RecipeId = recipeId,
                UserId = currentUserId,
                ParentCommentId = commentId,
                IsResponse = true
            };
            await context.Comments.AddAsync(response);
            await context.SaveChangesAsync();
        }
    }
}
