using Framework.DbContext;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.WebApi.Domain
{
    /// <summary>
    /// User Business layer logic
    /// </summary>
    public class UserBLL : IUserBLL
    {
        private readonly IDatabaseContext _db;

        public UserBLL(IDatabaseContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Method to create a user
        /// </summary>
        /// <param name="userDto"></param>
        /// <returns></returns>
        public bool CreateUser(UserDto userDto)
        {
            var res = _db.Insert(userDto) > 0;

            if (!res)
            {
                Log.Error("Unknown error occured when inserting user into DB");
            }
            return res;
        }

        /// <summary>
        /// Method to update user details
        /// </summary>
        /// <param name="userDto"></param>
        /// <returns></returns>
        public bool UpdateUser(UserDto userDto)
        {
            return _db.Update(userDto);
        }

        /// <summary>
        /// Get all users
        /// </summary>
        /// <returns></returns>
        public IEnumerable<UserDto> GetAllUsers()
        {
            return _db.GetAll<UserDto>();
        }

        /// <summary>
        /// Get user by Uid
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public UserDto GetByUserUid(string uid)
        {
            return _db.GetByUid<UserDto>(uid);
        }

        /// <summary>
        /// Method to get user by username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public UserDto GetUserByUsername(string username)
        {
            return _db.GetAll<UserDto>()
                        .FirstOrDefault(dto => dto.Username == username);
        }
    }
}
