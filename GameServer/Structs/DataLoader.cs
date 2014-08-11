using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using GameServer.Database;
using GameServer.Entities;
using ItemtypeData;

namespace GameServer.Structs
{
    /// <summary>
    /// Provides methods for loading various datas, such as Items, Item Plus Datas, Guilds, Maps, etc
    /// </summary>
    public partial class Struct
    {
        public static void LoadItemType(string FileLoc)
        {
            if (File.Exists(FileLoc))
            {
                int start = System.Environment.TickCount;

                ItemtypeData.Manager Items = new Manager(FileLoc);
                foreach (var item in Items.Items.Values)
                {
                    ItemData NewItem = new ItemData();
                    NewItem.ID = (int)item.ID;
                    NewItem.Name = item.Name;
                    NewItem.Class = (int)item.RequiredProfession;
                    NewItem.Level = item.RequiredLevel;

                    NewItem.Sex = (int)item.RequiredSex;

                    NewItem.force_Require = item.RequiredForce;
                    NewItem.Dex_Require = item.RequiredDexterity;
                    NewItem.Health_Require = item.RequiredHealth;
                    NewItem.soul_Require = item.RequiredSoul;

                    NewItem.soul_value = (int)item.Soul;

                    NewItem.Tradeable = item.Monopoly;
                    NewItem.Cost = (int)item.GoldPrice;

                    NewItem.ident = Convert.ToByte(item.Identified);
                    NewItem.gem1 = item.GemOne;
                    NewItem.gem2 = item.GemTwo;
                    NewItem.magic1 = item.MagicOne;
                    NewItem.magic2 = item.MagicTwo;
                    NewItem.magic3 = item.MagicThree;

                    NewItem.MaxDamage = item.PhysicalAttackMin;
                    NewItem.MinDamage = item.PhysicalAttackMax;
                    NewItem.DefenseAdd = item.PhysicalDefence;

                    NewItem.DodgeAdd = item.Dodge;
                    NewItem.HPAdd = 0;
                    NewItem.MPAdd = 0;
                    NewItem.Dura = (int)item.Amount;
                    NewItem.MaxDura = (int)item.AmountLimit;
                    NewItem.MinMagicAttack = item.MagicalAttackMin;
                    NewItem.MaxMagicAttack = item.MagicalAttackMax;

                    NewItem.MDefenseAdd = item.MagicalDefence;
                    NewItem.Range = item.AttackRange;
                    NewItem.Frequency = item.Hitrate;
                    NewItem.AttackRate = item.AttackSpeed;

                    NewItem.EPCost = (int)item.EPCost;

                    if (!MainGS.Items.ContainsKey(NewItem.ID))
                        MainGS.Items.Add(NewItem.ID, NewItem);
                }
                Console.WriteLine("[GameServer] Loaded: " + MainGS.Items.Count + " items in " + (System.Environment.TickCount - start) + "MS");
            }
            else
            {
                Console.WriteLine("[GameServer-ERROR] Itemtype.dat not found!, items will not work.");
            }


        }
        /// <summary>
        /// OLD LOADING METHOD
        /// </summary>
        /// <param name="FileLoc"></param>
        public static void LoadItems(string FileLoc)
        {
            if (File.Exists(FileLoc))
            {
                int start = System.Environment.TickCount;
                TextReader TR = new StreamReader(FileLoc);
                string Items = TR.ReadToEnd();
                TR.Close();
                Items = Items.Replace("\r", "");
                string[] AllItems = Items.Split('\n');
                foreach (string _item in AllItems)
                {
                    string _item_ = _item.Trim();
                    if (_item_.Length >= 2)
                    {
                        if (_item_.IndexOf("//", 0, 2) != 0)
                        {
                            string[] data = _item_.Split(' ');
                            if (data.Length >= 37)
                            {
                                ItemData NewItem = new ItemData();
                                NewItem.ID = Convert.ToInt32(data[0]);
                                NewItem.Name = data[1].Trim();
                                NewItem.Class = Convert.ToInt32(data[2]);
                                //NewItem.Prof = Convert.ToInt32(data[3]);
                                NewItem.Level = Convert.ToInt32(data[4]);
                                NewItem.Sex = Convert.ToInt32(data[5]);
                                //NewItem.Str_Require = Convert.ToInt32(data[6]);
                                NewItem.Dex_Require = Convert.ToInt32(data[7]);
                                NewItem.Health_Require = Convert.ToInt32(data[8]);
                                //NewItem.Spi_Require = Convert.ToInt32(data[9]);
                                NewItem.Tradeable = Convert.ToInt32(data[10]);
                                NewItem.Cost = Convert.ToInt32(data[12]);
                                NewItem.MaxDamage = Convert.ToInt32(data[14]);
                                NewItem.MinDamage = Convert.ToInt32(data[15]);
                                NewItem.DefenseAdd = Convert.ToInt32(data[16]);
                                //NewItem.DexAdd = Convert.ToInt32(data[17]);
                                NewItem.DodgeAdd = Convert.ToInt32(data[18]);
                                NewItem.HPAdd = Convert.ToInt32(data[19]);
                                NewItem.MPAdd = Convert.ToInt32(data[20]);
                                NewItem.Dura = Convert.ToInt32(data[21]);
                                NewItem.MaxDura = Convert.ToInt32(data[22]);
                                NewItem.MinMagicAttack = Convert.ToInt32(data[29]);
                                NewItem.MDefenseAdd = Convert.ToInt32(data[30]);
                                NewItem.Range = Convert.ToInt32(data[31]);
                                NewItem.Frequency = Convert.ToInt32(data[32]);
                                NewItem.EPCost = Convert.ToInt32(data[36]);
                                if (!MainGS.Items.ContainsKey(NewItem.ID))
                                    MainGS.Items.Add(NewItem.ID, NewItem);
                            }
                        }
                    }
                }
                Console.WriteLine("[GameServer] Loaded: " + MainGS.Items.Count + " items in " + (System.Environment.TickCount - start) + "MS");
            }
            else
            {
                Console.WriteLine("[GameServer] items.txt NOT FOUND! ITEMS WILL NOT WORK!");
            }
        }

        public static void LoadItemPlusesDatabase()
        {
            int start = System.Environment.TickCount;
            Database.Database.LoadItemPluses();
            Console.WriteLine("[GameServer] Loaded: " + MainGS.ItemPluses.Count + " item plus data in " + (System.Environment.TickCount - start) + "MS");
        }

        public static void LoadItemPluses(string FileLoc)
        {
            if (File.Exists(FileLoc))
            {
                int start = System.Environment.TickCount;
                TextReader TR = new StreamReader(FileLoc);
                string Items = TR.ReadToEnd();
                TR.Close();
                Items = Items.Replace("\r", "");
                string[] AllItems = Items.Split('\n');
                foreach (string _item in AllItems)
                {
                    string _item_ = _item.Trim();
                    if (_item_.Length >= 2)
                    {
                        if (_item_.IndexOf("//", 0, 2) != 0)
                        {
                            string[] data = _item_.Split(' ');
                            if (data.Length >= 10)
                            {
                                ItemPlus NewItem = new ItemPlus();
                                if (data[0] != "0")
                                {
                                    NewItem.ID = Convert.ToInt32(data[0]);
                                    NewItem.Plus = Convert.ToInt32(data[1]);
                                    NewItem.HPAdd = Convert.ToInt32(data[2]);
                                    NewItem.MinDmg = Convert.ToInt32(data[3]);
                                    NewItem.MaxDmg = Convert.ToInt32(data[4]);
                                    NewItem.DefenseAdd = Convert.ToInt32(data[5]);
                                    //NewItem.MDamageAdd = Convert.ToInt32(data[6]);
                                    NewItem.MDefAdd = Convert.ToInt32(data[7]);
                                    //NewItem.AccuracyAdd = Convert.ToInt32(data[8]);
                                    NewItem.DodgeAdd = Convert.ToInt32(data[9]);
                                }
                                if (!MainGS.ItemPluses.ContainsKey(NewItem.ID))
                                {
                                    ItemPlusDB DB = new ItemPlusDB();
                                    DB.DB.Add(NewItem.Plus, NewItem);
                                    MainGS.ItemPluses.Add(NewItem.ID, DB);
                                }
                                else
                                {
                                    ItemPlusDB DB = MainGS.ItemPluses[NewItem.ID];
                                    if (!DB.DB.ContainsKey(NewItem.Plus))
                                        DB.DB.Add(NewItem.Plus, NewItem);
                                }
                            }
                        }
                    }
                }
                Console.WriteLine("[GameServer] Loaded: " + MainGS.ItemPluses.Count + " item plus data in " + (System.Environment.TickCount - start) + "MS");
            }
            else
            {
                Console.WriteLine("[GameServer] " + FileLoc + " NOT FOUND!");
            }
        }
        public static void LoadMonsters()
        {
            Database.Database.LoadMonsters();
            Database.Database.LoadMonsterSpawns();
            foreach (KeyValuePair<int, MonsterSpawn> Spawn in MainGS.MonsterSpawns)
            {
                MonsterSpawn Mob = Spawn.Value;
                MonsterInfo MobInfo = null;
                if (MainGS.BaseMonsters.ContainsKey(Mob.MobID))
                {
                    MobInfo = MainGS.BaseMonsters[Mob.MobID];
                }
                if (MobInfo != null)
                {
                    if (Mob.MaxSpawnNumber > 0)
                    {
                        for (int x = 0; x < Mob.MaxSpawnNumber; x++)
                        {
                            bool ValidMap = false;
                            int X = Mob.X;
                            int XStop = Mob.X + Mob.XStop;
                            int Y = Mob.Y;
                            int YStop = Mob.Y + Mob.YStop;
                            int MobX = 0;
                            int MobY = 0;
                            int UID = 0;
                            //TODO: Dmaps
                            //Mon.X = MainGS.Rand.Next(X, XStop);
                            //Mon.Y = MainGS.Rand.Next(Y, YStop);
                            //Mon.X = Mon.SpawnX;
                            //Mon.Y = Mon.SpawnY;
                            MobX = MainGS.Rand.Next(Mob.X, (Mob.X + Mob.XStop));
                            MobY = MainGS.Rand.Next(Mob.Y, (Mob.Y + Mob.YStop));
                            if (MainGS.Maps.ContainsKey(Mob.Map))
                            {
                                DmapData Map = MainGS.Maps[Mob.Map];
                                while (!Map.CheckLoc((ushort)MobX, (ushort)MobY))
                                {
                                    MobX = MainGS.Rand.Next(Mob.X, (Mob.X + Mob.XStop));
                                    MobY = MainGS.Rand.Next(Mob.Y, (Mob.Y + Mob.YStop));
                                }
                                ValidMap = true;
                            }
                            if (ValidMap)
                            {
                                Monster Mon = new Monster();
                                UID = MainGS.Rand.Next(200000, 600000);
                                while (MainGS.Monsters.ContainsKey(UID))
                                {
                                    UID = MainGS.Rand.Next(200000, 600000);
                                }
                                Mon.CurrentHP = MobInfo.MaxHP;
                                Mon.MaxHP = MobInfo.MaxHP;
                                Mon.UID = UID;
                                Mon.Direction = 0;
                                Mon.ID = MobInfo.ID;
                                Mon.Info = MobInfo;
                                Mon.Level = MobInfo.Level;
                                Mon.Map = Mob.Map;
                                Mon.X = MobX;
                                Mon.SpawnX = MobX;
                                Mon.Y = MobY;
                                Mon.SpawnY = MobY;
                                Mon.SpawnID = Mob.SpawnID;
                                if (!MainGS.Monsters.ContainsKey(Mon.UID))
                                {
                                    MainGS.Monsters.Add(Mon.UID, Mon);
                                }
                                else
                                {
                                    while (MainGS.Monsters.ContainsKey(Mon.UID))
                                    {
                                        Mon.UID = MainGS.Rand.Next(200000, 600000);
                                    }
                                    MainGS.Monsters.Add(Mon.UID, Mon);
                                }
                            }
                        }
                    }
                    int RespawnTimer = MainGS.Rand.Next(7000, 15000);
                    Mob.RespawnTimer = new System.Timers.Timer();
                    Mob.RespawnTimer.Interval = RespawnTimer;
                    Mob.RespawnTimer.Elapsed += delegate { Mob.Respawn(); };
                    Mob.RespawnTimer.Start();
                }
            }
            Console.WriteLine("[GameServer] Spawned " + MainGS.Monsters.Count + " monsters into the world. ");
        }
        public static void LoadNpcs()
        {
            Database.Database.LoadNpcs();
        }
        public static void LoadServerskills()
        {
            Database.Database.GetServerSkills();
            Console.WriteLine("[GameServer] Loaded " + MainGS.ServerSkills.Count + " server skills.");
        }
        public static void LoadTNpcs()
        {
            Database.Database.LoadTNpcs();
            Console.WriteLine("[GameServer] Loaded " + MainGS.TerrainNpcs.Count + " Terrain NPCs.");
        }
        public static void LoadPortals()
        {
            Database.Database.LoadPortals();
        }
        public static void LoadMaps()
        {
            int start = System.Environment.TickCount;
            //GameMap DMD = new GameMap("gamemap.dat");
            GameMap DMD = new GameMap("gamemap.ini");
            foreach (DMap _Map in DMD.Maps)
            {
                if (Enum.IsDefined(typeof(Struct.Maps), (int)_Map.MapId))
                {
                    string Path = "maps\\" + _Map.MapFile;
                    if (File.Exists(Path))
                    {
                        if (!MainGS.Maps.ContainsKey((int)_Map.MapId))
                        {
                            DmapData NewMap = new DmapData(Path);
                            MainGS.Maps.Add((int)_Map.MapId, NewMap);
                            Console.WriteLine("[GameServer] Loaded Dmap " + _Map.MapFile + "[" + _Map.MapId + "]");
                        }
                    }
                }
                else
                {
                    //Console.WriteLine("[GameServer] Did not load DMap: " + _Map.MapFile + "[" + _Map.MapId + "]"); @TODO: show error again, maybe if a debug switch is flagged.
                }
            }

            Console.WriteLine("[GameServer] Looked at: " + DMD.MapCount + " Maps, Loaded: " + MainGS.Maps.Count + " DMaps in " + (System.Environment.TickCount - start) + "MS");
        }
    }
}
