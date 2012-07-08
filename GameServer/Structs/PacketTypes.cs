using System;


namespace GameServer.Structs
{
    public partial class Struct
    {
        /// <summary>
        /// Each packet, addressed by name rather than ids.
        /// </summary>
        public enum PacketTypeGame : int
        {
            /// <summary>
            /// No Packet Id/NULL
            /// </summary>
            None = 0,
            General = 1000,
            CreateAccount,
            Login,
            Logout,
            Chat,
            Walk,
            UserInfo,
            Attack,
            ItemInfo,
            ItemUsage,
            Action,
            Accident,
            Tick,
            Room,
            Player,
            Name,
            Weather,
            UserAttribute,
            Role,
            Friend,
            Effect,
            Quiz,
            Interact,
            Team,
            PointAllocation,
            WeaponSkill,
            TeamMember,
            GemEmbed,
            Connect = 1052,
            Trade =1056,
            BattleSystem,
            MapItem = 1101,
            Package,
            MagicInfo,
            FlushExp,
            MagicEffect,
            GuildInfo,
            GuildAction,
            ItemInfoX,
            NpcInfoX,
            MapInfo,
            MessageBoard,
            GuildMemberInfo,
            Dice,
            GuildInfoTwo,
            NpcInfo = 2030,
            Npc,
            Dialog,
            FriendInfo,
            PetInfo,
            DataArray,
            EudemonAttribute,
            Mentor,
            TaskAction,
            TaskList,
            AnnounceList,
            AnnouceInfo,
            Auction,
            ChatRoom,
            ItemAttribute,
            WalkEX = 3005
        }
    }
}
