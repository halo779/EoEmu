using System;


namespace GameServer.Structs
{
    /// <summary>
    /// Enum defining item Position in inventory
    /// </summary>
    public partial class Struct
    {
        public enum ItemPosition : int
        {
            Helm = 1,
            Necklace = 2,
            Armor = 3,
            WeaponRight = 4,
            WeaponLeft = 5,
            RingRight = 6,
            Bracelet = 7,
            Boots = 8,
            Mount = 9,
            Sprite = 10,
            Mantle = 11,

            Invetory = 50,

            GhostGemPack = 51, //@TODO: look into this one.
            EggBag = 52,
            EudemonBag = 53,
            EudemonHatcher = 205,
            EudemonStorrage = 206,

            AuctionStoragePlayer = 207,
            AuctionStorage = 208,

            Ground = 254,
            None = 255
        }
    }
}
