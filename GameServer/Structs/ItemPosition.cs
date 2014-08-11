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
        public enum ItemSorts : int
        {
            INVALID = -1,
            SHIELD = -1, //possibly 9
            WEAPON2 = -1, //possibly 5
            CLOTHING = 1,
            WEAPON1 = 4,
            HORSE = 6,
            OTHER = 7,
            CONSUMABLE = 10,

        }

        public enum ItemBaseTypes : int
        {
            //ItemSorts::Clothing
            Helmet = 10000,
            Necklace = 20000,
            Armor = 30000,
            Bracelet = 40000,
            Mantle = 50000,
            Boots = 60000,
            Ring = -1, //50000

            //ItemSorts::Consumable
            Invalid = -1,
            Physic = 10000,
            MedicineHP = 10000,
            MedicineMP = 11000,
            Poison = 12000,
            Scroll = 20000,
            ScrollSpecial = 20000,
            ScrollMSkill = 21000,
            ScrollSSkill = 22000,
            ScrollBSkill = 23000,
            GhostGem = 30000,
            GhostGemActiveAtk = 31000,
            GhostGemPassiveAtk = 32000,
            GhostGemEudemon = 33000,
            GhostGemRelease = 34000,
            GhostGemTrace = 35000,
            GhostGemProtective = 36000,
            GhostGemSpecial = 37000,
            GhostGemEmbedEquip = 38000,

            GhostGemForQuality = 1037160,
            GhostGemForGhostLevel = 1037150,
            GhostGemUpgradeEquipLevel = 1037170,

            NotDirectUse = 40000,
            SpecialUse = 50000,

            Sprite = 50000,
            SpritePAtak = 50000,
            SpritePDef = 51000,
            SpriteMAtak = 52000,
            SpriteMDef = 53000,
            SpriteSoul = 54000,

            Special = 60000,
            SpecialValuables = 60000,
            SpecialUnrepairable = 61000,

            Eudemon = 70000,
            EudemonSpeed = 71000,
            EudemonPAtak = 72000,
            EudemonDef = 73000,
            EudemonMAtak = 74000,
            EudemonBomb = 75000,
            EudemonProtective = 76000,
            EudemonAttach = 77000,
            EudemonVariational = 78000,

            EudemonEgg = 80000,

            //ItemSort::Other
            Gem = 00000,
            TaskItem = 10000,
            ActionItem = 20000,
            GameCard = 80000,

        }


        public enum Sex : int 
        {
            NONE = 0,
            MAN,
            WOMAN,
            ASEXUAL,
            ALL
        }
    }
}
