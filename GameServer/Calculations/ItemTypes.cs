using System;

namespace GameServer.Calculations
{
    /// <summary>
    /// Credits: LOTF Core
    /// Splits item ids into their types, etc
    /// </summary>
    public partial class Calculation
    {
        public static int Type1(string ID)
        {
            string Type = ID.Remove(1, ID.Length - 1);
            return (Convert.ToInt32(Type));
        }
        public static int Type2(string ID)
        {
            string Type = ID.Remove(2, ID.Length - 2);
            return (Convert.ToInt32(Type));
        }
        public static int WeaponType(string ID)
        {
            string Type = ID.Remove(3, ID.Length - 3);
            return (Convert.ToInt32(Type));
        }
        public static bool ArmorType(int item)
        {
            return (Type2(Convert.ToString(item)) == 11 || Type2(Convert.ToString(item)) == 13);
        }
        public static bool CanUpgrade(string item)
        {
            return (Type2(item) == 90 || Type2(item) == 11 || Type2(item) == 12 || Type2(item) == 13 || Type2(item) == 15 || Type2(item) == 16 || Type1(item) == 4 || Type1(item) == 5);
        }
    }
}
