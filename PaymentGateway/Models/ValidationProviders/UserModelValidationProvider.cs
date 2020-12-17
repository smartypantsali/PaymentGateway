using Framework.Interfaces;
using Framework.WebUtilities;
using PaymentGateway.WebApi.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.WebApi.Models.ValidationProviders
{
    /// <summary>
    /// Class to validate UserModel
    /// </summary>
    public class UserModelValidationProvider : IValidate<UserModel>
    {
        /// <summary>
        /// Validate UserModel
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ValidationResult Validate(UserModel model)
        {
            var validationResults = new Queue<ValidationResult>();

            // Check if username is missing
            if (string.IsNullOrWhiteSpace(model.Username))
            {
                validationResults.Enqueue(ValidationResult.ToTeapotResult(ApiOffences.Missing, nameof(model.Username)));
            }

            // Check if password is missing
            if (string.IsNullOrWhiteSpace(model.Password))
            {
                validationResults.Enqueue(ValidationResult.ToTeapotResult(ApiOffences.Missing, nameof(model.Password)));
            }
            // Check if password is at least 4 characters long
            else if (model.Password.Length < 4)
            {
                validationResults.Enqueue(ValidationResult.ToTeapotResult(ApiOffences.User.AtLeastFourCharacters, nameof(model.Password)));
            }

            return ValidationResult.GetUnifiedTeapotValidationResults(validationResults);
        }
    }
}
