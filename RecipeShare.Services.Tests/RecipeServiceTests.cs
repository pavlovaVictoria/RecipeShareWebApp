using Microsoft.AspNetCore.Identity;
using Moq;
using RecipeShare.Common.Enums;
using RecipeShare.Common.Exceptions;
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
        public async Task RcipesByCategoryAsync_ValidCategory_ReturnRecipesByCategory()
        {
            //Arrange
            Guid categoryId = Guid.NewGuid();
            RecipeByCategoryViewModel model= new RecipeByCategoryViewModel
            {
                CategoryName = "Test Category",
                CategoryId = categoryId,
                Recipes = new List<InfoRecipeViewModel>
                {
                    new InfoRecipeViewModel
                    {
                        Id = Guid.NewGuid(),
                        RecipeTitle = "Recipe 1",
                        Description = "Description 1",
                        DateOfRelease = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                        ImageUrl = "image1.png"
                    },
                    new InfoRecipeViewModel
                    {
                        Id = Guid.NewGuid(),
                        RecipeTitle = "Recipe 2",
                        Description = "Description 2",
                        DateOfRelease = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                        ImageUrl = "image2.png"
                    }
                }
            };
            recipeRepositoryMock.Setup(repo => repo.IsCategoryValidAsync(categoryId)).ReturnsAsync(true);
            recipeRepositoryMock.Setup(repo => repo.RcipesByCategoryAsync(categoryId)).ReturnsAsync(model);
            //Act
            RecipeByCategoryViewModel result = await recipeService.RcipesByCategoryAsync(categoryId);
            //Assert
            Assert.NotNull(result);
            Assert.That(model.CategoryName == result.CategoryName);
            Assert.That(model.Recipes.Count == result.Recipes.Count);
        }

        [Test]
        public void RcipesByCategoryAsync_InvalidCategory()
        {
            //Arrange
            Guid categoryId = Guid.NewGuid();
            recipeRepositoryMock.Setup(repo => repo.IsCategoryValidAsync(categoryId)).ReturnsAsync(false);
            //Act
            //Assert
            HttpStatusException exception = Assert.ThrowsAsync<HttpStatusException>(async () =>
            await recipeService.RcipesByCategoryAsync(categoryId));
            Assert.That(404 == exception.StatusCode);
        }

        [Test]
        public async Task ModelForAddAsync_ReturnsModel()
        {
            //Arrange
            AddRecipeViewModel model = new AddRecipeViewModel()
            {
                Categories = new List<CategoryViewModel>
                {
                    new CategoryViewModel {Id = Guid.NewGuid(), CategoryName = "Category 1"}
                },
                Allergens = new List<AllergenForAddAndEditRecipeViewModel>
                {
                    new AllergenForAddAndEditRecipeViewModel {AllergenId = Guid.NewGuid(), AllergenName = "Allergen 1"}
                },
                Products = new List<ProductsViewModel>
                {
                    new ProductsViewModel {ProductId = Guid.NewGuid(), ProductType = 10, ProductName = "Olive oil", }
                }
            };
            recipeRepositoryMock.Setup(repo => repo.ModelForAddAsync()).ReturnsAsync(model);
            //Act
            AddRecipeViewModel result = await recipeService.ModelForAddAsync();
            //Assert
            Assert.NotNull(result);
            Assert.That(model.Categories.Count == result.Categories.Count);
            Assert.That(model.Allergens.Count == result.Allergens.Count);
            Assert.That(model.Products.Count == result.Products.Count);
            for (int i = 0; i < model.Categories.Count; i++)
            {
                Assert.That(model.Categories[i].CategoryName == result.Categories[i].CategoryName);
            }

            for (int i = 0; i < model.Allergens.Count; i++)
            {
                Assert.That(model.Allergens[i].AllergenName == result.Allergens[i].AllergenName);
            }

            for (int i = 0; i < model.Products.Count; i++)
            {
                Assert.That(model.Products[i].ProductName == result.Products[i].ProductName);
            }
        }

        [Test]
        public async Task TaskModelForAddAsync_ReturnsNull()
        {
            //Arrange
            AddRecipeViewModel model = new AddRecipeViewModel()
            {
                Categories = new List<CategoryViewModel>(),
                Allergens = new List<AllergenForAddAndEditRecipeViewModel>(),
                Products = new List<ProductsViewModel>()
            };
            recipeRepositoryMock.Setup(repo => repo.ModelForAddAsync()).ReturnsAsync(model);
            //Act
            AddRecipeViewModel result = await recipeService.ModelForAddAsync();
            //Assert
            Assert.IsEmpty(result.Categories);
            Assert.IsEmpty(result.Allergens);
            Assert.IsEmpty(result.Products);
        }

        [Test]
        public async Task AddRecipeAsync_AddsNewRecipe()
        {
            //Arrange
            Guid currentUserId = Guid.NewGuid();
            AddRecipeViewModel model = new AddRecipeViewModel
            {
                RecipeTitle = "Test Recipe",
                Description = "Test Description",
                Preparation = "Test Preparation",
                MinutesForPrep = 30,
                MealType = 1,
                CategoryId = Guid.NewGuid(),
                Img = null,
                ProductsDetails = new List<ProductDetailsViewModel>
                {
                    new ProductDetailsViewModel
                    {
                        ProductId = Guid.NewGuid(),
                        Quantity = 2,
                        UnitType = 1
                    }
                },
                SelectedAllergenIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() }
            };

            recipeRepositoryMock
                .Setup(repo => repo.FindAllergenAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid allergenId) => new Allergen { Id = allergenId });

            recipeRepositoryMock
                .Setup(repo => repo.RecipeAllergensAnyAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(false);
            //Act
            await recipeService.AddRecipeAsync(model, currentUserId);
            //Assert
            recipeRepositoryMock.Verify(repo => repo.AddRecipeAsync(It.Is<Recipe>(recipe =>
                recipe.RecipeTitle == model.RecipeTitle &&
                recipe.NormalizedRecipeTitle == model.RecipeTitle.ToUpper() &&
                recipe.UserId == currentUserId &&
                recipe.Description == model.Description &&
                recipe.Preparation == model.Preparation &&
                recipe.MinutesForPrep == model.MinutesForPrep &&
                recipe.MealType == (MealType)model.MealType &&
                recipe.CategoryId == model.CategoryId &&
                recipe.Img == "~/images/recipes/Recipe.png" &&
                recipe.RecipesProductsDetails.Count == model.ProductsDetails.Count &&
                recipe.AllergensRecipes.Count == model.SelectedAllergenIds.Count
            )), Times.Once);
        }

        [Test]
        public async Task AddRecipeAsync_ExistsSuchRecipeAllergen()
        {
            Guid currentUserId = Guid.NewGuid();
            Guid allergenId = Guid.NewGuid();
            AddRecipeViewModel model = new AddRecipeViewModel
            {
                RecipeTitle = "Test Recipe",
                SelectedAllergenIds = new List<Guid> { allergenId }
            };

            recipeRepositoryMock
                .Setup(repo => repo.FindAllergenAsync(allergenId))
                .ReturnsAsync(new Allergen { Id = allergenId });

            recipeRepositoryMock
                .Setup(repo => repo.RecipeAllergensAnyAsync(It.IsAny<Guid>(), allergenId))
                .ReturnsAsync(true);

            // Act
            await recipeService.AddRecipeAsync(model, currentUserId);

            // Assert
            recipeRepositoryMock.Verify(repo => repo.AddRecipeAsync(It.Is<Recipe>(recipe =>
                recipe.AllergensRecipes.Count == 0
            )), Times.Once);
        }

        [Test]
        public async Task AddRecipeAsync_NoAllergensSelected()
        {
            //Arrange
            Guid currentUserId = Guid.NewGuid();
            AddRecipeViewModel model = new AddRecipeViewModel
            {
                RecipeTitle = "Test Recipe",
                Description = "Test Description",
                Preparation = "Test Preparation",
                MinutesForPrep = 30,
                MealType = 1,
                CategoryId = Guid.NewGuid(),
                Img = null,
                ProductsDetails = new List<ProductDetailsViewModel>
                {
                    new ProductDetailsViewModel
                    {
                        ProductId = Guid.NewGuid(),
                        Quantity = 2,
                        UnitType = 1
                    }
                },
                SelectedAllergenIds = new List<Guid>()
            };
            //Act
            await recipeService.AddRecipeAsync(model, currentUserId);
            //Assert
            recipeRepositoryMock.Verify(repo => repo.AddRecipeAsync(It.Is<Recipe>(recipe =>
                recipe.AllergensRecipes.Count == 0
            )), Times.Once);
        }

        [Test]
        public async Task ViewCreatedRecipesAsync_CalledOnce()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            int page = 2;
            int pageSize = 2;

            List<InfoRecipeViewModel> mockData = new List<InfoRecipeViewModel>
            {
                new InfoRecipeViewModel { Id = Guid.NewGuid(), RecipeTitle = "Recipe 1", Description = "Description 1", DateOfRelease = "2023-12-01", ImageUrl = "image1.png" },
                new InfoRecipeViewModel { Id = Guid.NewGuid(), RecipeTitle = "Recipe 2", Description = "Description 2", DateOfRelease = "2023-12-02", ImageUrl = "image2.png" },
                new InfoRecipeViewModel { Id = Guid.NewGuid(), RecipeTitle = "Recipe 3", Description = "Description 3", DateOfRelease = "2023-12-02", ImageUrl = "image3.png" },
                new InfoRecipeViewModel { Id = Guid.NewGuid(), RecipeTitle = "Recipe 4", Description = "Description 4", DateOfRelease = "2023-12-02", ImageUrl = "image4.png" }
            };

            recipeRepositoryMock
                .Setup(repo => repo.ViewCreatedRecipesAsync(userId))
                .ReturnsAsync(mockData);

            // Act
            var result = await recipeService.ViewCreatedRecipesAsync(userId, page, pageSize);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(pageSize == result.PageSize);
            Assert.That(2 == result.PageSize);
            Assert.IsTrue(result.Items.Any(r => r.RecipeTitle == "Recipe 3"));
            Assert.IsTrue(result.Items.Any(r => r.RecipeTitle == "Recipe 4"));
            recipeRepositoryMock.Verify(repo => repo.ViewCreatedRecipesAsync(userId), Times.Once);
        }

        [Test]
        public async Task ViewCreatedRecipesAsync_ReturnsEmpty()
        {
            //Arrange
            Guid userId = Guid.NewGuid();
            int page = 2;
            int pageSize = 2;
            recipeRepositoryMock
                .Setup(repo => repo.ViewCreatedRecipesAsync(userId))
                .ReturnsAsync(new List<InfoRecipeViewModel>());
            //Act
            var result = await recipeService.ViewCreatedRecipesAsync(userId, page, pageSize);
            //Assert
            Assert.IsNotNull(result);
            Assert.IsEmpty(result); // List is empty
            Assert.That(0 == result.Items.Count());
        }

        [Test]
        public async Task ViewLikedRecipesAsync_ColledOnce()
        {
            //Arrange
            Guid userId = Guid.NewGuid();
            int page = 2;
            int pageSize = 2;

            List<InfoRecipeViewModel> mockData = new List<InfoRecipeViewModel>
            {
                new InfoRecipeViewModel { Id = Guid.NewGuid(), RecipeTitle = "Recipe 1", Description = "Description 1", DateOfRelease = "2023-12-01", ImageUrl = "image1.png" },
                new InfoRecipeViewModel { Id = Guid.NewGuid(), RecipeTitle = "Recipe 2", Description = "Description 2", DateOfRelease = "2023-12-02", ImageUrl = "image2.png" },
                new InfoRecipeViewModel { Id = Guid.NewGuid(), RecipeTitle = "Recipe 3", Description = "Description 3", DateOfRelease = "2023-12-02", ImageUrl = "image3.png" },
                new InfoRecipeViewModel { Id = Guid.NewGuid(), RecipeTitle = "Recipe 4", Description = "Description 4", DateOfRelease = "2023-12-02", ImageUrl = "image4.png" }
            };

            recipeRepositoryMock
                .Setup(repo => repo.ViewLikedRecipesAsync(userId))
                .ReturnsAsync(mockData);

            // Act
            var result = await recipeService.ViewLikedRecipesAsync(userId, page, pageSize);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(pageSize == result.PageSize);
            Assert.That(2 == result.PageSize);
            Assert.IsTrue(result.Items.Any(r => r.RecipeTitle == "Recipe 3"));
            Assert.IsTrue(result.Items.Any(r => r.RecipeTitle == "Recipe 4"));
            recipeRepositoryMock.Verify(repo => repo.ViewLikedRecipesAsync(userId), Times.Once);
        }

        [Test]
        public async Task ViewLikedRecipesAsync_ReturnsEmpty()
        {
            //Arrange
            Guid userId = Guid.NewGuid();
            int page = 2;
            int pageSize = 2;
            recipeRepositoryMock
                .Setup(repo => repo.ViewLikedRecipesAsync(userId))
                .ReturnsAsync(new List<InfoRecipeViewModel>());
            //Act
            var result = await recipeService.ViewLikedRecipesAsync(userId, page, pageSize);
            //Assert
            Assert.IsNotNull(result);
            Assert.IsEmpty(result); // List is empty
            Assert.That(0 == result.Items.Count());
        }

        [Test]
        public async Task ModelForEdidAsync_RecipeFound_ReturnsCorrectModel()
        {
            // Arrange
            Guid recipeId = Guid.NewGuid();
            Guid currentUserId = Guid.NewGuid();
            Recipe recipe = new Recipe
            {
                Id = recipeId,
                RecipeTitle = "Test Recipe",
                Description = "Test Description",
                Preparation = "Test Preparation",
                MinutesForPrep = 30,
                MealType = MealType.Vegetarian,
                CategoryId = Guid.NewGuid(),
                Img = "~/images/recipe.jpg"
            };

            List<CategoryViewModel> categories = new List<CategoryViewModel>
            {
                new CategoryViewModel { Id = Guid.NewGuid(), CategoryName = "Dinner" }
            };

            List<AllergenForAddAndEditRecipeViewModel> allergens = new List<AllergenForAddAndEditRecipeViewModel>
            {
                new AllergenForAddAndEditRecipeViewModel { AllergenId = Guid.NewGuid(), AllergenName = "Peanut" }
            };

            List<ProductsViewModel> products = new List<ProductsViewModel>
            {
                new ProductsViewModel { ProductId = Guid.NewGuid(), ProductName = "Product 1", ProductType = 2 }
            };

            recipeRepositoryMock.Setup(repo => repo.FindRecipeAsync(recipeId, currentUserId))
                .ReturnsAsync(recipe);
            recipeRepositoryMock.Setup(repo => repo.GetAllCategoriesAsync())
                .ReturnsAsync(categories);
            recipeRepositoryMock.Setup(repo => repo.GetAllAllergensAsync())
                .ReturnsAsync(allergens);
            recipeRepositoryMock.Setup(repo => repo.GetProductsAsync())
                .ReturnsAsync(products);
            recipeRepositoryMock.Setup(repo => repo.AllergensOfResipeAsync(recipeId))
                .ReturnsAsync(new List<Guid> { allergens[0].AllergenId });
            recipeRepositoryMock.Setup(repo => repo.AlreadySelectedProductsDetailsAsync(recipeId))
                .ReturnsAsync(new List<ProductDetailsViewModel>
                {
                    new ProductDetailsViewModel { ProductId = products[0].ProductId, Quantity = 1, UnitType = 1 }
                });

            // Act
            var result = await recipeService.ModelForEdidAsync(recipeId, currentUserId);

            // Assert
            Assert.That(recipeId == result.RecipeId);
            Assert.That(recipe.RecipeTitle == result.RecipeTitle);
            Assert.That(recipe.Description == result.Description);
            Assert.That(recipe.Preparation == result.Preparation);
            Assert.That(recipe.MinutesForPrep == result.MinutesForPrep);
            Assert.That((int)recipe.MealType == result.MealType);
            Assert.That(recipe.CategoryId == result.CategoryId);
            Assert.That(recipe.Img == result.Img);
            Assert.That(categories.Count == result.Categories.Count);
            Assert.That(allergens.Count == result.Allergens.Count);
            Assert.That(products.Count == result.Products.Count);
        }

        [Test]
        public void ModelForEdidAsync_RecipeNotPermitted_Throws403()
        {
            Guid recipeId = Guid.NewGuid();
            Guid currentUserId = Guid.NewGuid();

            recipeRepositoryMock.Setup(repo => repo.FindRecipeAsync(recipeId, currentUserId))
                .ReturnsAsync((Recipe?)null); // In the repository is allowed to be null
            recipeRepositoryMock.Setup(repo => repo.IfRecipesAnyAsync(recipeId))
                .ReturnsAsync(true);

            // Act & Assert
            HttpStatusException ex = Assert.ThrowsAsync<HttpStatusException>(async () =>
                await recipeService.ModelForEdidAsync(recipeId, currentUserId));
            Assert.That(403 == ex.StatusCode);
        }

        [Test]
        public void ModelForEdidAsync_RecipeNotFound_Throws404()
        {
            Guid recipeId = Guid.NewGuid();
            Guid currentUserId = Guid.NewGuid();

            recipeRepositoryMock.Setup(repo => repo.FindRecipeAsync(recipeId, currentUserId))
                .ReturnsAsync((Recipe?)null); // In the repository is allowed to be null
            recipeRepositoryMock.Setup(repo => repo.IfRecipesAnyAsync(recipeId))
                .ReturnsAsync(false);

            // Act & Assert
            HttpStatusException ex = Assert.ThrowsAsync<HttpStatusException>(async () =>
                await recipeService.ModelForEdidAsync(recipeId, currentUserId));
            Assert.That(404 == ex.StatusCode);
        }

        [Test]
        public void EditRecipeAsync_RecipeNotFound_Throws404()
        {
            // Arrange
            EditRecipeViewModel model = new EditRecipeViewModel
            {
                RecipeTitle = "Updated Recipe",
                Description = "Updated Description",
                Preparation = "Updated Preparation",
                MinutesForPrep = 30,
                MealType = 1,
                CategoryId = Guid.NewGuid(),
                Img = "~/images/recipes/UpdatedImage.png",
                SelectedAllergenIds = new List<Guid> { Guid.NewGuid() },
                ProductsDetails = new List<ProductDetailsViewModel> 
                { new ProductDetailsViewModel() { ProductId = Guid.NewGuid(), Quantity = 1, UnitType = 3} }
            };
            Guid recipeId = Guid.NewGuid();
            Guid currentUserId = Guid.NewGuid();

            recipeRepositoryMock.Setup(repo => repo.FindRecipeAsync(recipeId, currentUserId))
                .ReturnsAsync((Recipe?)null);
            recipeRepositoryMock.Setup(repo => repo.IfRecipesAnyAsync(recipeId))
                .ReturnsAsync(false); 

            // Act & Assert
            HttpStatusException ex = Assert.ThrowsAsync<HttpStatusException>(async () =>
                await recipeService.EditRecipeAsync(model, recipeId, currentUserId));

            Assert.That(404 == ex.StatusCode);  // Expect 404 (Not Found)
        }

        [Test]
        public void EditRecipeAsync_RecipeNotPermitted_Throws403()
        {
            // Arrange
            EditRecipeViewModel model = new EditRecipeViewModel
            {
                RecipeTitle = "Updated Recipe",
                Description = "Updated Description",
                Preparation = "Updated Preparation",
                MinutesForPrep = 30,
                MealType = 1,
                CategoryId = Guid.NewGuid(),
                Img = "~/images/recipes/UpdatedImage.png",
                SelectedAllergenIds = new List<Guid> { Guid.NewGuid() },
                ProductsDetails = new List<ProductDetailsViewModel>
                { new ProductDetailsViewModel() { ProductId = Guid.NewGuid(), Quantity = 1, UnitType = 3} }
            };
            Guid recipeId = Guid.NewGuid();
            Guid currentUserId = Guid.NewGuid();

            recipeRepositoryMock.Setup(repo => repo.FindRecipeAsync(recipeId, currentUserId))
                .ReturnsAsync((Recipe?)null);
            recipeRepositoryMock.Setup(repo => repo.IfRecipesAnyAsync(recipeId))
                .ReturnsAsync(true);

            // Act & Assert
            HttpStatusException ex = Assert.ThrowsAsync<HttpStatusException>(async () =>
                await recipeService.EditRecipeAsync(model, recipeId, currentUserId));

            Assert.That(403 == ex.StatusCode);  // Expect 404 (Not Found)
        }

        [Test]
        public async Task EditRecipeAsync_RecipeFound_UpdatesRecipeSuccessfully()
        {
            // Arrange
            EditRecipeViewModel model = new EditRecipeViewModel
            {
                RecipeTitle = "Updated Recipe",
                Description = "Updated Description",
                Preparation = "Updated Preparation",
                MinutesForPrep = 30,
                MealType = 1,
                CategoryId = Guid.NewGuid(),
                Img = "~/images/recipes/UpdatedImage.png",
                SelectedAllergenIds = new List<Guid> { Guid.NewGuid() },
                ProductsDetails = new List<ProductDetailsViewModel>
                { new ProductDetailsViewModel() { ProductId = Guid.NewGuid(), Quantity = 1, UnitType = 3} }
            };
            Guid recipeId = Guid.NewGuid();
            Guid currentUserId = Guid.NewGuid();
            Recipe existingRecipe = new Recipe
            {
                Id = recipeId,
                RecipeTitle = "Old Recipe",
                Description = "Old Description",
                Preparation = "Old Preparation",
                MinutesForPrep = 20,
                MealType = MealType.Vegan,
                CategoryId = Guid.NewGuid(),
                Img = "~/images/recipes/OldImage.png",
                RecipesProductsDetails = new List<RecipeProductDetails>(),
                AllergensRecipes = new List<RecipeAllergen>()
            };

            recipeRepositoryMock.Setup(repo => repo.FindRecipeAsync(recipeId, currentUserId))
                .ReturnsAsync(existingRecipe);
            recipeRepositoryMock.Setup(repo => repo.RemoveCurrentRecipeAllergensAsync(recipeId))
                .Returns(Task.CompletedTask);
            recipeRepositoryMock.Setup(repo => repo.RemoveCurrentProductDetailsAsync(recipeId))
                .Returns(Task.CompletedTask);
            recipeRepositoryMock.Setup(repo => repo.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            recipeRepositoryMock.Setup(repo => repo.FindAllergenAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new Allergen { Id = Guid.NewGuid(), AllergenName = "Peanut" });
            recipeRepositoryMock.Setup(repo => repo.RecipeAllergensAnyAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(false);
            // Act
            await recipeService.EditRecipeAsync(model, recipeId, currentUserId);

            // Assert
            Assert.That("Updated Recipe" == existingRecipe.RecipeTitle);
            Assert.That("Updated Description" == existingRecipe.Description);
            Assert.That("Updated Preparation" == existingRecipe.Preparation);
            Assert.That(30 == existingRecipe.MinutesForPrep);
            Assert.That((MealType)model.MealType == existingRecipe.MealType);
            Assert.That(model.CategoryId == existingRecipe.CategoryId);
            Assert.That(model.Img == existingRecipe.Img);
            Assert.That(1 == existingRecipe.AllergensRecipes.Count);
            recipeRepositoryMock.Verify(repo => repo.RemoveCurrentRecipeAllergensAsync(recipeId), Times.Once);
            recipeRepositoryMock.Verify(repo => repo.RemoveCurrentProductDetailsAsync(recipeId), Times.Once);
            recipeRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task ModelForDeleteAsync_RecipeFound_ReturnsDeleteRecipeViewModel()
        {
            // Arrange
            Guid recipeId = Guid.NewGuid();
            Guid currentUserId = Guid.NewGuid();
            DeleteRecipeViewModel deleteRecipeViewModel = new DeleteRecipeViewModel
            {
                Id = recipeId,
                RecipeTitle = "Recipe to be deleted"
            };

            recipeRepositoryMock.Setup(repo => repo.ModelForDeleteAsync(recipeId, currentUserId))
                .ReturnsAsync(deleteRecipeViewModel);

            // Act
            var result = await recipeService.ModelForDeleteAsync(recipeId, currentUserId);

            // Assert
            Assert.That(deleteRecipeViewModel == result);
        }

        [Test]
        public void ModelForDeleteAsync_RecipeNotPermitted_Throws403()
        {
            // Arrange
            Guid recipeId = Guid.NewGuid();
            Guid currentUserId = Guid.NewGuid();

            recipeRepositoryMock.Setup(repo => repo.ModelForDeleteAsync(recipeId, currentUserId))
                .ReturnsAsync((DeleteRecipeViewModel?)null);
            recipeRepositoryMock.Setup(repo => repo.IfRecipeForDeleteAnyAsync(recipeId))
                .ReturnsAsync(true);

            // Act & Assert
            HttpStatusException ex = Assert.ThrowsAsync<HttpStatusException>(async () =>
                await recipeService.ModelForDeleteAsync(recipeId, currentUserId));

            Assert.That(403 == ex.StatusCode);  // Expect 403 (Forbidden)
        }

        [Test]
        public void ModelForDeleteAsync_RecipeNotFound_ThrowsNotFoundException()
        {
            // Arrange
            Guid recipeId = Guid.NewGuid();
            Guid currentUserId = Guid.NewGuid();

            recipeRepositoryMock.Setup(repo => repo.ModelForDeleteAsync(recipeId, currentUserId))
                .ReturnsAsync((DeleteRecipeViewModel?)null);
            recipeRepositoryMock.Setup(repo => repo.IfRecipeForDeleteAnyAsync(recipeId))
                .ReturnsAsync(false); 

            // Act & Assert
            var ex = Assert.ThrowsAsync<HttpStatusException>(async () =>
                await recipeService.ModelForDeleteAsync(recipeId, currentUserId));

            Assert.That(404 == ex.StatusCode);
        }

        [Test]
        public async Task DeleteOrArchiveAsync_RecipeFound_DeleteAction_SetsIsDeletedTrue()
        {
            // Arrange
            Guid recipeId = Guid.NewGuid();
            Guid currentUserId = Guid.NewGuid();
            string action = "delete";
            Recipe recipe = new Recipe { Id = recipeId, IsDeleted = false, IsArchived = false };

            recipeRepositoryMock.Setup(repo => repo.FindRecipeAsync(recipeId, currentUserId))
                .ReturnsAsync(recipe);

            recipeRepositoryMock.Setup(repo => repo.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            await recipeService.DeleteOrArchiveAsync(recipeId, currentUserId, action);

            // Assert
            Assert.IsTrue(recipe.IsDeleted);
            Assert.IsFalse(recipe.IsArchived);
            recipeRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task DeleteOrArchiveAsync_RecipeFound_ArchiveAction_SetsIsArchivedTrue()
        {
            // Arrange
            Guid recipeId = Guid.NewGuid();
            Guid currentUserId = Guid.NewGuid();
            string action = "archive";
            Recipe recipe = new Recipe { Id = recipeId, IsDeleted = false, IsArchived = false };

            recipeRepositoryMock.Setup(repo => repo.FindRecipeAsync(recipeId, currentUserId))
                .ReturnsAsync(recipe);

            recipeRepositoryMock.Setup(repo => repo.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            await recipeService.DeleteOrArchiveAsync(recipeId, currentUserId, action);

            // Assert
            Assert.IsTrue(recipe.IsArchived);
            Assert.IsFalse(recipe.IsDeleted);
            recipeRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void DeleteOrArchiveAsync_RecipeNotPermitted_Throws403()
        {
            // Arrange
            Guid recipeId = Guid.NewGuid();
            Guid currentUserId = Guid.NewGuid();
            string action = "delete";

            recipeRepositoryMock.Setup(repo => repo.FindRecipeAsync(recipeId, currentUserId))
                .ReturnsAsync((Recipe?)null);
            recipeRepositoryMock.Setup(repo => repo.IfRecipeForDeleteAnyAsync(recipeId))
                .ReturnsAsync(true);

            // Act & Assert
            HttpStatusException ex = Assert.ThrowsAsync<HttpStatusException>(async () =>
                await recipeService.DeleteOrArchiveAsync(recipeId, currentUserId, action));

            Assert.That(403 == ex.StatusCode);
        }

        [Test]
        public void DeleteOrArchiveAsync_RecipeNotFound_Throws404()
        {
            // Arrange
            Guid recipeId = Guid.NewGuid();
            Guid currentUserId = Guid.NewGuid();
            string action = "delete";

            recipeRepositoryMock.Setup(repo => repo.FindRecipeAsync(recipeId, currentUserId))
                .ReturnsAsync((Recipe?)null);
            recipeRepositoryMock.Setup(repo => repo.IfRecipeForDeleteAnyAsync(recipeId))
                .ReturnsAsync(false);

            // Act & Assert
            HttpStatusException ex = Assert.ThrowsAsync<HttpStatusException>(async () =>
                await recipeService.DeleteOrArchiveAsync(recipeId, currentUserId, action));

            Assert.That(404 == ex.StatusCode);
        }

        [Test]
        public void UnarchiveRecipeAsync_RecipeNotPermitted_Throws403()
        {
            // Arrange
            Guid recipeId = Guid.NewGuid();
            Guid currentUserId = Guid.NewGuid();

            recipeRepositoryMock.Setup(repo => repo.FindRecipeAsync(recipeId, currentUserId))
                .ReturnsAsync((Recipe?)null);
            recipeRepositoryMock.Setup(repo => repo.IfRecipeForDeleteAnyAsync(recipeId))
                .ReturnsAsync(true);

            // Act & Assert
            HttpStatusException ex = Assert.ThrowsAsync<HttpStatusException>(async () =>
                await recipeService.UnarchiveRecipeAsync(recipeId, currentUserId));

            Assert.That(403 == ex.StatusCode);
        }

        [Test]

        public void UnarchiveRecipeAsync_RecipeNotFound_Throws404()
        {
            Guid recipeId = Guid.NewGuid();
            Guid currentUserId = Guid.NewGuid();

            recipeRepositoryMock.Setup(repo => repo.FindRecipeAsync(recipeId, currentUserId))
                .ReturnsAsync((Recipe?)null);
            recipeRepositoryMock.Setup(repo => repo.IfRecipeForDeleteAnyAsync(recipeId))
                .ReturnsAsync(false);

            // Act & Assert
            HttpStatusException ex = Assert.ThrowsAsync<HttpStatusException>(async () =>
                await recipeService.UnarchiveRecipeAsync(recipeId, currentUserId));

            Assert.That(404 == ex.StatusCode);
        }

        [Test]
        public async Task UnarchiveRecipeAsync_RecipeFound_RecipeIsArchived_UnarchivesRecipe()
        {
            // Arrange
            Guid recipeId = Guid.NewGuid();
            Guid currentUserId = Guid.NewGuid();
            Recipe recipe = new Recipe { Id = recipeId, IsArchived = true };

            recipeRepositoryMock.Setup(repo => repo.FindArchivedRecipeAsync(recipeId, currentUserId))
                .ReturnsAsync(recipe);

            recipeRepositoryMock.Setup(repo => repo.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            await recipeService.UnarchiveRecipeAsync(recipeId, currentUserId);

            // Assert
            Assert.IsFalse(recipe.IsArchived); 
            recipeRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task UnarchiveRecipeAsync_RecipeFound_RecipeIsNotArchived_NoChangesMade()
        {
            // Arrange
            Guid recipeId = Guid.NewGuid();
            Guid currentUserId = Guid.NewGuid();
            Recipe recipe = new Recipe { Id = recipeId, IsArchived = false };

            recipeRepositoryMock.Setup(repo => repo.FindArchivedRecipeAsync(recipeId, currentUserId))
                .ReturnsAsync(recipe);

            recipeRepositoryMock.Setup(repo => repo.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            await recipeService.UnarchiveRecipeAsync(recipeId, currentUserId);

            // Assert
            Assert.IsFalse(recipe.IsArchived);
            recipeRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task ViewArchivedRecipesAsync_ValidData_ReturnsPaginatedList()
        {
            // Arrange
            Guid currentUserId = Guid.NewGuid();
            int page = 1;
            int pageSize = 2;
            List<InfoRecipeViewModel> mockData = new List<InfoRecipeViewModel>
            {
                new InfoRecipeViewModel { Id = Guid.NewGuid(), RecipeTitle = "Recipe 1", Description = "Description 1", DateOfRelease = "2023-12-01", ImageUrl = "image1.png" },
                new InfoRecipeViewModel { Id = Guid.NewGuid(), RecipeTitle = "Recipe 2", Description = "Description 2", DateOfRelease = "2023-12-01", ImageUrl = "image2.png" },
                new InfoRecipeViewModel { Id = Guid.NewGuid(), RecipeTitle = "Recipe 3", Description = "Description 3", DateOfRelease = "2023-12-01", ImageUrl = "image3.png" },
            };

            recipeRepositoryMock.Setup(repo => repo.ViewArchivedRecipesAsync(currentUserId))
                .ReturnsAsync(mockData);

            // Act
            var result = await recipeService.ViewArchivedRecipesAsync(currentUserId, page, pageSize);

            // Assert
            Assert.That(2 == result.PageSize);
            Assert.That(2 == result.Items.Count());
            Assert.That(1 == result.CurrentPage);
            recipeRepositoryMock.Verify(repo => repo.ViewArchivedRecipesAsync(currentUserId), Times.Once);
        }

        [Test]
        public async Task ViewArchivedRecipesAsync_NoRecipesExist_ReturnsEmptyPaginatedList()
        {
            // Arrange
            Guid currentUserId = Guid.NewGuid();
            int page = 1;
            int pageSize = 2;
            List<InfoRecipeViewModel> mockData = new List<InfoRecipeViewModel>();

            recipeRepositoryMock.Setup(repo => repo.ViewArchivedRecipesAsync(currentUserId))
                .ReturnsAsync(mockData);

            // Act
            var result = await recipeService.ViewArchivedRecipesAsync(currentUserId, page, pageSize);

            // Assert
            Assert.That(0 == result.Items.Count());
            Assert.That(1 == result.CurrentPage);
            recipeRepositoryMock.Verify(repo => repo.ViewArchivedRecipesAsync(currentUserId), Times.Once);
        }

        [Test]
        public void LikeRecipeAsync_RecipeDoesNotExist_Throws404()
        {
            // Arrange
            Guid recipeId = Guid.NewGuid();
            Guid userId = Guid.NewGuid();

            recipeRepositoryMock.Setup(repo => repo.FindRecipeForLikeRecipeAsync(recipeId))
                .ReturnsAsync((Recipe?)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<HttpStatusException>(async () =>
                await recipeService.LikeRecipeAsync(recipeId, userId));

            Assert.That(404 == ex.StatusCode);
        }

        [Test]
        public async Task LikeRecipeAsync_UserLikesRecipe_LikeAddedAndCountIncreases()
        {
            // Arrange
            Guid recipeId = Guid.NewGuid();
            Guid userId = Guid.NewGuid();
            Recipe recipe = new Recipe { Id = recipeId, LikedRecipes = new List<LikedRecipe>() };

            recipeRepositoryMock.Setup(repo => repo.FindRecipeForLikeRecipeAsync(recipeId))
                .ReturnsAsync(recipe);
            recipeRepositoryMock.Setup(repo => repo.IfLikedRecipesAnyAsync(userId, recipeId))
                .ReturnsAsync(false);
            recipeRepositoryMock.Setup(repo => repo.GetCountOfLikes(recipeId))
                .ReturnsAsync(1);

            // Act
            var result = await recipeService.LikeRecipeAsync(recipeId, userId);

            // Assert
            Assert.IsTrue(result.isLiked);
            Assert.That(1 == result.likes);
            recipeRepositoryMock.Verify(repo => repo.AddLikedRecipeAsync(It.IsAny<LikedRecipe>()), Times.Once);
            recipeRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task LikeRecipeAsync_UserUnlikesRecipe_LikeRemovedAndCountDecreases()
        {
            // Arrange
            Guid recipeId = Guid.NewGuid();
            Guid userId = Guid.NewGuid();
            Recipe recipe = new Recipe { Id = recipeId, LikedRecipes = new List<LikedRecipe>() };
            LikedRecipe likedRecipe = new LikedRecipe { UserId = userId, RecipeId = recipeId };

            recipe.LikedRecipes.Add(likedRecipe);

            recipeRepositoryMock.Setup(repo => repo.FindRecipeForLikeRecipeAsync(recipeId))
                .ReturnsAsync(recipe);
            recipeRepositoryMock.Setup(repo => repo.IfLikedRecipesAnyAsync(userId, recipeId))
                .ReturnsAsync(true);
            recipeRepositoryMock.Setup(repo => repo.FindLikedRecipeAsync(userId, recipeId))
                .ReturnsAsync(likedRecipe);
            recipeRepositoryMock.Setup(repo => repo.GetCountOfLikes(recipeId))
                .ReturnsAsync(0);

            // Act
            var result = await recipeService.LikeRecipeAsync(recipeId, userId);

            // Assert
            Assert.IsFalse(result.isLiked);
            Assert.That(0 == result.likes);
            recipeRepositoryMock.Verify(repo => repo.RemoveFromLikedRecipe(It.IsAny<LikedRecipe>()), Times.Once);
            recipeRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }
    }
}

//Arrange
//Act
//Assert
