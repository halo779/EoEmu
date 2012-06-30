using System;

namespace GameServer.Calculations
{
    public partial class Calculation
    {
        public static int GetDistance(int x1, int y1, int x2, int y2)
        {
            int x = Math.Abs(x1 - x2);
            int y = Math.Abs(y1 - y2);
            int bg = Math.Max(x, y);
            return (bg);
        }
    }
}
