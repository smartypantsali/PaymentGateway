using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Framework.Enums;
using Framework.Interfaces;
using Framework.WebUtilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentGateway.WebApi.Domain;
using PaymentGateway.WebApi.Models;

namespace PaymentGateway.WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentBLL _paymentBLL;
        private readonly IValidate<PaymentModel> _paymentModelValidationProvider;

        public PaymentController(IPaymentBLL paymentBLL, IValidate<PaymentModel> paymentModelValidationProvider)
        {
            _paymentBLL = paymentBLL;
            _paymentModelValidationProvider = paymentModelValidationProvider;
        }

        /// <summary>
        /// Endpoint to create payment
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("")]
        [RequireApiPermission(Permission.Payment_View, Permission.Payment_Create)]
        public async Task<ActionResult<PaymentModel>> CreatePaymentAsync(PaymentModel model)
        { 
            var validationResult = _paymentModelValidationProvider.Validate(model);
            if (validationResult != null)
            {
                return validationResult;
            }

            var dto = new PaymentDto
            {
                CardHolderName = model.CardHolderName,
                CardNumber = model.CardNumber,
                ExpiryDate = model.ExpiryDate,
                Amount = model.Amount,
                Currency = model.Currency,
                Cvv = model.Cvv,
                PaymentDate = DateTime.Now,
                State = PaymentState.New
            };

            var res = await _paymentBLL.CreatePaymentAsync(dto);
            if (!res)
            {
                return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
            }

            return dto.ToModel();
        }

        /// <summary>
        /// Get payments by Uid
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        [HttpGet("{uid}")]
        [RequireApiPermission(Permission.Payment_View)]
        public ActionResult<PaymentModel> GetPaymentByUid(string uid)
        {
            var dto = _paymentBLL.GetPaymentByUid(uid);
            if (dto == null)
            {
                return NotFound();
            }

            return dto.ToModel();
        }

        /// <summary>
        /// Get all payments
        /// </summary>
        /// <returns></returns>
        [HttpGet("all")]
        [RequireApiPermission(Permission.Payment_View)]
        public ActionResult<IEnumerable<PaymentModel>> GetAllPayments()
        {
            var dtos = _paymentBLL.GetAllPayments();

            return dtos.Select(dto => dto.ToModel()).ToArray();
        }
    }
}
