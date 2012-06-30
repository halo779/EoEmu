using System;


namespace GameServer.Calculations
{
    public partial class Calculation
    {
        /// <summary>
        /// Returns true if rX,rY are in range of MyX,MyY
        /// </summary>
        public static bool InRange(int rX, int rY, int MyX, int MyY, int Distance)
        {
            return (Math.Max(Math.Abs(rX - MyX), Math.Abs(rY - MyY)) <= Distance);
        }
    }
}
