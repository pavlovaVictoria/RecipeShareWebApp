using Moq;
using RecipeShare.Repositories.Interfaces;
using RecipeShare.Services.Data;
using RecipeShare.Services.Data.Interfaces;
using RecipeShare.Web.ViewModels.RecipeViewModels;

namespace RecipeShare.Services.Tests
{
    [TestFixture]
    public class HomeServiceTests
    {
        private Mock<IRecipeRepository> recipeRepositoryMock;
        private HomeService homeService;

        [SetUp]
        public void SetUp()
        {
            recipeRepositoryMock = new Mock<IRecipeRepository>();
            homeService = new HomeService(recipeRepositoryMock.Object);
        }

        [Test]
        public async Task Top3RecipesAsync_ReturnsTop3Recipes()
        {
            // Arrange
            List<InfoRecipeViewModel> recipes = new List<InfoRecipeViewModel>
            {
                new InfoRecipeViewModel { Id = Guid.NewGuid(), RecipeTitle = "Recipe 1", Description = "Desc 1", DateOfRelease = "2023-12-01", ImageUrl = "img1.png" },
                new InfoRecipeViewModel { Id = Guid.NewGuid(), RecipeTitle = "Recipe 2", Description = "Desc 2", DateOfRelease = "2023-12-02", ImageUrl = "img2.png" },
                new InfoRecipeViewModel { Id = Guid.NewGuid(), RecipeTitle = "Recipe 3", Description = "Desc 3", DateOfRelease = "2023-12-03", ImageUrl = "img3.png" }
            };

            recipeRepositoryMock
                .Setup(repo => repo.GetTop3RecipesAsync())
                .ReturnsAsync(recipes);

            // Act
            List<InfoRecipeViewModel> result = await homeService.Top3RecipesAsync();

            // Assert
            Assert.That(result.Count, Is.EqualTo(3));
            Assert.That("Recipe 1" == result[0].RecipeTitle);
            Assert.That("Recipe 2" == result[1].RecipeTitle);
            Assert.That("Recipe 3" == result[2].RecipeTitle);
        }

        [Test]
        public async Task Top3RecipesAsync_ReturnsEmptyListWhenNoRecipes()
        {
            // Arrange
            recipeRepositoryMock
                .Setup(repo => repo.GetTop3RecipesAsync())
                .ReturnsAsync(new List<InfoRecipeViewModel>());

            // Act
            var result = await homeService.Top3RecipesAsync();

            // Assert
            Assert.IsEmpty(result);
        }

        [Test]
        public async Task SearchForRecipesAsync_ReturnsMatchingRecipes()
        {
            // Arrange
            string searchInput = "Cake";
            List<InfoRecipeViewModel> recipes = new List<InfoRecipeViewModel>
            {
                new InfoRecipeViewModel { Id = Guid.NewGuid(), RecipeTitle = "Chocolate Cake", Description = "Desc 1", DateOfRelease = "2023-12-01", ImageUrl = "img1.png" },
                new InfoRecipeViewModel { Id = Guid.NewGuid(), RecipeTitle = "Vanilla Cake", Description = "Desc 2", DateOfRelease = "2023-12-02", ImageUrl = "img2.png" }
            };

            recipeRepositoryMock
                .Setup(repo => repo.SearchRecipesAsync(searchInput))
                .ReturnsAsync(recipes);

            // Act
            List<InfoRecipeViewModel> result = await homeService.SearchForRecipesAsync(searchInput);

            // Assert
            Assert.That(2 == result.Count);
            Assert.That("Chocolate Cake" == result[0].RecipeTitle);
            Assert.That("Vanilla Cake" == result[1].RecipeTitle);
        }

        [Test]
        public async Task SearchForRecipesAsync_ReturnsemptyListWhenNoMatches()
        {
            // Arrange
            string searchInput = "Cake";
            
            recipeRepositoryMock
                .Setup(repo => repo.SearchRecipesAsync(searchInput))
                .ReturnsAsync(new List<InfoRecipeViewModel>());

            // Act
            List<InfoRecipeViewModel> result = await homeService.SearchForRecipesAsync(searchInput);

            // Assert
            Assert.IsEmpty(result);
        }
    }
}
