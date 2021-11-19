using Logic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models;
using System;

namespace UnitTestProject.Logic.MoveLogicTests
{
    /*
    first move from 24 to 1
    second move from 1 to 24
    */
    [TestClass]
    public class CanMoveDiceTests
    {
        [TestMethod]
        public void CanMoveToEmpty()
        {
            Pawn firstPlayerPawn = new Pawn { IsFirstPlayer = true, SlotAt = 10 };
            Pawn secondPlayerPawn = new Pawn { IsFirstPlayer = false, SlotAt = 10 };

            PawnSlot empty1 = new PawnSlot { IdNumber = 5 };
            PawnSlot empty2 = new PawnSlot { IdNumber = 15 };
            int[] dice = new int[] { 5 };
            Assert.AreEqual(MoveLogic.CanMoveDice(firstPlayerPawn, empty1, dice), 5);
            Assert.AreEqual(MoveLogic.CanMoveDice(secondPlayerPawn, empty2, dice), 5);
        }
        [TestMethod]
        public void CanNotMoveToOppositeDirection()
        {
            Pawn firstPlayerPawn = new Pawn { IsFirstPlayer = true, SlotAt = 10 };
            Pawn secondPlayerPawn = new Pawn { IsFirstPlayer = false, SlotAt = 10 };

            PawnSlot empty1 = new PawnSlot { IdNumber = 15 };
            PawnSlot empty2 = new PawnSlot { IdNumber = 5 };
            int[] dice = new int[] { 5 };
            Assert.AreNotEqual(MoveLogic.CanMoveDice(firstPlayerPawn, empty1, dice), 5);
            Assert.AreNotEqual(MoveLogic.CanMoveDice(secondPlayerPawn, empty2, dice), 5);
        }
        [TestMethod]
        public void CanNotMoveWithWrongDice()
        {
            Pawn firstPlayerPawn = new Pawn { IsFirstPlayer = true, SlotAt = 10 };
            Pawn secondPlayerPawn = new Pawn { IsFirstPlayer = false, SlotAt = 10 };

            PawnSlot empty1 = new PawnSlot { IdNumber = 5 };
            PawnSlot empty2 = new PawnSlot { IdNumber = 15 };
            int[] dice = new int[] { 3 };
            Assert.AreEqual(MoveLogic.CanMoveDice(firstPlayerPawn, empty1, dice), -1);
            Assert.AreEqual(MoveLogic.CanMoveDice(secondPlayerPawn, empty2, dice), -1);
        }
        [TestMethod]
        public void CanMoveToSingle()
        {
            Pawn firstPlayerPawn = new Pawn { IsFirstPlayer = true, SlotAt = 10 };
            Pawn secondPlayerPawn = new Pawn { IsFirstPlayer = false, SlotAt = 10 };

            PawnSlot singleFirstPlayer = new PawnSlot { IdNumber = 5 };
            singleFirstPlayer.AddPawn(new Pawn { IsFirstPlayer = true });
            PawnSlot singleSecondPlayer = new PawnSlot { IdNumber = 15 };
            singleSecondPlayer.AddPawn(new Pawn { IsFirstPlayer = false });
            int[] dice = new int[] { 5 };
            Assert.AreEqual(MoveLogic.CanMoveDice(firstPlayerPawn, singleFirstPlayer, dice), 5);
            Assert.AreEqual(MoveLogic.CanMoveDice(secondPlayerPawn, singleSecondPlayer, dice), 5);

            // now to the other PawnSlot
            singleFirstPlayer.IdNumber = 15;
            singleSecondPlayer.IdNumber = 5;

            Assert.AreEqual(MoveLogic.CanMoveDice(firstPlayerPawn, singleSecondPlayer, dice), 5);
            Assert.AreEqual(MoveLogic.CanMoveDice(secondPlayerPawn, singleFirstPlayer, dice), 5);




        }
        //  assert no problem to move to multi used slot  => can move only to own target
        [TestMethod]
        public void CanMoveToMulti()
        {
            Pawn firstPlayerPawn = new Pawn { IsFirstPlayer = true, SlotAt = 10 };
            Pawn secondPlayerPawn = new Pawn { IsFirstPlayer = false, SlotAt = 10 };
            PawnSlot multiFirstPlayer = new PawnSlot { IdNumber = 5 };
            PawnSlot multiSecondPlayer = new PawnSlot { IdNumber = 15 };
            multiFirstPlayer.Collection.Push(new Pawn { IsFirstPlayer = true });
            multiFirstPlayer.Collection.Push(new Pawn { IsFirstPlayer = true });
            multiSecondPlayer.Collection.Push(new Pawn { IsFirstPlayer = false });
            multiSecondPlayer.Collection.Push(new Pawn { IsFirstPlayer = false });
            int[] dice = new int[] { 5 };
            Assert.AreEqual(MoveLogic.CanMoveDice(firstPlayerPawn, multiFirstPlayer, dice), 5);
            Assert.AreEqual(MoveLogic.CanMoveDice(secondPlayerPawn, multiSecondPlayer, dice), 5);
        }
        [TestMethod]
        // assert problem when move to multi used slot of opponent  => cant do it
        public void CanNotMoveToMulti()
        {
            Pawn firstPlayerPawn = new Pawn { IsFirstPlayer = true, SlotAt = 10 };
            Pawn secondPlayerPawn = new Pawn { IsFirstPlayer = false, SlotAt = 10 };
            PawnSlot multiFirstPlayer = new PawnSlot { IdNumber = 15 };
            PawnSlot multiSecondPlayer = new PawnSlot { IdNumber = 5 };
            multiFirstPlayer.Collection.Push(new Pawn { IsFirstPlayer = true });
            multiFirstPlayer.Collection.Push(new Pawn { IsFirstPlayer = true });
            multiSecondPlayer.Collection.Push(new Pawn { IsFirstPlayer = false });
            multiSecondPlayer.Collection.Push(new Pawn { IsFirstPlayer = false });
            int[] dice = new int[] { 5 };
            Assert.AreEqual(MoveLogic.CanMoveDice(firstPlayerPawn, multiSecondPlayer, dice), -1);
            Assert.AreEqual(MoveLogic.CanMoveDice(secondPlayerPawn, multiFirstPlayer, dice), -1);
        }
        [TestMethod]
        public void CanMoveDiceThrowsExceptions()
        {
            Assert.ThrowsException<NullReferenceException>(
                () => MoveLogic.CanMoveDice(null, null, new int[0]));
            Assert.ThrowsException<NullReferenceException>(
                () => MoveLogic.CanMoveDice(new Pawn(), null, new int[0]));
            Assert.ThrowsException<NullReferenceException>(
                () => MoveLogic.CanMoveDice(null, new PawnSlot(), new int[0]));
            Assert.ThrowsException<ArgumentException>(
                () => MoveLogic.CanMoveDice(new Pawn(), new PawnSlot(), new int[0]));
        }

        [TestMethod]
        public void PerformanceMoveDice()
        {

            Pawn pawn = new Pawn { IsFirstPlayer = true, SlotAt = 10 };
            PawnSlot ps = new PawnSlot { IdNumber = 5 };
            int[] dice = new int[] { 5 };
            Performance.PerformanceTest(() =>
            {
                MoveLogic.CanMoveDice(pawn, ps, dice);
            });
        }
    }
}
