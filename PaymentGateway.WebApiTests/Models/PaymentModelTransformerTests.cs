using Framework.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PaymentGateway.WebApi.Domain;
using PaymentGateway.WebApi.Models;
using PaymentGateway.WebApi.Models.ValidationProviders;
using System;
using System.Text;

namespace PaymentGateway.WebApiTests.Models
{
    [TestClass]
    public class PaymentModelTransformerTests
    {
        #region ToModel Tests

        [TestMethod]
        public void ToModel_Returns_UserModel()
        {
            // Arrange
            var paymentDto = new PaymentDto()
            {
                Uid = "123-45",
                CardNumber = "1234567890123456",
                ExpiryDate = "2020-12-02",
                Amount = 123,
                Cvv = "321",
                PaymentDate = DateTime.UtcNow,
                State = PaymentState.Completed
            };

            // Act
            var res = paymentDto.ToModel();

            // Assert
            Assert.AreEqual(res.Uid, paymentDto.Uid);
            Assert.AreEqual(res.CardNumber, "***********23456");
            Assert.AreEqual(res.ExpiryDate, "********02");
            Assert.AreEqual(res.Amount, paymentDto.Amount);
            Assert.AreEqual(res.Cvv, "***");
            Assert.AreEqual(res.PaymentDate, paymentDto.PaymentDate);
            Assert.AreEqual(res.State, paymentDto.State);
        }

        #endregion ToModel Tests
    }
}
