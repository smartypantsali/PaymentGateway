using Framework.Enums;
using Framework.Interfaces;
using Framework.WebUtilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PaymentGateway.WebApi.Controllers;
using PaymentGateway.WebApi.Domain;
using PaymentGateway.WebApi.Models;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PaymentGateway.WebApiTests.Controllers
{
    [TestClass]
    public class UserControllerTests
    {
        private MockRepository _mocks = new MockRepository(MockBehavior.Strict);

        #region Login Tests

        [TestMethod]
        public void Login_Validation_Error_Returns_418()
        {
            // Arrange
            var validationProviderMock = _mocks.Create<IValidate<UserModel>>();
            validationProviderMock.Setup(m => m.Validate(It.IsAny<UserModel>()))
                .Returns(ValidationResult.ToTeapotResult(ApiOffences.Missing, "Test"));

            var controller = new UserController(null, validationProviderMock.Object, null);

            // Act
            var res = controller.Login(null).Result;

            // Assert
            _mocks.Verify();
            Assert.AreEqual(((ValidationResult)res.Result).Content.Length, 32);
            Assert.AreEqual(((ValidationResult)res.Result).StatusCode, 418);
        }

        [TestMethod]
        public void Login_Invalid_Username_Returns_418()
        {
            // Arrange
            var userModel = new UserModel
            {
                Username = "Test"
            };

            var validationProviderMock = _mocks.Create<IValidate<UserModel>>();
            validationProviderMock.Setup(m => m.Validate(It.IsAny<UserModel>()))
                .Returns<ValidationResult>(null);

            var userBLLMock = _mocks.Create<IUserBLL>();
            userBLLMock.Setup(m => m.GetUserByUsername(userModel.Username))
                .Returns<UserDto>(null);

            var controller = new UserController(userBLLMock.Object, validationProviderMock.Object, null);

            // Act
            var res = controller.Login(userModel).Result;

            // Assert
            _mocks.Verify();
            Assert.AreEqual(((ValidationResult)res.Result).Content.Length, 66);
            Assert.AreEqual(((ValidationResult)res.Result).StatusCode, 418);
        }

        [TestMethod]
        public void Login_Invalid_Password_Returns_418()
        {
            // Arrange
            var userModel = new UserModel
            {
                Username = "Test",
                Password = "Test2"
            };

            var userDto = new UserDto 
            { 
                Password = "Test3" 
            };

            var validationProviderMock = _mocks.Create<IValidate<UserModel>>();
            validationProviderMock.Setup(m => m.Validate(It.IsAny<UserModel>()))
                .Returns<ValidationResult>(null);

            var userBLLMock = _mocks.Create<IUserBLL>();
            userBLLMock.Setup(m => m.GetUserByUsername(userModel.Username))
                .Returns(userDto);

            var pswHasherMock = _mocks.Create<IPasswordHasher<UserModel>>();
            pswHasherMock.Setup(m => m.VerifyHashedPassword(It.IsAny<UserModel>(), userDto.Password, userModel.Password))
                .Returns(PasswordVerificationResult.Failed);

            var controller = new UserController(userBLLMock.Object, validationProviderMock.Object, pswHasherMock.Object);

            // Act
            var res = controller.Login(userModel).Result;

            // Assert
            _mocks.Verify();
            Assert.AreEqual(((ValidationResult)res.Result).Content.Length, 66);
            Assert.AreEqual(((ValidationResult)res.Result).StatusCode, 418);
        }

        [TestMethod]
        public void Login_Success_Return_UserModel()
        {
            // Arrange
            var userModel = new UserModel
            {
                Username = "Test",
                Password = "Test2"
            };

            var userDto = new UserDto
            {
                Password = "Test2"
            };

            var validationProviderMock = _mocks.Create<IValidate<UserModel>>();
            validationProviderMock.Setup(m => m.Validate(It.IsAny<UserModel>()))
                .Returns<ValidationResult>(null);

            var userBLLMock = _mocks.Create<IUserBLL>();
            userBLLMock.Setup(m => m.GetUserByUsername(userModel.Username))
                .Returns(userDto);

            var pswHasherMock = _mocks.Create<IPasswordHasher<UserModel>>();
            pswHasherMock.Setup(m => m.VerifyHashedPassword(It.IsAny<UserModel>(), userDto.Password, userModel.Password))
                .Returns(PasswordVerificationResult.Success);

            var authServiceMock = _mocks.Create<IAuthenticationService>();
            authServiceMock.Setup(m => m.SignOutAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<AuthenticationProperties>()))
                .Returns(Task.CompletedTask);
            authServiceMock.Setup(m => m.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.Is<ClaimsPrincipal>(m => m.Claims.Count() == 2), It.IsAny<AuthenticationProperties>()))
                .Returns(Task.CompletedTask);

            var serviceProviderMock = _mocks.Create<IServiceProvider>();
            serviceProviderMock.Setup(m => m.GetService(typeof(IAuthenticationService)))
                .Returns(authServiceMock.Object);

            var httpContextMock = _mocks.Create<HttpContext>();
            httpContextMock.SetupGet(m => m.RequestServices)
                .Returns(serviceProviderMock.Object);

            var controller = new UserController(userBLLMock.Object, validationProviderMock.Object, pswHasherMock.Object);

            // assign the fake context
            var context = new ControllerContext()
            {
                HttpContext = httpContextMock.Object
            };
            controller.ControllerContext = context;

            // Act
            var res = controller.Login(userModel).Result;

            // Assert
            _mocks.Verify();
            Assert.IsInstanceOfType(res.Value, typeof(UserModel));
        }

        #endregion Login Tests

        #region SignOut Tests

        [TestMethod]
        public void SignOut_Returns_Ok()
        {
            // Arrange
            var authServiceMock = _mocks.Create<IAuthenticationService>();
            authServiceMock.Setup(m => m.SignOutAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<AuthenticationProperties>()))
                .Returns(Task.CompletedTask);

            var serviceProviderMock = _mocks.Create<IServiceProvider>();
            serviceProviderMock.Setup(m => m.GetService(typeof(IAuthenticationService)))
                .Returns(authServiceMock.Object);

            var httpContextMock = _mocks.Create<HttpContext>();
            httpContextMock.SetupGet(m => m.RequestServices)
                .Returns(serviceProviderMock.Object);

            var controller = new UserController(null, null, null);

            var context = new ControllerContext()
            {
                HttpContext = httpContextMock.Object
            };
            controller.ControllerContext = context;

            // Act
            var res = controller.SignOut();

            // Assert
            _mocks.Verify();
            Assert.IsInstanceOfType(res.Result, typeof(OkResult));
        }

        #endregion SignOut Tests

        #region CreateUser Tests

        [TestMethod]
        public void CreateUser_Validation_Error_Returns_418()
        {
            // Arrange
            var validationProviderMock = _mocks.Create<IValidate<UserModel>>();
            validationProviderMock.Setup(m => m.Validate(It.IsAny<UserModel>()))
                .Returns(ValidationResult.ToTeapotResult(ApiOffences.Missing, "Test"));

            var controller = new UserController(null, validationProviderMock.Object, null);

            // Act
            var res = controller.CreateUser(null);

            // Assert
            _mocks.Verify();
            Assert.AreEqual(((ValidationResult)res.Result).Content.Length, 32);
            Assert.AreEqual(((ValidationResult)res.Result).StatusCode, 418);
        }

        [TestMethod]
        public void CreateUser_Username_Exists_Returns_418()
        {
            // Arrange
            var userModel = new UserModel
            {
                Username = "Test",
                Password = "Test2"
            };

            var validationProviderMock = _mocks.Create<IValidate<UserModel>>();
            validationProviderMock.Setup(m => m.Validate(It.IsAny<UserModel>()))
                .Returns<ValidationResult>(null);

            var userBLLMock = _mocks.Create<IUserBLL>();
            userBLLMock.Setup(m => m.GetUserByUsername(userModel.Username))
                .Returns(new UserDto());

            var controller = new UserController(userBLLMock.Object, validationProviderMock.Object, null);

            // Act
            var res = controller.CreateUser(userModel);

            // Assert
            _mocks.Verify();
            Assert.AreEqual(((ValidationResult)res.Result).Content.Length, 48);
            Assert.AreEqual(((ValidationResult)res.Result).StatusCode, 418);
        }

        [TestMethod]
        public void CreateUser_Failed_Return_500()
        {
            // Arrange
            var userModel = new UserModel
            {
                Username = "Test",
                Password = "Test2"
            };

            var validationProviderMock = _mocks.Create<IValidate<UserModel>>();
            validationProviderMock.Setup(m => m.Validate(It.IsAny<UserModel>()))
                .Returns<ValidationResult>(null);

            var userBLLMock = _mocks.Create<IUserBLL>();
            userBLLMock.Setup(m => m.GetUserByUsername(userModel.Username))
                .Returns<UserDto>(null);
            userBLLMock.Setup(m => m.CreateUser(It.IsAny<UserDto>()))
                .Returns(false);

            var pswHasherMock = _mocks.Create<IPasswordHasher<UserModel>>();
            pswHasherMock.Setup(m => m.HashPassword(It.IsAny<UserModel>(), userModel.Password))
                .Returns("Hashed");

            var controller = new UserController(userBLLMock.Object, validationProviderMock.Object, pswHasherMock.Object);

            // Act
            var res = controller.CreateUser(userModel);

            // Assert
            _mocks.Verify();
            Assert.IsInstanceOfType(res.Result, typeof(StatusCodeResult));
            Assert.AreEqual(((StatusCodeResult)res.Result).StatusCode, 500);
        }

        [TestMethod]
        public void CreateUser_Success_Return_UserModel()
        {
            // Arrange
            var userModel = new UserModel
            {
                Username = "Test",
                Password = "Test2"
            };

            var validationProviderMock = _mocks.Create<IValidate<UserModel>>();
            validationProviderMock.Setup(m => m.Validate(It.IsAny<UserModel>()))
                .Returns<ValidationResult>(null);

            var userBLLMock = _mocks.Create<IUserBLL>();
            userBLLMock.Setup(m => m.GetUserByUsername(userModel.Username))
                .Returns<UserDto>(null);
            userBLLMock.Setup(m => m.CreateUser(It.IsAny<UserDto>()))
                .Returns(true);

            var pswHasherMock = _mocks.Create<IPasswordHasher<UserModel>>();
            pswHasherMock.Setup(m => m.HashPassword(It.IsAny<UserModel>(), userModel.Password))
                .Returns("Hashed");

            var controller = new UserController(userBLLMock.Object, validationProviderMock.Object, pswHasherMock.Object);

            // Act
            var res = controller.CreateUser(userModel);

            // Assert
            _mocks.Verify();
            Assert.IsInstanceOfType(res.Value, typeof(UserModel));
        }

        #endregion CreateUser Tests

        #region SetPermissions Tests

        [TestMethod]
        public void SetPermissions_Unable_To_Find_User_Returns_404()
        {
            // Arrange
            var uid = "Uid";
            var userBLLMock = _mocks.Create<IUserBLL>();
            userBLLMock.Setup(m => m.GetByUserUid(uid))
                .Returns<UserDto>(null);

            var controller = new UserController(userBLLMock.Object, null, null);

            // Act
            var res = controller.SetPermissions(uid, null).Result;

            // Assert
            _mocks.Verify();
            Assert.IsInstanceOfType(res.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void SetPermissions_Update_User_Failed_Return_500()
        {
            // Arrange
            var uid = "Uid";
            var userBLLMock = _mocks.Create<IUserBLL>();
            userBLLMock.Setup(m => m.GetByUserUid(uid))
                .Returns(new UserDto());
            userBLLMock.Setup(m => m.UpdateUser(It.IsAny<UserDto>()))
                .Returns(false);

            var controller = new UserController(userBLLMock.Object, null, null);

            // Act
            var res = controller.SetPermissions(uid, null).Result;

            // Assert
            _mocks.Verify();
            Assert.AreEqual(((StatusCodeResult)res.Result).StatusCode, 500);
        }

        [TestMethod]
        [DataRow("Name")]
        [DataRow(null)]
        public void SetPermissions_Success_No_Reload_Permissions_Return_UserModel(string claimName)
        {
            // Arrange
            var uid = "Uid";
            var userBLLMock = _mocks.Create<IUserBLL>();
            userBLLMock.Setup(m => m.GetByUserUid(uid))
                .Returns(new UserDto() { Username = "Test" });
            userBLLMock.Setup(m => m.UpdateUser(It.IsAny<UserDto>()))
                .Returns(true);

            var claimsPrincipleMock = _mocks.Create<ClaimsPrincipal>();
            _ = claimName != null ? claimsPrincipleMock.Setup(m => m.FindFirst(ClaimTypes.Name))
                .Returns(new Claim(ClaimTypes.Name, claimName)) : 
                claimsPrincipleMock.Setup(m => m.FindFirst(ClaimTypes.Name))
                .Returns<Claim>(null);

            var httpContextMock = _mocks.Create<HttpContext>();
            httpContextMock.SetupGet(m => m.User)
                .Returns(claimsPrincipleMock.Object);

            var controller = new UserController(userBLLMock.Object, null, null);

            // assign the fake context
            var context = new ControllerContext()
            {
                HttpContext = httpContextMock.Object
            };
            controller.ControllerContext = context;           

            // Act
            var res = controller.SetPermissions(uid, null).Result;

            // Assert
            _mocks.Verify();
            Assert.IsInstanceOfType(res.Value, typeof(UserModel));
        }

        [TestMethod]
        public void SetPermissions_Success_With_Reload_Permissions_Return_UserModel()
        {
            // Arrange
            var uid = "Uid";
            var userBLLMock = _mocks.Create<IUserBLL>();
            userBLLMock.Setup(m => m.GetByUserUid(uid))
                .Returns(new UserDto() { Username = "Test", Permissions = new Permission[] { Permission.Payment_Create } });
            userBLLMock.Setup(m => m.UpdateUser(It.IsAny<UserDto>()))
                .Returns(true);

            var claimsIdentityMock = _mocks.Create<ClaimsIdentity>();
            claimsIdentityMock.SetupGet(m => m.AuthenticationType)
                .Returns(Constants.PaymentIdentity);
            claimsIdentityMock.Setup(m => m.RemoveClaim(It.IsAny<Claim>()));
            claimsIdentityMock.Setup(m => m.AddClaim(It.IsAny<Claim>()));

            var claimsPrincipleMock = _mocks.Create<ClaimsPrincipal>();
            claimsPrincipleMock.SetupGet(m => m.Identities)
                .Returns(new ClaimsIdentity[] { claimsIdentityMock.Object });
            claimsPrincipleMock.Setup(m => m.FindFirst(ClaimTypes.Name))
                .Returns(new Claim(ClaimTypes.Name, "Test"));
            claimsPrincipleMock.Setup(m => m.FindFirst(Constants.PermissionClaimType))
                .Returns(new Claim(ClaimTypes.Name, "Test"));

            var authServiceMock = _mocks.Create<IAuthenticationService>();
            authServiceMock.Setup(m => m.SignOutAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<AuthenticationProperties>()))
                .Returns(Task.CompletedTask);
            authServiceMock.Setup(m => m.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()))
                .Returns(Task.CompletedTask);

            var serviceProviderMock = _mocks.Create<IServiceProvider>();
            serviceProviderMock.Setup(m => m.GetService(typeof(IAuthenticationService)))
                .Returns(authServiceMock.Object);

            var httpContextMock = _mocks.Create<HttpContext>();
            httpContextMock.SetupGet(m => m.User)
                .Returns(claimsPrincipleMock.Object);
            httpContextMock.SetupGet(m => m.RequestServices)
                .Returns(serviceProviderMock.Object);

            var controller = new UserController(userBLLMock.Object, null, null);

            // assign the fake context
            var context = new ControllerContext()
            {
                HttpContext = httpContextMock.Object
            };
            controller.ControllerContext = context;

            // Act
            var res = controller.SetPermissions(uid, null).Result;

            // Assert
            _mocks.Verify();
            Assert.IsInstanceOfType(res.Value, typeof(UserModel));
        }

        #endregion SetPermission Tests

        #region GetUserByUid Tests

        [TestMethod]
        public void GetUserByUid_Unable_To_Find_User_Returns_404()
        {
            // Arrange
            var uid = "Uid";
            var userBLLMock = _mocks.Create<IUserBLL>();
            userBLLMock.Setup(m => m.GetByUserUid(uid))
                .Returns<UserDto>(null);

            var controller = new UserController(userBLLMock.Object, null, null);

            // Act
            var res = controller.GetUserByUid(uid);

            // Assert
            _mocks.Verify();
            Assert.IsInstanceOfType(res.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void GetUserByUid_Found_Returns_UserModel()
        {
            // Arrange
            var uid = "Uid";
            var userBLLMock = _mocks.Create<IUserBLL>();
            userBLLMock.Setup(m => m.GetByUserUid(uid))
                .Returns(new UserDto());

            var controller = new UserController(userBLLMock.Object, null, null);

            // Act
            var res = controller.GetUserByUid(uid);

            // Assert
            _mocks.Verify();
            Assert.IsInstanceOfType(res.Value, typeof(UserModel));
        }

        #endregion GetUserByUid Tests

        #region GetAllUsers Tests

        [TestMethod]
        public void GetUsers_Returns_UserModelModelArray()
        {
            // Arrange
            var userBLLMock = _mocks.Create<IUserBLL>();
            userBLLMock.Setup(m => m.GetAllUsers())
                .Returns(new UserDto[] { new UserDto(), new UserDto() });

            var controller = new UserController(userBLLMock.Object, null, null);

            // Act
            var res = controller.GetAllUsers();

            // Assert
            _mocks.Verify();
            Assert.AreEqual(res.Value.Count(), 2);
            Assert.IsInstanceOfType(res.Value, typeof(UserModel[]));
        }

        #endregion GetAllUsers Tests
    }
}
