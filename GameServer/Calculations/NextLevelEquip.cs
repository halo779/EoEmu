using System;
using System.Collections;
using System.Collections.Generic;
using GameServer.Connections;
using GameServer.Entities;
using GameServer.Structs;
using GameServer.Packets;
using GameServer.Calculations;
using GameServer.Database;

namespace GameServer.Calculations
{
    public partial class Calculation
    {
        /// <summary>
        /// Generates the next item equip level.
        /// </summary>
        /// <param name="ItemID">The current item id to level</param>
        /// <returns>the new item id</returns>
        public static int NextEquipLevel(int ItemId)
        {
            int NewItem = ItemId;
            Struct.ItemData Item = MainGS.Items[ItemId];

            if (ArmorType(ItemId) == false || WeaponType(Convert.ToString(ItemId)) == 117)
            {
                if (Type2(Convert.ToString(ItemId)) != 12 && Type2(Convert.ToString(ItemId)) != 15 && Type2(Convert.ToString(ItemId)) != 16 || Item.Level == 45 && Type2(Convert.ToString(ItemId)) == 12 || Item.Level >= 112 && Type2(Convert.ToString(ItemId)) == 12)
                    NewItem += 10;
                else if (Type2(Convert.ToString(ItemId)) == 12 && Item.Level < 45)
                    NewItem += 20;
                else if (Type2(Convert.ToString(ItemId)) == 12 && Item.Level >= 52 && Item.Level < 112)
                    NewItem += 30;
                else if (Type2(Convert.ToString(ItemId)) == 15 && Item.Level == 1 || Type2(Convert.ToString(ItemId)) == 15 && Item.Level >= 110)
                    NewItem += 10;
                else if (Type2(Convert.ToString(ItemId)) == 15)
                    NewItem += 20;
                else if (Type2(Convert.ToString(ItemId)) == 16 && Item.Level < 124)
                    NewItem += 20;
                else if (Type2(Convert.ToString(ItemId)) == 16)
                    NewItem += 10;

                if (WeaponType(Convert.ToString(NewItem)) == 421)
                {
                    NewItem = ItemId;
                    if (Item.Level == 45 || Item.Level == 55)
                        NewItem += 20;
                    else
                        NewItem += 10;
                }
            }
            else if (Type2(Convert.ToString(ItemId)) != 12 && Type2(Convert.ToString(ItemId)) != 15)
            {
                if (Item.Class == 21)
                    if (Type2(Convert.ToString(ItemId)) == 13)
                    {
                        if (Item.Level < 110)
                            NewItem += 10;
                        else
                            NewItem += 5000;
                    }
                if (Item.Class == 11)
                    if (Type2(Convert.ToString(ItemId)) == 13)
                    {
                        if (Item.Level < 110)
                            NewItem += 10;
                        else
                            NewItem += 5000;
                    }
                if (Item.Class == 40)
                    if (Type2(Convert.ToString(ItemId)) == 13)
                    {
                        if (Item.Level < 112)
                            NewItem += 10;
                        else
                            NewItem += 5000;
                    }
                if (Item.Class == 190)
                    if (Type2(Convert.ToString(ItemId)) == 13)
                    {
                        if (Item.Level < 115)
                            NewItem += 10;
                        else
                            NewItem += 5000;
                    }
                if (Item.Class == 21)
                    if (Type2(Convert.ToString(ItemId)) == 11)
                    {
                        if (Item.Level < 112)
                            NewItem += 10;
                        else
                            NewItem += 920;
                    }
                if (Item.Class == 11)
                    if (Type2(Convert.ToString(ItemId)) == 11)
                    {
                        if (Item.Level < 112)
                            NewItem += 10;
                        else
                            NewItem -= 6010;
                    }
                if (Item.Class == 40)
                    if (Type2(Convert.ToString(ItemId)) == 11)
                    {
                        if (Item.Level < 117)
                            NewItem += 10;
                        else
                            NewItem -= 1060;
                    }
                if (Item.Class == 190)
                    if (Type2(Convert.ToString(ItemId)) == 11)
                    {
                        if (Item.Level < 112)
                            NewItem += 10;
                        else
                            NewItem -= 2050;
                    }

            }

            if (ItemId == 500301)
                NewItem = 500005;

            if (ItemId == 410301)
                NewItem = 410005;

            return NewItem;
        }
    }
}
