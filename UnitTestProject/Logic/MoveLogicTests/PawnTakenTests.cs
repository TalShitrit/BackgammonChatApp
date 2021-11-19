using Logic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models;
using System;

namespace UnitTestProject.Logic.MoveLogicTests
{

    [TestClass]
    public class PawnTakenTests
    {
        [TestMethod]
        public void CanTakePawn()
        {
            Pawn firstPlayerPawn = new Pawn { IsFirstPlayer = true, SlotAt = 10 };
            Pawn secondPlayerPawn = new Pawn { IsFirstPlayer = false, SlotAt = 10 };
            PawnSlot singleFirstPlayer = new PawnSlot();
            singleFirstPlayer.AddPawn(new Pawn { IsFirstPlayer = true });
            PawnSlot singleSecondPlayer = new PawnSlot();
            singleSecondPlayer.AddPawn(new Pawn { IsFirstPlayer = false });

            Assert.IsTrue(MoveLogic.IsPawnTaken(firstPlayerPawn, singleSecondPlayer));
            Assert.IsTrue(MoveLogic.IsPawnTaken(secondPlayerPawn, singleFirstPlayer));

        }
        /// <summary>
        /// cant take if target.collection.Count > 1
        /// </summary>
        [TestMethod]
        public void CanNotTakePawnCollection()
        {
            Pawn firstPlayerPawn = new Pawn { IsFirstPlayer = true, SlotAt = 10 };
            Pawn secondPlayerPawn = new Pawn { IsFirstPlayer = false, SlotAt = 10 };
            PawnSlot singleFirstPlayer = new PawnSlot();
            singleFirstPlayer.AddPawn(new Pawn { IsFirstPlayer = true });
            singleFirstPlayer.AddPawn(new Pawn { IsFirstPlayer = true });
            PawnSlot singleSecondPlayer = new PawnSlot();
            singleSecondPlayer.AddPawn(new Pawn { IsFirstPlayer = false });
            singleSecondPlayer.AddPawn(new Pawn { IsFirstPlayer = false });

            Assert.IsFalse(MoveLogic.IsPawnTaken(firstPlayerPawn, singleSecondPlayer));
            Assert.IsFalse(MoveLogic.IsPawnTaken(secondPlayerPawn, singleFirstPlayer));
        }
        [TestMethod]
        public void CanNotTakePawnOfSamePlayer()
        {
            Pawn firstPlayerPawn = new Pawn { IsFirstPlayer = true, SlotAt = 10 };
            Pawn secondPlayerPawn = new Pawn { IsFirstPlayer = false, SlotAt = 10 };
            PawnSlot singleFirstPlayer = new PawnSlot();
            singleFirstPlayer.AddPawn(new Pawn { IsFirstPlayer = true });
            PawnSlot singleSecondPlayer = new PawnSlot();
            singleSecondPlayer.AddPawn(new Pawn { IsFirstPlayer = false });

            Assert.IsFalse(MoveLogic.IsPawnTaken(firstPlayerPawn, singleFirstPlayer));
            Assert.IsFalse(MoveLogic.IsPawnTaken(secondPlayerPawn, singleSecondPlayer));

        }
        [TestMethod]
        public void IsPawnTakenExceptions()
        {
            Assert.ThrowsException<NullReferenceException>(
                () => MoveLogic.IsPawnTaken(null, null));
            Assert.ThrowsException<NullReferenceException>(
                () => MoveLogic.IsPawnTaken(null, new PawnSlot()));
            Assert.ThrowsException<NullReferenceException>(
                () => MoveLogic.IsPawnTaken(new Pawn(), null));

        }
        [TestMethod]
        public void PerformancePawnTaken()
        {
            Pawn pawn = new Pawn { IsFirstPlayer = true, SlotAt = 10 };
            PawnSlot ps = new PawnSlot();
            ps.AddPawn(new Pawn { IsFirstPlayer = false });
            Performance.PerformanceTest(() =>
            {
                MoveLogic.IsPawnTaken(pawn, ps);
            });
        }

    }
}
