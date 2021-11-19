using Microsoft.AspNetCore.SignalR;
using Models;
using Services.API;
using System;
using System.Threading.Tasks;

namespace WebUi.Hubs
{
    public class BackgammonHub : Hub
    {
        public static void SetPlayers(string player1, string player2)
        {
            _player1 = player1;
            _player2 = player2;
        }
        public static int IsPlayer(string username)
        {
            if (_player1 == username)
                return 1;
            if (_player2 == username)
                return 2;
            return -1;

        }
        private static string _player1;
        private static string _player2;

        private readonly IGameService _gameService;
        public BackgammonHub(IGameService gameService) => _gameService = gameService;

        public async Task GetGame()
        {
            try
            {
                var http = Context.GetHttpContext();
                var username = http.Request.Cookies["UserName"];
                if (username is null)
                    await Clients.Caller.SendAsync("GetGame", _gameService.Slots);
                else // if the person is a player send with who he fights with
                {
                    if (username == _player1)
                        await Clients.Caller.SendAsync("GetGame", _gameService.Slots, _player2);
                    else if (username == _player2)
                        await Clients.Caller.SendAsync("GetGame", _gameService.Slots, _player1);
                    else
                        await Clients.Caller.SendAsync("GetGame", _gameService.Slots);
                }
            }
            catch (Exception)
            {
                await Clients.Caller.SendAsync("ServerMessage", "Some problem was made");
            }
        }
        public async Task RollDice()
        {
            try
            {
                if (!_gameService.IsGameEnded())
                {
                    if (IsPlayerTurn())
                    {
                        var dice = _gameService.RollDices();
                        await Clients.All.SendAsync("RollDice", dice);
                    }
                    else
                        await Clients.Caller.SendAsync("NotYourTurn");
                }
                else
                    await Clients.Caller.SendAsync("GameOver");
            }
            catch (Exception)
            {
                await Clients.Caller.SendAsync("ServerMessage", "Some problem was made");
            }
        }
        public async Task InitNewGame()
        {
            try
            {
                if (IsPlayerTurn())
                {
                    if (!_gameService.IsGameOn)
                    {
                        var gameBoard = _gameService.InitNewGame();
                        await Clients.All.SendAsync("InitNewGame", gameBoard);
                    }
                    else
                        await Clients.Caller.SendAsync("ServerMessage", "You cant start new game while game is on");
                }
                else
                    await Clients.Caller.SendAsync("NotYourTurn");
            }
            catch (Exception)
            {
                await Clients.Caller.SendAsync("ServerMessage", "Some problem was made");
            }
        }
        public async Task GiveUpOnTurn()
        {
            try
            {
                if (_gameService.IsGameOn)
                {
                    if (IsPlayerTurn())
                    {
                        _gameService.Player1Turn = !_gameService.Player1Turn;
                        _gameService.ClearDice();
                        await Clients.All.SendAsync("GiveUpOnTurn");
                    }
                    else
                        await Clients.Caller.SendAsync("NotYourTurn");
                }
                else
                    await Clients.Caller.SendAsync("GameOver");
            }
            catch (Exception)
            {
                await Clients.Caller.SendAsync("ServerMessage", "Some problem was made");
            }
        }
        public async Task<bool> Move(int from, int target)
        {
            try
            {
                if (!_gameService.IsGameEnded())
                {
                    if (IsPlayerTurn())
                    {
                        var didMoved = _gameService.Move(from, target);
                        if (didMoved)
                        {
                            int dice = _gameService.LastUsedDice;

                            if (_gameService.IsCollected)
                            {
                                _gameService.IsCollected = false;
                                if (_gameService.IsGameEnded())
                                {
                                    var http = Context.GetHttpContext();
                                    var username = http.Request.Cookies["UserName"];
                                    if (username == _player1)
                                    {
                                        await Clients.All.SendAsync("GameOver", "player 1 win");
                                    }

                                    else if (username == _player2)
                                    {
                                        await Clients.All.SendAsync("GameOver", "player 2 win");
                                    }
                                    _gameService.EndGame();
                                }
                                else
                                    await Clients.All.SendAsync("PieceCollected", from, dice);
                                return true;
                            }
                            else
                            {

                                PawnTaken pt = _gameService.LastPawnTaken;
                                if (pt != null)
                                {
                                    await Clients.All.SendAsync("GameBoardChange", pt.TakenFrom, pt.Pawn.SlotAt);
                                    _gameService.LastPawnTaken = null;
                                }
                                if (_gameService.IsCollected)
                                {
                                    //await Clients.All.SendAsync("GameBoardChange", from, target, dice);
                                    _gameService.IsCollected = false;
                                }
                                await Clients.All.SendAsync("GameBoardChange", from, target, dice);
                                return true;
                            }
                        }
                        else
                        {
                            await Clients.Caller.SendAsync("InvalidMove");
                            return false;
                        }
                    }
                    else
                    {
                        await Clients.Caller.SendAsync("NotYourTurn");
                        return false;
                    }
                }
                else
                {
                    await Clients.Caller.SendAsync("GameOver");
                    return false;
                }
            }
            catch (Exception e)
            {
                await Clients.Caller.SendAsync("Exception", e);
                return false;
            }

        }
        public async Task GiveUp()
        {
            try
            {
                if (_gameService.IsGameOn)
                {
                    if (_player1 != null || _player2 != null)
                    {
                        var http = Context.GetHttpContext();
                        var username = http.Request.Cookies["UserName"];
                        if (username == _player1)
                        {
                            _gameService.EndGame();
                            await Clients.All.SendAsync("GameOver", "player 2 win");
                        }

                        else if (username == _player2)
                        {
                            _gameService.EndGame();
                            await Clients.All.SendAsync("GameOver", "player 1 win");
                        }
                        else
                            await Clients.Caller.SendAsync("NotYourTurn");
                    }
                    else
                        await Clients.Caller.SendAsync("NotYourTurn");
                }
                else
                    await Clients.Caller.SendAsync("GameOver");

            }
            catch (Exception)
            {
                await Clients.Caller.SendAsync("ServerMessage", "Some problem was made");
            }
        }

        private bool IsPlayerTurn()
        {
            if (_player1 is null || _player2 is null)
                return false;
            var http = Context.GetHttpContext();
            var username = http.Request.Cookies["UserName"];
            if (_gameService.Player1Turn)
                return username == _player1;
            return username == _player2;
        }

    }
}

