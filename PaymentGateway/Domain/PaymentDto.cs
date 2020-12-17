using Framework.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.WebApi.Domain
{
    public class PaymentDto
    {
        public long Id { get; set; }

        public string Uid { get; set; }

        public string CardNumber { get; set; }

        public string ExpiryDate { get; set; }

        public decimal? Amount { get; set; }

        public string Currency { get; set; }

        public string Cvv { get; set; }

        public DateTime PaymentDate { get; set; }

        public PaymentState State { get; set; }
    }
}
