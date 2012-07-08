using System;


namespace GameServer.Structs
{
    /// <summary>
    /// Enum defining item usage types for Conquer Packets
    /// </summary>
    public partial class Struct
    {
        public enum MonopolyMaskValues :int
        {
            MonopolyMask = 0x01,
            StorageMask = 0x02,
            NoDropMask = 0x03,
            NoSellMask = 0x04,
            NoDeathDropMask = 0x10,
            SellDisableMask = 0x20
        }
        public enum ItemUsage : int
        {
            None = 0,
            BuyItem = 1,
            SellItem = 2,
            RemoveItem = 3,
            EquipItem = 4,
            UpdateItem = 5,
            UnequipItem = 6,
            SplitItem = 7,
            CombineItem = 8,
            ViewWarehouse = 9,
            DepositCash = 10,
            WithdrawCash = 11,
            DropMoney = 12,
            Repair = 14,
            RepairAll = 15,
            Identify = 16,
            DurabilityUpdate = 17,
            DeleteItemTRYME = 18,//@TODO: try this
            DBUpgrade = 19,
            MetUpgrade = 20,
            BoothView = 21,
            BoothAdd = 22,
            BoothDelete = 23,
            BoothBuy = 24,
            Fireworks = 26,
            CompleteTask = 27,
            EudemonEvolve = 28,
            EudemonReborn = 29,
            EudemonEnhance = 30,
            EudemonSummon = 31,
            EudemonKill = 32,
            EudemonEvolveTwo = 33,
            EudemonAtkMode = 34,
            EudemonAttach = 35,
            EudemonDetach = 36

        }
    }
}
