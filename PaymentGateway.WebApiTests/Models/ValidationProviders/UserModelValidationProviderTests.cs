using Microsoft.VisualStudio.TestTools.UnitTesting;
using PaymentGateway.WebApi.Models;
using PaymentGateway.WebApi.Models.ValidationProviders;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentGateway.WebApiTests.Models.ValidationProviders
{
    [TestClass]
    public class UserModelValidationProviderTests
    {
        #region Validate Tests

        [TestMethod]
        public void Validate_Test_All_Missing_Errors_Returned()
        {
            // Arrange
            var model = new UserModel
            {
            };

            var provider = new UserModelValidationProvider();

            // Act
            var res = provider.Validate(model);

            // Assert
            var decodedContent = Encoding.UTF8.GetString(res.Content);
            Assert.AreEqual(decodedContent, "{\"Username\":{\"ErrorCode\":\"missing\"},\"Password\":{\"ErrorCode\":\"missing\"}}");
        }

        [TestMethod]
        public void Validate_All_Invalid_Returned()
        {
            // Arrange
            var model = new UserModel
            {
                Username = "ABC",
                Password = "12"
            };

            var provider = new UserModelValidationProvider();

            // Act
            var res = provider.Validate(model);

            // Assert
            var decodedContent = Encoding.UTF8.GetString(res.Content);
            Assert.AreEqual(decodedContent, "{\"Password\":{\"ErrorCode\":\"must_be_at_least_four_characters\"}}");
        }

        [TestMethod]
        public void Validate_All_Passed_Returns_Null()
        {
            // Arrange
            var model = new UserModel
            {
                Username = "ABC",
                Password = "1234"
            };

            var provider = new UserModelValidationProvider();

            // Act
            var res = provider.Validate(model);

            // Assert
            Assert.AreEqual(res, null);
        }

        #endregion Validate Tests
    }
}
