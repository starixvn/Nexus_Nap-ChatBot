using AutoFixture;
using Fanex.NAP.Core.Models;
using Fanex.NAP.Core.Service.Interface;
using Fanex.NCB.Common.MetalModels;
using Fanex.NCB.Swagger.AuthGate.Services;
using Moq;
using Xunit;

namespace Fanex.NCB.Swagger.AuthGate.UnitTests.Services
{
    public class AuthServiceTest
    {
        [Fact]
        public void Authorize_SameData_UserNameCorrectly()
        {
            //Arrange
            var fixture = new Fixture();
            var userService = new Mock<IUserService>();
            var authService = new AuthService(userService.Object);

            var userName = fixture.Create<string>();
            var password = fixture.Create<string>();
            var userAuth = new AuthenticationUser() { UserName = userName, Password = password };
            var user = new User() { Username = userName, Password = password };

            userService.Setup(_ => _.Get(userName)).Returns(user);

            //Act
            var result = authService.Authorize(userAuth);

            //Assert
            Assert.Equal(userName, result.UserName);
        }
    }
}