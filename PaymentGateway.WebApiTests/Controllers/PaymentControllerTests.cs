using Framework.Interfaces;
using Framework.WebUtilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PaymentGateway.WebApi.Controllers;
using PaymentGateway.WebApi.Domain;
using PaymentGateway.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.WebApiTests.Controllers
{
    [TestClass]
    public class PaymentControllerTests
    {
        private MockRepository _mocks = new MockRepository(MockBehavior.Strict);

        #region CreatePaymentAsync Tests

        [TestMethod]
        public void CreatePaymentAsync_Validation_Error_Returns_418()
        {
            // Arrange
            var validationProviderMock = _mocks.Create<IValidate<PaymentModel>>();
            validationProviderMock.Setup(m => m.Validate(It.IsAny<PaymentModel>()))
                .Returns(ValidationResult.ToTeapotResult(ApiOffences.Missing, "Test"));

            var controller = new PaymentController(null, validationProviderMock.Object);

            // Act
            var res = controller.CreatePaymentAsync(null).Result;

            // Assert
            _mocks.Verify();
            Assert.AreEqual(((ValidationResult)res.Result).StatusCode, 418);
        }

        [TestMethod]
        public void CreatePaymentAsync_Create_Error_Returns_500()
        {
            // Arrange
            var validationProviderMock = _mocks.Create<IValidate<PaymentModel>>();
            validationProviderMock.Setup(m => m.Validate(It.IsAny<PaymentModel>()))
                .Returns<ValidationResult>(null);

            var paymentBLLMock = _mocks.Create<IPaymentBLL>();
            paymentBLLMock.Setup(m => m.CreatePaymentAsync(It.IsAny<PaymentDto>()))
                .Returns(Task.FromResult(false));

            var controller = new PaymentController(paymentBLLMock.Object, validationProviderMock.Object);

            // Act
            var res = controller.CreatePaymentAsync(new PaymentModel()).Result;

            // Assert
            _mocks.Verify();
            Assert.AreEqual(((StatusCodeResult)res.Result).StatusCode, 500);
        }

        #endregion CreatePaymentAsync Tests

        #region GetPaymentByUid Tests

        [TestMethod]
        public void GetPaymentByUid_Returns_NotFound()
        {
            // Arrange
            var uid = "Test";
            var paymentBLLMock = _mocks.Create<IPaymentBLL>();
            paymentBLLMock.Setup(m => m.GetPaymentByUid(uid))
                .Returns<PaymentDto>(null);

            var controller = new PaymentController(paymentBLLMock.Object, null);

            // Act
            var res = controller.GetPaymentByUid(uid);

            // Assert
            _mocks.Verify();
            Assert.IsInstanceOfType(res.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void GetPaymentByUid_Returns_PaymentModel()
        {
            // Arrange
            var uid = "Test";
            var paymentBLLMock = _mocks.Create<IPaymentBLL>();
            paymentBLLMock.Setup(m => m.GetPaymentByUid(uid))
                .Returns(new PaymentDto());

            var controller = new PaymentController(paymentBLLMock.Object, null);

            // Act
            var res = controller.GetPaymentByUid(uid);

            // Assert
            _mocks.Verify();
            Assert.IsInstanceOfType(res.Value, typeof(PaymentModel));
        }

        #endregion GetPaymentByUid Tests

        #region GetAllPayments Tests

        [TestMethod]
        public void GetAllPayments_Returns_PaymentModelArray()
        {
            // Arrange
            var paymentBLLMock = _mocks.Create<IPaymentBLL>();
            paymentBLLMock.Setup(m => m.GetAllPayments())
                .Returns(new PaymentDto[] { new PaymentDto(), new PaymentDto() });

            var controller = new PaymentController(paymentBLLMock.Object, null);

            // Act
            var res = controller.GetAllPayments();

            // Assert
            _mocks.Verify();
            Assert.AreEqual(res.Value.Count(), 2);
            Assert.IsInstanceOfType(res.Value, typeof(PaymentModel[]));
        }

        #endregion GetAllPayments Tests
    }
}
