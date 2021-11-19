using DAL.API;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DAL.Mock
{
    public class MockChatMsgRepository : IChatMsgRepository
    {
        private List<ChatMsg> _chatMsgs;
        public MockChatMsgRepository()
        {
            _chatMsgs = new List<ChatMsg>();
            for (int i = 0; i < 50; i++)
            {
                _chatMsgs.Add(new ChatMsg
                {
                    ChatMsgId = i,
                    Content = $"msg {i}",
                    TimeSend = DateTime.Now,
                    Destination = "Public",
                    SenderName = "bot"
                });
            }
            _chatMsgs[45].TimeSend = new DateTime(2008, 3, 1, 7, 0, 0);
        }

        public IEnumerable<ChatMsg> GetAll(string destination, string sender, MsgDestination msgDestination)
        {
            if (msgDestination == MsgDestination.Private)
                return _chatMsgs.Where(m => (m.Destination == destination && m.SenderName == sender) || (m.Destination == sender && m.SenderName == destination));
            return _chatMsgs.Where(m => (m.Destination == destination));
        }

        public IEnumerable<ChatMsg> GetPrevious(string destination, string sender, MsgDestination msgDestination, int skip, int count = 10)
        {
            //Thread.Sleep(2000);
            var list = GetAll(destination, sender, msgDestination); // 1->50               
            list = list.Reverse();         // 50->1
            list = list.Skip(skip);        // 50->1   skip(0)        40-1   skip(10)      
            list = list.Take(count);        // 50->40  count(10) =>   40->30  Take(10)
            //return list.Reverse();          // 40->50  Reverse()      30->40 Reverse()
            return list;          // 40->50  Reverse()      30->40 Reverse()
        }

        public void Send(ChatMsg msg) => _chatMsgs.Add(msg);
    }
}
