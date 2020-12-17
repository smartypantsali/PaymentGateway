using Framework.WebUtilities;
using PaymentGateway.WebApi.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.WebApi.Models
{
    public static class PaymentModelTransformer
    {
        public static PaymentModel ToModel(this PaymentDto dto)
        {
            if (dto == null)
                return null;

            return new PaymentModel
            {
                Uid = dto.Uid,
                CardNumber = CommonMethods.MaskString(dto.CardNumber, 5),
                ExpiryDate = CommonMethods.MaskString(dto.ExpiryDate, 2),
                Amount = dto.Amount,
                Currency = dto.Currency,
                Cvv = CommonMethods.MaskString(dto.Cvv, 0),
                PaymentDate = dto.PaymentDate,
                State = dto.State
            };
        }
    }
}
