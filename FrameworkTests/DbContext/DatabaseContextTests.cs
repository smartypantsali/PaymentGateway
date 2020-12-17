using Framework.DbContext;
using LiteDB;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrameworkTests.DbContext
{
    [TestClass]
    public class DatabaseContextTests
    {
        MockRepository _mocks = new MockRepository(MockBehavior.Strict);

        #region Insert Tests

        [TestMethod]
        public void Insert_Returns_Positive_Number()
        {
            // Arrange
            var bsonValueMock = _mocks.Create<BsonValue>();
            bsonValueMock.SetupGet(m => m.RawValue)
                .Returns(1).Verifiable();

            var liteCollectionMock = _mocks.Create<ILiteCollection<object>>();
            liteCollectionMock.Setup(m => m.Insert(It.IsAny<object>()))
                .Returns(bsonValueMock.Object).Verifiable();

            var liteDbMock = _mocks.Create<ILiteDbWrapper>();
            liteDbMock.Setup(m => m.GetCollection<object>(It.IsAny<string>()))
                .Returns(liteCollectionMock.Object).Verifiable();

            var dbContext = new DatabaseContext(liteDbMock.Object);

            // Act
            var res = dbContext.Insert(new object());

            // Assert
            _mocks.Verify();
            Assert.AreEqual(res, 1);
        }

        #endregion Insert Tests

        #region Update Tests

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void Update_Returns_True_Or_False(bool isTrue)
        {
            // Arrange
            var bsonValueMock = _mocks.Create<BsonValue>();
            bsonValueMock.SetupGet(m => m.RawValue)
                .Returns(isTrue).Verifiable();

            var liteCollectionMock = _mocks.Create<ILiteCollection<object>>();
            liteCollectionMock.Setup(m => m.Update(It.IsAny<object>()))
                .Returns(bsonValueMock.Object).Verifiable();

            var liteDbMock = _mocks.Create<ILiteDbWrapper>();
            liteDbMock.Setup(m => m.GetCollection<object>(It.IsAny<string>()))
                .Returns(liteCollectionMock.Object).Verifiable();

            var dbContext = new DatabaseContext(liteDbMock.Object);

            // Act
            var res = dbContext.Update(new object());

            // Assert
            _mocks.Verify();
            Assert.AreEqual(res, isTrue);
        }

        #endregion Update Tests

        #region GetAll Tests

        [TestMethod]
        public void GetAll_Returns_object()
        {
            // Arrange
            var liteCollectionMock = _mocks.Create<ILiteCollection<object>>();
            liteCollectionMock.Setup(m => m.FindAll())
                .Returns(new object[1]).Verifiable();

            var liteDbMock = _mocks.Create<ILiteDbWrapper>();
            liteDbMock.Setup(m => m.GetCollection<object>(It.IsAny<string>()))
                .Returns(liteCollectionMock.Object).Verifiable();

            var dbContext = new DatabaseContext(liteDbMock.Object);

            // Act
            var res = dbContext.GetAll<object>();

            // Assert
            _mocks.Verify();
            Assert.AreEqual(res.Count(), 1);
        }

        #endregion GetAll Tests

        #region GetByUid Tests

        [TestMethod]
        public void GetByUid_Returns_Object()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var liteCollectionMock = _mocks.Create<ILiteCollection<object>>();
            liteCollectionMock.Setup(m => m.Find(It.IsAny<BsonExpression>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new object[1] { new { Test = "Test" } }).Verifiable();

            var liteDbMock = _mocks.Create<ILiteDbWrapper>();
            liteDbMock.Setup(m => m.GetCollection<object>(It.IsAny<string>()))
                .Returns(liteCollectionMock.Object).Verifiable();

            var dbContext = new DatabaseContext(liteDbMock.Object);

            // Act
            var res = dbContext.GetByUid<object>(guid.ToString());

            // Assert
            _mocks.Verify();
            Assert.IsNotNull(res);
        }

        #endregion GetByUid Tests
    }
}
