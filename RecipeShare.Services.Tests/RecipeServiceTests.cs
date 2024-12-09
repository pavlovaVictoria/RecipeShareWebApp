using Microsoft.AspNetCore.Identity;
using Moq;
using RecipeShare.Data.Models;
using RecipeShare.Repositories.Interfaces;
using RecipeShare.Services.Data;
using RecipeShare.Services.Data.Interfaces;
using RecipeShare.Web.ViewModels.CommentViewModels;
using RecipeShare.Web.ViewModels.RecipeViewModels;

namespace RecipeShare.Services.Tests
{
    [TestFixture]
    public class RecipeServiceTests
    {
        private Mock<IRecipeRepository> recipeRepositoryMock;
        private RecipeService recipeService;

        [SetUp]
        public void SetUp()
        {
            recipeRepositoryMock = new Mock<IRecipeRepository>();
            recipeService = new RecipeService(recipeRepositoryMock.Object);
        }

        [Test]
        public async Task IndexPageOfRecipesAsync_ReturnsRecipesAndCategories()
        {
            //Arrange
            List<InfoRecipeViewModel> recipes = new List<InfoRecipeViewModel>
            {
                new InfoRecipeViewModel { Id = Guid.NewGuid(), RecipeTitle = "Recipe 1", Description = "Description 1", DateOfRelease = "2023-12-01", ImageUrl = "image1.png" },
                new InfoRecipeViewModel { Id = Guid.NewGuid(), RecipeTitle = "Recipe 2", Description = "Description 2", DateOfRelease = "2023-12-02", ImageUrl = "image2.png" }
            };
            List<CategoryViewModel> categories = new List<CategoryViewModel>
            {
                new CategoryViewModel { Id = Guid.NewGuid(), CategoryName = "Category 1"},
                new CategoryViewModel { Id = Guid.NewGuid(), CategoryName = "Category 2"}
            };
            recipeRepositoryMock.Setup(repo => repo.GetTop3RecipesAsync()).ReturnsAsync(recipes);
            recipeRepositoryMock.Setup(repo => repo.GetAllCategoriesAsync()).ReturnsAsync(categories);
            //Act
            RecipesIndexViewModel result = await recipeService.IndexPageOfRecipesAsync();
            //Assert
            Assert.IsNotNull(result);
            Assert.That(2 == result.Recipes.Count);
            Assert.That(2 == result.Categories.Count);
            recipeRepositoryMock.Verify(repo => repo.GetTop3RecipesAsync(), Times.Once);
            recipeRepositoryMock.Verify(repo => repo.GetAllCategoriesAsync(), Times.Once);
        }

        [Test]
        public async Task IndexPageOfRecipesAsync_ReturnsNullWhenNullRecipesAndCategories()
        {
            //Arrange
            recipeRepositoryMock.Setup(repo => repo.GetTop3RecipesAsync()).ReturnsAsync(new List<InfoRecipeViewModel>());
            recipeRepositoryMock.Setup(repo => repo.GetAllCategoriesAsync()).ReturnsAsync(new List<CategoryViewModel>());
            //Act
            RecipesIndexViewModel result = await recipeService.IndexPageOfRecipesAsync();
            //Assert
            Assert.IsEmpty(result.Recipes);
            Assert.IsEmpty(result.Categories);
            recipeRepositoryMock.Verify(repo => repo.GetTop3RecipesAsync(), Times.Once);
            recipeRepositoryMock.Verify(repo => repo.GetAllCategoriesAsync(), Times.Once);
        }

        [Test]
        public async Task RecipeDetailsAsync_ReturnRecipeDetails()
        {
            //Arrange
            Guid recipeId = Guid.NewGuid();
            Guid userId = Guid.NewGuid();
            recipeRepositoryMock
            .Setup(repo => repo.RecipeDetailsAsync(recipeId, userId))
            .ReturnsAsync(new RecipeDetailsViewModel
            {
                Id = recipeId,
                RecipeTitle = "Test Recipe",
                Description = "Test Description",
                UserName = "Test User",
                Preparation = "Test Preparation",
                MinutesForPrep = 10,
                MealType = "Main Course",
                Category = "Test Category",
                DateOfRelease = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                Comments = new List<CommentViewModel>(),
                Allergens = new List<AllergenViewModel>
                {
                    new AllergenViewModel
                    {
                        AllergenImage = "image.png",
                        AllergenName = "Peanuts"
                    }
                },
                Likes = 5,
                IsLikedByCurrentUser = true,
                ProductDetails = new List<RecipeProductDetailsViewModel>
                {
                    new RecipeProductDetailsViewModel
                    {
                        ProductName = "Product1",
                        Quantity = 2,
                        UnitType = "kg"
                    }
                }
            });
            //Act
            RecipeDetailsViewModel? result = await recipeService.RecipeDetailsAsync(recipeId, userId);
            //Assert
            Assert.NotNull(result);
            Assert.That("Test Recipe" == result.RecipeTitle);
            Assert.That("Test Description" == result.Description);
            Assert.That("Peanuts" == result.Allergens[0].AllergenName);
            Assert.That(1 == result.ProductDetails.Count);
            Assert.That(5 == result.Likes); 
            Assert.IsTrue(result.IsLikedByCurrentUser);
        }

        [Test]
        public async Task RecipeDetailsAsync_ReturnNullRecipeDetails()
        {
            //Arrange
            Guid recipeId = Guid.NewGuid();
            Guid userId = Guid.NewGuid();
            recipeRepositoryMock
            .Setup(repo => repo.RecipeDetailsAsync(recipeId, userId))
            .ReturnsAsync((RecipeDetailsViewModel?)null);
            //Act
            RecipeDetailsViewModel? result = await recipeService.RecipeDetailsAsync(recipeId, userId);
            //Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task RcipesByCategoryAsync_ReturnRecipesByCategory()
        {
            //Arrange
            Guid categoryId = Guid.NewGuid();
            recipeRepositoryMock.Setup(repo => repo.IsCategoryValidAsync(categoryId)).ReturnsAsync(true);

            //Act
            //Assert
        }

    }
}

//Arrange
//Act
//Assert
