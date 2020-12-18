using Framework.Enums;
using Framework.WebUtilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;

namespace FrameworkTests.WebUtilities
{
    [TestClass]
    public class RequireApiPermissionAttributeTests
    {
        class TestController : Controller
        {

        }

        #region OnActionExecuting Tests

        [TestMethod]
        [DataRow(Permission.Payment_Create, null)]
        [DataRow(null, Permission.Payment_View)]
        [DataRow(Permission.Payment_Create, Permission.Payment_View)]
        public void OnActionExecuting_User_Has_All_Perms_Does_Not_Return_403(Permission? p1, Permission? p2)
        {
            // Arrange
            var permissions = new List<Permission>();
            if (p1 != null)
            {
                permissions.Add(p1.Value);
            }
            if (p2 != null)
            {
                permissions.Add(p2.Value);
            }

            var permArray = permissions.ToArray();

            var jsonPermissions = JsonSerializer.Serialize(permArray);
            var paymentClaims = new List<Claim>()
            {
                new Claim(Constants.PermissionClaimType, jsonPermissions)
            };
            var paymentIdentity = new ClaimsIdentity(paymentClaims, Constants.PaymentIdentity);
            var userPrincipal = new ClaimsPrincipal(new[] { paymentIdentity });

            var defaultHttpContext = new DefaultHttpContext();
            defaultHttpContext.User = userPrincipal;

            var actExecutingContext = new ActionExecutingContext(new ActionContext(defaultHttpContext, new RouteData(), new ActionDescriptor()), new IFilterMetadata[0], new Dictionary<string, object>(), new TestController());

            var reqPermAttr = new RequireApiPermissionAttribute(permArray);

            // Act
            reqPermAttr.OnActionExecuting(actExecutingContext);

            // Assert
            Assert.AreEqual(actExecutingContext.Result, null);
        }

        [TestMethod]
        [DataRow(Permission.Payment_Create, null)]
        [DataRow(null, Permission.Payment_View)]
        [DataRow(null, null)]
        public void OnActionExecuting_User_Has_Missing_Perms_Returns_403(Permission? p1, Permission? p2)
        {
            // Arrange
            var permissions = new List<Permission>();
            if (p1 != null)
            {
                permissions.Add(p1.Value);
            }
            if (p2 != null)
            {
                permissions.Add(p2.Value);
            }

            var jsonPermissions = JsonSerializer.Serialize(permissions);
            var paymentClaims = new List<Claim>()
            {
                new Claim(Constants.PermissionClaimType, jsonPermissions)
            };
            var paymentIdentity = new ClaimsIdentity(paymentClaims, Constants.PaymentIdentity);
            var userPrincipal = new ClaimsPrincipal(new[] { paymentIdentity });

            var defaultHttpContext = new DefaultHttpContext();
            defaultHttpContext.User = userPrincipal;

            var actExecutingContext = new ActionExecutingContext(new ActionContext(defaultHttpContext, new RouteData(), new ActionDescriptor()), new IFilterMetadata[0], new Dictionary<string, object>(), new TestController());

            var reqPermAttr = new RequireApiPermissionAttribute(Permission.Payment_Create, Permission.Payment_View);

            // Act
            reqPermAttr.OnActionExecuting(actExecutingContext);

            // Assert
            Assert.AreEqual(((StatusCodeResult)actExecutingContext.Result).StatusCode, 403);
        }

        #endregion OnActionExecuting Tests
    }
}
