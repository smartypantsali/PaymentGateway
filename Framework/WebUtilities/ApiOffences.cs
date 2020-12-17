using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.WebUtilities
{
    /// <summary>
    /// List of offenses that can be returned by API, Struct can be used by Front-end as reference
    /// </summary>
    public struct ApiOffences
    {
        /// <summary>
        /// Generic missing error
        /// </summary>
        public static ApiOffence Missing = "missing";

        public struct Payment
        {
            /// <summary>
            /// Amount too low
            /// </summary>
            public static ApiOffence AmountTooLow = "amount_too_low";

            /// <summary>
            /// Amount too high
            /// </summary>
            public static ApiOffence AmountTooHigh = "amount_too_high";

            /// <summary>
            /// Invalid card number
            /// </summary>
            public static ApiOffence InvalidCardNumber = "invalid_card_number";

            /// <summary>
            /// ExpiryDate in the past
            /// </summary>
            public static ApiOffence ExpiryDateInThePast = "expirydate_in_the_past";

            /// <summary>
            /// Incorrect ExpiryDate format
            /// </summary>
            public static ApiOffence ExpiryDateIncorrectFormat = "incorrect_format";

            /// <summary>
            /// Currency not supported
            /// </summary>
            public static ApiOffence CurrenyNotSupported = "currency_not_supported";

            /// <summary>
            /// Invalid CVV
            /// </summary>
            public static ApiOffence InvalidCVV = "invalid_cvv";
        }

        public struct User
        {
            /// <summary>
            /// Missing at least four characters
            /// </summary>
            public static ApiOffence AtLeastFourCharacters = "must_be_at_least_four_characters";

            /// <summary>
            /// Name already exists
            /// </summary>
            public static ApiOffence NameAlreadyExists = "name_already_exists";

            /// <summary>
            /// Invalid Username or Password
            /// </summary>
            public static ApiOffence InvalidUsernameOrPassword = "invalid_username_or_password";
        }
    }
}
