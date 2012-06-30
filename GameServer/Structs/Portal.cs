using System;


namespace GameServer.Structs
{
    /// <summary>
    /// Struct for all world portals
    /// </summary>
    public partial class Struct
    {
        public class Portal
        {
            public int ID;

            public int StartX;
            public int StartY;
            public int StartMap;
            public int StartInstance;
            public int EndX;
            public int EndY;
            public int EndMap;
            public int EndInstance;
        }
    }
}
