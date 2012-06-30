using System;


namespace GameServer.Structs
{
    /// <summary>
    /// Contains a struct for a NPC - a non-moving, immobile object
    /// </summary>
    public partial class Struct
    {
        public class NPC
        {
            public int ID;
            public int Type;
            public int SubType;
            public int X;
            public int Y;
            public int Map;
            public int Direction;
            public int Flag;
        }
    }
}
