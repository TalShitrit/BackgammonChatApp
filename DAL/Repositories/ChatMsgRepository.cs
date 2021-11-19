using DAL.API;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DAL
{
    public class ChatMsgRepository : IChatMsgRepository
    {
        private readonly Data _data;

        public ChatMsgRepository(Data data)
        {
            this._data = data;
        }
        public IEnumerable<ChatMsg> GetAll(string destination, string sender, MsgDestination msgDestination)
        {
            if (msgDestination == MsgDestination.Private)
                return _data.ChatMsgs.Where(m => (m.Destination == destination && m.SenderName == sender) || (m.Destination == sender && m.SenderName == destination));
            return _data.ChatMsgs.Where(m => m.Destination == destination);
        }

        public IEnumerable<ChatMsg> GetPrevious(string destination, string sender, MsgDestination msgDestination, int skip, int count = 10)
        {
            var list = GetAll(destination, sender, msgDestination);
            list = list.Reverse();
            list = list.Skip(skip);
            list = list.Take(count);
            return list;
        }

        public void Send(ChatMsg msg)
        {
            _data.ChatMsgs.Add(msg);
            _data.SaveChanges();
        }
    }
}
