using Framework.DbContext;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PaymentGateway.WebApi.Domain;
using System.Linq;

namespace PaymentGateway.WebApiTests.Domain
{
    [TestClass]
    public class UserBLLTests
    {
        private MockRepository _mocks = new MockRepository(MockBehavior.Strict);

        #region CreateUser Tests

        [TestMethod]
        public void CreateUser_Returns_True()
        {
            // Arrange
            var dbContextMock = _mocks.Create<IDatabaseContext>();
            dbContextMock.Setup(m => m.Insert(It.IsAny<UserDto>()))
                .Returns(1);

            var userBLL = new UserBLL(dbContextMock.Object);

            // Act
            var res = userBLL.CreateUser(new UserDto());

            // Assert
            Assert.AreEqual(res, true);
        }

        [TestMethod]
        public void CreateUser_Returns_False()
        {
            // Arrange
            var dbContextMock = _mocks.Create<IDatabaseContext>();
            dbContextMock.Setup(m => m.Insert(It.IsAny<UserDto>()))
                .Returns(-1);

            var userBLL = new UserBLL(dbContextMock.Object);

            // Act
            var res = userBLL.CreateUser(new UserDto());

            // Assert
            Assert.AreEqual(res, false);
        }

        #endregion CreateUser Tests

        #region UpdateUser Tests

        [TestMethod]
        public void UpdateUser_Returns_Bool()
        {
            // Arrange
            var dbContextMock = _mocks.Create<IDatabaseContext>();
            dbContextMock.Setup(m => m.Update(It.IsAny<UserDto>()))
                .Returns(true);

            var userBLL = new UserBLL(dbContextMock.Object);

            // Act
            var res = userBLL.UpdateUser(new UserDto());

            // Assert
            Assert.AreEqual(res, true);
        }

        #endregion UpdateUser Tests

        #region GetAllUsers Tests

        [TestMethod]
        public void GetAllUsers_Returns_UserDtoArray()
        {
            // Arrange
            var dbContextMock = _mocks.Create<IDatabaseContext>();
            dbContextMock.Setup(m => m.GetAll<UserDto>())
                .Returns(new UserDto[] { new UserDto(), new UserDto() });

            var userBLL = new UserBLL(dbContextMock.Object);

            // Act
            var res = userBLL.GetAllUsers();

            // Assert
            Assert.AreEqual(res.Count(), 2);
            Assert.IsInstanceOfType(res, typeof(UserDto[]));
        }

        #endregion GetAllUsers Tests

        #region GetByUserUid Tests

        [TestMethod]
        public void GetByUserUid_Returns_UserDto()
        {
            // Arrange
            var uid = "uid";
            var dbContextMock = _mocks.Create<IDatabaseContext>();
            dbContextMock.Setup(m => m.GetByUid<UserDto>(uid))
                .Returns(new UserDto());

            var userBLL = new UserBLL(dbContextMock.Object);

            // Act
            var res = userBLL.GetByUserUid(uid);

            // Assert
            Assert.IsInstanceOfType(res, typeof(UserDto));
        }

        #endregion GetByUserUid Tests

        #region GetUserByUsername Tests

        [TestMethod]
        public void GetUserByUsername_Returns_UserDto()
        {
            // Arrange
            var name = "name";
            var dbContextMock = _mocks.Create<IDatabaseContext>();
            dbContextMock.Setup(m => m.GetAll<UserDto>())
                .Returns(new UserDto[] { new UserDto(), new UserDto() { Username = name } });

            var userBLL = new UserBLL(dbContextMock.Object);

            // Act
            var res = userBLL.GetUserByUsername(name);

            // Assert
            Assert.IsInstanceOfType(res, typeof(UserDto));
        }

        #endregion GetUserByUsername Tests
    }
}
