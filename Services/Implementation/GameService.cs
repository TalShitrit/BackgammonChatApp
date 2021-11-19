using Logic;
using Models;
using Services.API;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Services.Implementation
{
    public class GameService : IGameService
    {
        #region field
        private List<int> _dices = new List<int>();
        private Pawn InitFirstPlayer => new Pawn { IsFirstPlayer = true };
        private Pawn InitSecondPlayer => new Pawn { IsFirstPlayer = false };
        private bool _player1Turn;
        private int _lastDice;
        private int _player1Collected;
        private int _player2Collected;
        private bool _isCollected;
        private bool _gameEnd;
        private bool _isGameOn;
        private PawnTaken _lastPawnTaken;
        #endregion

        #region prop
        public PawnSlot[] Slots { get; private set; }
        public PawnSlot FirstPlayerTaken { get; set; }
        public PawnSlot SecondPlayerTaken { get; set; }
        public int LastUsedDice => _lastDice;
        public bool Player1Turn
        {
            get { return _player1Turn; }
            set { _player1Turn = value; }
        }
        public bool IsCollected
        {
            get { return _isCollected; }
            set { _isCollected = value; }

        }
        public PawnTaken LastPawnTaken
        {
            get { return _lastPawnTaken; }
            set { _lastPawnTaken = value; }
        }
        public bool IsGameOn
        {
            get
            {
                //if (IsGameEnded() == true)
                //    return false;
                return _isGameOn;
            }

            set { _isGameOn = value; }
        }

        #endregion
        public GameService()
        {
            Player1Turn = true;
        }
        /// <summary>
        /// start the game with player 1 as the start player
        /// </summary>
        /// <returns>the collection of the paawns on the board</returns>
        public PawnSlot[] InitNewGame()
        {
            IsGameOn = true;
            _player1Collected = 0;
            _player2Collected = 0;
            Player1Turn = true;
            _gameEnd = false;
            // index 0 is player1 taken && goal
            // index 25 is player2 taken && goal

            Slots = new PawnSlot[26]; //0->25 include
            for (int i = 0; i < Slots.Length; i++)
                Slots[i] = new PawnSlot { IdNumber = i };

            FirstPlayerTaken = Slots[25];
            SecondPlayerTaken = Slots[0];
            #region init board
            //------------------------------top row  ------------------------------
            Slots[24].AddPawn(InitFirstPlayer);
            Slots[24].AddPawn(InitFirstPlayer);

            Slots[19].AddPawn(InitSecondPlayer);
            Slots[19].AddPawn(InitSecondPlayer);
            Slots[19].AddPawn(InitSecondPlayer);
            Slots[19].AddPawn(InitSecondPlayer);
            Slots[19].AddPawn(InitSecondPlayer);

            Slots[17].AddPawn(InitSecondPlayer);
            Slots[17].AddPawn(InitSecondPlayer);
            Slots[17].AddPawn(InitSecondPlayer);

            Slots[13].AddPawn(InitFirstPlayer);
            Slots[13].AddPawn(InitFirstPlayer);
            Slots[13].AddPawn(InitFirstPlayer);
            Slots[13].AddPawn(InitFirstPlayer);
            Slots[13].AddPawn(InitFirstPlayer);
            //------------------------------bot row  ------------------------------
            Slots[12].AddPawn(InitSecondPlayer);
            Slots[12].AddPawn(InitSecondPlayer);
            Slots[12].AddPawn(InitSecondPlayer);
            Slots[12].AddPawn(InitSecondPlayer);
            Slots[12].AddPawn(InitSecondPlayer);

            Slots[8].AddPawn(InitFirstPlayer);
            Slots[8].AddPawn(InitFirstPlayer);
            Slots[8].AddPawn(InitFirstPlayer);

            Slots[6].AddPawn(InitFirstPlayer);
            Slots[6].AddPawn(InitFirstPlayer);
            Slots[6].AddPawn(InitFirstPlayer);
            Slots[6].AddPawn(InitFirstPlayer);
            Slots[6].AddPawn(InitFirstPlayer);

            Slots[1].AddPawn(InitSecondPlayer);
            Slots[1].AddPawn(InitSecondPlayer);
            #endregion
            return Slots;
        }
        public int[] RollDices()
        {
            _dices = DiceLogic.RollDice();
            return _dices.ToArray();
        }
        public bool Move(int from, int target)
        {
            if (IsGameOn)
            {
                Pawn pawn = null;
                PawnSlot ps = null;
                try
                {
                    ps = Slots[from];
                    if (from == target)
                        if (TryCollect(ps))
                        {
                            // player can collect
                            ps.GetHead();//take it out
                            return true;
                        }
                        else return false;
                    pawn = ps.GetHead();
                    var psTarget = Slots[target];
                    if (Move(pawn, psTarget))
                    {
                        return true;
                    }
                    if (pawn != null && ps != null)// bring the pawn back 
                        ps.AddPawn(pawn);
                    return false;
                }
                catch (Exception e)
                {
                    if (pawn != null && ps != null)// bring the pawn back 
                        ps.AddPawn(pawn);
                    throw e;
                }
            }
            else return false;

        }
        public bool IsGameEnded()
        {
            if (_gameEnd)
            {
                IsGameOn = false;
                return true;
            }

            if (_player1Collected == 15 || _player2Collected == 15)
            {
                IsGameOn = false;
                return true;
            }
            return false;
        }
        public void ClearDice() => _dices = new List<int>();
        public void EndGame()
        {
            _gameEnd = true;
            Player1Turn = true;
            Slots = new PawnSlot[26];
            IsGameEnded();
        }


        private bool Move(Pawn pawn, PawnSlot target)
        {
            //if (MoveLogic.CanMove(pawn, target))
            int dice = MoveLogic.CanMoveDice(pawn, target, _dices.ToArray());
            if (dice != -1)
            {
                UseDice(dice);
                if (MoveLogic.IsPawnTaken(pawn, target))
                {
                    Pawn taken = target.GetHead();
                    _lastPawnTaken = new PawnTaken { Pawn = taken, TakenFrom = target.IdNumber };
                    // second kill first
                    if (taken.IsFirstPlayer)
                    {
                        FirstPlayerTaken.AddPawn(taken);
                        taken.SlotAt = FirstPlayerTaken.IdNumber;
                    }
                    else
                    {
                        SecondPlayerTaken.AddPawn(taken);
                        taken.SlotAt = SecondPlayerTaken.IdNumber;
                    }

                }
                // can move but no pawn taken
                target.AddPawn(pawn);
                pawn.SlotAt = target.IdNumber;
                return true;
            }
            return false;
        }
        private bool TryCollect(PawnSlot from)
        {
            _dices = _dices.OrderBy(i => i).ToList();
            foreach (int dice in _dices)
            {
                if (MoveLogic.CanCollect(from, dice))
                {
                    // player cant collect if he has pawn taken
                    var pawn = from.PeekHead();
                    if (pawn.IsFirstPlayer)
                    {
                        if (FirstPlayerTaken.PeekHead() != null)
                            return false;
                    }
                    else
                    {
                        if (SecondPlayerTaken.PeekHead() != null)
                            return false;
                    }

                    if (Player1Turn)
                        _player1Collected++;
                    else
                        _player2Collected++;
                    UseDice(dice);
                    IsCollected = true;
                    return true;
                }
            }
            return false;
        }
        private void UseDice(int dice)
        {
            _dices.Remove(dice);
            _lastDice = dice;
            if (_dices.Count == 0)
                Player1Turn = !Player1Turn;
        }



    }
}
