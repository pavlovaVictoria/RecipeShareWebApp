using Moq;
using RecipeShare.Common.Exceptions;
using RecipeShare.Data.Models;
using RecipeShare.Repositories.Interfaces;
using RecipeShare.Services.Data;

namespace RecipeShare.Services.Tests
{
    [TestFixture]
    public class CommentServiceTests
    {
        private Mock<ICommentRepository> commentRepositoryMock;
        private CommentService commentService;

        [SetUp]
        public void SetUp()
        {
            commentRepositoryMock = new Mock<ICommentRepository>();
            commentService = new CommentService(commentRepositoryMock.Object);
        }

        [Test]
        public void AddCommentAsync_RecipeDoesNotExist_Throws404()
        {
            // Arrange
            Guid recipeId = Guid.NewGuid();
            Guid userId = Guid.NewGuid();
            string text = "This is a test comment.";

            commentRepositoryMock.Setup(repo => repo.FindRecipeAsync(recipeId))
                .ReturnsAsync((Recipe?)null);

            // Act & Assert
            HttpStatusException ex = Assert.ThrowsAsync<HttpStatusException>(async () =>
                await commentService.AddCommentAsync(text, recipeId, userId));

            Assert.That(404 == ex.StatusCode);
        }

        [Test]
        public async Task AddCommentAsync_RecipeExists_CommentAdded()
        {
            // Arrange
            Guid recipeId = Guid.NewGuid();
            Guid userId = Guid.NewGuid();
            string text = "This is a test comment.";
            Recipe recipe = new Recipe { Id = recipeId, RecipeTitle = "Test Recipe" };

            var comment = new Comment
            {
                Text = text,
                DateOfRelease = DateTime.UtcNow,
                RecipeId = recipeId,
                UserId = userId,
                IsResponse = false
            };

            commentRepositoryMock.Setup(repo => repo.FindRecipeAsync(recipeId))
                .ReturnsAsync(recipe);

            commentRepositoryMock.Setup(repo => repo.AddCommentAsync(It.IsAny<Comment>()))
                .Returns(Task.CompletedTask);
            commentRepositoryMock.Setup(repo => repo.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            await commentService.AddCommentAsync(text, recipeId, userId);

            // Assert
            commentRepositoryMock.Verify(repo => repo.AddCommentAsync(It.Is<Comment>(c =>
                c.Text == text &&
                c.RecipeId == recipeId &&
                c.UserId == userId &&
                c.IsResponse == false
            )), Times.Once);

            commentRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void DeleteCommentAsync_CommentNotFound_Throws404()
        {
            // Arrange
            Guid commentId = Guid.NewGuid();
            Guid userId = Guid.NewGuid();

            commentRepositoryMock.Setup(repo => repo.FindCommentForDeletingAsync(commentId, userId))
                .ReturnsAsync((Comment?)null);

            // Act & Assert
            HttpStatusException ex = Assert.ThrowsAsync<HttpStatusException>(async () =>
                await commentService.DeleteCommentAsync(commentId, userId));

            Assert.That(404 == ex.StatusCode);
        }

        [Test]
        public void DeleteCommentAsync_CommentNotPermitted_Throws403()
        {
            // Arrange
            Guid commentId = Guid.NewGuid();
            Guid userId = Guid.NewGuid();
            Comment comment = new Comment
            {
                Id = commentId,
                UserId = Guid.NewGuid(),
                IsDeleted = false
            };

            commentRepositoryMock.Setup(repo => repo.FindCommentForDeletingAsync(commentId, userId))
                .ReturnsAsync((Comment?)null);

            commentRepositoryMock.Setup(repo => repo.IfCommentAnyAsync(commentId))
                .ReturnsAsync(true);

            // Act & Assert
            HttpStatusException ex = Assert.ThrowsAsync<HttpStatusException>(async () =>
                await commentService.DeleteCommentAsync(commentId, userId));

            Assert.That(403 == ex.StatusCode);
        }

        [Test]
        public async Task DeleteCommentAsync_CommentExistsAndOwnedByUser_CommentMarkedAsDeleted()
        {
            // Arrange
            Guid commentId = Guid.NewGuid();
            Guid userId = Guid.NewGuid();
            Comment comment = new Comment
            {
                Id = commentId,
                UserId = userId,
                IsDeleted = false
            };

            commentRepositoryMock.Setup(repo => repo.FindCommentForDeletingAsync(commentId, userId))
                .ReturnsAsync(comment);

            commentRepositoryMock.Setup(repo => repo.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            await commentService.DeleteCommentAsync(commentId, userId);

            // Assert
            Assert.IsTrue(comment.IsDeleted);

            commentRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void AddResponseAsync_RecipeNotFound_Throws404()
        {
            // Arrange
            Guid recipeId = Guid.NewGuid();
            Guid commentId = Guid.NewGuid();
            Guid currentUserId = Guid.NewGuid();
            string text = "This is a response";

            commentRepositoryMock.Setup(repo => repo.FindRecipeAsync(recipeId))
                                 .ReturnsAsync((Recipe?)null);

            // Act & Assert
            HttpStatusException ex = Assert.ThrowsAsync<HttpStatusException>(async () =>
                await commentService.AddResponseAsync(text, recipeId, currentUserId, commentId));

            // Assert
            Assert.That(404 == ex.StatusCode);
        }

        [Test]
        public void AddResponseAsync_CommentNotFound_Throws404()
        {
            // Arrange
            Guid recipeId = Guid.NewGuid();
            Guid commentId = Guid.NewGuid();
            Guid currentUserId = Guid.NewGuid();
            string text = "This is a response";

            commentRepositoryMock.Setup(repo => repo.FindRecipeAsync(recipeId))
                                 .ReturnsAsync(new Recipe { Id = recipeId });

            commentRepositoryMock.Setup(repo => repo.FindCommentAsync(commentId))
                                 .ReturnsAsync((Comment?)null);

            // Act & Assert
            HttpStatusException ex = Assert.ThrowsAsync<HttpStatusException>(async () =>
                await commentService.AddResponseAsync(text, recipeId, currentUserId, commentId));

            // Assert
            Assert.That(404 == ex.StatusCode);
        }

        [Test]
        public async Task AddResponseAsync_ValidRecipeAndComment_AddsResponse()
        {
            // Arrange
            var recipeId = Guid.NewGuid();
            var commentId = Guid.NewGuid();
            var currentUserId = Guid.NewGuid();
            var text = "This is a response";

            commentRepositoryMock.Setup(repo => repo.FindRecipeAsync(recipeId))
                                 .ReturnsAsync(new Recipe { Id = recipeId });

            commentRepositoryMock.Setup(repo => repo.FindCommentAsync(commentId))
                                 .ReturnsAsync(new Comment { Id = commentId });

            commentRepositoryMock.Setup(repo => repo.AddCommentAsync(It.IsAny<Comment>()))
                                 .Returns(Task.CompletedTask);

            commentRepositoryMock.Setup(repo => repo.SaveChangesAsync())
                                 .Returns(Task.CompletedTask);

            // Act
            await commentService.AddResponseAsync(text, recipeId, currentUserId, commentId);

            // Assert
            commentRepositoryMock.Verify(repo => repo.AddCommentAsync(It.Is<Comment>(c =>
                c.Text == text &&
                c.RecipeId == recipeId &&
                c.UserId == currentUserId &&
                c.ParentCommentId == commentId &&
                c.IsResponse == true)), Times.Once);

            commentRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }
    }
}
