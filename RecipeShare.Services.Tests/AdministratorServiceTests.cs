using Microsoft.AspNetCore.Identity;
using Moq;
using RecipeShare.Common.Exceptions;
using RecipeShare.Data.Models;
using RecipeShare.Repositories.Interfaces;
using RecipeShare.Services.Data;
using RecipeShare.Web.ViewModels.ApplicationUserViewModels;

namespace RecipeShare.Services.Tests
{
    [TestFixture]
    public class AdministratorServiceTests
    {
        private Mock<IAdministratorRepository> adminRepositoryMock;
        private Mock<UserManager<ApplicationUser>> userManagerMock;
        private AdministratorService administratorService;

        [SetUp]
        public void SetUp()
        {
            userManagerMock = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(),
                null, null, null, null, null, null, null, null);
            adminRepositoryMock = new Mock<IAdministratorRepository>();
            administratorService = new AdministratorService(
                userManagerMock.Object,
                adminRepositoryMock.Object
            );
        }

        [Test]
        public async Task GetUsersAsync_ShouldReturnListOfUsers_WhenUsersExist()
        {
            // Arrange
            Guid adminId = Guid.NewGuid();
            List<ViewUserViewModel> users = new List<ViewUserViewModel>
            {
                new ViewUserViewModel { Id = Guid.NewGuid(), Username = "User1", Email = "email1@example.com", RoleName = "User" },
                new ViewUserViewModel { Id = Guid.NewGuid(), Username = "User2", Email = "email2@example.com", RoleName = "User"}
            };

            adminRepositoryMock.Setup(repo => repo.GetUsersAsync(adminId))
                .ReturnsAsync(users);

            // Act
            List<ViewUserViewModel> result = await administratorService.GetUsersAsync(adminId);

            // Assert
            Assert.That(2 == result.Count);
            Assert.That("User1" == result[0].Username);
            Assert.That("User2" == result[1].Username);
        }

        [Test]
        public async Task GetUsersAsync_ShouldReturnEmptyList_WhenNoUsersExist()
        {
            // Arrange
            Guid adminId = Guid.NewGuid();
            adminRepositoryMock.Setup(repo => repo.GetUsersAsync(adminId))
                .ReturnsAsync(new List<ViewUserViewModel>());

            // Act
            List<ViewUserViewModel> result = await administratorService.GetUsersAsync(adminId);

            // Assert
            Assert.That(0 == result.Count);
        }

        [Test]
        public async Task DeleteUserAsync_ShouldDeleteUser_WhenUserExists()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            Guid currentUserId = Guid.NewGuid();
            ApplicationUser user = new ApplicationUser { Id = userId, UserName = "User1", IsDeleted = false };

            adminRepositoryMock.Setup(repo => repo.FindUserAsync(userId, currentUserId))
                .ReturnsAsync(user);

            // Act
            await administratorService.DeleteUserAsync(userId, currentUserId);

            // Assert
            Assert.IsTrue(user.IsDeleted);
            adminRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void DeleteUserAsync_UserNotFound_Throws404()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            Guid currentUserId = Guid.NewGuid();

            adminRepositoryMock.Setup(repo => repo.FindUserAsync(userId, currentUserId))
                .ReturnsAsync((ApplicationUser?)null);

            // Act & Assert
            HttpStatusException ex = Assert.ThrowsAsync<HttpStatusException>(async () => await administratorService.DeleteUserAsync(userId, currentUserId));
            Assert.That(404 == ex.StatusCode);
        }

        [Test]
        public async Task ModelForDeleteAsync_ShouldReturnDeleteUserViewModel_WhenUserFound()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            Guid currentUserId = Guid.NewGuid();
            DeleteUserViewModel deleteUserViewModel = new DeleteUserViewModel { Id = userId, UserName = "Test User" };

            adminRepositoryMock.Setup(repo => repo.ModelForDeleteAsync(userId, currentUserId))
                .ReturnsAsync(deleteUserViewModel);

            // Act
            var result = await administratorService.ModelForDeleteAsync(userId, currentUserId);

            // Assert
            Assert.That(deleteUserViewModel == result);
        }

        [Test]
        public void ModelForDeleteAsync_UserNotFound_Throws404()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            Guid currentUserId = Guid.NewGuid();

            adminRepositoryMock.Setup(repo => repo.ModelForDeleteAsync(userId, currentUserId))
                .ReturnsAsync((DeleteUserViewModel?)null);

            // Act & Assert
            HttpStatusException ex = Assert.ThrowsAsync<HttpStatusException>(async () => await administratorService.ModelForDeleteAsync(userId, currentUserId));
            Assert.That(404 == ex.StatusCode);
        }

        [Test]
        public async Task ModelForChangingRoleAsync_ShouldReturnChangeRoleViewModel_WhenModelFound()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            string userName = "userName";
            Guid currentUserId = Guid.NewGuid();
            ChangeRoleViewModel changeRoleViewModel = new ChangeRoleViewModel
            {
                UserId = userId,
                Username = userName,
                RoleName = "User",
                Roles = new List<RoleViewModel>()
                {
                    new RoleViewModel()
                    {
                        RoleId = Guid.NewGuid(),
                        RoleName = "User",
                    }
                }
            };

            adminRepositoryMock.Setup(repo => repo.ModelForChangingRoleAsync(userId, currentUserId))
                .ReturnsAsync(changeRoleViewModel);
            adminRepositoryMock.Setup(repo => repo.GetRolesAsync(changeRoleViewModel.RoleName))
                .ReturnsAsync(new List<RoleViewModel> { new RoleViewModel { RoleId = Guid.NewGuid(), RoleName = "User"} });

            // Act
            var result = await administratorService.ModelForChangingRoleAsync(userId, currentUserId);

            // Assert
            Assert.That(changeRoleViewModel == result);
            Assert.That(1 == result.Roles.Count);
        }

        [Test]
        public void ModelForChangingRoleAsync_ModelNotFound_Throws404()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            Guid currentUserId = Guid.NewGuid();

            adminRepositoryMock.Setup(repo => repo.ModelForChangingRoleAsync(userId, currentUserId))
                .ReturnsAsync((ChangeRoleViewModel?)null);

            // Act & Assert
            HttpStatusException ex = Assert.ThrowsAsync<HttpStatusException>(async () => await administratorService.ModelForChangingRoleAsync(userId, currentUserId));
            Assert.That(404 == ex.StatusCode);
        }

        [Test]
        public async Task ChangeRoleAsync_ShouldChangeRole_WhenUserAndRoleExist()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            Guid roleId = Guid.NewGuid();
            Guid currentUserId = Guid.NewGuid();
            string oldRoleName = "User";
            string newRoleName = "Administrator";

            ApplicationUser user = new ApplicationUser { Id = userId, UserName = "testuser" };

            adminRepositoryMock.Setup(repo => repo.FindUserAsync(userId, currentUserId))
                .ReturnsAsync(user);
            adminRepositoryMock.Setup(repo => repo.GetNewRoleNameAsync(roleId))
                .ReturnsAsync(newRoleName);

            userManagerMock.Setup(um => um.RemoveFromRoleAsync(user, oldRoleName))
                .ReturnsAsync(IdentityResult.Success);
            userManagerMock.Setup(um => um.AddToRoleAsync(user, newRoleName))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            await administratorService.ChangeRoleAsync(userId, roleId, currentUserId, oldRoleName);

            // Assert
            userManagerMock.Verify(um => um.RemoveFromRoleAsync(user, oldRoleName), Times.Once);
            userManagerMock.Verify(um => um.AddToRoleAsync(user, newRoleName), Times.Once);
            adminRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void ChangeRoleAsync_UserNotFound_Throws404()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            Guid roleId = Guid.NewGuid();
            Guid currentUserId = Guid.NewGuid();
            string oldRoleName = "User";

            adminRepositoryMock.Setup(repo => repo.FindUserAsync(userId, currentUserId))
                .ReturnsAsync((ApplicationUser?)null);

            // Act & Assert
            HttpStatusException ex = Assert.ThrowsAsync<HttpStatusException>(async () =>
                await administratorService.ChangeRoleAsync(userId, roleId, currentUserId, oldRoleName));
            Assert.That(404 == ex.StatusCode);
        }

        [Test]
        public void ChangeRoleAsync_RoleNotFound_Throw404()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            Guid roleId = Guid.NewGuid();
            Guid currentUserId = Guid.NewGuid();
            string oldRoleName = "User";

            ApplicationUser user = new ApplicationUser { Id = userId, UserName = "testuser" };

            adminRepositoryMock.Setup(repo => repo.FindUserAsync(userId, currentUserId))
                .ReturnsAsync(user);
            adminRepositoryMock.Setup(repo => repo.GetNewRoleNameAsync(roleId))
                .ReturnsAsync((string?)null);

            // Act & Assert
            HttpStatusException ex = Assert.ThrowsAsync<HttpStatusException>(async () =>
                await administratorService.ChangeRoleAsync(userId, roleId, currentUserId, oldRoleName));
            Assert.That(404 == ex.StatusCode);
        }
    }
}
