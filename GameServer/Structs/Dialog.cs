using System;

namespace GameServer.Structs
{
    /// <summary>
    /// Dialog Types
    /// </summary>
    public partial class Struct
    {
        public enum Dialog : int
        {
            ShopWindow = 1, //ShoppingMall and Gold Shops
            Warehouse = 3,
            EudemonHatcher = 13,
            EudemonReviver = 14, //@TODO: Look into Functions of this
            EudemonEnchantment = 15, //@TODO: Look into Functions of this
            EudemonWarehouse = 16,
            Booth = 18,
            //BoothStartPedeling= 19, //@TODO: Look into Functions of this
            //PlaceBet = 20, //@TODO: Look into Functions of this
            ItemQualityUpgrade = 23,
            ItemBonus = 24,
            ItemLevelUpgrade = 25,
            ItemGemSocket = 26,
            AuctionHouse = 27,
            ItemIdentification = 28,
            EvolutionOven = 29,
            //EudemonRebuilding = 30, //@TODO: Look into Functions of this
            //EudemonTraining = 31, //@TODO: Look into Functions of this
            ItemSpecialRepair = 35,
            ItemRepair = 36,
            ComposeWindow = 37,
            EudemonSubCompose = 38,
            EudemonEvaluation = 39,
            UseExpBallSelf = 40,
            UseExpBallEudemon = 41,
            EudemonIntAttrCompose = 42,
            EudemonCrystal = 43,
            EudemonLuckCompose = 45, 
            EudemonLuckCompose2 = 46,
            ItemGemEmbed = 47,
            EudemonIdCompose = 48,
            ItemHighBonus = 49,
            ItemTokenExchange = 50,
            //EudemonChallangeAward = 51, //@TODO: Look into Functions of this
            EudemonPurchase = 52, 
            EudemonEggExchange = 53,
            EudemonPurchase2 = 55,
            EudemonEggHatcher = 56,
            VipEudemonWarehouse = 58,
            //EudemonCandiate = 60, //@TODO: Look into Functions of this
            ClothSwap = 300,
            Castle = 301,

        }
    }
}
