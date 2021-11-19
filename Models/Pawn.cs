using System;

namespace Models
{
    public class Pawn
    {
        public bool IsFirstPlayer { get; set; }
        public bool IsTaken { get; set; }
        public bool IsCollected { get; set; }
        public int SlotAt { get; set; }
    }
}
