using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models;
using Services.API;
using Services.Mock;
using System;
using System.Linq;

namespace UnitTestProject.Services
{
    [TestClass]
    public class ChatMsgServiceTests
    {
        readonly string dest;
        readonly ChatMsg msg;
        public ChatMsgServiceTests()
        {
            dest = "Test";
            msg = new ChatMsg
            {
                Content = "i am content",
                Destination = dest,
                SenderName = "it is me",
                TimeSend = DateTime.Now
            };
        }

        [TestMethod]
        public void SendMsgWork()
        {
            IChatMsgService chatMsgService = new MockChatMsgService();
            chatMsgService.SendAsync(msg).GetAwaiter().GetResult();
            var lastMsg = chatMsgService.GetPreviousAsync(dest, msg.SenderName, MsgDestination.Private, 0, 1).Result.FirstOrDefault();
            Assert.AreEqual(lastMsg, msg);
        }
        [TestMethod]
        public void GetAllMsg()
        {
            IChatMsgService chatMsgService = new MockChatMsgService();
            var all1 = chatMsgService.GetAllAsync(dest, msg.SenderName, MsgDestination.Private).GetAwaiter().GetResult().ToList();
            int count1 = all1.Count;
            Assert.IsNotNull(all1);

            chatMsgService.SendAsync(msg).GetAwaiter().GetResult();
            var all2 = chatMsgService.GetAllAsync(dest, msg.SenderName, MsgDestination.Private).GetAwaiter().GetResult().ToList();
            int count2 = all2.Count;

            Assert.IsNotNull(all2);
            Assert.AreEqual(count1 + 1, count2);

        }
        [TestMethod]
        public void GetAllGroupMsg()
        {
            var publicMsg1 = new ChatMsg
            {
                Content = "i am content",
                Destination = "Public",
                SenderName = "it is me1",
                TimeSend = DateTime.Now
            };
            var publicMsg2 = new ChatMsg
            {
                Content = "i am content",
                Destination = "Public",
                SenderName = "it is me2",
                TimeSend = DateTime.Now
            };
            IChatMsgService chatMsgService = new MockChatMsgService();
            var all1 = chatMsgService.GetAllAsync(publicMsg2.Destination, publicMsg1.SenderName, MsgDestination.Group).GetAwaiter().GetResult().ToList();
            int count1 = all1.Count;
            Assert.IsNotNull(all1);

            chatMsgService.SendAsync(publicMsg1).GetAwaiter().GetResult();
            chatMsgService.SendAsync(publicMsg2).GetAwaiter().GetResult();
            var all2 = chatMsgService.GetAllAsync(publicMsg2.Destination, publicMsg1.SenderName, MsgDestination.Group).GetAwaiter().GetResult().ToList();
            int count2 = all2.Count;

            Assert.IsNotNull(all2);
            Assert.AreEqual(count1 + 2, count2);

        }
        [TestMethod]
        public void GetPrevious()
        {
            IChatMsgService chatMsgService = new MockChatMsgService();
            chatMsgService.SendAsync(msg).GetAwaiter().GetResult();
            var previous = chatMsgService.GetPreviousAsync(dest, msg.SenderName, MsgDestination.Private, 0).Result.ToList();

            Assert.IsNotNull(previous);
            Assert.IsTrue(previous.Count >= 1);
        }

        //---------------------------------------Performance Tests-----------------------
        [TestMethod]
        public void PerformanceSendMsg()
        {
            IChatMsgService chatMsgService = new MockChatMsgService();
            Performance.PerformanceTest(() =>
            { chatMsgService.SendAsync(msg).GetAwaiter().GetResult(); });
        }
        [TestMethod]
        public void PerformanceGetAll()
        {
            IChatMsgService chatMsgService = new MockChatMsgService();
            Performance.PerformanceTest(() =>
            { chatMsgService.GetAllAsync(dest, msg.SenderName, MsgDestination.Private).GetAwaiter().GetResult(); }, 2);
        }
        [TestMethod]
        public void PerformanceGetPrevious()
        {
            IChatMsgService chatMsgService = new MockChatMsgService();

            Performance.PerformanceTest(() =>
            { chatMsgService.GetPreviousAsync(dest, msg.SenderName, MsgDestination.Private, 0).Result.ToList(); });

        }
    }
}
