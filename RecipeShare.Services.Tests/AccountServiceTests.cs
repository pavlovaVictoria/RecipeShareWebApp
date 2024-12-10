using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using RecipeShare.Data.Models;
using RecipeShare.Services.Data;
using RecipeShare.Web.ViewModels.ApplicationUserViewModels;

namespace RecipeShare.Services.Tests
{
    [TestFixture]
    public class AccountServiceTests
    {
        private Mock<SignInManager<ApplicationUser>> signInManagerMock;
        private Mock<UserManager<ApplicationUser>> userManagerMock;
        private AccountService accountService;

        [SetUp]
        public void SetUp()
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();

            // Here I am Mocking IHttpContextAccessor, IUserClaimsPrincipalFactory, IOptions<IdentityOptions>, ILogger, and IAuthenticationSchemeProvider
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            var userClaimsPrincipalFactoryMock = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            var identityOptionsMock = new Mock<IOptions<IdentityOptions>>();
            var loggerMock = new Mock<ILogger<SignInManager<ApplicationUser>>>();
            var authenticationSchemeProviderMock = new Mock<IAuthenticationSchemeProvider>();

            userManagerMock = new Mock<UserManager<ApplicationUser>>(
                userStoreMock.Object,
                null, null, null, null, null, null, null, null
            );

            signInManagerMock = new Mock<SignInManager<ApplicationUser>>(
                userManagerMock.Object,
                httpContextAccessorMock.Object,
                userClaimsPrincipalFactoryMock.Object,
                identityOptionsMock.Object,
                loggerMock.Object,
                authenticationSchemeProviderMock.Object
            );

            accountService = new AccountService(signInManagerMock.Object, userManagerMock.Object);
        }

        [Test]
        public async Task LoginAsync_UserNotFoundOrDeleted_ReturnsFalse()
        {
            // Arrange
            LoginViewModel model = new LoginViewModel { Email = "test@example.com", Password = "Password123", RememberMe = false };
            userManagerMock.Setup(u => u.FindByEmailAsync(model.Email)).ReturnsAsync((ApplicationUser?)null);

            // Act
            var result = await accountService.LoginAsync(model);

            // Assert
            Assert.IsFalse(result);
            userManagerMock.Verify(u => u.FindByEmailAsync(model.Email), Times.Once);
        }

        [Test]
        public async Task LoginAsync_UserAddedToRole_ReturnsTrue_SuccessfulLogin()
        {
            // Arrange
            ApplicationUser user = new ApplicationUser { UserName = "testuser", IsDeleted = false };
            LoginViewModel model = new LoginViewModel { Email = "test@example.com", Password = "Password123", RememberMe = false };

            userManagerMock.Setup(u => u.FindByEmailAsync(model.Email)).ReturnsAsync(user);
            userManagerMock.Setup(u => u.IsInRoleAsync(user, "User")).ReturnsAsync(false);
            userManagerMock.Setup(u => u.IsInRoleAsync(user, "Moderator")).ReturnsAsync(false);
            userManagerMock.Setup(u => u.IsInRoleAsync(user, "Administrator")).ReturnsAsync(false);
            userManagerMock.Setup(u => u.CheckPasswordAsync(user, model.Password)).ReturnsAsync(true);
            signInManagerMock.Setup(s => s.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, false))
                .ReturnsAsync(SignInResult.Success);

            // Act
            bool result = await accountService.LoginAsync(model);

            // Assert
            Assert.IsTrue(result);
            userManagerMock.Verify(u => u.AddToRoleAsync(user, "User"), Times.Once);
        }

        [Test]
        public async Task LoginAsync_InvalidPassword_ReturnsFalsePassword()
        {
            // Arrange
            ApplicationUser user = new ApplicationUser { UserName = "testuser", IsDeleted = false };
            LoginViewModel model = new LoginViewModel { Email = "test@example.com", Password = "WrongPassword", RememberMe = false };

            userManagerMock.Setup(u => u.FindByEmailAsync(model.Email)).ReturnsAsync(user);
            userManagerMock.Setup(u => u.CheckPasswordAsync(user, model.Password)).ReturnsAsync(false);

            // Act
            bool result = await accountService.LoginAsync(model);

            // Assert
            Assert.IsFalse(result);
            userManagerMock.Verify(u => u.CheckPasswordAsync(user, model.Password), Times.Once);
        }

        [Test]
        public async Task LoginAsync_SuccessfulLogin_ReturnsTrue_RoleUser()
        {
            // Arrange
            ApplicationUser user = new ApplicationUser { UserName = "testuser", IsDeleted = false };
            LoginViewModel model = new LoginViewModel { Email = "test@example.com", Password = "Password123", RememberMe = true };

            userManagerMock.Setup(u => u.FindByEmailAsync(model.Email)).ReturnsAsync(user);
            userManagerMock.Setup(u => u.IsInRoleAsync(user, "User")).ReturnsAsync(true);
            userManagerMock.Setup(u => u.CheckPasswordAsync(user, model.Password)).ReturnsAsync(true);
            signInManagerMock.Setup(s => s.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, false))
                .ReturnsAsync(SignInResult.Success);

            // Act
            bool result = await accountService.LoginAsync(model);

            // Assert
            Assert.IsTrue(result);
            signInManagerMock.Verify(s => s.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, false), Times.Once);
        }

        [Test]
        public async Task LogoutAsync_ShouldCallSignOutAsync()
        {
            // Act
            await accountService.LogoutAsync();

            // Assert
            signInManagerMock.Verify(x => x.SignOutAsync(), Times.Once);
        }

        [Test]
        public async Task RegisterAsync_ShouldReturnFalse_WhenUserCreationFails()
        {
            // Arrange
            RegisterViewModel model = new RegisterViewModel
            {
                IsMale = true,
                Email = "test@example.com",
                UserName = "testuser",
                AccountBio = "Bio",
                Password = "ValidPassword123!"
            };

            userManagerMock
                .Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Error" }));

            // Act
            bool result = await accountService.RegisterAsync(model);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public async Task ForgotPasswordAsync_ShouldReturnTrue_WhenPasswordIsChangedSuccessfully()
        {
            // Arrange
            ChangePasswordViewModel model = new ChangePasswordViewModel
            {
                Email = "test@example.com",
                NewPassword = "NewValidPassword123!"
            };

            ApplicationUser user = new ApplicationUser
            {
                Email = "test@example.com",
                UserName = "testuser",
                IsDeleted = false
            };

            userManagerMock.Setup(um => um.FindByEmailAsync(model.Email))
                .ReturnsAsync(user);

            userManagerMock.Setup(um => um.RemovePasswordAsync(user))
                .ReturnsAsync(IdentityResult.Success);

            userManagerMock.Setup(um => um.AddPasswordAsync(user, model.NewPassword))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            bool result = await accountService.ForgotPasswordAsync(model);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async Task ForgotPasswordAsync_ShouldReturnFalse_WhenUserNotFound()
        {
            // Arrange
            ChangePasswordViewModel model = new ChangePasswordViewModel
            {
                Email = "nonexistent@example.com",
                NewPassword = "NewValidPassword123!"
            };

            userManagerMock.Setup(um => um.FindByEmailAsync(model.Email))
                .ReturnsAsync((ApplicationUser?)null);

            // Act
            bool result = await accountService.ForgotPasswordAsync(model);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public async Task ForgotPasswordAsync_ShouldReturnFalse_WhenRemovePasswordFails()
        {
            // Arrange
            ChangePasswordViewModel model = new ChangePasswordViewModel
            {
                Email = "test@example.com",
                NewPassword = "NewValidPassword123!"
            };

            ApplicationUser user = new ApplicationUser
            {
                Email = "test@example.com",
                UserName = "testuser",
                IsDeleted = false
            };

            userManagerMock.Setup(um => um.FindByEmailAsync(model.Email))
                .ReturnsAsync(user);

            userManagerMock.Setup(um => um.RemovePasswordAsync(user))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Remove password failed" }));

            // Act
            bool result = await accountService.ForgotPasswordAsync(model);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public async Task ForgotPasswordAsync_ShouldReturnFalse_WhenAddPasswordFails()
        {
            // Arrange
            ChangePasswordViewModel model = new ChangePasswordViewModel
            {
                Email = "test@example.com",
                NewPassword = "NewValidPassword123!"
            };

            ApplicationUser user = new ApplicationUser
            {
                Email = "test@example.com",
                UserName = "testuser",
                IsDeleted = false
            };

            userManagerMock.Setup(um => um.FindByEmailAsync(model.Email))
                .ReturnsAsync(user);

            userManagerMock.Setup(um => um.RemovePasswordAsync(user))
                .ReturnsAsync(IdentityResult.Success);

            userManagerMock.Setup(um => um.AddPasswordAsync(user, model.NewPassword))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Add password failed" }));

            // Act
            bool result = await accountService.ForgotPasswordAsync(model);

            // Assert
            Assert.IsFalse(result);
        }
    }
}
