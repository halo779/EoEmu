using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using GameServer.Entities;
using GameServer.Connections;
using GameServer.Structs;
namespace GameServer.Database
{
    /// <summary>
    /// Provides connections for information retrevial from the CoEmu database, which is MySQL based.
    /// </summary>
    public static class Database
    {
        public static bool CharExists(string Name)
        {
            bool exists = false;
            //Name = MakeSafeString(Name);
            MySqlCommand Cmd = new MySqlCommand("SELECT * FROM `characters` WHERE `Name` = \"" + Name + "\"", DatabaseConnection.NewConnection());
            MySqlDataReader DR = Cmd.ExecuteReader(CommandBehavior.CloseConnection);
            while (DR.Read())
            {
                if (Convert.ToInt32(DR["CharID"]) > 0)
                    exists = true;
            }
            DR.Close();
            Cmd.Dispose();
            return exists;
        }
        public static int NewCharacter(string Name, int Mesh, int Class, string Account)
        {
            if (Mesh == 1003 || Mesh == 1004)
            {
                Mesh = (Mesh + 10000);
            }
            else if (Mesh == 2002 || Mesh == 2001)
            {
                Mesh = (Mesh + 2010000);
            }
            else
            {
                //
            }
            int Hair = 420;
            //Name = MakeSafeString(Name);
            //Account = MakeSafeString(Account);
            MySqlCommand Cmd = new MySqlCommand("INSERT INTO characters(Name, Server, Account, Str, Dex, Spi, Vit, HairStyle, Model, Class) VALUES('" + Name + "','" + "ALL" + "','" + Account + "', 10,10,10,10," + Hair + "," + Mesh + "," + Class + ")", DatabaseConnection.NewConnection());
            Cmd.ExecuteNonQuery();
            Cmd.Connection.Close();
            Cmd.Connection.Dispose();
            Cmd.Dispose();
            MySqlCommand Cmd2 = new MySqlCommand("SELECT * FROM `characters` WHERE `Account` = \"" + Account + "\"", DatabaseConnection.NewConnection());
            MySqlDataReader DR = Cmd2.ExecuteReader(CommandBehavior.CloseConnection);
            int CharID = -1;
            while (DR.Read())
            {
                CharID = Convert.ToInt32(DR["CharID"]);
            }
            DR.Close();
            Cmd2.Dispose();
            InitialItems(CharID, Class);
            return CharID;
        }
        public static void InitialItems(int CharID, int Class)
        {
            try
            {
                switch (Class)
                {
                    case 10:
                        {
                            MySqlCommand Cmd = new MySqlCommand("INSERT INTO items(CharID, Position, ItemID, Plus, Minus, Enchant, Soc1, Soc2, Dura, MaxDura) VALUES(" + CharID + "," + 4 + "," + 410302 + "," + 0 + "," + 0 + "," + 0 + "," + 0 + "," + 0 + "," + 10 + "," + 10 + ")", DatabaseConnection.NewConnection());
                            Cmd.ExecuteNonQuery();
                            Cmd.Connection.Close();
                            Cmd.Dispose();
                            MySqlCommand Cmd2 = new MySqlCommand("INSERT INTO items(CharID, Position, ItemID, Plus, Minus, Enchant, Soc1, Soc2, Dura, MaxDura) VALUES(" + CharID + "," + 3 + "," + 132003 + "," + 0 + "," + 0 + "," + 0 + "," + 0 + "," + 0 + "," + 10 + "," + 10 + ")", DatabaseConnection.NewConnection());
                            Cmd2.ExecuteNonQuery();
                            Cmd2.Connection.Close();
                            Cmd2.Dispose();
                            break;
                        }
                    case 20:
                        {
                            MySqlCommand Cmd = new MySqlCommand("INSERT INTO items(CharID, Position, ItemID, Plus, Minus, Enchant, Soc1, Soc2, Dura, MaxDura) VALUES(" + CharID + "," + 4 + "," + 410302 + "," + 0 + "," + 0 + "," + 0 + "," + 0 + "," + 0 + "," + 10 + "," + 10 + ")", DatabaseConnection.NewConnection());
                            Cmd.ExecuteNonQuery();
                            Cmd.Connection.Close();
                            Cmd.Dispose();
                            MySqlCommand Cmd2 = new MySqlCommand("INSERT INTO items(CharID, Position, ItemID, Plus, Minus, Enchant, Soc1, Soc2, Dura, MaxDura) VALUES(" + CharID + "," + 3 + "," + 132003 + "," + 0 + "," + 0 + "," + 0 + "," + 0 + "," + 0 + "," + 10 + "," + 10 + ")", DatabaseConnection.NewConnection());
                            Cmd2.ExecuteNonQuery();
                            Cmd2.Connection.Close();
                            Cmd2.Dispose();
                            break;
                        }
                    case 40:
                        {
                            MySqlCommand Cmd = new MySqlCommand("INSERT INTO items(CharID, Position, ItemID, Plus, Minus, Enchant, Soc1, Soc2, Dura, MaxDura) VALUES(" + CharID + "," + 4 + "," + 500301 + "," + 0 + "," + 0 + "," + 0 + "," + 0 + "," + 0 + "," + 10 + "," + 10 + ")", DatabaseConnection.NewConnection());
                            Cmd.ExecuteNonQuery();
                            Cmd.Connection.Close();
                            Cmd.Dispose();
                            MySqlCommand Cmd2 = new MySqlCommand("INSERT INTO items(CharID, Position, ItemID, Plus, Minus, Enchant, Soc1, Soc2, Dura, MaxDura) VALUES(" + CharID + "," + 3 + "," + 132003 + "," + 0 + "," + 0 + "," + 0 + "," + 0 + "," + 0 + "," + 10 + "," + 10 + ")", DatabaseConnection.NewConnection());
                            Cmd2.ExecuteNonQuery();
                            Cmd2.Connection.Close();
                            Cmd2.Dispose();
                            MySqlCommand Cmd3 = new MySqlCommand("INSERT INTO items(CharID, Position, ItemID, Plus, Minus, Enchant, Soc1, Soc2, Dura, MaxDura) VALUES(" + CharID + "," + 5 + "," + 1050000 + "," + 0 + "," + 0 + "," + 0 + "," + 0 + "," + 0 + "," + 10 + "," + 10 + ")", DatabaseConnection.NewConnection());
                            Cmd3.ExecuteNonQuery();
                            Cmd3.Connection.Close();
                            Cmd3.Dispose();
                            break;
                        }
                    case 50:
                        {
                            MySqlCommand Cmd = new MySqlCommand("INSERT INTO items(CharID, Position, ItemID, Plus, Minus, Enchant, Soc1, Soc2, Dura, MaxDura) VALUES(" + CharID + "," + 4 + "," + 601301 + "," + 0 + "," + 0 + "," + 0 + "," + 0 + "," + 0 + "," + 10 + "," + 10 + ")", DatabaseConnection.NewConnection());
                            Cmd.ExecuteNonQuery();
                            Cmd.Connection.Close();
                            Cmd.Dispose();
                            MySqlCommand Cmd2 = new MySqlCommand("INSERT INTO items(CharID, Position, ItemID, Plus, Minus, Enchant, Soc1, Soc2, Dura, MaxDura) VALUES(" + CharID + "," + 3 + "," + 132003 + "," + 0 + "," + 0 + "," + 0 + "," + 0 + "," + 0 + "," + 10 + "," + 10 + ")", DatabaseConnection.NewConnection());
                            Cmd2.ExecuteNonQuery();
                            Cmd2.Connection.Close();
                            Cmd2.Dispose();
                            break;
                        }
                    case 100:
                        {
                            MySqlCommand Cmd = new MySqlCommand("INSERT INTO items(CharID, Position, ItemID, Plus, Minus, Enchant, Soc1, Soc2, Dura, MaxDura) VALUES(" + CharID + "," + 4 + "," + 421301 + "," + 0 + "," + 0 + "," + 0 + "," + 0 + "," + 0 + "," + 10 + "," + 10 + ")", DatabaseConnection.NewConnection());
                            Cmd.ExecuteNonQuery();
                            Cmd.Connection.Close();
                            Cmd.Dispose();
                            MySqlCommand Cmd2 = new MySqlCommand("INSERT INTO items(CharID, Position, ItemID, Plus, Minus, Enchant, Soc1, Soc2, Dura, MaxDura) VALUES(" + CharID + "," + 3 + "," + 132003 + "," + 0 + "," + 0 + "," + 0 + "," + 0 + "," + 0 + "," + 10 + "," + 10 + ")", DatabaseConnection.NewConnection());
                            Cmd2.ExecuteNonQuery();
                            Cmd2.Connection.Close();
                            Cmd2.Dispose();
                            break;
                        }
                    default:
                        {
                            Console.WriteLine("[GameServer] Unknown Classtype: " + Class);
                            break;
                        }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        public static Character GetCharacter(string UserName)
        {
            Character Client = new Character();
            MySqlCommand Cmd = new MySqlCommand("SELECT * FROM `characters` WHERE `Account` = \"" + UserName + "\"", DatabaseConnection.NewConnection());
            MySqlDataReader DR = Cmd.ExecuteReader(CommandBehavior.CloseConnection);
            while (DR.Read())
            {
                Client.ID = Convert.ToInt32(DR["CharID"]);
                Client.Map = (Struct.Maps)Convert.ToInt32(DR["Map"]);
                Client.Model = Convert.ToInt32(DR["Model"]);
                string Model = Convert.ToString((int)DR["Model"]);
                string Avatar = "";
                if (Model.Length == 5)
                {
                    Avatar = Model.Remove(1);
                }
                if (Model.Length == 6)
                {
                    Avatar = Model.Remove(2);
                }
                if (Model.Length == 7)
                {
                    Avatar = Model.Remove(3);
                }
                if (Model.EndsWith("1"))
                {
                    Client.GhostModel = (Convert.ToInt32(Avatar) * 10000) + 990000000;
                }
                else if (Model.EndsWith("2"))
                {
                    Client.GhostModel = (Convert.ToInt32(Avatar) * 10000) + 990000000;
                }
                else if (Model.EndsWith("3"))
                {
                    Client.GhostModel = (Convert.ToInt32(Avatar) * 10000) + 980000000;
                }
                else if (Model.EndsWith("4"))
                {
                    Client.GhostModel = (Convert.ToInt32(Avatar) * 10000) + 980000000;
                }
                Client.Name = Convert.ToString(DR["Name"]);
                Client.Spouse = Convert.ToString(DR["Spouse"]);
                Client.X = (ushort)Convert.ToInt32(DR["xCord"]);
                Client.Y = (ushort)Convert.ToInt32(DR["yCord"]);
                Client.EPs = Convert.ToInt32(DR["EPoints"]);
                Client.Level = Convert.ToInt32(DR["Level"]);
                Client.CurrentHP = Convert.ToInt32(DR["HP"]);
                Client.CurrentMP = Convert.ToInt32(DR["MP"]);
                Client.Dexterity = Convert.ToInt32(DR["Dex"]);
                Client.Exp = Convert.ToUInt64(DR["Exp"]);
                Client.Hair = Convert.ToInt32(DR["HairStyle"]);
                Client.Money = Convert.ToInt32(DR["Money"]);
                Client.WHMoney = Convert.ToInt32(DR["WHMoney"]);
                Client.Power = Convert.ToInt32(DR["Power"]);
                Client.Soul = Convert.ToInt32(DR["Soul"]);
                Client.EudBagSize = Convert.ToByte(DR["EudBagSize"]);
                Client.PkPoints = Convert.ToInt32(DR["PkPoints"]);
                Client.Vitality = Convert.ToInt32(DR["Vit"]);
                int GM = Convert.ToInt32(DR["isGM"]);
                if (GM == 1)
                    Client.isGM = true;
                int PM = Convert.ToInt32(DR["isPM"]);
                if (PM == 1)
                    Client.isPM = true;
                int FirstLog = Convert.ToInt32(DR["FirstLog"]);
                if (FirstLog == 0)
                    Client.First = true;
                else
                    Client.First = false;
                Client.Class = (Struct.ClassType)Convert.ToInt32(DR["Class"]);
            }
            DR.Close();
            Cmd.Dispose();
            if (Client.ID > 0)
                return Client;
            else
                return null;
        }
        public static void SaveCharacter(Character Client)
        {
            MySqlCommand Cmd = new MySqlCommand("UPDATE `characters` SET `Level` = " + Client.Level + ", `WHMoney` = " + Client.WHMoney + ", `PkPoints` = " + Client.PkPoints + ", `xCord` = " + Client.X + ", `yCord` = " + Client.Y + ", `Map` = " + (int)Client.Map + ", `HairStyle` = " + Client.Hair + ", `Class` = " + (int)Client.Class + ", `Exp` = " + Client.Exp + ", `Money` = " + Client.Money + ",`Vit` = " + Client.Vitality + ", `Dex` = " + Client.Dexterity + ", `FirstLog` = " + 1 + ", `HP` = " + Client.CurrentHP + ", `MP` = " + Client.CurrentMP + " WHERE `CharID` = " + Client.ID, DatabaseConnection.NewConnection());
            Cmd.ExecuteNonQuery();
            Cmd.Connection.Close();
            Cmd.Connection.Dispose();
            Cmd.Dispose();
        }
        public static void BanPlayer(string Charactername)
        {
            MySqlCommand Cmd = new MySqlCommand("SELECT * FROM `characters` WHERE `Name` = \"" + Charactername + "\"", DatabaseConnection.NewConnection());
            MySqlDataReader DR = Cmd.ExecuteReader(CommandBehavior.CloseConnection);
            while (DR.Read())
            {
                int ID = Convert.ToInt32(DR["CharID"]);
                string Account = Convert.ToString(DR["Account"]);
                if (ID > 0)
                {
                    MySqlCommand Cmd2 = new MySqlCommand("UPDATE `accounts` SET `Password` = \"" + "banned" + "\" WHERE `AccountID` = \"" + Account + "\"", DatabaseConnection.NewConnection());
                    Cmd2.ExecuteNonQuery();
                    Cmd2.Connection.Close();
                    Cmd2.Dispose();
                }
            }
            DR.Close();
            Cmd.Dispose();
        }
        public static void GetServerSkills()
        {
            MySqlCommand Cmd = new MySqlCommand("SELECT * FROM `serverskill`", DatabaseConnection.NewConnection());
            MySqlDataReader DR = Cmd.ExecuteReader(CommandBehavior.CloseConnection);
            while (DR.Read())
            {
                int ID = Convert.ToInt32(DR["type"]);
                if (!MainGS.ServerSkills.ContainsKey(ID))
                {
                    Struct.ServerSkill Skill = new Struct.ServerSkill();
                    Skill.ID = ID;
                    int Level = Convert.ToInt32(DR["level"]);
                    Skill.MaxLevel = Level;
                    int NeedExp = Convert.ToInt32(DR["need_exp"]);
                    int NeedLevel = Convert.ToInt32(DR["need_level"]);
                    int[] Needs = new int[2];
                    Needs[0] = NeedExp;
                    Needs[1] = NeedLevel;
                    Skill.RequiredExp.Add(Level, Needs);
                    MainGS.ServerSkills.Add(ID, Skill);
                }
                else
                {
                    Struct.ServerSkill Skill = MainGS.ServerSkills[ID];
                    int Level = Convert.ToInt32(DR["level"]);
                    if (Level > Skill.MaxLevel)
                        Skill.MaxLevel = Level;
                    int NeedExp = Convert.ToInt32(DR["need_exp"]);
                    int NeedLevel = Convert.ToInt32(DR["need_level"]);
                    int[] Needs = new int[2];
                    Needs[0] = NeedExp;
                    Needs[1] = NeedLevel;
                    Skill.RequiredExp.Add(Level, Needs);
                }
            }
            DR.Close();
            Cmd.Dispose();
        }
        public static void LoadTNpcs()
        {
            MySqlCommand Cmd = new MySqlCommand("SELECT * FROM `tnpcs`", DatabaseConnection.NewConnection());
            MySqlDataReader DR = Cmd.ExecuteReader(CommandBehavior.CloseConnection);
            while (DR.Read())
            {
                Struct.TerrainNPC Tnpc = new Struct.TerrainNPC();
                Tnpc.UID = Convert.ToInt32(DR["UID"]);
                if (Tnpc.UID == 6700)
                {
                    Tnpc.CurrentHP = 20000000;
                    Tnpc.MaximumHP = 20000000;
                }
                else if (Tnpc.UID == 6701 || Tnpc.UID == 6702)
                {
                    Tnpc.CurrentHP = 10000000;
                    Tnpc.MaximumHP = 10000000;
                }
                else
                {
                    Tnpc.CurrentHP = 20000000;
                    Tnpc.MaximumHP = 20000000;
                }
                Tnpc.Flag = Convert.ToInt32(DR["Flags"]);
                Tnpc.Type = Convert.ToInt32(DR["Type"]);
                Tnpc.X = Convert.ToInt32(DR["X"]);
                Tnpc.Y = Convert.ToInt32(DR["Y"]);
                Tnpc.Map = Convert.ToInt32(DR["Map"]);
                if (Tnpc.Type == 420 || Tnpc.Type == 430)
                    Tnpc.Level = 20;
                else if (Tnpc.Type == 450 || Tnpc.Type == 460)
                    Tnpc.Level = 25;
                else if (Tnpc.Type == 480 || Tnpc.Type == 490)
                    Tnpc.Level = 30;
                else if (Tnpc.Type == 510 || Tnpc.Type == 520)
                    Tnpc.Level = 35;
                else if (Tnpc.Type == 540 || Tnpc.Type == 550)
                    Tnpc.Level = 40;
                else if (Tnpc.Type == 570 || Tnpc.Type == 580)
                    Tnpc.Level = 45;
                else if (Tnpc.Type == 600 || Tnpc.Type == 610)
                    Tnpc.Level = 50;
                else if (Tnpc.Type == 630 || Tnpc.Type == 640)
                    Tnpc.Level = 55;
                else if (Tnpc.Type == 660 || Tnpc.Type == 670)
                    Tnpc.Level = 60;
                else if (Tnpc.Type == 690 || Tnpc.Type == 700)
                    Tnpc.Level = 65;
                else if (Tnpc.Type == 720 || Tnpc.Type == 730)
                    Tnpc.Level = 70;
                else if (Tnpc.Type == 750 || Tnpc.Type == 760)
                    Tnpc.Level = 75;
                else if (Tnpc.Type == 780 || Tnpc.Type == 790)
                    Tnpc.Level = 80;
                else if (Tnpc.Type == 810 || Tnpc.Type == 820)
                    Tnpc.Level = 85;
                else if (Tnpc.Type == 840 || Tnpc.Type == 850)
                    Tnpc.Level = 90;
                else if (Tnpc.Type == 870 || Tnpc.Type == 880)
                    Tnpc.Level = 95;
                else if (Tnpc.Type == 900 || Tnpc.Type == 910)
                    Tnpc.Level = 100;
                else if (Tnpc.Type == 930 || Tnpc.Type == 940)
                    Tnpc.Level = 105;
                else if (Tnpc.Type == 960 || Tnpc.Type == 970)
                    Tnpc.Level = 110;
                else if (Tnpc.Type == 990 || Tnpc.Type == 1000)
                    Tnpc.Level = 115;
                else if (Tnpc.Type == 1020 || Tnpc.Type == 1030)
                    Tnpc.Level = 120;
                MainGS.TerrainNpcs.Add(Tnpc.UID, Tnpc);
            }
            DR.Close();
            Cmd.Dispose();
        }
        public static void GetItems(ClientSocket CSocket)
        {
            MySqlCommand Cmd = new MySqlCommand("SELECT * FROM `items` WHERE `CharID` = \"" + CSocket.Client.ID + "\"", DatabaseConnection.NewConnection());
            MySqlDataReader DR = Cmd.ExecuteReader(CommandBehavior.CloseConnection);
            while (DR.Read())
            {
                Struct.ItemInfo Item = new Struct.ItemInfo();
                Item.Bless = Convert.ToInt32(DR["Minus"]);
                Item.Dura = Convert.ToInt32(DR["Dura"]);
                Item.ItemID = Convert.ToInt32(DR["ItemID"]);
                Item.MaxDura = Convert.ToInt32(DR["MaxDura"]);
                Item.Plus = Convert.ToInt32(DR["Plus"]);
                Item.Position = Convert.ToInt32(DR["Position"]);
                Item.Soc1 = Convert.ToInt32(DR["Soc1"]);
                Item.Soc2 = Convert.ToInt32(DR["Soc2"]);
                Item.UID = Convert.ToInt32(DR["ItemUID"]);
                Item.Enchant = Convert.ToInt32(DR["Enchant"]);
                Item.Color = Convert.ToInt32(DR["Color"]);
                if (Item.Position == 50)
                {
                    if (CSocket.Client.Inventory.Count < 40 && !CSocket.Client.Inventory.ContainsKey(Item.UID))
                    {
                        CSocket.Client.Inventory.Add(Item.UID, Item);
                    }
                }
                else if (Item.Position == 1002)
                {
                    if (CSocket.Client.TCWhs.Count < 20 && !CSocket.Client.TCWhs.ContainsKey(Item.UID))
                        CSocket.Client.TCWhs.Add(Item.UID, Item);
                }
                else
                {
                    if (!CSocket.Client.Equipment.ContainsKey(Item.Position))
                    {
                        CSocket.Client.Equipment.Add(Item.Position, Item);
                    }
                }
            }
            DR.Close();
            Cmd.Dispose();
        }
        public static void UpdateItem(Struct.ItemInfo Item)
        {
            MySqlCommand Cmd = new MySqlCommand("UPDATE `items` SET `Position` = " + Item.Position + ", `Minus` = " + Item.Bless + ", `Plus` = " + Item.Plus + ", `Enchant` = " + Item.Enchant + ", `Soc1` = " + Item.Soc1 + ", `Soc2` = " + Item.Soc2 + ", `Dura` = " + Item.Dura + ", `MaxDura` = " + Item.MaxDura + " WHERE `ItemUID` = \"" + Item.UID + "\"", DatabaseConnection.NewConnection());
            Cmd.ExecuteNonQuery();
            Cmd.Connection.Close();
            Cmd.Connection.Dispose();
            Cmd.Dispose();
        }
        public static void GetSkills(ClientSocket CSocket)
        {
            MySqlCommand Cmd = new MySqlCommand("SELECT * FROM `skills` WHERE `CharID` = " + CSocket.Client.ID, DatabaseConnection.NewConnection());
            MySqlDataReader DR = Cmd.ExecuteReader(CommandBehavior.CloseConnection);
            while (DR.Read())
            {
                Struct.CharSkill Skill = new Struct.CharSkill();
                Skill.Exp = Convert.ToUInt32(DR["SkillExp"]);
                Skill.ID = Convert.ToInt32(DR["SkillID"]);
                Skill.Level = Convert.ToInt32(DR["SkillLevel"]);
                if (!CSocket.Client.Skills.ContainsKey(Skill.ID))
                    CSocket.Client.Skills.Add(Skill.ID, Skill);
            }
            DR.Close();
            Cmd.Dispose();
        }
        public static void LoadMonsters()
        {
            MySqlCommand Cmd = new MySqlCommand("SELECT * FROM `monsters`", DatabaseConnection.NewConnection());
            MySqlDataReader DR = Cmd.ExecuteReader(CommandBehavior.CloseConnection);
            while (DR.Read())
            {
                MonsterInfo MI = new MonsterInfo();
                MI.AggroRange = Convert.ToInt32(DR["jrange"]);
                MI.AttackRange = Convert.ToInt32(DR["arange"]);
                MI.AttackSpeed = Convert.ToInt32(DR["attack_speed"]);
                MI.Defense = Convert.ToInt32(DR["pdef"]);
                MI.Dodge = Convert.ToInt32(DR["dodge"]);
                MI.ID = Convert.ToInt32(DR["id"]);
                MI.Level = Convert.ToInt32(DR["level"]);
                MI.MagicID = Convert.ToInt32(DR["atype"]);
                MI.MaxAttack = Convert.ToInt32(DR["atkmax"]);
                MI.MaxHP = Convert.ToInt32(DR["hp"]);
                MI.MDefense = Convert.ToInt32(DR["mdef"]);
                MI.Mesh = Convert.ToInt32(DR["mech"]);
                MI.MinAttack = Convert.ToInt32(DR["atkmin"]);
                MI.Name = Convert.ToString(DR["name"]);
                MI.Speed = Convert.ToInt32(DR["speed"]);
                if (!MainGS.BaseMonsters.ContainsKey(MI.ID))
                {
                    MainGS.BaseMonsters.Add(MI.ID, MI);
                }
            }
            Console.WriteLine("[GameServer] Loaded " + MainGS.BaseMonsters.Count + " base monster info from the DB.");
            DR.Close();
            Cmd.Dispose();
        }
        public static void LoadMonsterSpawns()
        {
            MySqlCommand Cmd = new MySqlCommand("SELECT * FROM `mobspawns`", DatabaseConnection.NewConnection());
            MySqlDataReader DR = Cmd.ExecuteReader(CommandBehavior.CloseConnection);
            int TotalSpawns = 0;
            while (DR.Read())
            {
                MonsterSpawn MS = new MonsterSpawn();
                MS.Map = Convert.ToInt32(DR["Map"]);
                MS.MobID = Convert.ToInt32(DR["ID"]);
                MS.SpawnID = Convert.ToInt32(DR["UniSpawnID"]);
                MS.SpawnNumber = Convert.ToInt32(DR["NumberToSpawnf"]);
                MS.MaxSpawnNumber = MS.SpawnNumber;
                TotalSpawns += MS.SpawnNumber;
                MS.X = Convert.ToInt32(DR["x-start"]);
                MS.XStop = Convert.ToInt32(DR["x-stop"]);
                MS.Y = Convert.ToInt32(DR["y-start"]);
                MS.YStop = Convert.ToInt32(DR["y-stop"]);
                if (!MainGS.MonsterSpawns.ContainsKey(MS.SpawnID))
                {
                    MainGS.MonsterSpawns.Add(MS.SpawnID, MS);
                }
            }
            Console.WriteLine("[GameServer] Loaded " + MainGS.MonsterSpawns.Count + " unique monster spawns.");
            Console.WriteLine("[GameServer] Will theoretically spawn " + TotalSpawns + " monsters into the world.");
            DR.Close();
            Cmd.Dispose();
        }
        public static void LoadNpcs()
        {
            MySqlCommand Cmd = new MySqlCommand("SELECT * FROM `npcs`", DatabaseConnection.NewConnection());
            MySqlDataReader DR = Cmd.ExecuteReader(CommandBehavior.CloseConnection);
            while (DR.Read())
            {
                Struct.NPC NPC = new Struct.NPC();
                NPC.Direction = Convert.ToInt32(DR["Direction"]);
                NPC.Flag = Convert.ToInt32(DR["Flag"]);
                NPC.ID = Convert.ToInt32(DR["NpcID"]);
                NPC.Map = Convert.ToInt32(DR["MapID"]);
                NPC.SubType = Convert.ToInt32(DR["SubType"]);
                NPC.Type = Convert.ToInt32(DR["NpcType"]);
                NPC.X = Convert.ToInt32(DR["Xcord"]);
                NPC.Y = Convert.ToInt32(DR["Ycord"]);
                if (!MainGS.Npcs.ContainsKey(NPC.ID))
                    MainGS.Npcs.Add(NPC.ID, NPC);
            }
            Console.WriteLine("[GameServer] Loaded " + MainGS.Npcs.Count + " npcs into the world.");
            DR.Close();
            Cmd.Dispose();
        }
        public static void LoadItemPluses()
        {
            MySqlCommand CMD = new MySqlCommand("SELECT * FROM `itemplus`", DatabaseConnection.NewConnection());
            MySqlDataReader DR = CMD.ExecuteReader(CommandBehavior.CloseConnection);

            while (DR.Read())
            {
                Struct.ItemPlus NewItem = new Struct.ItemPlus();
                if (DR["typeid"] != "0")
                {
                    NewItem.ID = Convert.ToInt32(DR["typeid"]);
                    NewItem.Plus = Convert.ToInt32(DR["plus"]);
                    NewItem.HPAdd = Convert.ToInt32(DR["life"]);
                    NewItem.MinDmg = Convert.ToInt32(DR["attack_min"]);
                    NewItem.MaxDmg = Convert.ToInt32(DR["attack_max"]);
                    NewItem.DefenseAdd = Convert.ToInt32(DR["defense"]);
                    NewItem.MinMDamageAdd = Convert.ToInt32(DR["mgcatk_min"]);
                    NewItem.MaxMDamageAdd = Convert.ToInt32(DR["mgcatk_max"]);
                    NewItem.MDefAdd = Convert.ToInt32(DR["magic_def"]);
                    NewItem.DexAdd = Convert.ToInt32(DR["dexterity"]);
                    NewItem.DodgeAdd = Convert.ToInt32(DR["dodge"]);
                }
                if (!MainGS.ItemPluses.ContainsKey(NewItem.ID))
                {
                    Struct.ItemPlusDB DB = new Struct.ItemPlusDB();
                    DB.DB.Add(NewItem.Plus, NewItem);
                    MainGS.ItemPluses.Add(NewItem.ID, DB);
                }
                else
                {
                    Struct.ItemPlusDB DB = MainGS.ItemPluses[NewItem.ID];
                    if (!DB.DB.ContainsKey(NewItem.Plus))
                        DB.DB.Add(NewItem.Plus, NewItem);
                }
            }
        }
        public static void LoadPortals()
        {
            MySqlCommand Cmd = new MySqlCommand("SELECT * FROM `portals`", DatabaseConnection.NewConnection());
            MySqlDataReader DR = Cmd.ExecuteReader(CommandBehavior.CloseConnection);
            while (DR.Read())
            {
                Struct.Portal Port = new Struct.Portal();
                Port.ID = Convert.ToInt32(DR["PortalID"]);
                Port.StartInstance = 0;
                Port.StartMap = Convert.ToInt32(DR["StartMap"]);
                Port.StartX = Convert.ToInt32(DR["StartX"]);
                Port.StartY = Convert.ToInt32(DR["StartY"]);
                Port.EndInstance = 0;
                Port.EndMap = Convert.ToInt32(DR["EndMap"]);
                Port.EndX = Convert.ToInt32(DR["EndX"]);
                Port.EndY = Convert.ToInt32(DR["EndY"]);
                string PID = Port.StartX + "," + Port.StartY + "," + Port.StartMap + "," + Port.StartInstance;
                MainGS.Portals.Add(PID, Port);
            }
            DR.Close();
            Cmd.Dispose();
            Console.WriteLine("[GameServer] Loaded " + MainGS.Portals.Count + " portals into the world.");
        }
        public static void PurgeGuilds()
        {
            MySqlCommand Cmd = new MySqlCommand("SELECT * FROM `guilds`", DatabaseConnection.NewConnection());
            MySqlCommand Cmd2 = new MySqlCommand("SELECT * FROM `characters`", DatabaseConnection.NewConnection());
            MySqlDataReader DR = Cmd.ExecuteReader(CommandBehavior.CloseConnection);
            MySqlDataReader DR2 = Cmd2.ExecuteReader(CommandBehavior.CloseConnection);
            ArrayList Guilds = new ArrayList();
            while (DR2.Read())
            {
                int GID = Convert.ToInt32(DR2["Guild"]);
                if (!Guilds.Contains(GID))
                    Guilds.Add(GID);
            }
            Console.WriteLine(Guilds.Count + " unique guilds");
            int delete = 0;
            while (DR.Read())
            {
                int ID = Convert.ToInt32(DR["GuildID"]);
                if (!Guilds.Contains(ID))
                {
                    delete++;
                    DeleteGuild(ID);
                }

            }
            Console.WriteLine("purged " + delete + " guilds.");
            DR.Close();
            DR2.Close();
            Cmd.Dispose();
            Cmd2.Dispose();
        }
        public static void NewProf(int ID, int Level, uint Exp, int Client)
        {
            MySqlCommand Cmd = new MySqlCommand("insert into `prof`(`CharID`,`ProfID`,`ProfLvl`, `ProfExp`) values (" + Client + "," + ID + "," + Level + "," + Exp + ")", DatabaseConnection.NewConnection());
            Cmd.ExecuteNonQuery();
            Cmd.Connection.Close();
            Cmd.Connection.Dispose();
            Cmd.Dispose();
        }
        public static void UpdateProf(int ID, int Level, uint Exp, int Client)
        {
            MySqlCommand Cmd = new MySqlCommand("UPDATE `prof` SET `ProfLvl` = " + Level + ", `ProfExp` = " + Exp + " WHERE `CharID` = " + Client + " AND `ProfID` = " + ID + "", DatabaseConnection.NewConnection());
            Cmd.ExecuteNonQuery();
            Cmd.Connection.Close();
            Cmd.Connection.Dispose();
            Cmd.Dispose();
        }
        public static void SetSkill(int ID, int Level, uint Exp, int Client, bool has)
        {
            if (!has)
            {
                MySqlCommand Cmd = new MySqlCommand("insert into `skills`(`CharID`,`SkillID`,`SkillLevel`) values (" + Client + "," + ID + "," + Level + ")", DatabaseConnection.NewConnection());
                Cmd.ExecuteNonQuery();
                Cmd.Connection.Close();
                Cmd.Connection.Dispose();
                Cmd.Dispose();
            }
            else
            {
                MySqlCommand Cmd = new MySqlCommand("UPDATE `skills` SET `SkillExp` = " + Exp + ", `SkillLevel` = " + Level + " WHERE `CharID` = " + Client + " AND `SkillID` = " + ID + "", DatabaseConnection.NewConnection());
                Cmd.ExecuteNonQuery();
                Cmd.Connection.Close();
                Cmd.Connection.Dispose();
                Cmd.Dispose();
            }
        }
        public static void DeleteGuild(int ID)
        {
            MySqlCommand Cmd = new MySqlCommand("DELETE FROM `guilds` Where `GuildID` = " + ID + "", DatabaseConnection.NewConnection());
            Cmd.ExecuteNonQuery();
            Cmd.Connection.Close();
            Cmd.Connection.Dispose();
            Cmd.Dispose();
        }
        public static void DeleteItem(int UID)
        {
            MySqlCommand Cmd = new MySqlCommand("DELETE FROM `items` WHERE `ItemUID` = " + UID, DatabaseConnection.NewConnection());
            Cmd.ExecuteNonQuery();
            Cmd.Connection.Close();
            Cmd.Connection.Dispose();
            Cmd.Dispose();
        }
        public static bool NewItem(Struct.ItemInfo Item, ClientSocket CSocket)
        {
            try
            {
                MySqlCommand Cmd = new MySqlCommand("INSERT INTO items(ItemUID, CharID, Position, ItemID, Plus, Minus, Enchant, Soc1, Soc2, Dura, MaxDura, Color) VALUES(" + Item.UID + "," + CSocket.Client.ID + "," + Item.Position + "," + Item.ItemID + "," + Item.Plus + "," + Item.Bless + "," + Item.Enchant + "," + Item.Soc1 + "," + Item.Soc2 + "," + Item.Dura + "," + Item.MaxDura + "," + Item.Color + ")", DatabaseConnection.NewConnection());
                Cmd.ExecuteNonQuery();
                Cmd.Connection.Close();
                Cmd.Connection.Dispose();
                Cmd.Dispose();
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
