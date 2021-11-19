using DAL.API;
using Models;
using System.Collections.Generic;
using System.Linq;

namespace DAL
{
    public class UserRepository : IUserRepository
    {
        readonly Data _data;
        public UserRepository(Data data)
        {
            _data = data;
        }

        public IEnumerable<string> GetAllUserNames() => _data.Users.Select(u => u.UserName);

        public bool Login(User user)
        {
            foreach (var u in _data.Users)
                if (u.UserName == user.UserName && u.Password == user.Password)
                    return true;
            return false;
        }
        public bool Register(User user)
        {
            var res = _data.Users.FirstOrDefault(u => u.UserName == user.UserName);
            if (res != null)
                return false;// user exist
            _data.Users.Add(user);
            _data.SaveChanges();
            return true;
        }
    }
}
