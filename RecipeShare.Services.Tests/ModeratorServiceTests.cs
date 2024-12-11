using Moq;
using RecipeShare.Common.Exceptions;
using RecipeShare.Data.Models;
using RecipeShare.Repositories.Interfaces;
using RecipeShare.Services.Data;
using RecipeShare.Web.ViewModels.RecipeViewModels;

namespace RecipeShare.Services.Tests
{
    [TestFixture]
    public class ModeratorServiceTests
    {
        private Mock<IModeratorRepository> moderatorRepositoryMock;
        private ModeratorService moderatorService;

        [SetUp]
        public void SetUp()
        {
            moderatorRepositoryMock = new Mock<IModeratorRepository>();
            moderatorService = new ModeratorService(moderatorRepositoryMock.Object);
        }

        [Test]
        public async Task ViewAllUnapprovedRecipesAsync_ShouldReturnPaginatedRecipes()
        {
            // Arrange
            List<InfoRecipeViewModel> unapprovedRecipes = new List<InfoRecipeViewModel>
            {
                new InfoRecipeViewModel { Id = Guid.NewGuid(), RecipeTitle = "Recipe1", DateOfRelease = "2023-12-01", ImageUrl = "img1.png" },
                new InfoRecipeViewModel { Id = Guid.NewGuid(), RecipeTitle = "Recipe2", DateOfRelease = "2023-12-02", ImageUrl = "img2.png" },
                new InfoRecipeViewModel { Id = Guid.NewGuid(), RecipeTitle = "Recipe3", DateOfRelease = "2023-12-03", ImageUrl = "img3.png" }
            };

            moderatorRepositoryMock.Setup(repo => repo.ViewAllUnapprovedRecipesAsync())
                .ReturnsAsync(unapprovedRecipes);

            int page = 1;
            int pageSize = 2;

            // Act
            var result = await moderatorService.ViewAllUnapprovedRecipesAsync(page, pageSize);

            // Assert
            Assert.That(2 == result.Items.Count());
        }

        [Test]
        public async Task ViewAllUnapprovedRecipesAsync_ShouldHandleEmptyList()
        {
            // Arrange
            List<InfoRecipeViewModel> unapprovedRecipes = new List<InfoRecipeViewModel>();

            moderatorRepositoryMock.Setup(repo => repo.ViewAllUnapprovedRecipesAsync())
                .ReturnsAsync(unapprovedRecipes);

            int page = 1;
            int pageSize = 2;

            // Act
            var result = await moderatorService.ViewAllUnapprovedRecipesAsync(page, pageSize);

            // Assert
            Assert.That(0 == result.Items.Count());
            Assert.That(0 == result.Count());
        }

        [Test]
        public async Task RecipeDetailsAsync_ShouldReturnRecipeDetails_WhenRecipeExists()
        {
            // Arrange
            Guid recipeId = Guid.NewGuid();
            Guid userId = Guid.NewGuid();
            RecipeDetailsViewModel expectedRecipeDetails = new RecipeDetailsViewModel
            {
                Id = recipeId,
                RecipeTitle = "Test Recipe",
                Description = "Test Description",
                UserName = "UserName",
                Preparation = "Some info",
                MinutesForPrep = 10,
                MealType = "Vegan",
                Category = "Snack",
                DateOfRelease = "2023-12-01",
                IsLikedByCurrentUser = false,
                Likes = 1
            };

            moderatorRepositoryMock.Setup(repo => repo.ModelForDetailsAsync(recipeId, userId))
                .ReturnsAsync(expectedRecipeDetails);

            // Act
            RecipeDetailsViewModel? result = await moderatorService.RecipeDetailsAsync(recipeId, userId);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(expectedRecipeDetails.Id == result.Id);
            Assert.That(expectedRecipeDetails.RecipeTitle == result.RecipeTitle);
            Assert.That(expectedRecipeDetails.Description == result.Description);
        }

        [Test]
        public async Task RecipeDetailsAsync_ShouldReturnNull_WhenRecipeDoesNotExist()
        {
            // Arrange
            Guid recipeId = Guid.NewGuid();
            Guid userId = Guid.NewGuid();

            moderatorRepositoryMock.Setup(repo => repo.ModelForDetailsAsync(recipeId, userId))
                .ReturnsAsync((RecipeDetailsViewModel?)null);

            // Act
            RecipeDetailsViewModel? result = await moderatorService.RecipeDetailsAsync(recipeId, userId);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task UnapproveRecipeAsync_ShouldMarkRecipeAsDeleted_WhenRecipeExists()
        {
            // Arrange
            Guid recipeId = Guid.NewGuid();
            Recipe recipe = new Recipe
            {
                Id = recipeId,
                IsDeleted = false
            };

            moderatorRepositoryMock.Setup(repo => repo.FindRecipeAsync(recipeId))
                .ReturnsAsync(recipe);

            moderatorRepositoryMock.Setup(repo => repo.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            await moderatorService.UnapproveRecipeAsync(recipeId);

            // Assert
            Assert.IsTrue(recipe.IsDeleted);
            moderatorRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void UnapproveRecipeAsync_RecipeNotFound_Throws404()
        {
            // Arrange
            Guid recipeId = Guid.NewGuid();

            moderatorRepositoryMock.Setup(repo => repo.FindRecipeAsync(recipeId))
                .ReturnsAsync((Recipe?)null);

            // Act & Assert
            HttpStatusException exception = Assert.ThrowsAsync<HttpStatusException>(() => moderatorService.UnapproveRecipeAsync(recipeId));
            Assert.That(404 == exception.StatusCode);
        }

        [Test]
        public async Task ApproveRecipeAsync_ShouldMarkRecipeAsDeleted_WhenRecipeExists()
        {
            // Arrange
            Guid recipeId = Guid.NewGuid();
            Recipe recipe = new Recipe
            {
                Id = recipeId,
                IsApproved = false
            };

            moderatorRepositoryMock.Setup(repo => repo.FindRecipeAsync(recipeId))
                .ReturnsAsync(recipe);

            moderatorRepositoryMock.Setup(repo => repo.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            await moderatorService.ApproveRecipeAsync(recipeId);

            // Assert
            Assert.IsTrue(recipe.IsApproved);
            moderatorRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void ApproveRecipeAsync_RecipeNotFound_Throws404()
        {
            // Arrange
            Guid recipeId = Guid.NewGuid();

            moderatorRepositoryMock.Setup(repo => repo.FindRecipeAsync(recipeId))
                .ReturnsAsync((Recipe?)null);

            // Act & Assert
            HttpStatusException exception = Assert.ThrowsAsync<HttpStatusException>(() => moderatorService.UnapproveRecipeAsync(recipeId));
            Assert.That(404 == exception.StatusCode);
        }

        [Test]
        public async Task ViewAllRecipesAsync_ShouldReturnPaginatedList_WhenRecipesExist()
        {
            // Arrange
            int page = 2;
            int pageSize = 3;
            List<InfoRecipeViewModel> allRecipes = new List<InfoRecipeViewModel>
            {
                new InfoRecipeViewModel { Id = Guid.NewGuid(), RecipeTitle = "Recipe1", DateOfRelease = "2023-12-01", ImageUrl = "img1.png" },
                new InfoRecipeViewModel { Id = Guid.NewGuid(), RecipeTitle = "Recipe2", DateOfRelease = "2023-12-02", ImageUrl = "img2.png" },
                new InfoRecipeViewModel { Id = Guid.NewGuid(), RecipeTitle = "Recipe3", DateOfRelease = "2023-12-03", ImageUrl = "img3.png" },
                new InfoRecipeViewModel { Id = Guid.NewGuid(), RecipeTitle = "Recipe4", DateOfRelease = "2023-12-04", ImageUrl = "img4.png" },
                new InfoRecipeViewModel { Id = Guid.NewGuid(), RecipeTitle = "Recipe5", DateOfRelease = "2023-12-05", ImageUrl = "img5.png" }
            };

            moderatorRepositoryMock.Setup(repo => repo.ViewAllRecipesAsync())
                .ReturnsAsync(allRecipes);

            // Act
            List<InfoRecipeViewModel> result = await moderatorService.ViewAllRecipesAsync(page, pageSize);

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public async Task ViewAllRecipesAsync_ShouldReturnEmptyPaginatedList_WhenNoRecipesExist()
        {
            // Arrange
            List<InfoRecipeViewModel> notFoundRecipes = new List<InfoRecipeViewModel>();

            moderatorRepositoryMock.Setup(repo => repo.ViewAllRecipesAsync())
                .ReturnsAsync(notFoundRecipes);

            // Act
            List<InfoRecipeViewModel> result = await moderatorService.ViewAllRecipesAsync(1, 3);

            // Assert
            Assert.That(0 == result.Count());
        }

        [Test]
        public async Task DeleteCommentAsync_ShouldMarkCommentAsDeleted_WhenCommentExists()
        {
            // Arrange
            Guid commentId = Guid.NewGuid();
            Comment comment = new Comment { Id = commentId, IsDeleted = false };

            moderatorRepositoryMock.Setup(repo => repo.FindCommentAsync(commentId))
                .ReturnsAsync(comment);
            moderatorRepositoryMock.Setup(repo => repo.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            await moderatorService.DeleteCommentAsync(commentId);

            // Assert
            Assert.IsTrue(comment.IsDeleted);
            moderatorRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void DeleteCommentAsync_ShouldThrowHttpStatusException_WhenCommentDoesNotExist()
        {
            // Arrange
            Guid commentId = Guid.NewGuid();

            moderatorRepositoryMock.Setup(repo => repo.FindCommentAsync(commentId))
                .ReturnsAsync((Comment?)null);

            // Act & Assert
            Assert.ThrowsAsync<HttpStatusException>(() => moderatorService.DeleteCommentAsync(commentId));
            moderatorRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Never);
        }
    }
}
