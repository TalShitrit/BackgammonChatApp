using Microsoft.AspNetCore.SignalR;
using Models;
using Services.API;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace WebUi.Hubs
{

    public class ChatHub : Hub
    {

        public static ConcurrentDictionary<string, string> Connections = new ConcurrentDictionary<string, string>();
        private readonly IChatMsgService _servise;
        private readonly IUserService _userService;

        public ChatHub(IChatMsgService servise, IUserService userService)
        {
            this._servise = servise;
            this._userService = userService;
        }

        public async Task SendMessage(string user, string content, string destination = "Public")
        {
            try
            {
                var http = Context.GetHttpContext();
                var username = http.Request.Cookies["UserName"];
                if (destination == username)
                {
                    string msg = "you can't send a message to yourself";
                    await Clients.Caller.SendAsync("ProblebInSendinMsg", msg);
                }
                else
                {
                    ChatMsg msg = new ChatMsg
                    {
                        Content = content,
                        TimeSend = DateTime.Now,
                        Destination = destination,
                        SenderName = user
                    };
                    var allUserNames = await _userService.GetAllUserNamesAsync();

                    if (destination == "Public")// Public Message
                    {
                        await _servise.SendAsync(msg);
                        await Clients.Caller.SendAsync("ReceiveMessage", msg, true);
                        await Clients.Others.SendAsync("ReceiveMessage", msg, false);
                    }
                    else if (allUserNames.Contains(destination)) //Private Message
                    {// username exis
                        await _servise.SendAsync(msg);

                        string connectionToSendMessage;
                        Connections.TryGetValue(destination, out connectionToSendMessage);
                        if (connectionToSendMessage != "")
                            if (!string.IsNullOrWhiteSpace(connectionToSendMessage))
                            {
                                await Clients.Client(connectionToSendMessage).SendAsync("ReceiveMessage", msg, false);
                                await Clients.Caller.SendAsync("ReceiveMessage", msg, true);
                            }
                            else// user exist but logged out
                            {
                                await Clients.Caller.SendAsync("ReceiveMessage", msg, true);
                            }
                    }
                    else
                    {
                        string errMsg = $"the destination { destination} is unknown";
                        await Clients.Caller.SendAsync("ProblebInSendinMsg", errMsg);
                    }

                }
            }
            catch (Exception)
            {
                await Clients.Caller.SendAsync("serverRespond", "Some problem was made");
            }
        }
        public async Task LoadLast(string user, int skip, string destination)
        {
            try
            {
                MsgDestination dest = MsgDestination.Private;
                if (destination == "Public")
                    dest = MsgDestination.Group;
                var msgs = (await _servise.GetPreviousAsync(destination, user, dest, skip, 10)).ToList();
                if (msgs.Count > 0)
                    await Clients.Caller.SendAsync("LoadLast", user, msgs);
                else
                    await Clients.Caller.SendAsync("serverRespond", "No more messages to load");
            }
            catch (Exception)
            {
                await Clients.Caller.SendAsync("serverRespond", "Some problem was made");
            }

        }

        public override Task OnConnectedAsync()
        {
            var id = Context.ConnectionId;
            var http = Context.GetHttpContext();
            var username = http.Request.Cookies["UserName"];
            Connections.TryAdd(username, id);
            return base.OnConnectedAsync();
        }
        public override Task OnDisconnectedAsync(Exception exception)
        {
            string id = Context.ConnectionId;
            var http = Context.GetHttpContext();
            var username = http.Request.Cookies["UserName"];
            Connections.TryRemove(username, out id);
            return base.OnDisconnectedAsync(exception);
        }

    }
}
