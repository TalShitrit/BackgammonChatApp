using DAL;
using DAL.API;
using Models;
using Services.API;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Implementation
{
    public class ChatMsgService : IChatMsgService
    {
        readonly IChatMsgRepository _repository;
        public ChatMsgService(Data data)
        {
            _repository = new ChatMsgRepository(data);
        }
        public Task<IEnumerable<ChatMsg>> GetAllAsync(string destination, string sender, MsgDestination msgDestination) => Task.Run(() => _repository.GetAll(destination, sender, msgDestination));

        public Task<IEnumerable<ChatMsg>> GetPreviousAsync(string destination, string sender, MsgDestination msgDestination, int skip, int count = 10)
       => Task.Run(() => _repository.GetPrevious(destination, sender, msgDestination, skip, count));

        public Task SendAsync(ChatMsg msg) => Task.Run(() => _repository.Send(msg));
    }
}
