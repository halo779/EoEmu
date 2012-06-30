using System;

namespace GameServer.Calculations
{
    /// <summary>
    /// Returns true if MyX, MyY can view SeeX,SeeY
    /// Credit: LOTF Core
    /// </summary>
    public partial class Calculation
    {
        public static bool CanSee(int SeeX, int SeeY, int MyX, int MyY)
        {
            //Console.WriteLine("[Debug]CanSee - seeX, SeeY, MyX, MyY - " + SeeX.ToString() + ", " + SeeY.ToString() + ", " + MyX.ToString() + ", " + MyY.ToString());
            //Console.WriteLine("X: " + (Math.Abs(SeeX - MyX).ToString() + " Y: " + Math.Abs(SeeY - MyY)).ToString());
            int x = Math.Abs(SeeX - MyX);
            int y = Math.Abs(SeeY - MyY);
            int bg = Math.Max(x, y);
            bool see = bg <= 16;
            //Console.WriteLine("Can See: " + see.ToString());
            return (see);
        }
    }
}