using System;


namespace GameServer.Structs
{
    /// <summary>
    /// Differt forms of status updates for the packet 'Status'
    /// </summary>
    public partial class Struct
    {
        public enum StatusTypes : int
        {
            Hp = 0, // ^ switch 2
            Mp = 2,// ^ switch 2
            MAXMANA = 3,
            InvMoney = 4,// ?
            Exp = 5,// ?
            PKPoints = 6,//switch 2
            // Modifier = 8,
            Stamina = 9,//switch 2
            AttributePoints = 10,// ?
            Model = 11,// switch 2
            Level = 12,//switch 2
            ManaStatPoints = 13,//switch 2
            VitalityStatPoints = 14,//switch 2
            StrengthStatPoints = 15,//switch 2
            DexterityStatPoints = 16,//switch 2
            BlessTimer = 18,// ?
            ExpTimer = 19,// ?
            BlueTimer = 20,// ?
            BlueTimer2 = 21,// ?
            StatusEffect = 25,// switch 2
            HairStyle = 26,// switch 2
            LuckyTimeTimer = 29,// ?
            InvEPoints = 29,// ?
            XpTimer = 31,
        }
    }
}
