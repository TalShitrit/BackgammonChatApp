using Models;

namespace Services.API
{
    public interface IGameService
    {
        bool Move(int from, int target);
        int[] RollDices();
        PawnSlot[] InitNewGame();
        PawnSlot[] Slots { get;  }
        int LastUsedDice { get; } // need for update the used Dice
        PawnTaken LastPawnTaken { get; set; }
        bool IsCollected { get; set; }
        bool Player1Turn { get; set; }
        bool IsGameEnded();
        bool IsGameOn { get; set; }
        void EndGame();
        void ClearDice();
    }
}
