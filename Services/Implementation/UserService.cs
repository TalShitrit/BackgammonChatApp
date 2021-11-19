using DAL;
using Models;
using Services.API;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Implementation
{
    public class UserService : IUserService
    {
        UserRepository _repository;
        public UserService(Data data)
        {
            _repository = new UserRepository(data);
        }
        public Task<IEnumerable<string>> GetAllUserNamesAsync() => Task.Run(() => _repository.GetAllUserNames());
        public Task<bool> LoginAsync(User user) => Task.Run(() => _repository.Login(user));

        public Task<bool> RegisterAsync(User user) => Task.Run(() => _repository.Register(user));


    }
}