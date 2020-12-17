using Framework.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.WebApi.Domain
{
    public class UserDto
    {
        public long Id { get; set; }

        public string Uid { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public Permission[] Permissions { get; set; }
    }
}
