using Framework.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PaymentGateway.WebApi.Domain;
using PaymentGateway.WebApi.Models;
using System.Linq;

namespace PaymentGateway.WebApiTests.Models
{
    [TestClass]
    public class UserModelTransformerTests
    {
        #region ToModel Tests

        [TestMethod]
        public void ToModel_Returns_UserModel()
        {
            // Arrange
            var userDto = new UserDto()
            {
                Uid = "123",
                Username = "Name",
                Permissions = new Permission[]
                {
                    Permission.Payment_Create,
                    Permission.Payment_View
                }
            };

            // Act
            var res = userDto.ToModel();

            // Assert
            Assert.AreEqual(res.Uid, userDto.Uid);
            Assert.AreEqual(res.Username, userDto.Username);
            Assert.AreEqual(res.Permissions.Count(), userDto.Permissions.Length);
        }

        [TestMethod]
        public void ToModel_UserDto_Null_Returns_Null()
        {
            // Arrange
            UserDto userDto = null;

            // Act
            var res = userDto.ToModel();

            // Assert
            Assert.AreEqual(res, userDto);
        }

        #endregion ToModel Tests
    }
}
