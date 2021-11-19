using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models;
using Services.API;
using Services.Mock;
using System.Linq;

namespace UnitTestProject.Services
{
    [TestClass]
    public class UserServiceTests
    {
        [TestMethod]
        public void Register()
        {
            IUserService userService = new MockUserService();
            var namesBefore = userService.GetAllUserNamesAsync().Result.ToList();
            string userName = "SomeRandomUserName123";
            var res = userService.RegisterAsync(new User
            { UserName = userName, Password = "aaa" }).Result;
            Assert.IsTrue(res);

            var namesAfter = userService.GetAllUserNamesAsync().Result.ToList();

            Assert.IsFalse(namesBefore.Contains(userName));
            Assert.IsTrue(namesAfter.Contains(userName));
        }

        [TestMethod]
        public void CantRegisterToExistUserName()
        {
            IUserService userService = new MockUserService();
            string userName = "SomeRandomUserName123";
            var res1 = userService.RegisterAsync(new User
            { UserName = userName, Password = "aaa" }).Result;
            var res2 = userService.RegisterAsync(new User
            { UserName = userName, Password = "aaa" }).Result;
            Assert.IsFalse(res2);
        }
        [TestMethod]
        public void CanLogin()
        {
            IUserService userService = new MockUserService();
            var user = new User { UserName = "SomeRandomUserName123", Password = "aaa" };
            var regisgter = userService.RegisterAsync(user).Result;
            var login = userService.LoginAsync(user).Result;
            Assert.IsTrue(login);
        }
        public void CanNotLogin()
        {
            IUserService userService = new MockUserService();
            var login = userService.LoginAsync(new User()).Result;
            Assert.IsFalse(login);
        }
        //---------------------------------------Performance Tests-----------------------
        [TestMethod]
        public void PerformanceRegister()
        {
            IUserService userService = new MockUserService();
            Performance.PerformanceTest(() =>
            {
                userService.RegisterAsync(new User
                { UserName = "SomeRandomUserName123", Password = "aaa" }).Result.ToString();
            });
        }
        [TestMethod]
        public void PerformanceLogin()
        {
            IUserService userService = new MockUserService();
            Performance.PerformanceTest(() =>
            {
                var user = new User { UserName = "SomeRandomUserName123", Password = "aaa" };
                var regisgter = userService.RegisterAsync(user).Result;
                var login = userService.LoginAsync(user).Result;
            });
        }
        [TestMethod]
        public void PerformanceGetAllUserNames()
        {
            IUserService userService = new MockUserService();
            Performance.PerformanceTest(() =>
            {
                var regisgter = userService.GetAllUserNamesAsync().Result;
            });
        }
    }
}
