using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.WebUtilities
{
    /// <summary>
    /// Common methods which can be used by Application
    /// </summary>
    public static class CommonMethods
    {
        /// <summary>
        /// Method to validate if card is valid instructions using Luhn formula (https://www.freeformatter.com/credit-card-number-generator-validator.html)
        /// My own formula to check
        /// </summary>
        /// <param name="creditCardNumber"></param>
        /// <returns></returns>
        public static bool IsCardNumberValid(string creditCardNumber)
        {
            // check whether input string is null or empty
            if (string.IsNullOrWhiteSpace(creditCardNumber))
            {
                return false;
            }

            // 1. Drop the last digit from the number. The last digit is what we want to check against
            var noLastDigit = creditCardNumber.Remove(creditCardNumber.Length - 1);

            // 2. Reverse numbers
            var reversedNumbers = noLastDigit.Reverse().ToArray();

            // 3. Multiply digits in odd positions by 2 and subtract 9 to all any result higher than 9
            string calculatedNumbers = string.Empty;
            for (int i = 0; i <= reversedNumbers.Length - 1; i++)
            {
                var position = i + 1;
                var intNumber = reversedNumbers[i] - 48;
                if (position % 2 == 1)
                {               
                    var numberMultipliedBy2 = intNumber * 2;
                    if (numberMultipliedBy2 > 9)
                    {
                        calculatedNumbers += (numberMultipliedBy2 - 9);
                        continue;
                    }
                    calculatedNumbers += numberMultipliedBy2;
                    continue;
                }
                calculatedNumbers += intNumber;
            }

            // 4. Add all numbers together
            var sum = calculatedNumbers.Sum(e => (e - 48));

            // 5. The check digit (the last number of the card) is the amount that you would need to add to get a multiple of 10 (Modulo 10)
            return sum % 10 == (10 - (creditCardNumber[creditCardNumber.Length - 1] - 48));
        }

        /// <summary>
        /// Check if Cvv is valid
        /// </summary>
        /// <param name="cvv"></param>
        /// <returns></returns>
        public static bool IsCvvValid(string cvv)
        {
            return (cvv.Length >= 3 && cvv.Length <= 4)
                && cvv.Select(c =>
                {
                    var val = c - 48;
                    return val >= 0 && val <= 9;
                }).All(b => b);
        }

        /// <summary>
        /// Used to mask a string with stars and return specific number of chars at the end
        /// </summary>
        /// <param name="string">string to mask</param>
        /// <param name="numberOfEndCharsToDisplay">number of characters to display after stars</param>
        /// <returns></returns>
        public static string MaskString(string @string, int numberOfEndCharsToDisplay)
        {
            if (@string == null)
            {
                return null;
            }

            if (@string.Length <= numberOfEndCharsToDisplay)
            {
                return @string;
            }

            var stars = string.Empty;
            for (int i = 0; i < @string.Length - numberOfEndCharsToDisplay; i++)
            {
                stars += '*';
            }

            @string = @string.Remove(0, stars.Length);
            @string = @string.Insert(0, stars);

            return @string;
        }
    }
}
