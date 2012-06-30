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
    public partial class Struct
    {
        /// <summary>
        /// A conquer online team, generated via packet 1023. Holds information including team members, etc.
        /// </summary>
        public class Team
        {
            public int LeaderID;
            public bool Forbid;
            public bool ForbidMoney = false;
            public bool ForbidItems = true;
            public Dictionary<int, ClientSocket> Members = new Dictionary<int, ClientSocket>();
        }
        public enum TeamOption : int
        {
            MakeTeam = 0,
            JoinTeam = 1,
            LeaveTeam = 2,
            InviteAccept = 3,
            Invite = 4,
            OnJoin = 5,
            DismissTeam = 6,
            Kick = 7,
            ForbidOn = 8,
            ForbidOff = 9,
            MembersGoldPickupForbid = 11,
            MembersGoldPickupOn = 12,
            MembersItemsPickupForbid = 13,
            MembersItemsPickupOn = 14
        }
    }
}
