using PaymentGateway.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.WebApi.Domain
{
    /// <summary>
    /// Payment Business layer logic
    /// </summary>
    public interface IPaymentBLL
    {
        /// <summary>
        /// Create payment and send request to "bank" of card details with amount
        /// </summary>
        /// <param name="payment"></param>
        /// <returns></returns>
        Task<bool> CreatePaymentAsync(PaymentDto payment);

        /// <summary>
        /// Get payment by Uid
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        PaymentDto GetPaymentByUid(string uid);

        /// <summary>
        /// Get all payments
        /// </summary>
        /// <returns></returns>
        IEnumerable<PaymentDto> GetAllPayments();
    }
}
