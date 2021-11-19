using DAL.Mock;
using Models;
using Services.API;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Mock
{
    public class MockUserService : IUserService
    {
        MockUserRepository _repository;

        public MockUserService()
        {
            _repository = new MockUserRepository();
        }

        public Task<IEnumerable<string>> GetAllUserNamesAsync() => Task.Run(() => _repository.GetAllUserNames());

        public Task<bool> LoginAsync(User user) => Task.Run(() => _repository.Login(user));

        public Task<bool> RegisterAsync(User user) => Task.Run(() => _repository.Register(user));
    }
}
