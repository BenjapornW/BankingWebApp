using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using AdminWebAPI.Controllers;
using DataModelLibrary.Models;
using Xunit;

namespace AdminWebAPI.Test
{
    public class TokenControllerTests
    {
        private readonly Mock<IConfiguration> _mockConfig;

        public TokenControllerTests()
        {
            _mockConfig = new Mock<IConfiguration>();
            _mockConfig.SetupGet(x => x["Jwt:Issuer"]).Returns("issuer");
            _mockConfig.SetupGet(x => x["Jwt:Audience"]).Returns("audience");
            _mockConfig.SetupGet(x => x["Jwt:Key"]).Returns("8EsB5#nUZzq^VW%6K@4&p$y2*dGxXr@e8EsB5#nUZzq^VW%6K@4&p$y2*dGxXr@e");

        }


        [Fact]
        public void CreateToken_ValidCredentials_ReturnsOkResultWithJwtToken()
        {
            // Arrange
            var user = new User { UserName = "admin", Password = "admin" };

            var controller = new TokenController(_mockConfig.Object);

            // Act
            var result = controller.CreateToken(user) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);

            var token = Assert.IsType<string>(result.Value);
            Assert.NotEmpty(token);
        }

        [Fact]
        public void CreateToken_InvalidCredentials_ReturnsUnauthorizedResult()
        {
            // Arrange
            var user = new User { UserName = "invaliduser", Password = "invalidpassword" };

            var controller = new TokenController(_mockConfig.Object);

            // Act
            var result = controller.CreateToken(user);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }
    }
}
