using Framework.Enums;
using Framework.Interfaces;
using Framework.WebUtilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PaymentGateway.WebApi.Domain;
using PaymentGateway.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace PaymentGateway.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserBLL _userBLL;
        private readonly IValidate<UserModel> _userModelValidationProvider;
        private readonly IPasswordHasher<UserModel> _passwordHasher;

        public UserController(IUserBLL userBLL, IValidate<UserModel> userModelValidationProvider, IPasswordHasher<UserModel> passwordHasher)
        {
            _userBLL = userBLL;
            _userModelValidationProvider = userModelValidationProvider;
            _passwordHasher = passwordHasher;
        }

        /// <summary>
        /// Method to login to created user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<UserModel>> Login(UserModel model)
        {
            var validationResult = _userModelValidationProvider.Validate(model);
            if (validationResult != null)
            {
                return validationResult;

            }
            var userDto = _userBLL.GetUserByUsername(model.Username);
            if (userDto == null)
            {
                return ValidationResult.ToTeapotResult(ApiOffences.User.InvalidUsernameOrPassword, "Username_Password");
            }

            var verificationResult = _passwordHasher.VerifyHashedPassword(model, userDto.Password, model.Password);
            if (verificationResult == PasswordVerificationResult.Failed)
            {
                return ValidationResult.ToTeapotResult(ApiOffences.User.InvalidUsernameOrPassword, "Username_Password");
            }

            // Destroy any existing authentication
            await HttpContext.SignOutAsync();

            var jsonPermissions = JsonSerializer.Serialize(userDto.Permissions);
            var paymentClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, model.Username),
                new Claim(Constants.PermissionClaimType, jsonPermissions ?? string.Empty)
            };

            var paymentIdentity = new ClaimsIdentity(paymentClaims, Constants.PaymentIdentity);

            var userPrincipal = new ClaimsPrincipal(new[] { paymentIdentity });

            await HttpContext.SignInAsync(userPrincipal);

            return userDto.ToModel();
        }

        /// <summary>
        /// Method to SignOut of platform
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet("signout")]
        [Authorize]
        public async Task<ActionResult> SignOut()
        {
            // Destroy any existing authentication
            await HttpContext.SignOutAsync();

            return Ok();
        }

        /// <summary>
        /// Create a user before being able to login
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("create")]
        [AllowAnonymous]
        public ActionResult<UserModel> CreateUser(UserModel model)
        {
            var validationResult = _userModelValidationProvider.Validate(model);
            if (validationResult != null)
            {
                return validationResult;
            }

            if (_userBLL.GetUserByUsername(model.Username) != null)
            {
                return ValidationResult.ToTeapotResult(ApiOffences.User.NameAlreadyExists, nameof(model.Username));
            }

            // Hash password
            var hashedPassword = _passwordHasher.HashPassword(model, model.Password);

            var userDto = new UserDto
            {
                Uid = Guid.NewGuid().ToString(),
                Username = model.Username,
                Password = hashedPassword,
                Permissions = model.Permissions
            };

            var res = _userBLL.CreateUser(userDto);
            if (!res)
            {
                return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
            }

            return userDto.ToModel();
        }

        /// <summary>
        /// Method to update User permissions
        /// </summary>
        /// <param name="permissions"></param>
        /// <returns></returns>
        [HttpPatch("{uid}/permissions/assign")]
        [Authorize]
        public async Task<ActionResult<UserModel>> SetPermissions([FromRoute] string uid, [FromBody] Permission[] permissions)
        {
            var userDto = _userBLL.GetByUserUid(uid);
            if (userDto == null)
            {
                return NotFound();
            }

            // Update permissions
            userDto.Permissions = permissions;
            var hasUpdated = _userBLL.UpdateUser(userDto);
            if (!hasUpdated)
            {
                return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
            }

            // Reload HttpContext by signing out then in again to reset Permissions in Claim
            var claimantsName = User.FindFirst(ClaimTypes.Name)?.Value;
            if (claimantsName != null && claimantsName == userDto.Username)
            {
                // Remove previous permission claim
                var permissionClaim = User.FindFirst(Constants.PermissionClaimType);
                User.Identities.FirstOrDefault(i => i.AuthenticationType == Constants.PaymentIdentity)?.RemoveClaim(permissionClaim);

                // Add new permission claim
                var jsonPermissions = JsonSerializer.Serialize(userDto.Permissions);
                var newPermissionClaim = new Claim(Constants.PermissionClaimType, jsonPermissions ?? string.Empty);
                User.Identities.FirstOrDefault(i => i.AuthenticationType == Constants.PaymentIdentity)?.AddClaim(newPermissionClaim);

                // Destroy any existing authentication and then re-login
                await HttpContext.SignOutAsync();
                await HttpContext.SignInAsync(User);
            }

            return userDto.ToModel();
        }

        /// <summary>
        /// Get users by Uid
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        [HttpGet("{uid}")]
        [Authorize]
        public ActionResult<UserModel> GetUserByUid(string uid)
        {
            var userDto = _userBLL.GetByUserUid(uid);
            if (userDto == null)
            {
                return NotFound();
            }

            return userDto.ToModel();
        }

        /// <summary>
        /// Get all users 
        /// </summary>
        /// <returns></returns>
        [HttpGet("all")]
        [Authorize]
        public ActionResult<IEnumerable<UserModel>> GetAllUsers()
        {
            return _userBLL.GetAllUsers().Select(m => m.ToModel()).ToArray();
        }
    }
}
