using System;


namespace GameServer.Structs
{
    /// <summary>
    /// ints for use with the Chat(1004 or 3F9) Conquer Packet
    /// </summary>
    public partial class Struct
    {
        public enum ChatType : int
        {
            Normal = 2000,
            Whisper = 2001,
            Action = 2002,
            Team = 2003,
            Guild = 2004,
            System = 2005,
            Family = 2006,
            Talk = 2007,
            Yell = 2008,
            Friend = 2009,
            Broadcast = 2010,
            CenterGm = 2011,
            WhisperTRYME = 2012,
            Ghost = 2013,
            Service = 2014,
            Dialog = 2100,
            LoginInformation = 2101,
            ShopTRYME = 2102,
            EudemonTalk = 2103,
            VendorHawk = 2104,
            WebpageTRYME = 2105,
            /// <summary>
            /// Useless, Doesn't Do anything in current version
            /// </summary>
            NewMessage = 2106,
            Task = 2107,
            ClearTopRight = 2108,
            TopRight = 2109,
            FriendsOfflineMessage = 2110,
            GuildBulletin = 2111,
            DialogMessageBox = 2112,
            RejectTRYME = 2113,
            SyntentTRYME = 2124,
            TradeBoard = 2201,
            FriendBoard = 2202,
            TeamBoard = 2203,
            GuildBoard = 2204,
            OthersBoard = 2205,
            NewBroadcast = 2500
        }
    }
}
