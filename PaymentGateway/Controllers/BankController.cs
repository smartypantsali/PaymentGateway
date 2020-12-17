using Microsoft.AspNetCore.Mvc;
using PaymentGateway.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.WebApi.Controllers
{
    /// <summary>
    /// Controller created to simulate bank endpoint
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class BankController : ControllerBase
    {
        /// <summary>
        /// Mocking Bank endpoint
        /// </summary>
        /// <param name="payment"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult<string> ProcessPayment(PaymentModel payment)
        {
            return $"HSBC-{Guid.NewGuid()}";
        }
    }
}
