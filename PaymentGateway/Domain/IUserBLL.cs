using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.WebApi.Domain
{
    /// <summary>
    /// User Business layer logic
    /// </summary>
    public interface IUserBLL
    {
        /// <summary>
        /// Method to create a user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        bool CreateUser(UserDto user);

        /// <summary>
        /// Method to update user details
        /// </summary>
        /// <param name="userDto"></param>
        /// <returns></returns>
        bool UpdateUser(UserDto userDto);

        /// <summary>
        /// Method to get user by username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        UserDto GetUserByUsername(string username);

        /// <summary>
        /// Method to get user by uid
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        UserDto GetByUserUid(string uid);

        /// <summary>
        /// Get all users
        /// </summary>
        /// <returns></returns>
        IEnumerable<UserDto> GetAllUsers();
    }
}
