using Microsoft.EntityFrameworkCore;
using RecipeShare.Common.Exceptions;
using RecipeShare.Data;
using RecipeShare.Data.Migrations;
using RecipeShare.Data.Models;
using RecipeShare.Repositories.Interfaces;
using RecipeShare.Services.Data.Interfaces;
using System.Data;

namespace RecipeShare.Services.Data
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository commentRepository;

        public CommentService(ICommentRepository _commentRepository)
        {
            commentRepository = _commentRepository;
        }

        public async Task AddCommentAsync(string text, Guid recipeId, Guid currentUserId)
        {
            Recipe? recipe = await commentRepository.FindRecipeAsync(recipeId);
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
            await commentRepository.AddCommentAsync(comment);
            await commentRepository.SaveChangesAsync();
        }

        public async Task DeleteCommentAsync(Guid commentId, Guid currentUserId)
        {
            Comment? comment = await commentRepository.FindCommentForDeletingAsync(commentId, currentUserId);
            if (comment == null)
            {
                if (await commentRepository.IfCommentAnyAsync(commentId))
                {
                    throw new HttpStatusException(403);
                }
                throw new HttpStatusException(404);
            }
            comment.IsDeleted = true;
            await commentRepository.SaveChangesAsync();
        }

        public async Task AddResponseAsync(string text, Guid recipeId, Guid currentUserId, Guid commentId)
        {
            Recipe? recipe = await commentRepository.FindRecipeAsync(recipeId);
            if (recipe == null)
            {
                throw new HttpStatusException(404);
            }
            Comment? comment = await commentRepository.FindCommentAsync(commentId);
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
            await commentRepository.AddCommentAsync(comment);
            await commentRepository.SaveChangesAsync();
        }
    }
}
