using Microsoft.VisualStudio.TestTools.UnitTesting;
using Services.API;
using Services.Implementation;
using System;
using System.Linq;

namespace UnitTestProject.Services
{
    [TestClass]
    public class GameServiceTest
    {
        [TestMethod]
        public void IsGameOnFlipAfterInit()
        {
            IGameService gameService = new GameService();
            Assert.IsFalse(gameService.IsGameOn);
            gameService.InitNewGame();
            Assert.IsTrue(gameService.IsGameOn);
            Assert.IsFalse(gameService.IsGameEnded());
        }
        [TestMethod]
        public void SlotsInitializeAfterInit()
        {
            IGameService gameService = new GameService();
            Assert.IsNull(gameService.Slots);
            gameService.InitNewGame();
            Assert.IsNotNull(gameService.Slots);
        }
        [TestMethod]
        public void EndGameMakeIsGameOnFalse()
        {
            IGameService gameService = new GameService();
            gameService.InitNewGame();
            Assert.IsTrue(gameService.IsGameOn);
            gameService.EndGame();
            Assert.IsFalse(gameService.IsGameOn);
            Assert.IsTrue(gameService.IsGameEnded());
        }
        [TestMethod]
        public void ThirtyPawnsSetAfterInit()
        {
            IGameService gameService = new GameService();
            Assert.IsNull(gameService.Slots);
            gameService.InitNewGame();
            int count = 0;
            foreach (var slot in gameService.Slots)
                count += slot.Collection.Count;
            Assert.AreEqual(30, count);
        }
        [TestMethod]
        public void PlayersPawnsAfterInitAreTheSameAmount()
        {
            IGameService gameService = new GameService();
            Assert.IsNull(gameService.Slots);
            gameService.InitNewGame();
            int countP1 = 0;
            int countP2 = 0;
            foreach (var slot in gameService.Slots)
                foreach (var pawn in slot.Collection)
                {
                    if (pawn.IsFirstPlayer)
                        countP1++;
                    else
                        countP2++;
                }
            Assert.AreEqual(countP2, countP1);
            Assert.AreEqual(countP2 + countP1, 30);
        }
        [TestMethod]
        public void RollDiceReturnIntArr()
        {
            IGameService gameService = new GameService();
            var res = gameService.RollDices();
            Assert.IsNotNull(res);
            Assert.IsTrue(res.Length == 2 || res.Length == 4);
        }
        [TestMethod]
        public void MoveWorks()
        { // test move and LastUsedDice
            IGameService gameService = new GameService();
            gameService.InitNewGame();
            int[] dice;
            do
            {
                dice = gameService.RollDices();

            } while (!dice.Contains(2));
            var pawn = gameService.Slots[8].PeekHead();
            var didMove = gameService.Move(8, 6);
            Assert.IsTrue(didMove);
            Assert.AreEqual(2, gameService.LastUsedDice);
            Assert.AreEqual(pawn, gameService.Slots[6].PeekHead());// the pawn did moved
        }
        [TestMethod]
        public void MoveChangePlayerTurn()
        {
            IGameService gameService = new GameService();
            gameService.InitNewGame();
            int[] dice;
            do
            {
                dice = gameService.RollDices();
            } while (dice.Length == 4);// give only 2 dice to move
            bool playerTurn = gameService.Player1Turn;

            foreach (var move in dice)
            // can always move 1->6 steps
            // use all the dices until empty
            {
                var didMove = gameService.Move(8, 8 - move);
                Assert.IsTrue(didMove);
            }

            Assert.IsTrue(playerTurn != gameService.Player1Turn);
        }
        [TestMethod]
        public void LastUsedDiceValueChangeAfterMove()
        { // test move and LastUsedDice
            IGameService gameService = new GameService();
            gameService.InitNewGame();
            int[] dice;
            do
            {
                dice = gameService.RollDices();

            } while (!dice.Contains(2));
            var didMove = gameService.Move(8, 6);
            Assert.IsTrue(didMove);
            Assert.AreEqual(gameService.LastUsedDice, 2);

        }

        //---------------------------------------Performance Tests-----------------------
        [TestMethod]
        public void PerformanceRollDices()
        {
            IGameService gameService = new GameService();
            Performance.PerformanceTest(() =>
            { var res = gameService.RollDices(); });
        }
        [TestMethod]
        public void PerformanceInitNewGame()
        {
            IGameService gameService = new GameService();
            Performance.PerformanceTest(() =>
            { var res = gameService.InitNewGame(); }, 2);
        }
        [TestMethod]
        public void PerformanceMove()
        {
            IGameService gameService = new GameService();
            gameService.InitNewGame();
            int[] dice = gameService.RollDices();
            Performance.PerformanceTest(() =>
            {
                var didMove = gameService.Move(8, 8 - dice[0]);
            });
        }


    }
}
