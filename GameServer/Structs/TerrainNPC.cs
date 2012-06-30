using System;
using System.Collections;
using System.Collections.Generic;
using GameServer.Connections;
using GameServer.Entities;
using GameServer.Structs;
using GameServer.Packets;
using GameServer.Calculations;
using GameServer.Database;

namespace GameServer.Structs
{
    /// <summary>
    /// Description of TerrainNPC.
    /// </summary>
    public partial class Struct
    {
        /// <summary>
        /// A conquer online Terrain NPC, things like TG targets, gw poles, and boxes.
        /// </summary>
        public class TerrainNPC
        {
            public int CurrentHP;
            public int MaximumHP;
            public int Flag;
            public int UID;
            public int Type;
            public int X;
            public int Y;
            public int Map;
            public int Level;
        }
    }
}
