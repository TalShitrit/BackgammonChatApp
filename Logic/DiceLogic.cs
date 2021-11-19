using System;
using System.Collections.Generic;
using System.Linq;

namespace Logic
{
    public class DiceLogic
    {
        public static List<int> RollDice()
        {
            Random rnd = new Random();
            int number1, number2;
            number1 = rnd.Next(1, 7);
            number2 = rnd.Next(1, 7);
            if (number1 == number2)
                return new int[] { number1, number1, number1, number1 }.ToList();
            return new int[] { number1, number2 }.ToList();
        }
    }
}
