using System;
using System.Timers;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using GameServer.Calculations;
using GameServer.Connections;
using GameServer.Structs;
using GameServer.Packets;

namespace GameServer.Entities
{
    /// <summary>
    ///  A Monster. This is a NPC(non-player character) which is controlled and given intelligence by the server.
    /// </summary>
    public class Monster
    {
        //A unique monster information class, calls MonsterInfo to find the base monster traits(like dodge, damage, etc)
        public int SpawnX;
        public int SpawnY;
        public int SpawnID;
        public int MaxHP;
        public int CurrentHP;
        public int X;
        public int Y;
        public int Map;
        public int Direction;
        public int ID;
        public int UID;
        public int Level;
        public MonsterInfo Info;
        protected System.Timers.Timer Despawn;
        protected System.Timers.Timer Move;
        protected bool Moving;
        public void Die(int KillerID)
        {
            GenerateDrop(KillerID);
            Despawn = new System.Timers.Timer();
            Despawn.Interval = 1500;
            Despawn.AutoReset = false;
            Despawn.Elapsed += delegate { FinalDie(); };
            Despawn.Start();
        }
        public void GenerateDrop(int Killer)
        {
            if (Calculation.PercentSuccess(30))
            {
                int Times = 1;
                if (Calculation.PercentSuccess(15))
                {
                    Times = MainGS.Rand.Next(1, 6);
                }
                for (int i = 0; i < Times; i++)
                {
                    int Money = MainGS.Rand.Next(1, 10);
                    if (Calculation.PercentSuccess(90))
                        Money = MainGS.Rand.Next(2, 240);
                    if (Calculation.PercentSuccess(70))
                        Money = MainGS.Rand.Next(60, 3000);
                    if (Calculation.PercentSuccess(50))
                        Money = MainGS.Rand.Next(200, 4000);
                    if (Calculation.PercentSuccess(30))
                        Money = MainGS.Rand.Next(1000, 30000);
                    if (Calculation.PercentSuccess(100))
                        Money = MainGS.Rand.Next(2000, 50000);
                    Money = Money / ((138 - Level) * 10);
                    if (Money < 1)
                        Money = 1;
                    Struct.ItemGround IG = new Struct.ItemGround();
                    Money *= 3;
                    IG.Money = Money;
                    if (Money < 10)
                        IG.ItemID = 1090000;
                    else if (Money < 100)
                        IG.ItemID = 1090010;
                    else if (Money < 1000)
                        IG.ItemID = 1090020;
                    else if (Money < 3000)
                        IG.ItemID = 1091000;
                    else if (Money < 10000)
                        IG.ItemID = 1091010;
                    else
                        IG.ItemID = 1091020;
                    IG.UID = MainGS.Rand.Next(1, 1000);
                    while (MainGS.ItemFloor.ContainsKey(IG.UID))
                    {
                        IG.UID = MainGS.Rand.Next(1, 1000);
                    }
                    IG.Map = Map;
                    IG.X = (X - MainGS.Rand.Next(4) + MainGS.Rand.Next(4));
                    IG.Y = (Y - MainGS.Rand.Next(4) + MainGS.Rand.Next(4));
                    if (MainGS.Maps.ContainsKey(IG.Map))
                    {
                        Struct.DmapData Mapping = MainGS.Maps[IG.Map];
                        byte tries = 0;
                        while (!Mapping.CheckLoc((ushort)IG.X, (ushort)IG.Y))
                        {
                            IG.X = (X - MainGS.Rand.Next(4) + MainGS.Rand.Next(4));
                            IG.Y = (Y - MainGS.Rand.Next(4) + MainGS.Rand.Next(4));
                            tries++;
                            if (tries > 8)
                                break;
                        }
                    }
                    IG.OwnerOnly = new System.Timers.Timer();
                    IG.OwnerOnly.Interval = 10000;
                    IG.OwnerOnly.AutoReset = false;
                    IG.OwnerID = Killer;
                    IG.Dispose = new System.Timers.Timer();
                    IG.Dispose.Interval = 60000;
                    IG.Dispose.AutoReset = false;
                    IG.Dispose.Elapsed += delegate { IG.Disappear(); };
                    IG.Dispose.Start();
                    IG.OwnerOnly.Start();
                    //lock(MainGS.ItemFloor)
                    //{
                    try
                    {
                        Monitor.Enter(MainGS.ItemFloor);
                        MainGS.ItemFloor.Add(IG.UID, IG);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                    finally
                    {
                        Monitor.Exit(MainGS.ItemFloor);
                    }
                    //}
                    EudemonPacket.ToLocal(EudemonPacket.DropItem(IG.UID, IG.ItemID, IG.X, IG.Y), IG.X, IG.Y, IG.Map, 0, 0);
                }
            }
            else
            {
                if (Calculation.PercentSuccess(3.7) && Level > 22)
                {
                    Struct.ItemGround IG = new Struct.ItemGround();
                    if (Calculation.PercentSuccess(.333333))
                        IG.ItemID = 1088000;
                    else
                        IG.ItemID = 1088001;
                    IG.Map = Map;
                    IG.X = (X - MainGS.Rand.Next(4) + MainGS.Rand.Next(4));
                    IG.Y = (Y - MainGS.Rand.Next(4) + MainGS.Rand.Next(4));
                    if (MainGS.Maps.ContainsKey(IG.Map))
                    {
                        Struct.DmapData Mapping = MainGS.Maps[IG.Map];
                        byte tries = 0;
                        while (!Mapping.CheckLoc((ushort)IG.X, (ushort)IG.Y))
                        {
                            IG.X = (X - MainGS.Rand.Next(4) + MainGS.Rand.Next(4));
                            IG.Y = (Y - MainGS.Rand.Next(4) + MainGS.Rand.Next(4));
                            tries++;
                            if (tries > 8)
                                break;
                        }
                    }
                    IG.OwnerOnly = new System.Timers.Timer();
                    IG.OwnerOnly.Interval = 10000;
                    IG.OwnerOnly.AutoReset = false;
                    IG.OwnerID = Killer;
                    IG.UID = MainGS.Rand.Next(1000, 9999999);
                    while (MainGS.ItemFloor.ContainsKey(IG.UID))
                    {
                        IG.UID = MainGS.Rand.Next(1000, 9999999);
                    }
                    //TODO: UID generation that is better.
                    IG.Dispose = new System.Timers.Timer();
                    IG.Dispose.Interval = 60000;
                    IG.Dispose.AutoReset = false;
                    IG.Dispose.Elapsed += delegate { IG.Disappear(); };
                    IG.Dispose.Start();
                    IG.OwnerOnly.Start();
                    //lock(MainGS.ItemFloor)
                    //{
                    try
                    {
                        Monitor.Enter(MainGS.ItemFloor);
                        MainGS.ItemFloor.Add(IG.UID, IG);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                    finally
                    {
                        Monitor.Exit(MainGS.ItemFloor);
                    }
                    //}
                    EudemonPacket.ToLocal(EudemonPacket.DropItem(IG.UID, IG.ItemID, IG.X, IG.Y), IG.X, IG.Y, IG.Map, 0, 0);
                }
                if (Calculation.PercentSuccess(27)) //Drop an item
                {
                    int Quality = MainGS.Rand.Next(3, 6);
                    int Soc1 = 0;
                    int Soc2 = 0;
                    int Bless = 0;
                    int Plus = 0;
                    if (Calculation.PercentSuccess(2))
                        Plus = 1;
                    if (Calculation.PercentSuccess(2))
                        Quality = 7;
                    if (Calculation.PercentSuccess(1))
                        Quality = 8;
                    if (Calculation.PercentSuccess(.578))
                        Quality = 9;
                    int ItemID = Calculation.Item(Level, Quality);
                    if (ItemID > 0)
                    {
                        if (Calculation.Type1(ItemID.ToString()) == 4 || Calculation.Type1(ItemID.ToString()) == 5)
                        {
                            if (Quality < 8)
                            {
                                if (Calculation.PercentSuccess(15))
                                {
                                    Soc1 = 255;
                                    if (Calculation.PercentSuccess(7))
                                    {
                                        Soc2 = 255;
                                    }
                                }
                            }
                            else if (Quality >= 8)
                            {
                                if (Calculation.PercentSuccess(5))
                                {
                                    Soc1 = 255;
                                    if (Calculation.PercentSuccess(2))
                                    {
                                        Soc2 = 255;
                                    }
                                }
                            }
                        }
                        if (Calculation.PercentSuccess(3))
                        {
                            Bless = MainGS.Rand.Next(1, 7);
                            while (Bless != 1 && Bless != 3 && Bless != 5 && Bless != 7)
                            {
                                Bless = MainGS.Rand.Next(1, 7);
                            }
                        }
                        Struct.ItemGround DropItem = new Struct.ItemGround();
                        DropItem.Bless = Bless;
                        DropItem.Plus = Plus;
                        DropItem.Soc1 = Soc1;
                        DropItem.Soc2 = Soc2;
                        DropItem.Color = MainGS.Rand.Next(3, 9);
                        DropItem.MaxDura = MainGS.Rand.Next(10, 70);
                        DropItem.Dura = MainGS.Rand.Next(0, DropItem.MaxDura);
                        DropItem.Enchant = 0;
                        DropItem.ItemID = ItemID;
                        DropItem.Map = Map;
                        DropItem.X = (X - MainGS.Rand.Next(4) + MainGS.Rand.Next(4));
                        DropItem.Y = (Y - MainGS.Rand.Next(4) + MainGS.Rand.Next(4));
                        if (MainGS.Maps.ContainsKey(DropItem.Map))
                        {
                            Struct.DmapData Mapping = MainGS.Maps[DropItem.Map];
                            byte tries = 0;
                            while (!Mapping.CheckLoc((ushort)DropItem.X, (ushort)DropItem.Y))
                            {
                                DropItem.X = (X - MainGS.Rand.Next(4) + MainGS.Rand.Next(4));
                                DropItem.Y = (Y - MainGS.Rand.Next(4) + MainGS.Rand.Next(4));
                                tries++;
                                if (tries > 8)
                                    break;
                            }
                        }
                        DropItem.OwnerID = Killer;
                        DropItem.OwnerOnly = new System.Timers.Timer();
                        DropItem.OwnerOnly.Interval = 10000;
                        DropItem.OwnerOnly.AutoReset = false;
                        DropItem.Dispose = new System.Timers.Timer();
                        DropItem.Dispose.Interval = 60000;
                        DropItem.Dispose.AutoReset = false;
                        DropItem.Dispose.Elapsed += delegate { DropItem.Disappear(); };
                        DropItem.UID = MainGS.Rand.Next(1000, 9999999);
                        while (MainGS.ItemFloor.ContainsKey(DropItem.UID))
                        {
                            DropItem.UID = MainGS.Rand.Next(1000, 9999999);
                        }
                        //lock(MainGS.ItemFloor)
                        //{
                        try
                        {
                            Monitor.Enter(MainGS.ItemFloor);
                            MainGS.ItemFloor.Add(DropItem.UID, DropItem);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                        }
                        finally
                        {
                            Monitor.Exit(MainGS.ItemFloor);
                        }
                        //}
                        DropItem.OwnerOnly.Start();
                        DropItem.Dispose.Start();
                        EudemonPacket.ToLocal(EudemonPacket.DropItem(DropItem.UID, DropItem.ItemID, DropItem.X, DropItem.Y), X, Y, Map, 0, 0);
                    }
                }
            }
        }
        protected void FinalDie()
        {
            Moving = false;
            if (Move != null)
            {
                if (Move.Enabled)
                    Move.Stop();
                Move.Dispose();
            }
            Despawn.Stop();
            Despawn.Dispose();
            EudemonPacket.ToLocal(EudemonPacket.General(UID, (ushort)X, (ushort)Y, (ushort)Direction, Struct.DataType.EntityRemove, 0), X, Y, Map, 0, 0);//@TODO: Convert Moster to use ushorts.
        }
        public void TriggerMove()
        {
            int Distance = Info.AggroRange;
            int CharToAttack = -1;
            if (Distance > 14)
                Distance = 14;
            try
            {
                Monitor.Enter(MainGS.ClientPool);
                foreach (KeyValuePair<int, ClientSocket> Clients in MainGS.ClientPool)
                {
                    ClientSocket Client = Clients.Value;
                    if ((int)Client.Client.Map == Map)
                    {
                        if (Calculation.InRange(Client.Client.X, Client.Client.Y, X, Y, Distance))
                        {
                            if (Info.Name == "CoEmuGuard")
                            {
                                if (Client.Client.Flashing)
                                {
                                    CharToAttack = Client.Client.ID;
                                    break;
                                }
                            }
                            else if (Info.Name == "CoEmuPatrol")
                            {
                                if (Client.Client.PkPoints > 100)
                                {
                                    CharToAttack = Client.Client.ID;
                                    break;
                                }
                            }
                            else if (Info.Name == "GuardReviver")
                            {
                                //Nothing
                            }
                            else
                            {
                                CharToAttack = Client.Client.ID;
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                Monitor.Exit(MainGS.ClientPool);
            }
            if (CharToAttack > -1 && CharToAttack > 0)
            {
                AttackMove(CharToAttack, true);
            }
        }
        public void AttackMove(int Target, bool First)
        {
            if (Move != null)
            {
                if (Move.Enabled)
                    return;
            }
            if (Moving && First)
                return;
            if (CurrentHP == 0)
                return;
            if (!MainGS.Monsters.ContainsKey(UID))
            {
                if (Move != null)
                {
                    if (Move.Enabled)
                        Move.Stop();
                    Move.Dispose();
                }
                return;
            }
            if (!MainGS.ClientPool.ContainsKey(Target))
            {
                if (Move != null)
                {
                    if (Move.Enabled)
                        Move.Stop();
                    Move.Dispose();
                }
                return;
            }
            Move = new System.Timers.Timer();
            Move.Elapsed += delegate { MoveAgain(Target); };
            Move.Interval = Info.Speed;
            Move.AutoReset = false;
            Moving = true;
            int AttackRange = Info.AttackRange;
            ClientSocket Attacked = MainGS.ClientPool[Target];
            if (Attacked.Client.Dead)
            {
                Moving = false;
                Move = null;
                return;
            }
            if (Calculation.InRange(Attacked.Client.X, Attacked.Client.Y, X, Y, Info.AggroRange) && !Calculation.InRange(Attacked.Client.X, Attacked.Client.Y, X, Y, AttackRange))
            {
                byte ToDir = (byte)(7 - (Math.Floor(Calculation.Direction(X, Y, Attacked.Client.X, Attacked.Client.Y) / 45 % 8)) - 1 % 8);
                ToDir = (byte)((int)ToDir % 8);
                short AddX = 0;
                short AddY = 0;
                if (ToDir == 255)
                    ToDir = 7;
                switch (ToDir)
                {
                    case 0:
                        {
                            AddY = 1;
                            break;
                        }
                    case 1:
                        {
                            AddX = -1;
                            AddY = 1;
                            break;
                        }
                    case 2:
                        {
                            AddX = -1;
                            break;
                        }
                    case 3:
                        {
                            AddX = -1;
                            AddY = -1;
                            break;
                        }
                    case 4:
                        {
                            AddY = -1;
                            break;
                        }
                    case 5:
                        {
                            AddX = 1;
                            AddY = -1;
                            break;
                        }
                    case 6:
                        {
                            AddX = 1;
                            break;
                        }
                    case 7:
                        {
                            AddY = 1;
                            AddX = 1;
                            break;
                        }
                }
                if (MainGS.Maps.ContainsKey(Map))
                {
                    Struct.DmapData Mapping = MainGS.Maps[Map];
                    if (!Mapping.CheckLoc((ushort)(X + AddX), (ushort)(Y + AddY)))
                    {
                        Move = null;
                        Moving = false;
                        return;
                    }
                }
                X += AddX;
                Y += AddY;
                EudemonPacket.ToLocal(EudemonPacket.Walk(ToDir, UID, X, Y), X, Y, Map, 0, 0);
                Move.Start();
            }
            else if (Calculation.InRange(Attacked.Client.X, Attacked.Client.Y, X, Y, Info.AggroRange) && Calculation.InRange(Attacked.Client.X, Attacked.Client.Y, X, Y, AttackRange))
            {
                int Damage = 0;
                if (Info.MinAttack < Info.MaxAttack)
                    Damage = MainGS.Rand.Next(Info.MinAttack, Info.MaxAttack);
                else
                    Damage = MainGS.Rand.Next(Info.MaxAttack, Info.MinAttack);
                Damage -= Attacked.Client.Defense;
                double Tort = 0;
                Tort += Attacked.Client.NBG * 0.05;
                Tort += Attacked.Client.RBG * 0.10;
                Tort += Attacked.Client.SBG * 0.15;
                Damage = (int)Math.Floor(Damage * (1 - Tort));
                if (Attacked.Client.Bless > 0)
                {
                    Damage = (int)Math.Floor(Damage * (1 - (Attacked.Client.Bless * 0.01)));
                }
                if (Damage < 0)
                    Damage = 1;
                if ((Attacked.Client.isGM || Attacked.Client.isPM) && Attacked.Client.Invincible)
                    Damage = 0;
                Calculation.doPlayer(this, Attacked, Damage, 2);
                Move.Interval = Info.AttackSpeed;
                Move.Start();
                if (Info.Name == "CoEmuGuard")
                    EudemonPacket.ToLocal(EudemonPacket.Chat(0, Info.Name, "ALL", "How dare you try to PK here, " + Attacked.Client.Name + ", you must die!", Struct.ChatType.Talk), X, Y, Map, 0, 0);
                else if (Info.Name == "CoEmuPatrol")
                    EudemonPacket.ToLocal(EudemonPacket.Chat(0, Info.Name, "ALL", "How dare you shows your shamed face here, " + Attacked.Client.Name + ", you must die!", Struct.ChatType.Talk), X, Y, Map, 0, 0);
              }
            else
            {
                Move = null;
                Moving = false;
            }
        }
        public void MoveAgain(int Target)
        {
            Move.Stop();
            Move.Dispose();
            if (!MainGS.ClientPool.ContainsKey(Target))
            {
                return;
            }
            ClientSocket Attacked = MainGS.ClientPool[Target];
            if (Calculation.InRange(Attacked.Client.X, Attacked.Client.Y, X, Y, Info.AggroRange))
            {
                AttackMove(Target, false);
            }
            else
            {
                Moving = false;
            }
        }
    }
}
