using Framework.WebUtilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace FrameworkTests.WebUtilities
{
    [TestClass]
    public class ValidationResultTests
    {
        #region ToTeapotResult Tests

        [TestMethod]
        public void ToTeapotResult_Return_418_Status_Code()
        {
            // Arrange
            var apiOffence = ApiOffences.Payment.AmountTooHigh;
            var propertyName = "Amount";

            // Act
            var res = ValidationResult.ToTeapotResult(apiOffence, propertyName);

            // Assert
            Assert.AreEqual(418, res.StatusCode);
        }

        [TestMethod]
        public void ToTeapotResult_ApiOffence_Cannot_Be_Missing()
        {
            // Arrange
            var propertyName = "Amount";

            // Act
            var res = ValidationResult.ToTeapotResult(default, propertyName);

            // Assert
            Assert.IsNull(res);
        }

        [TestMethod]
        [DataRow("")]
        [DataRow(null)]
        public void ToTeapotResult_PropertName_Cannot_Be_Missing(string propertName)
        {
            // Arrange
            var apiOffence = ApiOffences.Payment.AmountTooHigh;

            // Act
            var res = ValidationResult.ToTeapotResult(apiOffence, propertName);

            // Assert
            Assert.IsNull(res);
        }

        #endregion ToTeapotResult Tests

        #region GetUnifiedTeaPotValidationResults Tests

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void GetUnifiedTeaPotValidationResults_If_Null_Or_Count_0_Return_Null(bool isNull)
        {
            // Arrange
            var ValidationResults = new Queue<ValidationResult>();
            if (isNull)
            {
                ValidationResults = null;
            }

            // Act
            var res = ValidationResult.GetUnifiedTeapotValidationResults(ValidationResults);

            // Assert
            Assert.IsNull(res);
        }

        [TestMethod]
        public void GetUnifiedTeaPotValidationResults_Return_Multiple_Objects_If_Multiple_Responses()
        {
            // Arrange
            var ValidationResults = new Queue<ValidationResult>();
            ValidationResults.Enqueue(ValidationResult.ToTeapotResult(ApiOffences.Payment.AmountTooHigh, "Amount"));
            ValidationResults.Enqueue(ValidationResult.ToTeapotResult(ApiOffences.Payment.InvalidCardNumber, "CardNumber"));

            // Act
            var res = ValidationResult.GetUnifiedTeapotValidationResults(ValidationResults);

            // Assert
            var decoded = Encoding.UTF8.GetString(res.Content);
            Assert.AreEqual(decoded, "{\"Amount\":{\"ErrorCode\":\"amount_too_high\"},\"CardNumber\":{\"ErrorCode\":\"invalid_card_number\"}}");
        }

        #endregion GetUnifiedValidationResults Tests
    }
}
