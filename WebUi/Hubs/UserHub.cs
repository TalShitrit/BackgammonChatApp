using Microsoft.AspNetCore.SignalR;
using Services.API;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using WebUi.Controllers;

namespace WebUi.Hubs
{
    public class UserHub : Hub
    {
        public static ConcurrentDictionary<string, string> Connections = new ConcurrentDictionary<string, string>();
        private readonly IGameService _gameService;

        public UserHub(IGameService gameService) => this._gameService = gameService;
        public async Task AskForAGame(string target)
        {
            try
            {
                if (!_gameService.IsGameOn)
                {
                    var http = Context.GetHttpContext();
                    var username = http.Request.Cookies["UserName"];
                    Connections.TryGetValue(username, out string senderId);
                    Connections.TryGetValue(target, out string targetId);
                    if (target == username)
                    {
                        string msg = "you cant play with yourself";
                        await Clients.Caller.SendAsync("serverRespond", msg);
                        return;
                    }

                    if (!string.IsNullOrWhiteSpace(targetId))
                    {
                        await Clients.Client(targetId).SendAsync("AskForAGame", username);
                        string msg = "game request sent";
                        await Clients.Caller.SendAsync("serverRespond", msg);
                    }
                    else
                    {
                        string msg = "target not found or logged out";
                        await Clients.Caller.SendAsync("serverRespond", msg);
                    }
                }
                else
                {
                    string msg = "there is already a game played";
                    await Clients.Caller.SendAsync("serverRespond", msg);
                }
            }
            catch (Exception)
            {
                string msg = "some probleb was made";
                await Clients.Caller.SendAsync("serverRespond", msg);
            }
        }
        public async Task StartGame(string gameRequester)
        {
            try
            {
                Connections.TryGetValue(gameRequester, out string player1);
                var http = Context.GetHttpContext();
                var player2 = http.Request.Cookies["UserName"];
                if (!string.IsNullOrEmpty(player1) && !string.IsNullOrEmpty(player2))
                {
                    BackgammonHub.SetPlayers(gameRequester, player2);
                    await Clients.Client(player1).SendAsync("StartGame");
                    await Clients.Caller.SendAsync("StartGame");
                }
            }
            catch (Exception)
            {
                string msg = "some probleb was made";
                await Clients.Caller.SendAsync("serverRespond", msg);
            }

        }
        public async Task GameRefused(string target)
        {
            try
            {
                Connections.TryGetValue(target, out string targetId);
                if (!string.IsNullOrEmpty(targetId))
                    await Clients.Client(targetId).SendAsync("GameRefused");
            }
            catch (Exception)
            {
                string msg = "some probleb was made";
                await Clients.Caller.SendAsync("serverRespond", msg);
            }
        }
        public override Task OnConnectedAsync()
        {

            var id = Context.ConnectionId;
            var http = Context.GetHttpContext();
            var username = http.Request.Cookies["UserName"];
            if (username != null)
            {
                //if ( _userService.GetAllUserNamesAsync().Result.Contains(username))
                //{
                Connections.TryAdd(username, id);
                if (!UserController.LoggedList.Contains(username))
                    UserController.LoggedList.Add(username);
                //}
            }
            return base.OnConnectedAsync();
        }
        public override Task OnDisconnectedAsync(Exception exception)
        {
            var http = Context.GetHttpContext();
            var username = http.Request.Cookies["UserName"];
            if (username != null)
            {
                Connections.TryRemove(username, out _);
                if (UserController.LoggedList.Contains(username))
                    UserController.LoggedList.Remove(username);
            }
            return base.OnDisconnectedAsync(exception);
        }
    }
}
