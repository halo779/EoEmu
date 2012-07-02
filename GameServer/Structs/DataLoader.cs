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
                    NewItem.Str_Require = 0;//@TODO:Remove
                    NewItem.Dex_Require = item.RequiredDexterity;
                    NewItem.Vit_Require = 0;//@TODO:Remove
                    NewItem.Spi_Require = 0;//@TODO:Remove
                    NewItem.Tradeable = item.Monopoly;
                    NewItem.Cost = (int)item.GoldPrice;
                    NewItem.MaxDamage = item.PhysicalAttackMin;
                    NewItem.MinDamage = item.PhysicalAttackMax;
                    NewItem.DefenseAdd = item.PhysicalDefence;
                    NewItem.DexAdd = 0;
                    NewItem.DodgeAdd = item.Dodge;
                    NewItem.HPAdd = 0;
                    NewItem.MPAdd = 0;
                    NewItem.Dura = (int)item.Amount;
                    NewItem.MaxDura = (int)item.AmountLimit;
                    NewItem.MagicAttack = item.MagicalAttackMin;
                    NewItem.MDefenseAdd = item.MagicalDefence;
                    NewItem.Range = item.AttackRange;
                    NewItem.Frequency = item.Hitrate;
                    NewItem.CPCost = (int)item.EPCost;

                    if (!Nano.Items.ContainsKey(NewItem.ID))
                        Nano.Items.Add(NewItem.ID, NewItem);
                }
                Console.WriteLine("[GameServer] Loaded: " + Nano.Items.Count + " items in " + (System.Environment.TickCount - start) + "MS");
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
                                NewItem.Prof = Convert.ToInt32(data[3]);
                                NewItem.Level = Convert.ToInt32(data[4]);
                                NewItem.Sex = Convert.ToInt32(data[5]);
                                NewItem.Str_Require = Convert.ToInt32(data[6]);
                                NewItem.Dex_Require = Convert.ToInt32(data[7]);
                                NewItem.Vit_Require = Convert.ToInt32(data[8]);
                                NewItem.Spi_Require = Convert.ToInt32(data[9]);
                                NewItem.Tradeable = Convert.ToInt32(data[10]);
                                NewItem.Cost = Convert.ToInt32(data[12]);
                                NewItem.MaxDamage = Convert.ToInt32(data[14]);
                                NewItem.MinDamage = Convert.ToInt32(data[15]);
                                NewItem.DefenseAdd = Convert.ToInt32(data[16]);
                                NewItem.DexAdd = Convert.ToInt32(data[17]);
                                NewItem.DodgeAdd = Convert.ToInt32(data[18]);
                                NewItem.HPAdd = Convert.ToInt32(data[19]);
                                NewItem.MPAdd = Convert.ToInt32(data[20]);
                                NewItem.Dura = Convert.ToInt32(data[21]);
                                NewItem.MaxDura = Convert.ToInt32(data[22]);
                                NewItem.MagicAttack = Convert.ToInt32(data[29]);
                                NewItem.MDefenseAdd = Convert.ToInt32(data[30]);
                                NewItem.Range = Convert.ToInt32(data[31]);
                                NewItem.Frequency = Convert.ToInt32(data[32]);
                                NewItem.CPCost = Convert.ToInt32(data[36]);
                                if (!Nano.Items.ContainsKey(NewItem.ID))
                                    Nano.Items.Add(NewItem.ID, NewItem);
                            }
                        }
                    }
                }
                Console.WriteLine("[GameServer] Loaded: " + Nano.Items.Count + " items in " + (System.Environment.TickCount - start) + "MS");
            }
            else
            {
                Console.WriteLine("[GameServer] items.txt NOT FOUND! ITEMS WILL NOT WORK!");
            }
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
                                    NewItem.MDamageAdd = Convert.ToInt32(data[6]);
                                    NewItem.MDefAdd = Convert.ToInt32(data[7]);
                                    NewItem.AccuracyAdd = Convert.ToInt32(data[8]);
                                    NewItem.DodgeAdd = Convert.ToInt32(data[9]);
                                }
                                if (!Nano.ItemPluses.ContainsKey(NewItem.ID))
                                {
                                    ItemPlusDB DB = new ItemPlusDB();
                                    DB.DB.Add(NewItem.Plus, NewItem);
                                    Nano.ItemPluses.Add(NewItem.ID, DB);
                                }
                                else
                                {
                                    ItemPlusDB DB = Nano.ItemPluses[NewItem.ID];
                                    if (!DB.DB.ContainsKey(NewItem.Plus))
                                        DB.DB.Add(NewItem.Plus, NewItem);
                                }
                            }
                        }
                    }
                }
                Console.WriteLine("[GameServer] Loaded: " + Nano.ItemPluses.Count + " item plus data in " + (System.Environment.TickCount - start) + "MS");
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
            foreach (KeyValuePair<int, MonsterSpawn> Spawn in Nano.MonsterSpawns)
            {
                MonsterSpawn Mob = Spawn.Value;
                MonsterInfo MobInfo = null;
                if (Nano.BaseMonsters.ContainsKey(Mob.MobID))
                {
                    MobInfo = Nano.BaseMonsters[Mob.MobID];
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
                            //Mon.X = Nano.Rand.Next(X, XStop);
                            //Mon.Y = Nano.Rand.Next(Y, YStop);
                            //Mon.X = Mon.SpawnX;
                            //Mon.Y = Mon.SpawnY;
                            MobX = Nano.Rand.Next(Mob.X, (Mob.X + Mob.XStop));
                            MobY = Nano.Rand.Next(Mob.Y, (Mob.Y + Mob.YStop));
                            if (Nano.Maps.ContainsKey(Mob.Map))
                            {
                                DmapData Map = Nano.Maps[Mob.Map];
                                while (!Map.CheckLoc((ushort)MobX, (ushort)MobY))
                                {
                                    MobX = Nano.Rand.Next(Mob.X, (Mob.X + Mob.XStop));
                                    MobY = Nano.Rand.Next(Mob.Y, (Mob.Y + Mob.YStop));
                                }
                                ValidMap = true;
                            }
                            if (ValidMap)
                            {
                                Monster Mon = new Monster();
                                UID = Nano.Rand.Next(200000, 600000);
                                while (Nano.Monsters.ContainsKey(UID))
                                {
                                    UID = Nano.Rand.Next(200000, 600000);
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
                                if (!Nano.Monsters.ContainsKey(Mon.UID))
                                {
                                    Nano.Monsters.Add(Mon.UID, Mon);
                                }
                                else
                                {
                                    while (Nano.Monsters.ContainsKey(Mon.UID))
                                    {
                                        Mon.UID = Nano.Rand.Next(200000, 600000);
                                    }
                                    Nano.Monsters.Add(Mon.UID, Mon);
                                }
                            }
                        }
                    }
                    int RespawnTimer = Nano.Rand.Next(7000, 15000);
                    Mob.RespawnTimer = new System.Timers.Timer();
                    Mob.RespawnTimer.Interval = RespawnTimer;
                    Mob.RespawnTimer.Elapsed += delegate { Mob.Respawn(); };
                    Mob.RespawnTimer.Start();
                }
            }
            Console.WriteLine("[GameServer] Spawned " + Nano.Monsters.Count + " monsters into the world. ");
        }
        public static void LoadNpcs()
        {
            Database.Database.LoadNpcs();
        }
        public static void LoadServerskills()
        {
            Database.Database.GetServerSkills();
            Console.WriteLine("[GameServer] Loaded " + Nano.ServerSkills.Count + " server skills.");
        }
        public static void LoadTNpcs()
        {
            Database.Database.LoadTNpcs();
            Console.WriteLine("[GameServer] Loaded " + Nano.TerrainNpcs.Count + " Terrain NPCs.");
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
                        if (!Nano.Maps.ContainsKey((int)_Map.MapId))
                        {
                            DmapData NewMap = new DmapData(Path);
                            Nano.Maps.Add((int)_Map.MapId, NewMap);
                            Console.WriteLine("[GameServer] Loaded Dmap " + _Map.MapFile + "[" + _Map.MapId + "]");
                        }
                    }
                }
                else
                {
                    //Console.WriteLine("[GameServer] Did not load DMap: " + _Map.MapFile + "[" + _Map.MapId + "]"); @TODO: show error again, maybe if a debug switch is flagged.
                }
            }

            Console.WriteLine("[GameServer] Looked at: " + DMD.MapCount + " Maps, Loaded: " + Nano.Maps.Count + " DMaps in " + (System.Environment.TickCount - start) + "MS");
        }
    }
}
