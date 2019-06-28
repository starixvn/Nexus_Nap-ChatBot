using AutoFixture;
using Fanex.NCB.Common.DTO;
using Fanex.NCB.Common.MetalModels;
using Fanex.NCB.Swagger.AuthGate.Controllers;
using Fanex.NCB.Swagger.AuthGate.Services;
using Moq;
using Xunit;

namespace Fanex.NCB.Swagger.AuthGate.UnitTests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Fixture _fixture;
        private readonly Mock<IAuthService> _authService;
        private readonly AuthController _authController;

        public AuthControllerTests()
        {
            _fixture = new Fixture();
            _authService = new Mock<IAuthService>();
            _authController = new AuthController(_authService.Object);
        }

        [Fact]
        public void Verify_WhenCall_ReturnCorrectData()
        {
            //Arrange
            var authUser = _fixture.Create<AuthenticationUser>();
            var result = _fixture.Create<ApplicationUser>();
            _authService.Setup(_ => _.Authorize(authUser)).Returns(result);

            //Act
            var actual = _authController.Verify(authUser) as ApplicationUser;

            //Assert
            Assert.Equal(result, actual);
        }
    }
}