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
            public int Level;
            public int Sex;

            public int force_Require;
            public int Dex_Require;
            public int Health_Require;
            public int soul_Require;

            public int Tradeable;//monopoly

            public int weight;

            public int Cost;//price 
            public int EPCost;

            public int soul_value;

            public byte ident;//@TODO: look into use of this, is it useless.
            public byte gem1;//@TODO: look into use of this, is it useless.
            public byte gem2;//@TODO: look into use of this, is it useless.
            public byte magic1;//@TODO: look into use of this, is it useless.
            public byte magic2;//@TODO: look into use of this, is it useless.
            public byte magic3;//@TODO: look into use of this, is it useless.


            public int MaxDamage;
            public int MinDamage;
            public int DefenseAdd;

            public int DodgeAdd;
            public int HPAdd;
            public int MPAdd;
            public int Dura;
            public int MaxDura;

            public int MinMagicAttack;
            public int MaxMagicAttack;
            public int MDefenseAdd;
            public int Range;
            public int AttackRate; //speed off attacks
            public int Frequency;//atk hitrate

            //@TODO: Move these to their own class
            public int MonsterType;// if its a eudemon it has a type
            public int AbleMask;//@TODO: remind self of use of this.
            public int ExpType;

            public int Target;//@TODO: look into this too
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
            public int MinMDamageAdd;
            public int MaxMDamageAdd;
            public int DexAdd;
            public int MDefAdd;
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
