using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameServer.Calculations
{
    /// <summary>
    /// Determines a random chance of something being true based on a percent.
    /// Credit: LOTF Core
    /// </summary>
    public partial class Calculation
    {
        public static bool PercentSuccess(double percent)
        {
            return ((double)Nano.Rand.Next(1, 1000000)) / 10000 >= 100 - percent;
        }
    }
}
