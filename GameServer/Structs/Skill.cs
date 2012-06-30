using System;
using System.Collections;
using System.Collections.Generic;

namespace GameServer.Structs
{
    /// <summary>
    /// Small struct for a player's skill, as well as a struct for skills server-side that determine skill effects.
    /// </summary>
    public partial class Struct
    {
        public class CharSkill
        {
            public int Level;
            public int ID;
            public uint Exp;
        }
        public class ServerSkill
        {
            public int ID;
            public int MaxLevel;
            /// <summary>
            /// Stores [Level][RequiredExp] in a dictionary.
            /// </summary>
            public Dictionary<int, int[]> RequiredExp = new Dictionary<int, int[]>();
        }
    }
}
