using Logic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models;
using System;


namespace UnitTestProject.Logic.MoveLogicTests
{
    [TestClass]
    public class CanCollectTests
    {   /// <summary>
        /// need exactly the dice move to reach goal
        /// </summary>
        [TestMethod]
        public void CanCollectWithDiceEqualDest()
        {
            PawnSlot singleFirstPlayer = new PawnSlot { IdNumber = 5 };
            singleFirstPlayer.Collection.Push(new Pawn { IsFirstPlayer = true });
            PawnSlot singleSecondPlayer = new PawnSlot { IdNumber = 20 };
            singleSecondPlayer.Collection.Push(new Pawn { IsFirstPlayer = false });

            Assert.IsTrue(MoveLogic.CanCollect(singleFirstPlayer, 5));
            Assert.IsTrue(MoveLogic.CanCollect(singleSecondPlayer, 5));
        }
        /// <summary>
        /// can collect if dice will move out of destination
        /// </summary>
        [TestMethod]
        public void CanCollectWithDiceHigherThanDest()
        {
            PawnSlot singleFirstPlayer = new PawnSlot { IdNumber = 2 };
            singleFirstPlayer.Collection.Push(new Pawn { IsFirstPlayer = true });
            PawnSlot singleSecondPlayer = new PawnSlot { IdNumber = 22 };
            singleSecondPlayer.Collection.Push(new Pawn { IsFirstPlayer = false });

            Assert.IsTrue(MoveLogic.CanCollect(singleFirstPlayer, 6));
            Assert.IsTrue(MoveLogic.CanCollect(singleSecondPlayer, 6));
        }
        /// <summary>
        /// dice moves not enough for goal
        /// </summary>
        [TestMethod]
        public void CanNotCollectWithSmallDice()
        {
            PawnSlot singleFirstPlayer = new PawnSlot { IdNumber = 5 };
            singleFirstPlayer.Collection.Push(new Pawn { IsFirstPlayer = true });
            PawnSlot singleSecondPlayer = new PawnSlot { IdNumber = 20 };
            singleSecondPlayer.Collection.Push(new Pawn { IsFirstPlayer = false });

            Assert.IsFalse(MoveLogic.CanCollect(singleFirstPlayer, 2));
            Assert.IsFalse(MoveLogic.CanCollect(singleSecondPlayer, 2));

        }
        [TestMethod]
        public void IsCollectedException()
        {
            Assert.ThrowsException<NullReferenceException>(() => MoveLogic.CanCollect(null, 5));
            PawnSlot pawnSlot = new PawnSlot();
            Assert.ThrowsException<NullReferenceException>(() => MoveLogic.CanCollect(pawnSlot, 5));
            pawnSlot.AddPawn(new Pawn { IsFirstPlayer = true });
            Assert.ThrowsException<ArgumentException>(() => MoveLogic.CanCollect(pawnSlot, 0));
        }

        [TestMethod]
        public void PerformanceCanCollect()
        {
            PawnSlot ps = new PawnSlot { IdNumber = 5 };
            ps.Collection.Push(new Pawn { IsFirstPlayer = true });
            Performance.PerformanceTest(() =>
            {
                MoveLogic.CanCollect(ps, 5);
            });
        }
    }
}
