using Microsoft.AspNetCore.Identity;
using Moq;
using RecipeShare.Common.Exceptions;
using RecipeShare.Data.Models;
using RecipeShare.Repositories.Interfaces;
using RecipeShare.Services.Data;
using RecipeShare.Web.ViewModels.AccountSettingsViewModels;
using RecipeShare.Web.ViewModels.ApplicationUserViewModels;

namespace RecipeShare.Services.Tests
{
    [TestFixture]
    public class AccountSettingsServiceTests
    {
        private Mock<UserManager<ApplicationUser>> userManagerMock;
        private Mock<IAccountRepository> accountRepositoryMock;
        private AccountSettingsService accountSettingsService;

        [SetUp]
        public void SetUp()
        {
            userManagerMock = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(),
                null, null, null, null, null, null, null, null);

            accountRepositoryMock = new Mock<IAccountRepository>();

            accountSettingsService = new AccountSettingsService(userManagerMock.Object, accountRepositoryMock.Object);
        }

        [Test]
        public async Task AccountInfoModelAsync_ShouldReturnModel_WhenModelExists()
        {
            // Arrange
            Guid accountId = Guid.NewGuid();
            AccountInfoViewModel expectedModel = new AccountInfoViewModel { UserName = "TestUser", AccountBio = "Test Bio", IsMale = true };

            accountRepositoryMock.Setup(repo => repo.AccountInfoModelAsync(accountId))
                .ReturnsAsync(expectedModel);

            // Act
            AccountInfoViewModel result = await accountSettingsService.AccountInfoModelAsync(accountId);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(expectedModel.UserName == result.UserName);
            Assert.That(expectedModel.AccountBio == result.AccountBio);
            Assert.That(expectedModel.IsMale == result.IsMale);
        }

        [Test]
        public void AccountInfoModelAsync_AccountNotFound_Throws404()
        {
            // Arrange
            Guid accountId = Guid.NewGuid();

            accountRepositoryMock.Setup(repo => repo.AccountInfoModelAsync(accountId))
                .ReturnsAsync((AccountInfoViewModel?)null);

            accountRepositoryMock.Setup(repo => repo.IfAccountAnyAsync(accountId))
                .ReturnsAsync(true);

            // Act & Assert
            HttpStatusException exception = Assert.ThrowsAsync<HttpStatusException>(async () =>
                await accountSettingsService.AccountInfoModelAsync(accountId));

            Assert.That(404 == exception.StatusCode);
        }

        [Test]
        public void AccountInfoModelAsync_AccountNotPermitted_Throws403()
        {
            // Arrange
            Guid accountId = Guid.NewGuid();

            accountRepositoryMock.Setup(repo => repo.AccountInfoModelAsync(accountId))
                .ReturnsAsync((AccountInfoViewModel?)null);

            accountRepositoryMock.Setup(repo => repo.IfAccountAnyAsync(accountId))
                .ReturnsAsync(false);

            // Act & Assert
            var exception = Assert.ThrowsAsync<HttpStatusException>(async () =>
                await accountSettingsService.AccountInfoModelAsync(accountId));

            Assert.That(403 == exception.StatusCode);
        }

        [Test]
        public async Task DeleteUserAsync_ShouldMarkUserAsDeleted_WhenUserExists()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            ApplicationUser user = new ApplicationUser { Id = userId, IsDeleted = false };

            accountRepositoryMock.Setup(repo => repo.FindUserAsync(userId))
                .ReturnsAsync(user);

            // Act
            await accountSettingsService.DeleteUserAsync(userId);

            // Assert
            Assert.IsTrue(user.IsDeleted);
            accountRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void DeleteUserAsync_AccountNotFound_Throws404()
        {
            // Arrange
            Guid userId = Guid.NewGuid();

            accountRepositoryMock.Setup(repo => repo.FindUserAsync(userId))
                .ReturnsAsync((ApplicationUser?)null);

            accountRepositoryMock.Setup(repo => repo.IfAccountAnyAsync(userId))
                .ReturnsAsync(true);

            // Act & Assert
            HttpStatusException exception = Assert.ThrowsAsync<HttpStatusException>(async () =>
                await accountSettingsService.DeleteUserAsync(userId));

            Assert.That(404 == exception.StatusCode);
        }

        [Test]
        public void DeleteUserAsync_AccountNotPermitted_Throws403()
        {
            // Arrange
            Guid userId = Guid.NewGuid();

            accountRepositoryMock.Setup(repo => repo.FindUserAsync(userId))
                .ReturnsAsync((ApplicationUser?)null);

            accountRepositoryMock.Setup(repo => repo.IfAccountAnyAsync(userId))
                .ReturnsAsync(false);

            // Act & Assert
            HttpStatusException exception = Assert.ThrowsAsync<HttpStatusException>(async () =>
                await accountSettingsService.DeleteUserAsync(userId));

            Assert.That(403 == exception.StatusCode);
        }

        [Test]
        public async Task ModelForDeleteUserAsunc_ShouldReturnModel_WhenAccountExists()
        {
            // Arrange
            Guid accountId = Guid.NewGuid();
            DeleteUserViewModel expectedModel = new DeleteUserViewModel { Id = accountId, UserName = "testuser" };

            accountRepositoryMock.Setup(repo => repo.ModelForDeleteUserAsunc(accountId))
                .ReturnsAsync(expectedModel);

            // Act
            DeleteUserViewModel result = await accountSettingsService.ModelForDeleteUserAsunc(accountId);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(expectedModel == result);
        }

        [Test]
        public void ModelForDeleteUserAsunc_AccountNotFound_Throws404()
        {
            // Arrange
            Guid accountId = Guid.NewGuid();

            accountRepositoryMock.Setup(repo => repo.ModelForDeleteUserAsunc(accountId))
                .ReturnsAsync((DeleteUserViewModel?)null);

            accountRepositoryMock.Setup(repo => repo.IfAccountAnyAsync(accountId))
                .ReturnsAsync(true);

            // Act & Assert
            HttpStatusException exception = Assert.ThrowsAsync<HttpStatusException>(async () =>
                await accountSettingsService.ModelForDeleteUserAsunc(accountId));

            Assert.That(404 == exception.StatusCode);
        }

        [Test]
        public void ModelForDeleteUserAsunc_AccountNotPermitted_Throws403()
        {
            // Arrange
            Guid accountId = Guid.NewGuid();

            accountRepositoryMock.Setup(repo => repo.ModelForDeleteUserAsunc(accountId))
                .ReturnsAsync((DeleteUserViewModel?)null);

            accountRepositoryMock.Setup(repo => repo.IfAccountAnyAsync(accountId))
                .ReturnsAsync(false);

            // Act & Assert
            HttpStatusException exception = Assert.ThrowsAsync<HttpStatusException>(async () =>
                await accountSettingsService.ModelForDeleteUserAsunc(accountId));

            Assert.That(403 == exception.StatusCode);
        }

        [Test]
        public async Task SaveAccountInfoAsync_ShouldSave_WhenUserExists()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            AccountInfoViewModel model = new AccountInfoViewModel
            {
                UserName = "UpdatedUser",
                AccountBio = "UpdatedBio",
                IsMale = true
            };

            var user = new ApplicationUser
            {
                Id = userId,
                UserName = "OldUser",
                AccountBio = "OldBio",
                IsMale = false
            };

            accountRepositoryMock.Setup(repo => repo.FindUserAsync(userId))
                .ReturnsAsync(user);

            accountRepositoryMock.Setup(repo => repo.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            await accountSettingsService.SaveAccountInfoAsync(model, userId);

            // Assert
            accountRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
            Assert.That(model.UserName == user.UserName);
            Assert.That(model.AccountBio == user.AccountBio);
            Assert.That(model.IsMale == user.IsMale);
        }

        [Test]
        public void SaveAccountInfoAsync_AccountNotFound_Throws404()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            AccountInfoViewModel model = new AccountInfoViewModel
            {
                UserName = "UpdatedUser",
                AccountBio = "UpdatedBio",
                IsMale = true
            };

            accountRepositoryMock.Setup(repo => repo.FindUserAsync(userId))
                .ReturnsAsync((ApplicationUser?)null);

            accountRepositoryMock.Setup(repo => repo.IfAccountAnyAsync(userId))
                .ReturnsAsync(true);

            // Act & Assert
            HttpStatusException exception = Assert.ThrowsAsync<HttpStatusException>(async () =>
                await accountSettingsService.SaveAccountInfoAsync(model, userId));

            Assert.That(404 == exception.StatusCode);
        }

        [Test]
        public void SaveAccountInfoAsync_AccountNotPermitted_Thorws403()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            AccountInfoViewModel model = new AccountInfoViewModel
            {
                UserName = "UpdatedUser",
                AccountBio = "UpdatedBio",
                IsMale = true
            };

            accountRepositoryMock.Setup(repo => repo.FindUserAsync(userId))
                .ReturnsAsync((ApplicationUser?)null);

            accountRepositoryMock.Setup(repo => repo.IfAccountAnyAsync(userId))
                .ReturnsAsync(false);

            // Act & Assert
            HttpStatusException exception = Assert.ThrowsAsync<HttpStatusException>(async () =>
                await accountSettingsService.SaveAccountInfoAsync(model, userId));

            Assert.That(403 == exception.StatusCode);
        }
    }
}
