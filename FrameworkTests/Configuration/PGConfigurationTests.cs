using Framework.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace FrameworkTests.Configuration
{
    [TestClass]
    public class PGConfigurationTests
    {
        #region SetKeyValuesAndTryGetValue Tests

        [TestMethod]
        public void SetKeyValues_Key_Values_Set()
        {
            var key1 = "Key1";
            var value1 = "Value1";
            var key2 = "Key2";
            var value2 = "Value2";

            var configRootMock = new Mock<IConfigurationSection>();
            configRootMock.SetupGet(m => m.Key)
                .Returns(key1);
            configRootMock.SetupGet(m => m.Value)
                .Returns(value1);

            var configRootMock2 = new Mock<IConfigurationSection>();
            configRootMock2.SetupGet(m => m.Key)
                .Returns(key2);
            configRootMock2.SetupGet(m => m.Value)
                .Returns("Value2");

            var configurationSections = new IConfigurationSection[2]
            {
                configRootMock.Object,
                configRootMock2.Object
            };

            // Act
            PGConfiguration.SetKeyValues(configurationSections);
            var key1Result = PGConfiguration.TryGetValue(key1);
            var key2Result = PGConfiguration.TryGetValue(key2);
            var nonexistantKeyResult = PGConfiguration.TryGetValue("Random");

            // Assert
            configRootMock.Verify();
            configRootMock2.Verify();
            Assert.AreEqual(key1Result, value1);
            Assert.AreEqual(key2Result, value2);
            Assert.AreEqual(nonexistantKeyResult, null);
        }

        #endregion SetKeyValuesAndTryGetValue Tests
    }
}
