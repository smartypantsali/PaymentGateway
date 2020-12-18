using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FrameworkTests.WebUtilities
{
    [TestClass]
    public class ExceptionHandlerMiddlewareTests
    {
        private MockRepository _mocks = new MockRepository(MockBehavior.Strict);

        #region Invoke Tests

        [TestMethod]
        public void Invoke_Return_Exception_Object()
        {
            // Arrange
            var defaultHttpContext = new DefaultHttpContext();

            var exceptionHandlerFeatMock = _mocks.Create<IExceptionHandlerFeature>();
            exceptionHandlerFeatMock.SetupGet(m => m.Error)
                .Returns(new NullReferenceException("Error"));

            var featureCollMock = _mocks.Create<IFeatureCollection>();
            featureCollMock.Setup(m => m.Get<IExceptionHandlerFeature>())
                .Returns(exceptionHandlerFeatMock.Object);

            defaultHttpContext.Request.Scheme = "Scheme";

            var identityMock = _mocks.Create<IIdentity>();
            identityMock.Setup(m => m.Name)
                .Returns("Steve");

            var claimsPrincipleMock = _mocks.Create<ClaimsPrincipal>();
            claimsPrincipleMock.SetupGet(m => m.Identity)
                .Returns(identityMock.Object);

            var httpContext = _mocks.Create<HttpContext>();
            httpContext.SetupGet(m => m.Features)
                .Returns(featureCollMock.Object);
            httpContext.SetupGet(m => m.Response)
                .Returns(defaultHttpContext.Response);
            httpContext.Setup(m => m.User)
                .Returns(claimsPrincipleMock.Object);
            httpContext.Setup(m => m.TraceIdentifier)
                .Returns("TraceId");
            httpContext.SetupGet(m => m.Request)
                .Returns(defaultHttpContext.Request);

            var instance = new Framework.WebUtilities.ExceptionHandlerMiddleware((cxt) => Task.CompletedTask);

            // Act, Assert
            instance.Invoke(httpContext.Object).GetAwaiter().GetResult();
            _mocks.Verify();
            Assert.AreEqual(defaultHttpContext.Response.StatusCode, 500);
        }

        #endregion Invoke Tests
    }
}
