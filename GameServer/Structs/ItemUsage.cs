using System;


namespace GameServer.Structs
{
    /// <summary>
    /// Enum defining item usage types for Conquer Packets
    /// </summary>
    public partial class Struct
    {
        public enum ItemUsage : int
        {
            BuyItem = 1,
            SellItem = 2,
            RemoveItem = 3,
            EquipItem = 4,
            UpdateItem = 5,
            UnequipItem = 6,
            ViewWarehouse = 9,
            DepositCash = 10,
            WithdrawCash = 11,
            DropMoney = 12,
            DBUpgrade = 19,
            MetUpgrade = 20,
            Ping = 27
        }
    }
}
