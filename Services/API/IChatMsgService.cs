using Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.API
{
    public interface IChatMsgService
    {
        Task SendAsync(ChatMsg msg);
        Task<IEnumerable<ChatMsg>> GetAllAsync(string destination, string sender, MsgDestination msgDestination);
        Task<IEnumerable<ChatMsg>> GetPreviousAsync(string destination, string sender, MsgDestination msgDestination, int skip, int count = 10);
    }
}
