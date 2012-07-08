using System;
using System.Collections;
using System.Collections.Generic;
using GameServer.Connections;
using GameServer.Entities;
using GameServer.Structs;
using GameServer.Packets;
using GameServer.Calculations;
using GameServer.Database;

namespace GameServer.Handlers
{
    /// <summary>
    /// Provides methods for equiping items
    /// </summary>
    public partial class Handler
    {
        public static void ItemEquip(int Location, int UID, ClientSocket CSocket)
        {
            if (CSocket.Client.Inventory.ContainsKey(UID))
            {
                Struct.ItemInfo Item = CSocket.Client.Inventory[UID];
                int TrueLoc = DeterminePosition(Item.ItemID, Location);
                if (Item.ItemID == 1050002 || Item.ItemID == 1050001 || Item.ItemID == 1050000)
                    Location = 5;
                if (TrueLoc == Location)
                {
                    if (CSocket.Client.Equipment.ContainsKey(Location))
                    {
                        int Loc2 = CSocket.Client.Equipment[Location].Position;
                        int UID2 = CSocket.Client.Equipment[Location].UID;
                        ItemUnequip(Loc2, UID2, CSocket);
                    }
                    //TODO: Item bonuses(attack etc)
                    //TODO: Position checks, class requirements
                    if (CSocket.Client.Equipment.ContainsKey(Location))
                    {
                        CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Could not equip the item into that slot.", Struct.ChatType.System));
                        return;
                    }
                    CSocket.Client.Inventory.Remove(UID);
                    Item.Position = Location;
                    CSocket.Client.Equipment.Add(Location, Item);
                    Database.Database.UpdateItem(Item);
                    //CSocket.Send(ConquerPacket.ItemInfo(Item.UID, Item.ItemID, Item.Plus, Item.Bless, Item.Enchant, Item.Soc1, Item.Soc2, Item.Dura, Item.MaxDura, Item.Position, Item.Color));
                    CSocket.Send(ConquerPacket.ItemUsage(Item.UID, Location, Struct.ItemUsage.UpdateItem));
                    if (Location == 4 || Location == 5)
                    {
                        if (Item.ItemID != 1050002 && Item.ItemID != 1050001 & Item.ItemID != 1050000)
                        {
                            int Type = Calculation.WeaponType(Convert.ToString(Item.ItemID));
                        }
                    }
                    #region ItemCalculations
                    switch (Item.Soc1)
                    {
                        case 11:
                            {
                                CSocket.Client.NCG++;
                                break;
                            }
                        case 12:
                            {
                                CSocket.Client.RCG++;
                                break;
                            }
                        case 13:
                            {
                                CSocket.Client.SCG++;
                                break;
                            }
                        case 71:
                            {
                                CSocket.Client.NBG++;
                                break;
                            }
                        case 72:
                            {
                                CSocket.Client.RBG++;
                                break;
                            }
                        case 73:
                            {
                                CSocket.Client.SBG++;
                                break;
                            }
                        default:
                            {
                                CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Please report: Unknown GemID: " + Item.Soc1, Struct.ChatType.Talk));
                                break;
                            }

                    }
                    switch (Item.Soc2)
                    {
                        case 11:
                            {
                                CSocket.Client.NCG++;
                                break;
                            }
                        case 12:
                            {
                                CSocket.Client.RCG++;
                                break;
                            }
                        case 13:
                            {
                                CSocket.Client.SCG++;
                                break;
                            }
                        case 71:
                            {
                                CSocket.Client.NBG++;
                                break;
                            }
                        case 72:
                            {
                                CSocket.Client.RBG++;
                                break;
                            }
                        case 73:
                            {
                                CSocket.Client.SBG++;
                                break;
                            }
                        default:
                            {
                                CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Please report: Unknown GemID: " + Item.Soc2, Struct.ChatType.Talk));
                                break;
                            }

                    }
                    switch (Item.Bless)
                    {
                        case 7:
                            {
                                CSocket.Client.Bless += 7;
                                break;
                            }
                        case 5:
                            {
                                CSocket.Client.Bless += 5;
                                break;
                            }
                        case 3:
                            {
                                CSocket.Client.Bless += 3;
                                break;
                            }
                        case 1:
                            {
                                CSocket.Client.Bless += 1;
                                break;
                            }
                        case 0:
                            {
                                break;
                            }
                        default:
                            {
                                CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Invalid bless: " + Item.Bless, Struct.ChatType.System));
                                break;
                            }
                    }
                    if (Nano.Items.ContainsKey(Item.ItemID))
                    {
                        Struct.ItemData ItemD = Nano.Items[Item.ItemID];
                        CSocket.Client.BaseMagicAttack += ItemD.MinMagicAttack;
                        if (Item.Position == 5)
                        {
                            CSocket.Client.BaseMaxAttack += (int)Math.Floor(.5 * ItemD.MaxDamage);
                            CSocket.Client.BaseMinAttack += (int)Math.Floor(.5 * ItemD.MinDamage);
                        }
                        else
                        {
                            CSocket.Client.BaseMaxAttack += ItemD.MaxDamage;
                            CSocket.Client.BaseMinAttack += ItemD.MinDamage;
                        }
                        CSocket.Client.Defense += ItemD.DefenseAdd;
                        CSocket.Client.MaxHP += ItemD.HPAdd;
                        CSocket.Client.MaxHP += Item.Enchant;
                        CSocket.Client.MagicDefense += ItemD.MDefenseAdd;
                        CSocket.Client.MaxMP += ItemD.MPAdd;
                        CSocket.Client.Dodge += ItemD.DodgeAdd;
                    }
                    if (Item.Plus > 0)
                    {
                        string s_ItemID = Convert.ToString(Item.ItemID);
                        int itemidsimple = 0;
                        if ((Item.ItemID >= 900000 && Item.ItemID <= 900999) || (Item.ItemID >= 111303 && Item.ItemID <= 118999) || (Item.ItemID >= 130003 && Item.ItemID <= 139999))//Shields, Helms, Armors
                        {
                            /*s_ItemID = s_ItemID.Remove((s_ItemID.Length - 3), 1);
                            s_ItemID = s_ItemID.Insert((s_ItemID.Length - 2), "0");
                            s_ItemID = s_ItemID.Remove((s_ItemID.Length - 1), 1);
                            s_ItemID = s_ItemID.Insert((s_ItemID.Length), "0");*/
                            s_ItemID = s_ItemID.Remove((s_ItemID.Length - 1), 1);
                            s_ItemID = s_ItemID.Insert(s_ItemID.Length, "0");
                            itemidsimple = Convert.ToInt32(s_ItemID);
                        }
                        else if ((Item.ItemID >= 150000 && Item.ItemID <= 160250) || (Item.ItemID >= 500000 && Item.ItemID <= 500400) || (Item.ItemID >= 120003 && Item.ItemID <= 121249) || (Item.ItemID >= 421003 && Item.ItemID <= 421339))//BS's, Bows, Necky/Bags
                        {
                            s_ItemID = s_ItemID.Remove((s_ItemID.Length - 1), 1);
                            s_ItemID = s_ItemID.Insert((s_ItemID.Length), "0");
                            itemidsimple = Convert.ToInt32(s_ItemID);
                        }
                        else if (Item.ItemID >= 510000 && Item.ItemID <= 580400)//2 Hander
                        {
                            s_ItemID = s_ItemID.Remove(0, 3);
                            s_ItemID = s_ItemID.Insert(0, "555");
                            s_ItemID = s_ItemID.Remove((s_ItemID.Length - 1), 1);
                            s_ItemID = s_ItemID.Insert((s_ItemID.Length), "0");
                            itemidsimple = Convert.ToInt32(s_ItemID);
                        }
                        else if (Item.ItemID >= 410000 && Item.ItemID <= 490400 && itemidsimple == 0)//1 Handers
                        {
                            s_ItemID = s_ItemID.Remove(0, 3);
                            s_ItemID = s_ItemID.Insert(0, "444");
                            s_ItemID = s_ItemID.Remove((s_ItemID.Length - 1), 1);
                            s_ItemID = s_ItemID.Insert((s_ItemID.Length), "0");
                            itemidsimple = Convert.ToInt32(s_ItemID);
                        }
                        if (Nano.ItemPluses.ContainsKey(itemidsimple))
                        {
                            Struct.ItemPlusDB IPlus = Nano.ItemPluses[itemidsimple];
                            if (IPlus.DB.ContainsKey(Item.Plus))
                            {
                                Struct.ItemPlus iPlus = IPlus.DB[Item.Plus];
                                CSocket.Client.BaseMaxAttack += iPlus.MaxDmg;
                                CSocket.Client.BaseMinAttack += iPlus.MinDmg;
                                CSocket.Client.Defense += iPlus.DefenseAdd;
                               // CSocket.Client.BaseMagicAttack += iPlus.MDamageAdd;//@TODO: recalc this
                               // CSocket.Client.BonusMagicAttack += iPlus.MDamageAdd;
                                CSocket.Client.BonusMagicDefense += iPlus.MDefAdd;
                                CSocket.Client.MaxHP += iPlus.HPAdd;
                                CSocket.Client.Dodge += iPlus.DodgeAdd;
                                //TODO: Dodge, etc
                            }
                        }
                    }
                    #endregion
                    Calculation.Attack(CSocket);
                    ConquerPacket.ToLocal(ConquerPacket.SpawnCharacter(CSocket), CSocket.Client.X, CSocket.Client.Y, (int)CSocket.Client.Map, 0, CSocket.Client.ID);
                }
                else if (TrueLoc > 0 && TrueLoc != Location)
                {
                    CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Incorrect equip location.", Struct.ChatType.System));
                }
                else if (TrueLoc == -1)
                {
                    Handler.UseItem(Item, CSocket);
                }
            }
            else
            {
                CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Item doesn't exist.", Struct.ChatType.System));
            }
        }
        public static int DeterminePosition(int ItemID, int EquipLocation)
        {
            if (ItemID >= 111000 && ItemID <= 119999 && EquipLocation == 1)//Returns HeadGear Position
            {
                return 1;
            }
            else if (ItemID >= 120000 && ItemID <= 129999 && EquipLocation == 2)//Returns Necklace Position
            {
                return 2;
            }
            else if (ItemID >= 130000 && ItemID <= 139999 && EquipLocation == 3)//Returns Armor Position
            {
                return 3;
            }
            else if (ItemID >= 400000 && ItemID <= 499999 && EquipLocation == 4)//Returns Weapon Default Position
            {
                return 4;
            }
            else if (ItemID >= 140000 && ItemID <= 149999 && EquipLocation == 7)//Returns Brace Position
            {
                return 7;
            }
            else if (ItemID >= 160000 && ItemID <= 169999 && EquipLocation == 8)//Returns Boots Position
            {
                return 8;
            }
            else if ((ItemID >= 181300 && ItemID <= 192000 || ItemID >= 930000 && ItemID <= 940000) && EquipLocation == 12)//Returns Garment Position
            {
                return 12;
            }
            return -1;
        }
    }
}
