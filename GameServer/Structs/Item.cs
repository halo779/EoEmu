using System;
using System.Timers;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using GameServer.Packets;

namespace GameServer.Structs
{
    /// <summary>
    /// Description of Item.
    /// </summary>
    public partial class Struct
    {
        public class ItemData
        {
            public int ID;
            public string Name;

            public int Class;
            public int Prof;
            public int Level;
            public int Sex;
            public int Str_Require;
            public int Dex_Require;
            public int Vit_Require;
            public int Spi_Require;

            public int Tradeable;
            // public int Unknown1;
            public int Cost;
            public int CPCost;
            // public int Unknown2;
            public int MaxDamage;
            public int MinDamage;
            public int DefenseAdd;
            public int DexAdd;
            public int DodgeAdd;
            public int HPAdd;
            public int MPAdd;
            public int Dura;
            public int MaxDura;
            /* public int Unknown3;
             public int Unknwon4;
             public int Unknown5;
             public int Unknown6;
             public int Unknown7;
             public int Unknown8;*/
            public int MagicAttack;
            public int MDefenseAdd;
            public int Range;
            public int Frequency;
            /* public int Unknown9;
             public int Unknown10;
             public int Unknown11;
             public int Unknown12;*/
            //public string ItemType;
        }
        public class ItemInfo
        {
            public int UID;
            public int ItemID;
            public int Plus;
            public int Bless;
            public int Enchant;
            public int Soc1;
            public int Soc2;
            public int Dura;
            public int MaxDura;
            public int Position;
            public int Color = 4;
        }
        public class ItemPlus
        {
            public int ID;
            public int Plus;
            public int HPAdd;
            public int MinDmg;
            public int MaxDmg;
            public int DefenseAdd;
            public int MDamageAdd;
            public int MDefAdd;
            public int AccuracyAdd;
            public int DodgeAdd;
        }
        public class ItemPlusDB
        {
            //Contains an array of item pluses
            public Dictionary<int, ItemPlus> DB = new Dictionary<int, ItemPlus>();
        }
        public class ItemGround : ItemInfo
        {
            public System.Timers.Timer OwnerOnly;
            public System.Timers.Timer Dispose;
            public int OwnerID;
            public int X;
            public int Y;
            public int Map;
            public int Money;
            public void Disappear()
            {
                if (Nano.ItemFloor.ContainsKey(UID))
                {
                    //lock(Nano.ItemFloor)
                    //{
                    try
                    {
                        Monitor.Enter(Nano.ItemFloor);
                        Nano.ItemFloor.Remove(UID);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                    finally
                    {
                        Monitor.Exit(Nano.ItemFloor);
                    }
                    //}
                }
                ConquerPacket.ToLocal(ConquerPacket.RemoveItemDropEffect(UID, ItemID, X, Y), X, Y, Map, 0, 0);
                ConquerPacket.ToLocal(ConquerPacket.RemoveItemDrop(UID), X, Y, Map, 0, 0);
                Stop();
            }
            public void Stop()
            {
                Dispose.Stop();
                Dispose.Dispose();
                OwnerOnly.Stop();
                OwnerOnly.Dispose();
            }
            public void CopyItem(ItemInfo Item)
            {
                Bless = Item.Bless;
                Dura = Item.Dura;
                Enchant = Item.Enchant;
                ItemID = Item.ItemID;
                MaxDura = Item.MaxDura;
                Plus = Item.Plus;
                Position = Item.Position;
                Soc1 = Item.Soc1;
                Soc2 = Item.Soc2;
                UID = Item.UID;
            }
        }
    }
}
