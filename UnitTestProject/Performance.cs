using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestProject
{
    public class Performance
    {
        public static void PerformanceTest(Action action,int seconds=1)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Task.Run(() => action.Invoke()).Wait();
            sw.Stop();
            Assert.IsTrue(sw.Elapsed < new TimeSpan(0, 0, seconds));
        }
    }
}
