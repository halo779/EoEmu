using System;

namespace GameServer.Structs
{
    /// <summary>
    /// ints for use with the General(1010 or 3F2) Conquer Packet
    /// </summary>
    public partial class Struct
    {
        public enum DataType : int
        {
            actionGetItemSet = 15,
            actionSetPkMode = 29,
            actionGetWeaponSkillSet = 27,
            UnkownLogin = 210,//@TODO CHECK THIS - aka eudtype apparently.
            actionEnterMap = 14,
            actionKickBack = 57,
            HotKeys = 75,
            ConfirmFriends = 76,
            ConfirmProf = 77,
            ConfirmSkills = 78,
            Direction = 79,
            Action = 81,
            Portal = 85,
            ChangeMap = 86,
            Leveled = 92,
            EndXpList = 93,
            PkMode = 96,
            ConfirmGuild = 97,
            EntitySpawn = 102,
            CompleteMapChange = 104,
            CorrectCords = 108,
            TeleportMap = 164,
            Shop = 111,
            OpenShop = 113,
            RemoteCommands = 116, // SubType 1 - Quit, 2 - IG Quit, 39 - Team thing, 43 kick G Member, 44 donate money, 46 join guild, 47 quit guild, 48 join guild, 53 help , 54 freinds, 55 change chat + name in to box, 56 send freind msg, 57 delete freind,  60 set hotkeys, 61 furniture, 64 fux with window, 68 view msg, 69 send msg.., 71 selling price, 78 hawk msg, 80 black list, 88 remove gem warning, 89 show/hide names, 90 show/hide exp, 93 weird dialog box...never seen it before :x, 94 delete enemy, 98 font colour, 101 conqueronline.com, 105 show/hide counter, 109 exp ball, 111 closed the client! fuxors!, 113 ask to vend,
            PickupCashEffect = 121,
            Dialog = 9596,
            ConfirmLoginComplete = 130,
            EntityRemove = 132,
            Jump = 133,
            RemoveWeaponMesh = 135,
            RemoveWeaponMesh2 = 136,
            Unknown1 = 129,
            SpawnEffect = 131,
            MapShow2 = 148,
            MapShow3 = 232,
            uk2 = 1000
        }
    }
}
