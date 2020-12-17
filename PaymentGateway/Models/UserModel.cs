using Framework.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.WebApi.Models
{
    public class UserModel
    {
        public string Uid { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public Permission[] Permissions { get; set; }
    }
}
