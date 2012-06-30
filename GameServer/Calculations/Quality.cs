using System;


namespace GameServer.Calculations
{
    /// <summary>
    /// Slices an item ID to return it's base quality.
    /// </summary>
    public partial class Calculation
    {
        public static int Quality(string ID)
        {
            string qual = ID.Remove(0, ID.Length - 1);
            return (Convert.ToInt32(qual));
        }
    }
}
