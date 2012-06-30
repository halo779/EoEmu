using System;
using System.Timers;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using GameServer.Entities;
using GameServer.Packets;
using GameServer.Connections;
using GameServer.Structs;
using GameServer.Database;
using GameServer.Handlers;
using GameServer.Calculations;
using System.Globalization;

namespace GameServer
{
    /// <summary>
    /// Static class to handle all client packets.
    /// </summary>
    public class PacketProcessor
    {
        public static byte[] String_To_Bytes(string strInput)
        {
            // i variable used to hold position in string  
            int i = 0;
            // x variable used to hold byte array element position  
            int x = 0;
            // allocate byte array based on half of string length  
            byte[] bytes = new byte[(strInput.Length) / 2];
            // loop through the string - 2 bytes at a time converting  
            //  it to decimal equivalent and store in byte array  
            while (strInput.Length > i + 1)
            {
                long lngDecimal = Convert.ToInt32(strInput.Substring(i, 2), 16);
                bytes[x] = Convert.ToByte(lngDecimal);
                i = i + 2;
                ++x;
            }
            // return the finished byte array of decimal values  
            return bytes;
        }

        public static byte[] TQ_SERVER = Encoding.ASCII.GetBytes("TQServer");
        public static byte[] ReturnFinal(byte[] Data)
        {
            //Replaces "TQClient" with "TQServer" on the end of the packet so it may be looped back to the client.
            Array.Copy(TQ_SERVER, 0, Data, Data.Length - TQ_SERVER.Length, TQ_SERVER.Length);
            return Data;
        }
        public static int ReadLong(byte[] Data, int StartLocation)
        {
            if (Data.Length < StartLocation)
                return 0;
            return ((Data[StartLocation + 3] << 24) + (Data[StartLocation + 2] << 16) + (Data[StartLocation + 1] << 8) + (Data[StartLocation]));
        }
        public static int ReadShort(byte[] Data, int StartLocation)
        {
            if (Data.Length < StartLocation)
                return 0;
            return ((Data[StartLocation + 1] << 8) + Data[StartLocation]);
        }

        private static string StrHexToAnsi(string StrHex)
        {
            string[] Data = StrHex.Split(new char[] { ' ' });
            string Ansi = "";
            foreach (string tmpHex in Data)
            {
                if (tmpHex != "")
                {
                    byte ByteData = byte.Parse(tmpHex, NumberStyles.HexNumber);
                    if ((ByteData >= 32) & (ByteData <= 126))
                    {
                        Ansi = Ansi + ((char)(ByteData)).ToString();
                    }
                    else
                    {
                        Ansi = Ansi + ".";
                    }
                }
            }
            return Ansi;
        }

        public static object Dump(byte[] Bytes)
        {
            string Hex = "";
            foreach (byte b in Bytes)
            {
                Hex = Hex + b.ToString("X2") + " ";
            }
            string Out = "";
            while (Hex.Length != 0)
            {
                int SubLength = 0;
                if (Hex.Length >= 48)
                {
                    SubLength = 48;
                }
                else
                {
                    SubLength = Hex.Length;
                }
                string SubString = Hex.Substring(0, SubLength);
                int Remove = SubString.Length;
                SubString = SubString.PadRight(60, ' ') + StrHexToAnsi(SubString);
                Hex = Hex.Remove(0, Remove);
                Out = Out + SubString + "\r\n";
            }
            return Out;
        }
        public static void ProcessPacket(byte[] data, ClientSocket CSocket)
        {
            try
            {
                byte[] Split1 = null;
                byte[] Split2 = null;
                byte[] Split3 = null;
                //byte[] Split4;
                //byte[] Split5;
                int Type = (BitConverter.ToInt16(data, 2));
                int Length = (data[1] << 8) + data[0]; Length += 8;
                //if (data.Length > Length)
                //{
                //    int Len2 = (data[Length + 1] << 8) + data[Length]; Len2 += 8;
                //    Split1 = new byte[Len2];
                //    Array.Copy(data, Length, Split1, 0, Len2);
                //    if (Len2 + Length < data.Length)
                //    {
                //        int Len3 = (data[Length + Len2 + 1] << 8) + data[Length + Len2]; Len3 += 8;
                //        Split2 = new byte[Len3];
                //        Array.Copy(data, Length + Len2, Split2, 0, Len3);
                //        if (Len2 + Len3 + Length < data.Length)
                //        {
                //            int Len4 = (data[Length + Len2 + Len3 + 1] << 8) + data[Length + Len2 + Len3]; Len4 += 8;
                //            Split3 = new byte[Len4];
                //            Array.Copy(data, Length + Len2 + Len3, Split3, 0, Len4);
                //        }
                //    }
                //}
                //byte[] data = new byte[Length];
                //Array.Copy(data, data, Length);
                Console.WriteLine("[PacketLog] New Packet Recived, Type: " + Type);
                Console.WriteLine(Dump(data));
                switch (Type)
                {
                    case 1052: //End Authorization Process.
                        {
                            ulong Keys;
                            Keys = data[11];
                            Keys = (Keys << 8) | data[10];
                            Keys = (Keys << 8) | data[9];
                            Keys = (Keys << 8) | data[8];
                            Keys = (Keys << 8) | data[7];
                            Keys = (Keys << 8) | data[6];
                            Keys = (Keys << 8) | data[5];
                            Keys = (Keys << 8) | data[4];
                            Console.WriteLine("[GameServer] Confirming login with LoginServer");
                            if (Nano.AuthenticatedLogins.ContainsKey(Keys))
                            {
                                CSocket.AccountName = Nano.AuthenticatedLogins[Keys].Account;
                                Console.WriteLine("[GameServer] Authenticated Login.");
                                ConnectionRequest User = Nano.AuthenticatedLogins[Keys];
                                User.Expire(false);
                                CSocket.Client = Database.Database.GetCharacter(User.Account);
                                if (CSocket.Client == null)
                                {
                                    if (Database.Database.LoadNovaCharacter(User.Account))
                                    {
                                        Console.WriteLine("user has account");
                                        CSocket.Client = Database.Database.GetCharacter(User.Account);
                                    }
                                    else
                                    {
                                        Console.WriteLine("user making account");
                                        
                                        //CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", "ALLUSERS", "NEW_ROLE", Struct.ChatType.LoginInformation));
                                        CSocket.Send(ConquerPacket.Chat(5, "SYSTEM", "ALLUSERS", "NEW_ROLE", Struct.ChatType.LoginInformation));
                                        return;
                                    }
                                }
                                if (CSocket.Client == null)
                                {
                                   
                                    //CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", "ALLUSERS", "NEW_ROLE", Struct.ChatType.LoginInformation));
                                    CSocket.Send(ConquerPacket.Chat(5, "SYSTEM", "ALLUSERS", "NEW_ROLE", Struct.ChatType.LoginInformation));
                                    return;
                                }
                                Calculation.Vitals(CSocket, true);
                                if (CSocket.Client.First)
                                {
                                    CSocket.Client.CurrentMP = CSocket.Client.MaxMP;
                                    CSocket.Client.CurrentHP = CSocket.Client.MaxHP;
                                }
                                Database.Database.GetItems(CSocket);
                                if (Nano.ClientPool.ContainsKey(CSocket.Client.ID))
                                {
                                    ClientSocket C = Nano.ClientPool[CSocket.Client.ID];
                                    C.Send(ConquerPacket.Chat(0, "SYSTEM", C.Client.Name, "[ERROR] Your character has logged in from another location, you're being booted.", Struct.ChatType.Talk));
                                    C.Disconnect();
                                }
                                //lock(Nano.ClientPool)
                                //{
                                try
                                {
                                    Monitor.Enter(Nano.ClientPool);
                                    Nano.ClientPool.Add(CSocket.Client.ID, CSocket);
                                    //}
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.ToString());
                                }
                                finally
                                {
                                    Monitor.Exit(Nano.ClientPool);
                                }
                                //}
                                CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", "ALLUSERS", "ANSWER_OK", Struct.ChatType.LoginInformation));
                                CSocket.Send(ConquerPacket.CharacterInfo(CSocket));
                                //PLEASE DO NOT REMOVE THIS CODE!
                                CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "This server is a branch of CoEmu code. Portions are expressly owned by the CoEmu Foundation.", Struct.ChatType.Talk));
                                //
                                CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "Welcome to CoEmu v2: Nano, " + CSocket.Client.Name, Struct.ChatType.Talk));
                                CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "There are currently " + Nano.ClientPool.Count + " players online.", Struct.ChatType.Talk));
                                CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "Use of this server binds you to the Terms of Service (ToS) located on http://www.coemu.org", Struct.ChatType.Talk));
                                CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "Current server rates: Prof: " + Nano.PROF_MULTIPLER + "x, Skill: " + Nano.SKILL_MULTIPLER + "x, Exp: " + Nano.EXP_MULTIPLER + "x.", Struct.ChatType.Talk));
                                CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "Be kind to your fellow player and have a good time.", Struct.ChatType.Talk));
                                /*Handler.Text("Welcome to CoEmu.Nano. As you will undoubtedly notice, you can't do much right now.\n", CSocket);
                                Handler.Text("If you are connected and see this message, you are helping to test Nano out. Thank you.\n", CSocket);
                                Handler.Text("Please do not whisper or bug PMs/GMs, we know that you can't do most of the things you should be able to.\n", CSocket);
                                Handler.Text("As you can obviously see, you are naked. Go hunt!\n", CSocket);
                                Handler.End(CSocket);*/
                                if (CSocket.Client.First)
                                {
                                    Database.Database.SaveCharacter(CSocket.Client);
                                    //Handler.Text("Welcome to the WONDERFUL world of CoEmu.Nano,\n CoEmu v2's leveling server!", CSocket);
                                    //Handler.Text("\n On behalf of every GM, PM, and supporter of CoEmu v2,", CSocket);
                                    //Handler.Text("\n I would like to thank you all for the\n wonder that Nova was.", CSocket);
                                    //Handler.Text("\n This server is dedicated to not only\n Rimik from Pyramid", CSocket);
                                    //Handler.Text("\n But to every CoEmu v2 supporter that has\n ever been a part of our family.", CSocket);
                                    //Handler.Text("\n Good luck in this new world, conquerer.", CSocket);
                                    //Handler.Text("\n You've been started off with some basic items to help your journey.", CSocket);
                                    //Handler.Text("\n Use of this server BINDS YOU to the CoEmu v2 terms of service.", CSocket);
                                    //Handler.End(CSocket);
                                }
                                ConquerPacket.ToServer(ConquerPacket.Chat(0, "SYSTEM", "ALLUSERS", CSocket.Client.Name + " has come online.", Struct.ChatType.Top), 0);
                            }
                            else
                            {
                                Console.WriteLine("[GameServer] Unauthenticated Login.");
                                CSocket.Disconnect();
                            }
                            break;
                        }
                    #region CreateCharacter
                    case 1001://Create Character
                        {
                            Handler.NewCharacter(data, CSocket);
                            break;
                        }
                    #endregion
                    #region 0x3f2(1010) Multi-Function Packet
                    case 1010: // 0x3f2, Multi-Function Packet
                        {
                            if (data.Length < 0x16)
                                break;
                            int SubType = data[0x16];
                            switch (SubType)
                            {
                                case 74: //Start login sequence.
                                    {
                                        Console.WriteLine("[GameServer] Login Sequence started for " + CSocket.Client.Name);
                                        //CSocket.Send(ConquerPacket.General(CSocket.Client.ID, (int)CSocket.Client.Map, CSocket.Client.X, CSocket.Client.Y, 0, Struct.DataType.MapShow));
                                        CSocket.Send(ConquerPacket.General(CSocket.Client.ID, (int)CSocket.Client.Map, 0, CSocket.Client.X, CSocket.Client.Y, 0, Struct.DataType.MapShow));
                                        ConquerPacket.ToLocal(ConquerPacket.SpawnCharacter(CSocket), CSocket.Client.X, CSocket.Client.Y, (int)CSocket.Client.Map, 0, CSocket.Client.ID);
                                        Spawn.All(CSocket);
                                        /*CSocket.Client.Save = new Timer();
                                        CSocket.Client.Save.Elapsed += delegate {
                                            Database.Database.SaveCharacter(CSocket.Client);
                                            CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "Saved " + CSocket.Client.Name, Struct.ChatType.Top));
                                        };
                                        CSocket.Client.Save.Interval = 20000;
                                        CSocket.Client.Save.Start();*/
                                        CSocket.Client.CurrentStam = 100;
                                        CSocket.Client.UpStam = new System.Timers.Timer();
                                        CSocket.Client.UpStam.Interval = 100;
                                        CSocket.Client.UpStam.Elapsed += delegate { CSocket.AddStam(); };
                                        CSocket.Send(ConquerPacket.Status(CSocket, 2, CSocket.Client.CurrentStam, Struct.StatusTypes.Stamina));
                                        CSocket.Client.LastAttack = System.Environment.TickCount;
                                        break;
                                    }
                                case 114: //Request to show minimap
                                    {
                                        CSocket.Send(ConquerPacket.MiniMap(true));
                                        break;
                                    }
                                case 75: //Request Hotkeys
                                    {
                                        CSocket.Send(ConquerPacket.General(CSocket.Client.ID, 0, 0, 0, 0, 0, Struct.DataType.HotKeys));
                                        break;
                                    }
                                case 76: //Request Friends
                                    {
                                        //TODO: Friends lists
                                        CSocket.Send(ConquerPacket.General(CSocket.Client.ID, 0, 0, 0, 0, 0, Struct.DataType.ConfirmFriends));
                                        break;
                                    }
                                case 77://Weapon Prof.
                                    {
                                        Database.Database.GetProfs(CSocket);
                                        foreach (KeyValuePair<int, Struct.CharProf> Prof in CSocket.Client.Profs)
                                        {
                                            CSocket.Send(ConquerPacket.Prof(Prof.Value.ID, Prof.Value.Level, Prof.Value.Exp));
                                        }
                                        CSocket.Send(ConquerPacket.General(CSocket.Client.ID, 0, 0, 0, 0, 0, Struct.DataType.ConfirmProf));
                                        break;
                                    }
                                case 78://Confirm Skills
                                    {
                                        Database.Database.GetSkills(CSocket);
                                        foreach (KeyValuePair<int, Struct.CharSkill> Skill in CSocket.Client.Skills)
                                        {
                                            CSocket.Send(ConquerPacket.Skill(Skill.Value.ID, Skill.Value.Level, Skill.Value.Exp));
                                        }
                                        CSocket.Send(ConquerPacket.General(CSocket.Client.ID, 0, 0, 0, 0, 0, Struct.DataType.ConfirmSkills));
                                        break;
                                    }
                                case 85: //Portal
                                    {
                                        int X = ReadShort(data, 12);
                                        int Y = ReadShort(data, 14);
                                        Handler.Portal(X, Y, CSocket);
                                        break;
                                    }
                                case 97: //Confirm Guild
                                    {
                                        //TODO: Guild
                                        CSocket.Send(ConquerPacket.General(CSocket.Client.ID, 0, 0, 0, 0, 0, Struct.DataType.ConfirmGuild));
                                        break;
                                    }
                                case 130: //Confirm Login Complete
                                    {
                                        CSocket.Send(ConquerPacket.General(CSocket.Client.ID, 0, 0, 0, 0, 0, Struct.DataType.ConfirmLoginComplete));
                                        foreach (KeyValuePair<int, Struct.ItemInfo> Item in CSocket.Client.Inventory)
                                        {
                                            CSocket.Send(ConquerPacket.ItemInfo(Item.Value.UID, Item.Value.ItemID, Item.Value.Plus, Item.Value.Bless, Item.Value.Enchant, Item.Value.Soc1, Item.Value.Soc2, Item.Value.Dura, Item.Value.MaxDura, Item.Value.Position, Item.Value.Color));
                                        }
                                        #region ItemCalculations
                                        foreach (KeyValuePair<int, Struct.ItemInfo> Item in CSocket.Client.Equipment)
                                        {
                                            switch (Item.Value.Soc1)
                                            {
                                                case 1:
                                                    {
                                                        CSocket.Client.NPG++;
                                                        break;
                                                    }
                                                case 2:
                                                    {
                                                        CSocket.Client.RPG++;
                                                        break;
                                                    }
                                                case 3:
                                                    {
                                                        CSocket.Client.SPG++;
                                                        break;
                                                    }
                                                case 11:
                                                    {
                                                        CSocket.Client.NDG++;
                                                        break;
                                                    }
                                                case 12:
                                                    {
                                                        CSocket.Client.RDG++;
                                                        break;
                                                    }
                                                case 13:
                                                    {
                                                        CSocket.Client.SDG++;
                                                        break;
                                                    }
                                                case 71:
                                                    {
                                                        CSocket.Client.NTG++;
                                                        break;
                                                    }
                                                case 72:
                                                    {
                                                        CSocket.Client.RTG++;
                                                        break;
                                                    }
                                                case 73:
                                                    {
                                                        CSocket.Client.STG++;
                                                        break;
                                                    }
                                                default:
                                                    {
                                                        CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Please report: Unknown GemID: " + Item.Value.Soc1, Struct.ChatType.Talk));
                                                        break;
                                                    }

                                            }
                                            switch (Item.Value.Soc2)
                                            {
                                                case 1:
                                                    {
                                                        CSocket.Client.NPG++;
                                                        break;
                                                    }
                                                case 2:
                                                    {
                                                        CSocket.Client.RPG++;
                                                        break;
                                                    }
                                                case 3:
                                                    {
                                                        CSocket.Client.SPG++;
                                                        break;
                                                    }
                                                case 11:
                                                    {
                                                        CSocket.Client.NDG++;
                                                        break;
                                                    }
                                                case 12:
                                                    {
                                                        CSocket.Client.RDG++;
                                                        break;
                                                    }
                                                case 13:
                                                    {
                                                        CSocket.Client.SDG++;
                                                        break;
                                                    }
                                                case 71:
                                                    {
                                                        CSocket.Client.NTG++;
                                                        break;
                                                    }
                                                case 72:
                                                    {
                                                        CSocket.Client.RTG++;
                                                        break;
                                                    }
                                                case 73:
                                                    {
                                                        CSocket.Client.STG++;
                                                        break;
                                                    }
                                                default:
                                                    {
                                                        CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Please report: Unknown GemID: " + Item.Value.Soc2, Struct.ChatType.Talk));
                                                        break;
                                                    }

                                            }
                                            switch (Item.Value.Bless)
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
                                                        CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Invalid bless: " + Item.Value.Bless, Struct.ChatType.Top));
                                                        break;
                                                    }
                                            }
                                            if (Nano.Items.ContainsKey(Item.Value.ItemID))
                                            {
                                                Struct.ItemData ItemD = Nano.Items[Item.Value.ItemID];
                                                CSocket.Client.BaseMagicAttack += ItemD.MagicAttack;
                                                if (Item.Value.Position == 5)
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
                                                CSocket.Client.MaxHP += Item.Value.Enchant;
                                                CSocket.Client.MagicDefense += ItemD.MDefenseAdd;
                                                CSocket.Client.MaxMP += ItemD.MPAdd;
                                                CSocket.Client.Dodge += ItemD.DodgeAdd;
                                            }
                                            if (Item.Value.Plus > 0)
                                            {
                                                string s_ItemID = Convert.ToString(Item.Value.ItemID);
                                                int itemidsimple = 0;
                                                if ((Item.Value.ItemID >= 900000 && Item.Value.ItemID <= 900999) || (Item.Value.ItemID >= 111303 && Item.Value.ItemID <= 118999) || (Item.Value.ItemID >= 130003 && Item.Value.ItemID <= 139999))//Shields, Helms, Armors
                                                {
                                                    /*s_ItemID = s_ItemID.Remove((s_ItemID.Length - 3), 1);
                                                    s_ItemID = s_ItemID.Insert((s_ItemID.Length - 2), "0");
                                                    s_ItemID = s_ItemID.Remove((s_ItemID.Length - 1), 1);
                                                    s_ItemID = s_ItemID.Insert((s_ItemID.Length), "0");*/
                                                    s_ItemID = s_ItemID.Remove((s_ItemID.Length - 1), 1);
                                                    s_ItemID = s_ItemID.Insert(s_ItemID.Length, "0");
                                                    itemidsimple = Convert.ToInt32(s_ItemID);
                                                }
                                                else if ((Item.Value.ItemID >= 150000 && Item.Value.ItemID <= 160250) || (Item.Value.ItemID >= 500000 && Item.Value.ItemID <= 500400) || (Item.Value.ItemID >= 120003 && Item.Value.ItemID <= 121249) || (Item.Value.ItemID >= 421003 && Item.Value.ItemID <= 421339))//BS's, Bows, Necky/Bags
                                                {
                                                    s_ItemID = s_ItemID.Remove((s_ItemID.Length - 1), 1);
                                                    s_ItemID = s_ItemID.Insert((s_ItemID.Length), "0");
                                                    itemidsimple = Convert.ToInt32(s_ItemID);
                                                }
                                                else if (Item.Value.ItemID >= 510000 && Item.Value.ItemID <= 580400)//2 Hander
                                                {
                                                    s_ItemID = s_ItemID.Remove(0, 3);
                                                    s_ItemID = s_ItemID.Insert(0, "555");
                                                    s_ItemID = s_ItemID.Remove((s_ItemID.Length - 1), 1);
                                                    s_ItemID = s_ItemID.Insert((s_ItemID.Length), "0");
                                                    itemidsimple = Convert.ToInt32(s_ItemID);
                                                }
                                                else if (Item.Value.ItemID >= 410000 && Item.Value.ItemID <= 490400 && itemidsimple == 0)//1 Handers
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
                                                    if (IPlus.DB.ContainsKey(Item.Value.Plus))
                                                    {
                                                        Struct.ItemPlus iPlus = IPlus.DB[Item.Value.Plus];
                                                        CSocket.Client.BaseMaxAttack += iPlus.MaxDmg;
                                                        CSocket.Client.BaseMinAttack += iPlus.MinDmg;
                                                        CSocket.Client.Defense += iPlus.DefenseAdd;
                                                        CSocket.Client.BaseMagicAttack += iPlus.MDamageAdd;
                                                        CSocket.Client.BonusMagicAttack += iPlus.MDamageAdd;
                                                        CSocket.Client.BonusMagicDefense += iPlus.MDefAdd;
                                                        CSocket.Client.MaxHP += iPlus.HPAdd;
                                                        CSocket.Client.Dodge += iPlus.DodgeAdd;
                                                        //TODO: HP, etc
                                                    }
                                                }
                                            }
                                        #endregion
                                            CSocket.Send(ConquerPacket.ItemInfo(Item.Value.UID, Item.Value.ItemID, Item.Value.Plus, Item.Value.Bless, Item.Value.Enchant, Item.Value.Soc1, Item.Value.Soc2, Item.Value.Dura, Item.Value.MaxDura, Item.Value.Position, Item.Value.Color));
                                        }
                                        Calculation.Attack(CSocket);
                                        CSocket.Send(ConquerPacket.Status(CSocket, 2, 0, Struct.StatusTypes.StatusEffect));
                                        break;
                                    }
                                case 79: //Direction
                                    {
                                        int Direction = ReadShort(data, 20);
                                        if (Direction >= 0 && Direction <= 7)
                                            CSocket.Client.Direction = Direction;
                                        ConquerPacket.ToLocal(ConquerPacket.General(CSocket.Client.ID, 0, 0, CSocket.Client.X, CSocket.Client.Y, CSocket.Client.Direction, Struct.DataType.Direction), CSocket.Client.X, CSocket.Client.Y, (int)CSocket.Client.Map, 0, CSocket.Client.ID);
                                        break;
                                    }
                                case 102:
                                    {
                                        int Target = ReadLong(data, 12);
                                        if (Nano.ClientPool.ContainsKey(Target))
                                            CSocket.Send(ConquerPacket.SpawnCharacter(Nano.ClientPool[Target]));
                                        else if (Nano.Monsters.ContainsKey(Target))
                                        {
                                            Monster Mob = Nano.Monsters[Target];
                                            CSocket.Send(ConquerPacket.SpawnMonster(Mob.UID, Mob.Info.Mesh, Mob.X, Mob.Y, Mob.Info.Name, Mob.CurrentHP, Mob.Level, Mob.Direction));
                                        }
                                        break;
                                    }
                                case 81: //Action
                                    {
                                        int Action = ReadLong(data, 12);
                                        if (Enum.IsDefined(typeof(Struct.ActionType), (Struct.ActionType)Action))
                                        {
                                            CSocket.Client.Action = Action;
                                            ConquerPacket.ToLocal(ConquerPacket.General(CSocket.Client.ID, CSocket.Client.Action, 0, CSocket.Client.X, CSocket.Client.Y, CSocket.Client.Direction, Struct.DataType.Action), CSocket.Client.X, CSocket.Client.Y, (int)CSocket.Client.Map, 0, CSocket.Client.ID);
                                            if (Action == 250 && !CSocket.Client.UpStam.Enabled)
                                            {
                                                CSocket.Client.UpStam.Start();
                                            }
                                            else if (Action != 250 && CSocket.Client.UpStam.Enabled)
                                            {
                                                CSocket.Client.UpStam.Stop();
                                            }
                                        }
                                        else
                                        {
                                            CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Please report bug: Unknown action type: " + Action, Struct.ChatType.Talk));
                                        }
                                        break;
                                    }
                                case 96: //PK Mode
                                    {
                                        int PkType = data[12];
                                        if (PkType > 4)
                                            break;
                                        CSocket.Client.PKMode = (Struct.PkType)PkType;
                                        CSocket.Send(ConquerPacket.General(CSocket.Client.ID, PkType, 0, 0, 0, 0, Struct.DataType.PkMode));
                                        break;
                                    }
                                case 94://Revive
                                    {
                                        //TODO: Revive points
                                        if (CSocket.Client.Dead)
                                        {
                                            CSocket.Client.CurrentHP = CSocket.Client.MaxHP;
                                            CSocket.Client.Dead = false;
                                            CSocket.Send(ConquerPacket.Status(CSocket, 2, 0, Struct.StatusTypes.StatusEffect));
                                            CSocket.Send(ConquerPacket.Status(CSocket, 2, CSocket.Client.Model, Struct.StatusTypes.Model));
                                            CSocket.Send(ConquerPacket.Status(CSocket, 2, CSocket.Client.CurrentHP, Struct.StatusTypes.Hp));
                                            ConquerPacket.ToLocal(ConquerPacket.General(CSocket.Client.ID, CSocket.Client.X, CSocket.Client.Y, 0, 0, 0, Struct.DataType.EntityRemove), CSocket.Client.X, CSocket.Client.Y, (int)CSocket.Client.Map, 0, CSocket.Client.ID);
                                            ConquerPacket.ToLocal(ConquerPacket.SpawnCharacter(CSocket), CSocket.Client.X, CSocket.Client.Y, (int)CSocket.Client.Map, 0, 0);
                                            ConquerPacket.ToLocal(ConquerPacket.Effect(CSocket.Client.ID, "relive"), CSocket.Client.X, CSocket.Client.Y, (int)CSocket.Client.Map, 0, 0);
                                        }
                                        break;
                                    }
                                case 133: // Jump
                                    {
                                        CSocket.Client.LastAttack = 0;
                                        //TODO: Pet jumps
                                        int NewX = ReadShort(data, 12);
                                        int NewY = ReadShort(data, 14);
                                        if (CSocket.Client.Attack != null)
                                        {
                                            if (CSocket.Client.Attack.Enabled)
                                            {
                                                CSocket.Client.Attack.Stop();
                                                CSocket.Client.Attack.Dispose();
                                            }
                                        }
                                        if (CSocket.Client.UpStam.Enabled)
                                        {
                                            CSocket.Client.UpStam.Stop();
                                        }
                                        Handler.Jump(NewX, NewY, CSocket);
                                        break;
                                    }
                                default:
                                    {
                                        Console.WriteLine("[GameServer] Unknown 0x3F2 Packet Subtype: " + SubType);
                                        break;
                                    }
                            }
                            break;
                        }
                    #endregion
                    #region 0x3F1(1009) Item / Ping Packet
                    case 1009:
                        {
                            int Subtype = data[12];
                            //TODO: Prevent this packet from being used as a ghost.
                            if (CSocket.Client.Dead && Subtype != 27)
                            {
                                CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "You are dead.", Struct.ChatType.Top));
                                break;
                            }
                            if (CSocket.Client.Attack != null)
                            {
                                if (CSocket.Client.Attack.Enabled && Subtype != 27)
                                {
                                    CSocket.Client.Attack.Stop();
                                    CSocket.Client.Attack.Dispose();
                                }
                            }
                            switch (Subtype)
                            {
                                case 1:
                                    {
                                        Handler.ItemBuy(data, CSocket);
                                        //CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[TODO] ItemBuy.", Struct.ChatType.Top));
                                        break;
                                    }
                                case 2:
                                    {
                                        Handler.ItemSell(data, CSocket);
                                        break;
                                    }
                                case 4:
                                    {
                                        int UID = ReadLong(data, 4);
                                        int Location = data[8];
                                        Handler.ItemEquip(Location, UID, CSocket);
                                        break;
                                    }
                                case 6:
                                    {
                                        int UID = ReadLong(data, 4);
                                        int Location = data[8];
                                        Handler.ItemUnequip(Location, UID, CSocket);
                                        break;
                                    }
                                case 9://Request WHS Money
                                    {
                                        int ID = ReadLong(data, 4);
                                        CSocket.Send(ConquerPacket.ItemUsage(ID, CSocket.Client.WHMoney, Struct.ItemUsage.ViewWarehouse));
                                        break;
                                    }
                                case 10://Deposit WHS Money
                                    {
                                        int Amount = ReadLong(data, 8);
                                        if (CSocket.Client.Money >= Amount)
                                        {
                                            CSocket.Client.Money -= Amount;
                                            CSocket.Client.WHMoney += Amount;
                                            CSocket.Send(ConquerPacket.Status(CSocket, 2, CSocket.Client.Money, Struct.StatusTypes.InvMoney));
                                        }
                                        break;
                                    }
                                case 11://Withdraw WHS Money
                                    {
                                        int Amount = ReadLong(data, 8);
                                        if (CSocket.Client.WHMoney >= Amount)
                                        {
                                            CSocket.Client.WHMoney -= Amount;
                                            CSocket.Client.Money += Amount;
                                            CSocket.Send(ConquerPacket.Status(CSocket, 2, CSocket.Client.Money, Struct.StatusTypes.InvMoney));
                                        }
                                        break;
                                    }
                                case 20://Upgrade Item w/ met
                                    {
                                        int Reagent = ReadLong(data, 8);
                                        int ItemUID = ReadLong(data, 4);
                                        if (CSocket.Client.Inventory.ContainsKey(ItemUID) && CSocket.Client.Inventory.ContainsKey(Reagent))
                                        {
                                            Struct.ItemInfo Meteor = CSocket.Client.Inventory[Reagent];
                                            if (Meteor.ItemID != 1088001)
                                                break;
                                            Struct.ItemInfo Item = CSocket.Client.Inventory[ItemUID];
                                            if (Calculation.CanUpgrade(Convert.ToString(Item.ItemID)))
                                            {
                                                Struct.ItemData iData = Nano.Items[Item.ItemID];
                                                if (iData.Level < 130)
                                                {
                                                    if (Calculation.Type2(Convert.ToString(Item.ItemID)) == 11 && Calculation.WeaponType(Convert.ToString(Item.ItemID)) != 117 && iData.Level < 120 || Calculation.WeaponType(Convert.ToString(Item.ItemID)) == 117 && iData.Level < 112 || Calculation.Type2(Convert.ToString(Item.ItemID)) == 13 && iData.Level < 120 || Calculation.Type2(Convert.ToString(Item.ItemID)) == 15 && iData.Level < 127 || Calculation.Type2(Convert.ToString(Item.ItemID)) == 16 && iData.Level < 129 || Calculation.Type1(Convert.ToString(Item.ItemID)) == 4 || Calculation.Type1(Convert.ToString(Item.ItemID)) == 5 || Calculation.Type2(Convert.ToString(Item.ItemID)) == 12 || Calculation.WeaponType(Convert.ToString(Item.ItemID)) == 132 && iData.Level <= 12)
                                                    {
                                                        bool Upgraded = false;
                                                        double LessChance = iData.Level / 3;
                                                        Console.WriteLine(Calculation.Quality(Convert.ToString(Item.ItemID)) + ", " + (120 - LessChance));
                                                        if (Calculation.Quality(Convert.ToString(Item.ItemID)) == 3 || Calculation.Quality(Convert.ToString(Item.ItemID)) == 4 || Calculation.Quality(Convert.ToString(Item.ItemID)) == 5)
                                                        {
                                                            if (Calculation.PercentSuccess(99 - LessChance))
                                                                Upgraded = true;
                                                        }
                                                        else if (Calculation.Quality(Convert.ToString(Item.ItemID)) == 6)
                                                        {
                                                            if (Calculation.PercentSuccess(90 - LessChance))
                                                                Upgraded = true;
                                                        }
                                                        else if (Calculation.Quality(Convert.ToString(Item.ItemID)) == 7)
                                                        {
                                                            if (Calculation.PercentSuccess(87 - LessChance))
                                                                Upgraded = true;
                                                        }
                                                        else if (Calculation.Quality(Convert.ToString(Item.ItemID)) == 8)
                                                        {
                                                            if (Calculation.PercentSuccess(83 - LessChance))
                                                                Upgraded = true;
                                                        }
                                                        else if (Calculation.Quality(Convert.ToString(Item.ItemID)) == 9)
                                                        {
                                                            if (Calculation.PercentSuccess(79 - LessChance))
                                                            {
                                                                Upgraded = true;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (Calculation.PercentSuccess(99 - LessChance))
                                                                Upgraded = true;
                                                        }
                                                        if (Upgraded)
                                                        {
                                                            CSocket.Send(ConquerPacket.ItemUsage(Reagent, 255, Struct.ItemUsage.RemoveItem));
                                                            CSocket.Send(ConquerPacket.ItemUsage(Item.UID, 255, Struct.ItemUsage.RemoveItem));
                                                            Database.Database.DeleteItem(Reagent);
                                                            CSocket.Client.Inventory.Remove(Reagent);
                                                            Item.ItemID = Calculation.NextEquipLevel(Item.ItemID);
                                                            if (Item.Soc1 == 0)
                                                            {
                                                                if (Calculation.PercentSuccess(0.00033))
                                                                {
                                                                    Item.Soc1 = 255;
                                                                    CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "What a lucky person, your item has just gotten an extra socket! Congratulations!", Struct.ChatType.Top));
                                                                }
                                                            }
                                                            if (Item.Soc1 > 0)
                                                            {
                                                                if (Item.Soc2 == 0)
                                                                {
                                                                    if (Calculation.PercentSuccess(0.00022))
                                                                    {
                                                                        Item.Soc2 = 255;
                                                                        CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "What a lucky person, your item has just gotten an extra socket! Congratulations!", Struct.ChatType.Top));
                                                                    }
                                                                }
                                                            }
                                                            Database.Database.UpdateItem(Item);
                                                            CSocket.Send(ConquerPacket.ItemInfo(Item.UID, Item.ItemID, Item.Plus, Item.Bless, Item.Enchant, Item.Soc1, Item.Soc2, Item.Dura, Item.MaxDura, Item.Position, Item.Color));
                                                        }
                                                        else
                                                        {
                                                            CSocket.Send(ConquerPacket.ItemUsage(Reagent, 255, Struct.ItemUsage.RemoveItem));
                                                            Database.Database.DeleteItem(Reagent);
                                                            CSocket.Client.Inventory.Remove(Reagent);
                                                            CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "Better luck next time!", Struct.ChatType.Top));
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    }
                                case 19://DB Upgrade
                                    {
                                        int ItemUID = ReadLong(data, 4);
                                        int Reagent = ReadLong(data, 8);
                                        if (CSocket.Client.Inventory.ContainsKey(ItemUID) && CSocket.Client.Inventory.ContainsKey(Reagent))
                                        {
                                            Struct.ItemInfo Item = CSocket.Client.Inventory[ItemUID];
                                            Struct.ItemInfo DB = CSocket.Client.Inventory[Reagent];
                                            if (DB.ItemID != 1088000)
                                                break;
                                            if (Calculation.CanUpgrade(Convert.ToString(Item.ItemID)))
                                            {
                                                Struct.ItemData iData = Nano.Items[Item.ItemID];
                                                if (Calculation.Quality(Convert.ToString(Item.ItemID)) < 9)
                                                {
                                                    bool Upgraded = false;
                                                    double LessChance = iData.Level / 3;
                                                    if (Calculation.Quality(Convert.ToString(Item.ItemID)) == 3 || Calculation.Quality(Convert.ToString(Item.ItemID)) == 4 || Calculation.Quality(Convert.ToString(Item.ItemID)) == 5)
                                                    {
                                                        if (Calculation.PercentSuccess(90 - LessChance))
                                                            Upgraded = true;
                                                    }
                                                    else if (Calculation.Quality(Convert.ToString(Item.ItemID)) == 6)
                                                    {
                                                        if (Calculation.PercentSuccess(80 - LessChance))
                                                            Upgraded = true;
                                                    }
                                                    else if (Calculation.Quality(Convert.ToString(Item.ItemID)) == 7)
                                                    {
                                                        if (Calculation.PercentSuccess(70 - LessChance))
                                                            Upgraded = true;
                                                    }
                                                    else if (Calculation.Quality(Convert.ToString(Item.ItemID)) == 8)
                                                    {
                                                        if (Calculation.PercentSuccess(55 - LessChance))
                                                            Upgraded = true;
                                                    }
                                                    else
                                                    {
                                                        if (Calculation.PercentSuccess(90 - LessChance))
                                                            Upgraded = true;
                                                    }
                                                    if (Upgraded)
                                                    {
                                                        CSocket.Send(ConquerPacket.ItemUsage(Reagent, 255, Struct.ItemUsage.RemoveItem));
                                                        CSocket.Send(ConquerPacket.ItemUsage(Item.UID, 255, Struct.ItemUsage.RemoveItem));
                                                        Database.Database.DeleteItem(Reagent);
                                                        CSocket.Client.Inventory.Remove(Reagent);
                                                        if (Calculation.Quality(Convert.ToString(Item.ItemID)) == 3 || Calculation.Quality(Convert.ToString(Item.ItemID)) == 4)
                                                            Item.ItemID += 6 - Calculation.Quality(Convert.ToString(Item.ItemID));
                                                        else
                                                            Item.ItemID += 1;
                                                        if (Item.Soc1 == 0)
                                                        {
                                                            if (Calculation.PercentSuccess(0.00033))
                                                            {
                                                                Item.Soc1 = 255;
                                                                CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "What a lucky person, your item has just gotten an extra socket! Congratulations!", Struct.ChatType.Top));
                                                            }
                                                        }
                                                        if (Item.Soc1 > 0)
                                                        {
                                                            if (Item.Soc2 == 0)
                                                            {
                                                                if (Calculation.PercentSuccess(0.00022))
                                                                {
                                                                    Item.Soc2 = 255;
                                                                    CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "What a lucky person, your item has just gotten an extra socket! Congratulations!", Struct.ChatType.Top));
                                                                }
                                                            }
                                                        }
                                                        Database.Database.UpdateItem(Item);
                                                        CSocket.Send(ConquerPacket.ItemInfo(Item.UID, Item.ItemID, Item.Plus, Item.Bless, Item.Enchant, Item.Soc1, Item.Soc2, Item.Dura, Item.MaxDura, Item.Position, Item.Color));
                                                    }
                                                    else
                                                    {
                                                        CSocket.Send(ConquerPacket.ItemUsage(Reagent, 255, Struct.ItemUsage.RemoveItem));
                                                        Database.Database.DeleteItem(Reagent);
                                                        CSocket.Client.Inventory.Remove(Reagent);
                                                        CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "Better luck next time!", Struct.ChatType.Top));
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    }
                                case 27://Reply to ping
                                    {
                                        CSocket.Send(ReturnFinal(data));
                                        Database.Database.SaveCharacter(CSocket.Client);
                                        break;
                                    }
                                case 37://Drop item
                                    {
                                        int UID = ReadLong(data, 4);
                                        Handler.DropItem(UID, CSocket);
                                        break;
                                    }
                                default:
                                    {
                                        CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Please report: Unknown 1009 Subtype: " + Subtype, Struct.ChatType.Talk));
                                        break;
                                    }
                            }
                            break;
                        }
                    #endregion
                    #region 0x3ED(1005) Walk Packet
                    case 1005: //Walk packet
                        {
                            //CSocket.Client.LastAttack = 0;
                            if (CSocket.Client.Attack != null)
                            {
                                if (CSocket.Client.Attack.Enabled)
                                {
                                    CSocket.Client.Attack.Stop();
                                    CSocket.Client.Attack.Dispose();
                                }
                            }
                            Handler.Walk((ReadShort(data, 8) % 8), CSocket);
                            break;
                        }
                    #endregion
                    #region 3EC(1004) Chat Packet
                    case 1004:
                        {
                            Handler.Chat(data, CSocket);
                            break;
                        }
                    #endregion
                    #region NPC Talks
                    case 2031: //Initial NPC talk
                        {
                            int ID = ReadLong(data, 4);
                            Handler.NpcTalk(CSocket, ID, 0);
                            break;
                        }
                    case 2032: //Reply NPC Talk
                        {
                            int ID = CSocket.Client.LastNPC;
                            int LinkBack = data[10];
                            if (LinkBack != 255)
                                Handler.NpcTalk(CSocket, ID, LinkBack);
                            break;
                        }
                    #endregion
                    #region Attack
                    case 1022:
                        {
                            int AType = (data[23] << 24) + (data[22] << 16) + (data[21] << 8) + (data[20]);
                            //Console.WriteLine(System.Environment.TickCount - CSocket.Client.LastAttack);
                            if (System.Environment.TickCount - CSocket.Client.LastAttack < 450 && CSocket.Client.LastAttack > 0)
                            {
                                break;
                            }
                            if (CSocket.Client.Attack != null)
                            {
                                if (CSocket.Client.Attack.Enabled)
                                {
                                    CSocket.Client.Attack.Stop();
                                    CSocket.Client.Attack.Dispose();
                                }
                            }
                            if (CSocket.Client.UpStam.Enabled)
                            {
                                CSocket.Client.UpStam.Stop();
                            }
                            CSocket.Client.LastAttack = System.Environment.TickCount;
                            switch (AType)
                            {
                                case 2://Melee
                                    {
                                        int Target = (data[15] << 24) + (data[14] << 16) + (data[13] << 8) + (data[12]);
                                        Handler.Attack(Target, 0, 2, CSocket.Client.X, CSocket.Client.Y, CSocket);
                                        break;
                                    }
                                case 21: // Magical
                                    {
                                        int TargetID = 0;
                                        int decskill = 0;
                                        int decx = 0;
                                        int decy = 0;
                                        long targ = ((long)data[12] & 0xFF) | (((long)data[13] & 0xFF) << 8) | (((long)data[14] & 0xFF) << 16) | (((long)data[15] & 0xFF) << 24);
                                        targ = ((((targ & 0xffffe000) >> 13) | ((targ & 0x1fff) << 19)) ^ 0x5F2D2463 ^ CSocket.Client.ID) - 0x746F4AE6;
                                        TargetID = Convert.ToInt32(targ);
                                        ushort myvar = Convert.ToUInt16(((long)data[24] & 0xFF) | (((long)data[25] & 0xFF) << 8));
                                        myvar ^= (ushort)0x915d;
                                        myvar ^= (ushort)CSocket.Client.ID;
                                        myvar = (ushort)(myvar << 0x3 | myvar >> 0xd);
                                        myvar -= 0xeb42;
                                        decskill = myvar;
                                        long xx = (data[16] & 0xFF) | ((data[17] & 0xFF) << 8);
                                        long yy = (data[18] & 0xFF) | ((data[19] & 0xFF) << 8);
                                        xx = xx ^ (CSocket.Client.ID & 0xffff) ^ 0x2ed6;
                                        xx = ((xx << 1) | ((xx & 0x8000) >> 15)) & 0xffff;
                                        xx |= 0xffff0000;
                                        xx -= 0xffff22ee;
                                        yy = yy ^ (CSocket.Client.ID & 0xffff) ^ 0xb99b;
                                        yy = ((yy << 5) | ((yy & 0xF800) >> 11)) & 0xffff;
                                        yy |= 0xffff0000;
                                        yy -= 0xffff8922;
                                        decx = Convert.ToInt32(xx);
                                        decy = Convert.ToInt32(yy);
                                        Handler.Attack(TargetID, decskill, 21, decx, decy, CSocket);
                                        break;
                                    }
                                case 25:
                                    {
                                        int TargetID = (data[15] << 24) + (data[14] << 16) + (data[13] << 8) + (data[12]);
                                        Handler.Attack(TargetID, 0, 25, CSocket.Client.X, CSocket.Client.Y, CSocket);
                                        break;
                                    }
                                default:
                                    {
                                        CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[Handler-Error] Please report: Unable to handle 1002 subtype " + AType, Struct.ChatType.Top));
                                        break;
                                    }
                            }
                            break;
                        }
                    #endregion
                    #region ItemPickup
                    case 1101: //Item pickup
                        {
                            int UID = (int)((data[7] << 24) + (data[6] << 16) + (data[5] << 8) + data[4]);
                            Handler.PickupItem(UID, CSocket);
                            break;
                        }
                    #endregion
                    #region SocketGem
                    case 1027:
                        {
                            int ItemID = (data[11] << 24) + (data[10] << 16) + (data[9] << 8) + (data[8]);
                            int GemID = (data[15] << 24) + (data[14] << 16) + (data[13] << 8) + (data[12]);
                            int socket = (data[17] << 8) + (data[16]);
                            Handler.SocketGem(ItemID, GemID, socket, CSocket);
                            break;
                        }
                    #endregion
                    #region Attribute Points
                    case 1024:
                        {
                            int Str = data[4];
                            int Dex = data[5];
                            int Vit = data[6];
                            int Spi = data[7];
                            if (Str > 0)
                            {
                                if (CSocket.Client.StatPoints >= 1)
                                {
                                    CSocket.Client.StatPoints -= 1;
                                    CSocket.Client.Strength += 1;
                                    CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "You now have " + CSocket.Client.Strength + " strength!", Struct.ChatType.Top));
                                }
                                else
                                    CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You do not have enough attribute points to add more points into that attribute.", Struct.ChatType.Top));
                            }
                            if (Dex > 0)
                            {
                                if (CSocket.Client.StatPoints >= 1)
                                {
                                    CSocket.Client.StatPoints -= 1;
                                    CSocket.Client.Dexterity += 1;
                                    CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "You now have " + CSocket.Client.Dexterity + " dexterity!", Struct.ChatType.Top));
                                }
                                else
                                    CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You do not have enough attribute points to add more points into that attribute.", Struct.ChatType.Top));
                            }
                            if (Vit > 0)
                            {
                                if (CSocket.Client.StatPoints >= 1)
                                {
                                    CSocket.Client.StatPoints -= 1;
                                    CSocket.Client.Vitality += 1;
                                    CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "You now have " + CSocket.Client.Vitality + " vitality!", Struct.ChatType.Top));
                                }
                                else
                                    CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You do not have enough attribute points to add more points into that attribute.", Struct.ChatType.Top));
                            }
                            if (Spi > 0)
                            {
                                if (CSocket.Client.StatPoints >= 1)
                                {
                                    CSocket.Client.StatPoints -= 1;
                                    CSocket.Client.Spirit += 1;
                                    CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "You now have " + CSocket.Client.Spirit + " spirit!", Struct.ChatType.Top));
                                }
                                else
                                    CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You do not have enough attribute points to add more points into that attribute.", Struct.ChatType.Top));
                            }
                            Calculation.Vitals(CSocket, false);
                            break;
                        }
                    #endregion
                    #region Team
                    case 1023:
                        {
                            int subtype = data[4];
                            switch (subtype)
                            {
                                case 0://Create team
                                    {
                                        if (CSocket.Client.Team == null)
                                        {
                                            CSocket.Client.Team = new Struct.Team();
                                            CSocket.Client.Team.LeaderID = CSocket.Client.ID;
                                            CSocket.Client.Team.Forbid = false;
                                            CSocket.Client.Team.Members.Add(CSocket.Client.ID, CSocket);
                                            CSocket.Send(ConquerPacket.Team(CSocket.Client.ID, Struct.TeamOption.MakeTeam));
                                        }
                                        else
                                        {
                                            CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You are already in a team!", Struct.ChatType.Top));
                                        }
                                        break;
                                    }
                                case 1://Request to join
                                    {
                                        if (CSocket.Client.Team != null)
                                        {
                                            CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You are already in a team!", Struct.ChatType.Top));
                                            break;
                                        }
                                        int Leader = ReadLong(data, 8);
                                        if (Nano.ClientPool.ContainsKey(Leader))
                                        {
                                            ClientSocket TeamLeader = Nano.ClientPool[Leader];
                                            if (TeamLeader.Client.Team != null)
                                            {
                                                if (TeamLeader.Client.Team.LeaderID == TeamLeader.Client.ID)
                                                {
                                                    if (!TeamLeader.Client.Team.Forbid)
                                                    {
                                                        if (TeamLeader.Client.Team.Members.Count < 5)
                                                        {
                                                            TeamLeader.Send(ConquerPacket.Team(CSocket.Client.ID, Struct.TeamOption.JoinTeam));
                                                            CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[Team] Request to join team sent to " + TeamLeader.Client.Name, Struct.ChatType.Top));
                                                        }
                                                        else
                                                        {
                                                            CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] " + TeamLeader.Client.Name + "'s team is full.", Struct.ChatType.Top));
                                                        }
                                                    }
                                                    else
                                                    {
                                                        CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] " + TeamLeader.Client.Name + "'s team forbids new members.", Struct.ChatType.Top));
                                                    }
                                                }
                                                else
                                                {
                                                    CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] " + TeamLeader.Client.Name + " is not the team leader.", Struct.ChatType.Top));
                                                }
                                            }
                                            else
                                            {
                                                CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] " + TeamLeader.Client.Name + " has not created a team.", Struct.ChatType.Top));
                                            }
                                        }
                                        break;
                                    }
                                case 2://Exit Team
                                    {
                                        if (CSocket.Client.Team != null)
                                        {
                                            ClientSocket Leader = Nano.ClientPool[CSocket.Client.Team.LeaderID];
                                            foreach (KeyValuePair<int, ClientSocket> Member in Leader.Client.Team.Members)
                                            {
                                                if (Member.Value.Client.ID != CSocket.Client.ID)
                                                {
                                                    Member.Value.Send(ConquerPacket.Chat(0, "SYSTEM", Member.Value.Client.Name, "[Team] " + CSocket.Client.Name + " has just left the team!", Struct.ChatType.Top));
                                                    Member.Value.Send(ConquerPacket.Team(CSocket.Client.ID, Struct.TeamOption.LeaveTeam));
                                                }
                                            }
                                            Leader.Client.Team.Members.Remove(CSocket.Client.ID);
                                            CSocket.Client.Team = null;
                                            //CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[Team] You have left the team.", Struct.ChatType.Top));
                                            CSocket.Send(ConquerPacket.Team(CSocket.Client.ID, Struct.TeamOption.LeaveTeam));
                                        }
                                        else
                                        {
                                            CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You are not in a team!", Struct.ChatType.Top));
                                        }
                                        break;
                                    }
                                case 3: //Accept Invite
                                    {
                                        if (CSocket.Client.Team != null)
                                        {
                                            CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You are already in a team!", Struct.ChatType.Top));
                                            break;
                                        }
                                        int Inviter = ReadLong(data, 8);
                                        if (Nano.ClientPool.ContainsKey(Inviter))
                                        {
                                            ClientSocket TeamLeader = Nano.ClientPool[Inviter];
                                            if (TeamLeader.Client.Team != null)
                                            {
                                                if (TeamLeader.Client.Team.Members.Count < 5)
                                                {
                                                    if (!TeamLeader.Client.Team.Forbid)
                                                    {
                                                        foreach (KeyValuePair<int, ClientSocket> Member in TeamLeader.Client.Team.Members)
                                                        {
                                                            Member.Value.Send(ConquerPacket.TeamMember(CSocket.Client));
                                                            CSocket.Send(ConquerPacket.TeamMember(Member.Value.Client));
                                                        }
                                                        TeamLeader.Client.Team.Members.Add(CSocket.Client.ID, CSocket);
                                                        CSocket.Client.Team = TeamLeader.Client.Team;
                                                    }
                                                    else
                                                    {
                                                        CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Inviter's team does not accept new members.", Struct.ChatType.Top));
                                                    }
                                                }
                                                else
                                                {
                                                    CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Inviter's team is full.", Struct.ChatType.Top));
                                                }
                                            }
                                            else
                                            {
                                                CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Inviter no longer has a team.", Struct.ChatType.Top));
                                            }
                                        }
                                        else
                                        {
                                            CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Inviter is no longer online.", Struct.ChatType.Top));
                                        }
                                        break;
                                    }
                                case 4: //Invite to join
                                    {
                                        if (CSocket.Client.Team == null)
                                        {
                                            CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You are not in a team!", Struct.ChatType.Top));
                                            break;
                                        }
                                        else
                                        {
                                            if (CSocket.Client.Team.LeaderID != CSocket.Client.ID)
                                            {
                                                CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You are not the leader and cannot invite new members!", Struct.ChatType.Top));
                                                break;
                                            }
                                        }
                                        int Invited = ReadLong(data, 8);
                                        if (Nano.ClientPool.ContainsKey(Invited))
                                        {
                                            ClientSocket InvitedCSocket = Nano.ClientPool[Invited];
                                            if (InvitedCSocket.Client.Team == null)
                                            {
                                                if (!CSocket.Client.Team.Forbid)
                                                {
                                                    CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[Team] Invited " + InvitedCSocket.Client.Name + " to join your team.", Struct.ChatType.Top));
                                                    InvitedCSocket.Send(ConquerPacket.Team(CSocket.Client.ID, Struct.TeamOption.Invite));
                                                }
                                                else
                                                {
                                                    CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Your team forbids new members from joining.", Struct.ChatType.Top));
                                                }
                                            }
                                            else
                                            {
                                                CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Target is already in a team.", Struct.ChatType.Top));
                                            }
                                        }
                                        else
                                        {
                                            CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Target does not exist.", Struct.ChatType.Top));
                                        }
                                        break;
                                    }
                                case 5://Accept join request
                                    {
                                        if (CSocket.Client.Team == null)
                                        {
                                            CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You are not in a team!", Struct.ChatType.Top));
                                            break;
                                        }
                                        else
                                        {
                                            if (CSocket.Client.Team.LeaderID != CSocket.Client.ID)
                                            {
                                                CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You are not the leader and cannot accept join requests.", Struct.ChatType.Top));
                                                break;
                                            }
                                        }
                                        int ToJoin = ReadLong(data, 8);
                                        if (Nano.ClientPool.ContainsKey(ToJoin))
                                        {
                                            ClientSocket Invited = Nano.ClientPool[ToJoin];
                                            if (Invited.Client.Team == null)
                                            {
                                                if (!CSocket.Client.Team.Forbid)
                                                {
                                                    foreach (KeyValuePair<int, ClientSocket> Member in CSocket.Client.Team.Members)
                                                    {
                                                        Member.Value.Send(ConquerPacket.TeamMember(Invited.Client));
                                                        Invited.Send(ConquerPacket.TeamMember(Member.Value.Client));
                                                    }
                                                    CSocket.Client.Team.Members.Add(Invited.Client.ID, Invited);
                                                    Invited.Client.Team = CSocket.Client.Team;
                                                }
                                                else
                                                {
                                                    CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Your team forbids new members from joining.", Struct.ChatType.Top));
                                                }
                                            }
                                            else
                                            {
                                                CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Target is already in a team.", Struct.ChatType.Top));
                                            }
                                        }
                                        else
                                        {
                                            CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Target does not exist.", Struct.ChatType.Top));
                                        }
                                        break;
                                    }
                                case 6: //Dismiss
                                    {
                                        if (CSocket.Client.Team == null)
                                        {
                                            CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You are not in a team!", Struct.ChatType.Top));
                                            break;
                                        }
                                        else
                                        {
                                            if (CSocket.Client.Team.LeaderID != CSocket.Client.ID)
                                            {
                                                CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You are not the leader and cannot dismiss the team.", Struct.ChatType.Top));
                                                break;
                                            }
                                        }
                                        foreach (KeyValuePair<int, ClientSocket> Member in CSocket.Client.Team.Members)
                                        {
                                            if (Member.Value.Client.ID != CSocket.Client.ID)
                                            {
                                                Member.Value.Send(ConquerPacket.Team(Member.Value.Client.ID, Struct.TeamOption.DismissTeam));
                                                Member.Value.Client.Team = null;
                                            }
                                        }
                                        CSocket.Client.Team = null;
                                        CSocket.Send(ConquerPacket.Team(CSocket.Client.ID, Struct.TeamOption.DismissTeam));
                                        break;
                                    }
                                case 7: //Kick from team
                                    {
                                        if (CSocket.Client.Team == null)
                                        {
                                            CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You are not in a team!", Struct.ChatType.Top));
                                            break;
                                        }
                                        else
                                        {
                                            if (CSocket.Client.Team.LeaderID != CSocket.Client.ID)
                                            {
                                                CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You are not the leader and cannot kick team members.", Struct.ChatType.Top));
                                                break;
                                            }
                                        }
                                        int Kick = ReadLong(data, 8);
                                        if (Nano.ClientPool.ContainsKey(Kick))
                                        {
                                            if (CSocket.Client.Team.Members.ContainsKey(Kick))
                                            {
                                                foreach (KeyValuePair<int, ClientSocket> Member in CSocket.Client.Team.Members)
                                                {
                                                    Member.Value.Send(ConquerPacket.Team(Kick, Struct.TeamOption.Kick));
                                                    if (Member.Value.Client.ID == Kick)
                                                    {
                                                        Member.Value.Client.Team = null;
                                                    }
                                                }
                                                CSocket.Client.Team.Members.Remove(Kick);
                                            }
                                            else
                                            {
                                                CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Target not in team.", Struct.ChatType.Top));
                                            }
                                        }
                                        else
                                        {
                                            CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Target does not exist.", Struct.ChatType.Top));
                                        }
                                        break;
                                    }
                                case 8://Forbid
                                    {
                                        if (CSocket.Client.Team == null)
                                        {
                                            CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You are not in a team!", Struct.ChatType.Top));
                                            break;
                                        }
                                        else
                                        {
                                            if (CSocket.Client.Team.LeaderID != CSocket.Client.ID)
                                            {
                                                CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You are not the leader and cannot forbid new joins!", Struct.ChatType.Top));
                                                break;
                                            }
                                        }
                                        if (!CSocket.Client.Team.Forbid)
                                        {
                                            CSocket.Client.Team.Forbid = true;
                                        }
                                        break;
                                    }
                                case 9: //Unforbid
                                    {
                                        if (CSocket.Client.Team == null)
                                        {
                                            CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You are not in a team!", Struct.ChatType.Top));
                                            break;
                                        }
                                        else
                                        {
                                            if (CSocket.Client.Team.LeaderID != CSocket.Client.ID)
                                            {
                                                CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You are not the leader and cannot forbid new joins!", Struct.ChatType.Top));
                                                break;
                                            }
                                        }
                                        if (CSocket.Client.Team.Forbid)
                                        {
                                            CSocket.Client.Team.Forbid = false;
                                        }
                                        break;
                                    }
                                case 10://unForbidMoney
                                    {
                                        if (CSocket.Client.Team == null)
                                        {
                                            CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You are not in a team!", Struct.ChatType.Top));
                                            break;
                                        }
                                        else
                                        {
                                            if (CSocket.Client.Team.LeaderID != CSocket.Client.ID)
                                            {
                                                CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You are not the leader and cannot forbid new joins!", Struct.ChatType.Top));
                                                break;
                                            }
                                        }
                                        if (CSocket.Client.Team.ForbidMoney)
                                        {
                                            CSocket.Client.Team.ForbidMoney = false;
                                        }
                                        break;
                                    }
                                case 11://forbidMoney
                                    {
                                        if (CSocket.Client.Team == null)
                                        {
                                            CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You are not in a team!", Struct.ChatType.Top));
                                            break;
                                        }
                                        else
                                        {
                                            if (CSocket.Client.Team.LeaderID != CSocket.Client.ID)
                                            {
                                                CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You are not the leader and cannot forbid new joins!", Struct.ChatType.Top));
                                                break;
                                            }
                                        }
                                        if (CSocket.Client.Team.ForbidMoney)
                                        {
                                            CSocket.Client.Team.ForbidMoney = false;
                                        }
                                        break;
                                    }
                                case 12://ForbidItems
                                    {
                                        if (CSocket.Client.Team == null)
                                        {
                                            CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You are not in a team!", Struct.ChatType.Top));
                                            break;
                                        }
                                        else
                                        {
                                            if (CSocket.Client.Team.LeaderID != CSocket.Client.ID)
                                            {
                                                CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You are not the leader and cannot forbid new joins!", Struct.ChatType.Top));
                                                break;
                                            }
                                        }
                                        if (!CSocket.Client.Team.ForbidItems)
                                        {
                                            CSocket.Client.Team.ForbidItems = true;
                                        }
                                        break;
                                    }
                                case 13://unForbidItems
                                    {
                                        if (CSocket.Client.Team == null)
                                        {
                                            CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You are not in a team!", Struct.ChatType.Top));
                                            break;
                                        }
                                        else
                                        {
                                            if (CSocket.Client.Team.LeaderID != CSocket.Client.ID)
                                            {
                                                CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You are not the leader and cannot forbid new joins!", Struct.ChatType.Top));
                                                break;
                                            }
                                        }
                                        if (CSocket.Client.Team.ForbidItems)
                                        {
                                            CSocket.Client.Team.ForbidItems = false;
                                        }
                                        break;
                                    }
                                default:
                                    {
                                        CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Please report: Unknown team subtype: " + subtype, Struct.ChatType.Top));
                                        break;
                                    }
                            }
                            break;
                        }
                    #endregion
                    #region Warehouse
                    case 1102:
                        {
                            int ID = ReadLong(data, 4);
                            int SType = data[8];
                            if (SType == 0)
                            {
                                CSocket.Send(ConquerPacket.WarehouseItems(CSocket, ID));
                            }
                            else if (SType == 1)//Put into WHS
                            {
                                int UID = ReadLong(data, 12);
                                if (CSocket.Client.Inventory.ContainsKey(UID))
                                {
                                    Struct.ItemInfo Item = CSocket.Client.Inventory[UID];
                                    switch ((int)CSocket.Client.Map)
                                    {
                                        case 1002:
                                            {
                                                if (CSocket.Client.TCWhs.Count < 20)
                                                {
                                                    Item.Position = 1002;
                                                    Database.Database.UpdateItem(Item);
                                                    CSocket.Client.Inventory.Remove(UID);
                                                    CSocket.Send(ConquerPacket.ItemUsage(Item.UID, 255, Struct.ItemUsage.RemoveItem));
                                                    CSocket.Client.TCWhs.Add(UID, Item);
                                                    CSocket.Send(ConquerPacket.WarehouseItems(CSocket, ID));
                                                }
                                                else
                                                {
                                                    CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] TC Whs is full.", Struct.ChatType.Top));
                                                }
                                                break;
                                            }
                                        case 1011:
                                            {
                                                if (CSocket.Client.PCWhs.Count < 20)
                                                {
                                                    Item.Position = 1011;
                                                    Database.Database.UpdateItem(Item);
                                                    CSocket.Client.Inventory.Remove(UID);
                                                    CSocket.Send(ConquerPacket.ItemUsage(Item.UID, 255, Struct.ItemUsage.RemoveItem));
                                                    CSocket.Client.PCWhs.Add(UID, Item);
                                                    CSocket.Send(ConquerPacket.WarehouseItems(CSocket, ID));
                                                }
                                                else
                                                {
                                                    CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] PC Whs is full.", Struct.ChatType.Top));
                                                }
                                                break;
                                            }
                                        case 1000:
                                            {
                                                if (CSocket.Client.DCWhs.Count < 20)
                                                {
                                                    Item.Position = 1000;
                                                    Database.Database.UpdateItem(Item);
                                                    CSocket.Client.Inventory.Remove(UID);
                                                    CSocket.Send(ConquerPacket.ItemUsage(Item.UID, 255, Struct.ItemUsage.RemoveItem));
                                                    CSocket.Client.DCWhs.Add(UID, Item);
                                                    CSocket.Send(ConquerPacket.WarehouseItems(CSocket, ID));
                                                }
                                                else
                                                {
                                                    CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] DC Whs is full.", Struct.ChatType.Top));
                                                }
                                                break;
                                            }
                                        case 1015:
                                            {
                                                if (CSocket.Client.BIWhs.Count < 20)
                                                {
                                                    Item.Position = 1015;
                                                    Database.Database.UpdateItem(Item);
                                                    CSocket.Client.Inventory.Remove(UID);
                                                    CSocket.Send(ConquerPacket.ItemUsage(Item.UID, 255, Struct.ItemUsage.RemoveItem));
                                                    CSocket.Client.BIWhs.Add(UID, Item);
                                                    CSocket.Send(ConquerPacket.WarehouseItems(CSocket, ID));
                                                }
                                                else
                                                {
                                                    CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] BI Whs is full.", Struct.ChatType.Top));
                                                }
                                                break;
                                            }
                                        case 1020:
                                            {
                                                if (CSocket.Client.AMWhs.Count < 20)
                                                {
                                                    Item.Position = 1020;
                                                    Database.Database.UpdateItem(Item);
                                                    CSocket.Client.Inventory.Remove(UID);
                                                    CSocket.Send(ConquerPacket.ItemUsage(Item.UID, 255, Struct.ItemUsage.RemoveItem));
                                                    CSocket.Client.AMWhs.Add(UID, Item);
                                                    CSocket.Send(ConquerPacket.WarehouseItems(CSocket, ID));
                                                }
                                                else
                                                {
                                                    CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] AC(M) Whs is full.", Struct.ChatType.Top));
                                                }
                                                break;
                                            }
                                        case 1036:
                                            {
                                                if (CSocket.Client.MAWhs.Count < 40)
                                                {
                                                    Item.Position = 1036;
                                                    Database.Database.UpdateItem(Item);
                                                    CSocket.Client.Inventory.Remove(UID);
                                                    CSocket.Send(ConquerPacket.ItemUsage(Item.UID, 255, Struct.ItemUsage.RemoveItem));
                                                    CSocket.Client.MAWhs.Add(UID, Item);
                                                    CSocket.Send(ConquerPacket.WarehouseItems(CSocket, ID));
                                                }
                                                else
                                                {
                                                    CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] MA Whs is full.", Struct.ChatType.Top));
                                                }
                                                break;
                                            }
                                    }
                                }
                            }
                            else if (SType == 2)//Take out
                            {
                                int UID = ReadLong(data, 12);
                                switch ((int)CSocket.Client.Map)
                                {
                                    case 1002:
                                        {
                                            if (CSocket.Client.TCWhs.ContainsKey(UID))
                                            {
                                                if (CSocket.Client.Inventory.Count < 40)
                                                {
                                                    Struct.ItemInfo Item = CSocket.Client.TCWhs[UID];
                                                    Item.Position = 0;
                                                    Database.Database.UpdateItem(Item);
                                                    CSocket.Client.TCWhs.Remove(UID);
                                                    CSocket.Client.Inventory.Add(UID, Item);
                                                    CSocket.Send(ConquerPacket.WarehouseItems(CSocket, ID));
                                                    CSocket.Send(ConquerPacket.ItemInfo(Item.UID, Item.ItemID, Item.Plus, Item.Bless, Item.Enchant, Item.Soc1, Item.Soc2, Item.Dura, Item.MaxDura, Item.Position, Item.Color));
                                                }
                                                else
                                                {
                                                    CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Inventory is full.", Struct.ChatType.Top));
                                                }
                                            }
                                            else
                                            {
                                                CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Item isn't in TC Whs.", Struct.ChatType.Top));
                                            }
                                            break;
                                        }
                                    case 1011:
                                        {
                                            if (CSocket.Client.PCWhs.ContainsKey(UID))
                                            {
                                                if (CSocket.Client.Inventory.Count < 40)
                                                {
                                                    Struct.ItemInfo Item = CSocket.Client.PCWhs[UID];
                                                    Item.Position = 0;
                                                    Database.Database.UpdateItem(Item);
                                                    CSocket.Client.PCWhs.Remove(UID);
                                                    CSocket.Client.Inventory.Add(UID, Item);
                                                    CSocket.Send(ConquerPacket.WarehouseItems(CSocket, ID));
                                                    CSocket.Send(ConquerPacket.ItemInfo(Item.UID, Item.ItemID, Item.Plus, Item.Bless, Item.Enchant, Item.Soc1, Item.Soc2, Item.Dura, Item.MaxDura, Item.Position, Item.Color));
                                                }
                                                else
                                                {
                                                    CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Inventory is full.", Struct.ChatType.Top));
                                                }
                                            }
                                            else
                                            {
                                                CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Item isn't in PC Whs.", Struct.ChatType.Top));
                                            }
                                            break;
                                        }
                                    case 1000:
                                        {
                                            if (CSocket.Client.DCWhs.ContainsKey(UID))
                                            {
                                                if (CSocket.Client.Inventory.Count < 40)
                                                {
                                                    Struct.ItemInfo Item = CSocket.Client.DCWhs[UID];
                                                    Item.Position = 0;
                                                    Database.Database.UpdateItem(Item);
                                                    CSocket.Client.DCWhs.Remove(UID);
                                                    CSocket.Client.Inventory.Add(UID, Item);
                                                    CSocket.Send(ConquerPacket.WarehouseItems(CSocket, ID));
                                                    CSocket.Send(ConquerPacket.ItemInfo(Item.UID, Item.ItemID, Item.Plus, Item.Bless, Item.Enchant, Item.Soc1, Item.Soc2, Item.Dura, Item.MaxDura, Item.Position, Item.Color));
                                                }
                                                else
                                                {
                                                    CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Inventory is full.", Struct.ChatType.Top));
                                                }
                                            }
                                            else
                                            {
                                                CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Item isn't in DC Whs.", Struct.ChatType.Top));
                                            }
                                            break;
                                        }
                                    case 1015:
                                        {
                                            if (CSocket.Client.BIWhs.ContainsKey(UID))
                                            {
                                                if (CSocket.Client.Inventory.Count < 40)
                                                {
                                                    Struct.ItemInfo Item = CSocket.Client.BIWhs[UID];
                                                    Item.Position = 0;
                                                    Database.Database.UpdateItem(Item);
                                                    CSocket.Client.BIWhs.Remove(UID);
                                                    CSocket.Client.Inventory.Add(UID, Item);
                                                    CSocket.Send(ConquerPacket.WarehouseItems(CSocket, ID));
                                                    CSocket.Send(ConquerPacket.ItemInfo(Item.UID, Item.ItemID, Item.Plus, Item.Bless, Item.Enchant, Item.Soc1, Item.Soc2, Item.Dura, Item.MaxDura, Item.Position, Item.Color));
                                                }
                                                else
                                                {
                                                    CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Inventory is full.", Struct.ChatType.Top));
                                                }
                                            }
                                            else
                                            {
                                                CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Item isn't in BI Whs.", Struct.ChatType.Top));
                                            }
                                            break;
                                        }
                                    case 1020:
                                        {
                                            if (CSocket.Client.AMWhs.ContainsKey(UID))
                                            {
                                                if (CSocket.Client.Inventory.Count < 40)
                                                {
                                                    Struct.ItemInfo Item = CSocket.Client.AMWhs[UID];
                                                    Item.Position = 0;
                                                    Database.Database.UpdateItem(Item);
                                                    CSocket.Client.AMWhs.Remove(UID);
                                                    CSocket.Client.Inventory.Add(UID, Item);
                                                    CSocket.Send(ConquerPacket.WarehouseItems(CSocket, ID));
                                                    CSocket.Send(ConquerPacket.ItemInfo(Item.UID, Item.ItemID, Item.Plus, Item.Bless, Item.Enchant, Item.Soc1, Item.Soc2, Item.Dura, Item.MaxDura, Item.Position, Item.Color));
                                                }
                                                else
                                                {
                                                    CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Inventory is full.", Struct.ChatType.Top));
                                                }
                                            }
                                            else
                                            {
                                                CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Item isn't in AC(M) Whs.", Struct.ChatType.Top));
                                            }
                                            break;
                                        }
                                    case 1036:
                                        {
                                            if (CSocket.Client.MAWhs.ContainsKey(UID))
                                            {
                                                if (CSocket.Client.Inventory.Count < 40)
                                                {
                                                    Struct.ItemInfo Item = CSocket.Client.MAWhs[UID];
                                                    Item.Position = 0;
                                                    Database.Database.UpdateItem(Item);
                                                    CSocket.Client.MAWhs.Remove(UID);
                                                    CSocket.Client.Inventory.Add(UID, Item);
                                                    CSocket.Send(ConquerPacket.WarehouseItems(CSocket, ID));
                                                    CSocket.Send(ConquerPacket.ItemInfo(Item.UID, Item.ItemID, Item.Plus, Item.Bless, Item.Enchant, Item.Soc1, Item.Soc2, Item.Dura, Item.MaxDura, Item.Position, Item.Color));
                                                }
                                                else
                                                {
                                                    CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Inventory is full.", Struct.ChatType.Top));
                                                }
                                            }
                                            else
                                            {
                                                CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Item isn't in MA Whs.", Struct.ChatType.Top));
                                            }
                                            break;
                                        }
                                }
                            }
                            break;
                        }
                    #endregion
                    default:
                        {
                            Console.WriteLine("[GameServer] Unknown packet type: " + Type);
                            CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[Handler-Error] Please report: Unable to handle packet type " + Type, Struct.ChatType.Top));
                            break;
                        }
                }
                if (Split1 != null)
                    ProcessPacket(Split1, CSocket);
                if (Split2 != null)
                    ProcessPacket(Split2, CSocket);
                if (Split3 != null)
                    ProcessPacket(Split3, CSocket);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return;
            }
        }
    }
}
