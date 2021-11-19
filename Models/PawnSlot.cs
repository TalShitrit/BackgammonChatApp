using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
   public class PawnSlot
    {
        public int IdNumber { get; set; }
        public Stack<Pawn> Collection { get; set; }
        public PawnSlot()
        {
            Collection = new Stack<Pawn>();
        }
        /// <summary>
        /// add pawn and init its SlotAt
        /// </summary>
        public void AddPawn(Pawn pawn)
        {
            pawn.SlotAt = IdNumber;
            Collection.Push(pawn);
        }
        public Pawn GetHead()
        {
           if(Collection.Count>0)
                return Collection.Pop();
            return null;
        }
        public Pawn PeekHead()
        {
            if (Collection.Count > 0)
                return Collection.Peek();
            return null;
        }
    }
}
