using DAL.API;
using Models;
using System.Collections.Generic;
using System.Linq;

namespace DAL.Mock
{
    public class MockUserRepository : IUserRepository
    {
        readonly List<User> Users;
        public MockUserRepository()
        {
            Users = new List<User> {
                new User { UserName = "a", Password = "1" } ,
                new User { UserName = "r", Password = "r" } };
        }
        public IEnumerable<string> GetAllUserNames() => Users.Select(u => u.UserName);
        public bool Login(User user)
        => Users.FirstOrDefault(u => u.UserName == user.UserName && u.Password == user.Password) != null;
        public bool Register(User user)
        {
            var res = Users.FirstOrDefault(u => u.UserName == user.UserName);
            if (res != null)
                return false;// user exist
            Users.Add(user);
            return true;
        }
    }
}
