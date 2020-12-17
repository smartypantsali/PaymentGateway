using PaymentGateway.WebApi.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.WebApi.Models
{
    public static class UserModelTransformer
    {
        public static UserModel ToModel(this UserDto userDto)
        {
            if (userDto == null)
                return null;

            return new UserModel
            {
                Uid = userDto.Uid,
                Username = userDto.Username,
                Permissions = userDto.Permissions
            };
        }
    }
}
