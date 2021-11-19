using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class ChatMsg
    {
        public int ChatMsgId { get; set; }
        public string SenderName { get; set; }
        public string Content { get; set; }
        public string Destination { get; set; }
        public DateTime TimeSend { get; set; }
    }
}
