using Models;
using System.Collections.Generic;

namespace DAL.API
{
    public interface IChatMsgRepository
    {
        void Send(ChatMsg msg);
        IEnumerable<ChatMsg> GetAll(string destination, string sender, MsgDestination msgDestination);
        IEnumerable<ChatMsg> GetPrevious(string destination, string sender, MsgDestination msgDestination, int skip, int count = 10);
    }
}
