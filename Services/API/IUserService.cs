using Models;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Services.API
{
    public interface IUserService
    {
        Task<bool> RegisterAsync(User user);
        Task<bool> LoginAsync(User user);
        Task<IEnumerable<string>> GetAllUserNamesAsync();
    }
}
