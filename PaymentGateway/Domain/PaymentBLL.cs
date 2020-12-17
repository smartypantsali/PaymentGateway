using Framework.Configuration;
using Framework.DbContext;
using Framework.Enums;
using Framework.WebUtilities;
using Newtonsoft.Json;
using PaymentGateway.WebApi.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace PaymentGateway.WebApi.Domain
{
    /// <summary>
    /// Payment Business layer logic
    /// </summary>
    public class PaymentBLL : IPaymentBLL
    {
        private readonly IDatabaseContext _db;
        private readonly IHttpClientWrapper _httpClient;

        private readonly string _bankUri;

        public PaymentBLL(IDatabaseContext db, IHttpClientWrapper httpClient)
        {
            _db = db;
            _httpClient = httpClient;
            _bankUri = PGConfiguration.TryGetValue("ExternalBankUri");
        }

        /// <summary>
        /// Create payment and send request to "bank" of card details with amount
        /// </summary>
        /// <param name="payment"></param>
        /// <returns></returns>
        public async Task<bool> CreatePaymentAsync(PaymentDto payment)
        {
            Log.Information($"Connecting with bank to process payment");
            using (var req = _httpClient.PostAsync(_bankUri, new StringContent(JsonConvert.SerializeObject(new UserModel()).ToString(), Encoding.UTF8, "application/json")))
            {
                var bankResponse = await req;
                switch (bankResponse.StatusCode)
                {
                    case HttpStatusCode.OK:
                        Log.Information("Payment has been successfully made");
                        var result = await bankResponse.Content.ReadAsStringAsync();

                        payment.Uid = result;
                        payment.State = PaymentState.Completed;
                        // Insert payment into database
                        var res = _db.Insert(payment) > 0;
                        if (!res)
                        {
                            Log.Error("Unknown error occured when inserting payment into DB");
                            return false;
                        }
                        break;
                    default:
                        Log.Error($"Unknown error code with status code: {bankResponse.StatusCode} and TraceIdentifier: {bankResponse.Headers.GetValues("TraceIdentifier")?.ElementAt(0)}");
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Get payment by Uid
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public PaymentDto GetPaymentByUid(string uid)
        {
            return _db.GetByUid<PaymentDto>(uid);
        }

        /// <summary>
        /// Get all payments
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PaymentDto> GetAllPayments()
        {
            return _db.GetAll<PaymentDto>();
        }
    }
}
