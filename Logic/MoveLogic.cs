using Models;
using System;
using System.Collections.Generic;

namespace Logic
{
    public class MoveLogic
    {
        /*
         first move from 24 to 1
         second move from 1 to 24
         */
        /// <summary>
        /// cheack if movment from pawn to targe twith the given dice can be done
        /// </summary>
        /// <returns>-1 if not, if can the dice number</returns>
        ///<exception cref="ArgumentException">dices is empty</exception>
        ///<exception cref="NullReferenceException">pawn/target are null</exception>
        public static int CanMoveDice(Pawn pawn, PawnSlot target, int[] dices)
        {
            if (pawn is null || target is null)
                throw new NullReferenceException();
            if (dices.Length < 1)
                throw new ArgumentException();
            // not moving to the other direction
            if (pawn.IsFirstPlayer)
            {   // first cant move from high to low
                if (pawn.SlotAt <= target.IdNumber)
                    return -1;
            }
            else
            {
                if (pawn.SlotAt >= target.IdNumber)
                    return -1;
            }

            // target is not of the other player control with more than one pawn
            if (target.Collection.Count > 1)
                if (target.PeekHead().IsFirstPlayer != pawn.IsFirstPlayer)
                    return -1;

            int f = pawn.SlotAt;
            int t = target.IdNumber;
            if (pawn.IsFirstPlayer)
            { // f - d =t
                foreach (int dice in dices)
                    if (f - dice == t)
                        return dice;
            }
            else
            {// f + d =t
                foreach (int dice in dices)
                    if (f + dice == t)
                        return dice;

            }
            return -1;
        }
        ///<exception cref="NullReferenceException">pawn/target are null</exception>
        public static bool IsPawnTaken(Pawn pawn, PawnSlot target)
        {
            if (pawn is null || target is null)
                throw new NullReferenceException();
            /*
             * one to many =>no
             * one to one =>yes
             * one to zero =>no
             * player can only take pawn if it move to single pawn stack of oposite team
             */
            if (target.Collection.Count == 1)
                return target.PeekHead().IsFirstPlayer != pawn.IsFirstPlayer;
            else
                return false;
        }
        ///<exception cref="ArgumentException">moves is 0 or negative</exception>
        ///<exception cref="NullReferenceException">from or its collection/top is null</exception>
        public static bool CanCollect(PawnSlot from, int moves)
        {
            if (moves <= 0)
                throw new ArgumentException();

            if (from != null && from.PeekHead() != null)
            {
                if (from.Collection.Count == 0)
                    throw new NullReferenceException();
                Pawn pawn = from.PeekHead();
                if (pawn != null)
                {
                    if (pawn.IsFirstPlayer)
                        return from.IdNumber - moves <= 0;
                    else
                        return from.IdNumber + moves >= 25;
                }
            }
            throw new NullReferenceException();
        }
    }
}
