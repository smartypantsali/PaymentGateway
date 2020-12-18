using Framework.Configuration;
using Framework.DbContext;
using Framework.TestHelpers;
using Framework.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PaymentGateway.WebApi.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentGateway.WebApiTests.Domain
{
    [TestClass]
    public class PaymentBLLTests
    {
        private MockRepository _mocks = new MockRepository(MockBehavior.Strict);

        #region CreatePaymentAsync Tests

        [TestMethod]
        public void CreatePaymentAsync_Payment_Insert_Failed_Returns_False()
        {
            // Arrange
            var bankUrl = "someUrl";

            var configRootMock = new Mock<IConfigurationSection>();
            configRootMock.SetupGet(m => m.Key)
                .Returns("ExternalBankUri");
            configRootMock.SetupGet(m => m.Value)
                .Returns(bankUrl);

            var configurationSections = new IConfigurationSection[1]
            {
                configRootMock.Object,
            };

            PGConfiguration.SetKeyValues(configurationSections);

            var dbContextMock = _mocks.Create<IDatabaseContext>();
            dbContextMock.Setup(m => m.Insert(It.IsAny<PaymentDto>()))
                .Returns(-1);

            var responseMessage = new HttpResponseMessage();
            responseMessage.StatusCode = HttpStatusCode.OK;
            responseMessage.Content = new FakeHttpContent("Fake");

            var httpClientWrapperMock = _mocks.Create<IHttpClientWrapper>();
            httpClientWrapperMock.Setup(m => m.PostAsync(bankUrl, It.IsAny<StringContent>()))
                .Returns(Task.FromResult(responseMessage));
            httpClientWrapperMock.Setup(m => m.Dispose());

            var paymentBLL = new PaymentBLL(dbContextMock.Object, httpClientWrapperMock.Object);

            // Act
            var res = paymentBLL.CreatePaymentAsync(new PaymentDto()).Result;

            // Assert
            _mocks.Verify();
            Assert.AreEqual(res, false);
        }

        [TestMethod]
        public void CreatePaymentAsync_Status_Code_Not_200_Returns_False()
        {
            // Arrange
            var bankUrl = "someUrl";

            var configRootMock = new Mock<IConfigurationSection>();
            configRootMock.SetupGet(m => m.Key)
                .Returns("ExternalBankUri");
            configRootMock.SetupGet(m => m.Value)
                .Returns(bankUrl);

            var configurationSections = new IConfigurationSection[1]
            {
                configRootMock.Object,
            };

            PGConfiguration.SetKeyValues(configurationSections);

            var responseMessage = new HttpResponseMessage();
            responseMessage.StatusCode = HttpStatusCode.NotFound;
            responseMessage.Content = new FakeHttpContent("Fake");

            var httpClientWrapperMock = _mocks.Create<IHttpClientWrapper>();
            httpClientWrapperMock.Setup(m => m.PostAsync(bankUrl, It.IsAny<StringContent>()))
                .Returns(Task.FromResult(responseMessage));
            httpClientWrapperMock.Setup(m => m.Dispose());

            var paymentBLL = new PaymentBLL(null, httpClientWrapperMock.Object);

            // Act
            var res = paymentBLL.CreatePaymentAsync(new PaymentDto()).Result;

            // Assert
            _mocks.Verify();
            Assert.AreEqual(res, false);
        }

        [TestMethod]
        public void CreatePaymentAsync_Payment_Inserted_Returns_true()
        {
            // Arrange
            var bankUrl = "someUrl";

            var configRootMock = new Mock<IConfigurationSection>();
            configRootMock.SetupGet(m => m.Key)
                .Returns("ExternalBankUri");
            configRootMock.SetupGet(m => m.Value)
                .Returns(bankUrl);

            var configurationSections = new IConfigurationSection[1]
            {
                configRootMock.Object,
            };

            PGConfiguration.SetKeyValues(configurationSections);

            var dbContextMock = _mocks.Create<IDatabaseContext>();
            dbContextMock.Setup(m => m.Insert(It.IsAny<PaymentDto>()))
                .Returns(1);

            var responseMessage = new HttpResponseMessage();
            responseMessage.StatusCode = HttpStatusCode.OK;
            responseMessage.Content = new FakeHttpContent("Fake");

            var httpClientWrapperMock = _mocks.Create<IHttpClientWrapper>();
            httpClientWrapperMock.Setup(m => m.PostAsync(bankUrl, It.IsAny<StringContent>()))
                .Returns(Task.FromResult(responseMessage));
            httpClientWrapperMock.Setup(m => m.Dispose());

            var paymentBLL = new PaymentBLL(dbContextMock.Object, httpClientWrapperMock.Object);

            // Act
            var res = paymentBLL.CreatePaymentAsync(new PaymentDto()).Result;

            // Assert
            _mocks.Verify();
            Assert.AreEqual(res, true);
        }

        #endregion CreatePaymentAsync Tests

        #region GetPaymentByUid Tests

        [TestMethod]
        public void GetPaymentByUid_Returns_PaymentDto()
        {
            // Arrange
            var uid = "uid";
            var dbContextMock = _mocks.Create<IDatabaseContext>();
            dbContextMock.Setup(m => m.GetByUid<PaymentDto>(uid))
                .Returns(new PaymentDto());

            var paymentBLL = new PaymentBLL(dbContextMock.Object, null);

            // Act
            var res = paymentBLL.GetPaymentByUid(uid);

            // Assert
            _mocks.Verify();
            Assert.IsInstanceOfType(res, typeof(PaymentDto));
        }

        #endregion GetPaymentByUid Tests

        #region GetAllPayments Tests

        [TestMethod]
        public void GetAllPayments_Returns_PaymentDtoArray()
        {
            // Arrange
            var dbContextMock = _mocks.Create<IDatabaseContext>();
            dbContextMock.Setup(m => m.GetAll<PaymentDto>())
                .Returns(new PaymentDto[] { new PaymentDto(), new PaymentDto() });

            var paymentBLL = new PaymentBLL(dbContextMock.Object, null);

            // Act
            var res = paymentBLL.GetAllPayments();

            // Assert
            _mocks.Verify();
            Assert.AreEqual(res.Count(), 2);
            Assert.IsInstanceOfType(res, typeof(PaymentDto[]));
        }

        #endregion GetAllPayments Tests
    }
}
