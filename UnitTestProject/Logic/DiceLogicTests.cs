using Logic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestProject.Logic
{
    [TestClass]
    public class DiceLogicTests
    {
        [TestMethod]
        public void RollDiceValues()
        {
            bool valueInRange = true;
            var dice = DiceLogic.RollDice();
            foreach (var item in dice)
                if (item < 1 || item > 6)
                    valueInRange = false;

            Assert.IsTrue(valueInRange);
        }
        [TestMethod]
        public void RollDiceGivetwoOrFourItems()
        {
            var dice = DiceLogic.RollDice();
            if (dice.Count == 2)
            {
                Assert.AreNotEqual(dice[0], dice[1]);
            }
            else
            if (dice.Count == 4)
            {
                bool valuesEquals = true;
                int first = dice[0];
                foreach (var item in dice)
                    if (first != item)
                        valuesEquals = false;
                    Assert.IsTrue(valuesEquals);
            }
            else
                Assert.IsTrue(false);
        }
        [TestMethod]
        public void PerformanceRollDice()
        {         
            Performance.PerformanceTest(() =>
            {
                DiceLogic.RollDice();
            });
        }
    }
}

