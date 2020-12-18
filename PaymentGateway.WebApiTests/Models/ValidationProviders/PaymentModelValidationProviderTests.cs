using Microsoft.VisualStudio.TestTools.UnitTesting;
using PaymentGateway.WebApi.Models;
using PaymentGateway.WebApi.Models.ValidationProviders;
using System;
using System.Text;

namespace PaymentGateway.WebApiTests.Models.ValidationProviders
{
    [TestClass]
    public class PaymentModelValidationProviderTests
    {
        #region Validate Tests

        [TestMethod]
        public void Validate_Test_All_Missing_Errors_Returned()
        {
            // Arrange
            var model = new PaymentModel
            {
            };

            var provider = new PaymentModelValidationProvider();

            // Act
            var res = provider.Validate(model);

            // Assert
            var decodedContent = Encoding.UTF8.GetString(res.Content);
            Assert.AreEqual(decodedContent, "{\"CardNumber\":{\"ErrorCode\":\"missing\"},\"Cvv\":{\"ErrorCode\":\"missing\"},\"ExpiryDate\":{\"ErrorCode\":\"missing\"},\"Amount\":{\"ErrorCode\":\"missing\"},\"Currency\":{\"ErrorCode\":\"missing\"}}");
        }

        [TestMethod]
        [DataRow(true, "-1")]
        [DataRow(false, "1000000000000000")]
        public void Validate_All_Invalid_Returned(bool dateTimeIsPast, string amount)
        {
            // Arrange
            var model = new PaymentModel
            {
                CardNumber = "11223344",
                Cvv = "12",
                ExpiryDate = dateTimeIsPast ? DateTime.UtcNow.AddDays(-1).ToString() : "123",
                Amount = decimal.Parse(amount),
                Currency = "AAA"
            };

            var provider = new PaymentModelValidationProvider();

            // Act
            var res = provider.Validate(model);

            // Assert
            var decodedContent = Encoding.UTF8.GetString(res.Content);
            if (dateTimeIsPast)
            {
                // Test: Amount too low, date is in past
                Assert.AreEqual(decodedContent, "{\"Cvv\":{\"ErrorCode\":\"invalid_cvv\"},\"ExpiryDate\":{\"ErrorCode\":\"expirydate_in_the_past\"},\"Amount\":{\"ErrorCode\":\"amount_too_low\"},\"Currency\":{\"ErrorCode\":\"currency_not_supported\"}}");
            }
            else
            {
                // Test: Amount too high, date is incorrect format
                Assert.AreEqual(decodedContent, "{\"Cvv\":{\"ErrorCode\":\"invalid_cvv\"},\"ExpiryDate\":{\"ErrorCode\":\"incorrect_format\"},\"Amount\":{\"ErrorCode\":\"amount_too_high\"},\"Currency\":{\"ErrorCode\":\"currency_not_supported\"}}");
            }
        }

        [TestMethod]
        public void Validate_All_Passed_Returns_Null()
        {
            // Arrange
            var model = new PaymentModel
            {
                CardNumber = "4916132996393639",
                Cvv = "123",
                ExpiryDate = DateTime.UtcNow.AddDays(1).ToString(),
                Amount = 5,
                Currency = "GBP"
            };

            var provider = new PaymentModelValidationProvider();

            // Act
            var res = provider.Validate(model);

            // Assert
            Assert.AreEqual(res, null);
        }

        #endregion Validate Tests
    }
}
