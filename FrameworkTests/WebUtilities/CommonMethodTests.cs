using Framework.WebUtilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace FrameworkTests.WebUtilities
{
    [TestClass]
    public class CommonMethodTests
    {
        #region IsCardNumberValid Tests

        [TestMethod]
        [DataRow("4916132996393639")]
        [DataRow("6011527216256475")]
        [DataRow("4913001810469912")]
        [DataRow("375189682508716")]
        [DataRow("5515928257947837")]
        [DataRow("5018820917911893")]
        [DataRow("6378947078895243")]
        public void IsCardNumberValid_Valid_Passes(string cardNumber)
        {
            // Arrange, Act
            var res = CommonMethods.IsCardNumberValid(cardNumber);

            // Assert
            Assert.AreEqual(res, true);
        }

        [TestMethod]
        [DataRow("4916132393639")]
        [DataRow("6011527215647565")]
        [DataRow("4913001810412")]
        [DataRow("3751896825016")]
        [DataRow("5515928257947835437")]
        [DataRow("501882091791189553")]
        [DataRow("6378947078895243444333")]
        [DataRow("637894fdssdf95243444333")]
        [DataRow("637894gfddfg444333")]
        [DataRow("fdsfshhisjdofsdf")]
        public void IsCardNumberValid_Invalid_Numbers_Fails(string cardNumber)
        {
            // Arrange, Act
            var res = CommonMethods.IsCardNumberValid(cardNumber);

            // Assert
            Assert.AreEqual(res, false);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void IsCardNumberValid_Null_Or_Empty_Fails(string cardNumber)
        {
            // Arrange, Act
            var res = CommonMethods.IsCardNumberValid(cardNumber);

            // Assert
            Assert.AreEqual(res, false);
        }

        #endregion IsCardNumberValid Tests

        #region IsCvvValid Tests

        [TestMethod]
        [DataRow("323")]
        [DataRow("4432")]
        public void IsCvvValid_Length_3_Or_4_Passes(string cvv)
        {
            // Arrange, Act
            var res = CommonMethods.IsCvvValid(cvv);

            // Assert
            Assert.AreEqual(res, true);
        }

        [TestMethod]
        [DataRow("23")]
        [DataRow("43242")]
        public void IsCvvValid_Length_Less_Than_3_Or_Greater_Than_4_Fails(string cvv)
        {
            // Arrange, Act
            var res = CommonMethods.IsCvvValid(cvv);

            // Assert
            Assert.AreEqual(res, false);
        }

        [TestMethod]
        [DataRow("23f")]
        [DataRow("43s4")]
        public void IsCvvValid_Cannot_Contain_Letters(string cvv)
        {
            // Arrange, Act
            var res = CommonMethods.IsCvvValid(cvv);

            // Assert
            Assert.AreEqual(res, false);
        }

        #endregion IsCvvValid Tests

        #region MaskString Tests

        [TestMethod]
		[DataRow("43532", 4)]
		[DataRow("43532", 1)]
		[DataRow("4353243532", 6)]
		[DataRow("4353243532", 3)]
		[DataRow("43532435324353243532", 15)]
		[DataRow("43532435324353243532", 8)]
		public void MaskString_Return_Correct_Number_Of_Characters(string number, int numberOfCharsToDisplay)
		{
			// Arrange
			var @string = number;

			var stars = string.Empty;
			for (int i = 0; i < @string.Length - numberOfCharsToDisplay; i++)
			{
				stars = stars + '*';
			}

			var characterLeftPostRemoval = @string.Remove(0, stars.Length);
			var maskedString = characterLeftPostRemoval.Insert(0, stars);

			// Act
			var res = CommonMethods.MaskString(@string, numberOfCharsToDisplay);

			// Assert
			Assert.AreEqual(maskedString, res);
		}

		[TestMethod]
		[DataRow("4352", 4)]
		[DataRow("432", 4)]
		public void MaskString_If_String_Length_Less_Or_Equal_To_Character_To_Display_Length_Return_Original_String(string number, int numberOfCharsToDisplay)
		{
			// Arrange
			var @string = number;

			// Act
			var res = CommonMethods.MaskString(@string, numberOfCharsToDisplay);

			// Assert
			Assert.AreEqual(@string, res);
		}

		[TestMethod]
		public void MaskString_If_String_Null_Return_Null()
		{
			// Arrange
			string @string = null;

			// Act
			var res = CommonMethods.MaskString(@string, 4);

			// Assert
			Assert.AreEqual(@string, res);
		}

        #endregion MaskString Tests
    }
}
