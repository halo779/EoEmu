using System;


namespace GameServer.Structs
{
    /// <summary>
    /// Contains definitions of Class ID Numbers.
    /// </summary>
    public partial class Struct
    {
        public enum ClassType : int
        {
            Mage = 10,
            Warrior = 20,
            Palidin = 30
        }
    }
}
