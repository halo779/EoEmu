using System;

namespace GameServer.Structs
{
    public partial class Struct
    {
        /// <summary>
        /// Object Types From source.
        /// </summary>
        public enum ObjTypes: int
        {
            OBJ_NONE = 0x1000,
            OBJ_USER = 0x1001,
            OBJ_MONSTER = 0x1002,
            OBJ_ITEM = 0x1004,
            OBJ_MAP = 0x1008,
            OBJ_FRIEND = 0x1010,
            OBJ_NPCTYPE =0x1020,
            OBJ_NPC = 0x1040,
            OBJ_MAPITEM = 0x1080,
            OBJ_SYN = 0x1100,
            OBJ_BOOTH = 0x1200,
            OBJ_TRAP = 0x1400,
            OBJ_TUTOR = 0x1800
        }
    }
}
