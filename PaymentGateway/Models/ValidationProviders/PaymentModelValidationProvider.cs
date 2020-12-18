using Framework.Configuration;
using Framework.Interfaces;
using Framework.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PaymentGateway.WebApi.Models.ValidationProviders
{
    /// <summary>
    /// Class to validate PaymentModel
    /// </summary>
    public class PaymentModelValidationProvider : IValidate<PaymentModel>
    {
        private readonly decimal _minAmount;
        private readonly decimal _maxAmount;

        private readonly HashSet<string> _supportedCurrencies;

        public PaymentModelValidationProvider()
        {
            // Initialise config values to validate model, defaults are assigned if not found
            _minAmount = decimal.TryParse(PGConfiguration.TryGetValue("MinAmount"), out var minAmount) ? minAmount : 0;
            _maxAmount = decimal.TryParse(PGConfiguration.TryGetValue("MaxAmount"), out var maxAmount) ? maxAmount : 10000;
            _supportedCurrencies = PGConfiguration.TryGetValue("SupportedCurrencies")?
                                        .Split(',', StringSplitOptions.RemoveEmptyEntries).Select(str => str.Trim()).ToHashSet() ?? new HashSet<string>() { "GBP", "EUR", "USD" };
        }
        
        /// <summary>
        /// Validate PaymentModel object from request
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ValidationResult Validate(PaymentModel model)
        {
            var validationResults = new Queue<ValidationResult>();

            // Check card number is valid
            if (string.IsNullOrWhiteSpace(model.CardNumber))
            {
                validationResults.Enqueue(ValidationResult.ToTeapotResult(ApiOffences.Missing, nameof(model.CardNumber)));
            }
            else if (!CommonMethods.IsCardNumberValid(model.CardNumber)) 
            {
                validationResults.Enqueue(ValidationResult.ToTeapotResult(ApiOffences.Payment.InvalidCardNumber, nameof(model.CardNumber)));
            }

            // Check if Cvv is valid
            if (string.IsNullOrWhiteSpace(model.Cvv))
            {
                validationResults.Enqueue(ValidationResult.ToTeapotResult(ApiOffences.Missing, nameof(model.Cvv)));
            }
            else if (!CommonMethods.IsCvvValid(model.Cvv))
            {
                validationResults.Enqueue(ValidationResult.ToTeapotResult(ApiOffences.Payment.InvalidCVV, nameof(model.Cvv)));
            }

            // Check if CardHolderName is missing
            if (string.IsNullOrWhiteSpace(model.CardHolderName))
            {
                validationResults.Enqueue(ValidationResult.ToTeapotResult(ApiOffences.Missing, nameof(model.CardHolderName)));
            }

            // Check if ExpiryDate is valid
            if (!string.IsNullOrWhiteSpace(model.ExpiryDate))
            {
                // Check ExpiryDate is in correct format
                if (!DateTime.TryParse(model.ExpiryDate, out var dateTime))
                {
                    validationResults.Enqueue(ValidationResult.ToTeapotResult(ApiOffences.Payment.ExpiryDateIncorrectFormat, nameof(model.ExpiryDate)));
                }
                else
                {
                    // Check ExpiryDate is valid
                    if (dateTime.CompareTo(DateTime.UtcNow.Date) < 0)
                    {
                        validationResults.Enqueue(ValidationResult.ToTeapotResult(ApiOffences.Payment.ExpiryDateInThePast, nameof(model.ExpiryDate)));
                    }
                }               
            }
            else
            {
                validationResults.Enqueue(ValidationResult.ToTeapotResult(ApiOffences.Missing, nameof(model.ExpiryDate)));
            }

            // Check if Amount is valid
            if (model.Amount != null)
            {
                // Check if maxAmount has been exceeded
                if (model.Amount > _maxAmount)
                {
                    validationResults.Enqueue(ValidationResult.ToTeapotResult(ApiOffences.Payment.AmountTooHigh, nameof(model.Amount)));
                }
                // Check if Amount is below minAmount
                if (model.Amount < _minAmount)
                {
                    validationResults.Enqueue(ValidationResult.ToTeapotResult(ApiOffences.Payment.AmountTooLow, nameof(model.Amount)));
                }               
            }
            else
            {
                validationResults.Enqueue(ValidationResult.ToTeapotResult(ApiOffences.Missing, nameof(model.Amount)));
            }

            // Check if Currency is valid
            if (string.IsNullOrWhiteSpace(model.Currency))
            {
                validationResults.Enqueue(ValidationResult.ToTeapotResult(ApiOffences.Missing, nameof(model.Currency)));
            }
            else if (!_supportedCurrencies.TryGetValue(model.Currency, out _))
            {
                validationResults.Enqueue(ValidationResult.ToTeapotResult(ApiOffences.Payment.CurrenyNotSupported, nameof(model.Currency)));
            }

            return ValidationResult.GetUnifiedTeapotValidationResults(validationResults);
        }
    }
}
