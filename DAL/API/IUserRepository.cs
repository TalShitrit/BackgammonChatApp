using Models;
using System.Collections.Generic;

namespace DAL.API
{
    public interface IUserRepository
    {
        bool Login(User user);
        bool Register(User user);
        IEnumerable<string> GetAllUserNames();
    }
}
