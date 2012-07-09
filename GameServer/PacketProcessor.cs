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
        public void WritePacketToFile(byte[] data, int startid, bool Recv)
        {
            
        }
        public static void ProcessPacket(byte[] data, ClientSocket CSocket)
        {
            try
            {
                #region Packet Splitter
                //@TODO: move packetsplitting to a faster function, possibly async for faster packet processing (consider thread counts of doing so however.. also need to remove the creation of a new thread for each client, upon a lot of load it wouldn't work out well)
                
                byte[] Split1 = null;
                byte[] Split2 = null;
                byte[] Split3 = null;
                byte[] Split4 = null;
                byte[] Split5 = null;

                int Type = (BitConverter.ToInt16(data, 2));
                int Length = (BitConverter.ToInt16(data, 0)); 
                String Tmp = BitConverter.ToString(data);
                if (data.Length > Length)
                {
                    int Len2 = BitConverter.ToInt16(data, 0 + Length);
                    Split1 = new byte[Len2];
                    Array.Copy(data,Length,Split1,0,Len2);

                    if (data.Length > Length + Len2)
                    {

                        int Len3 = BitConverter.ToInt16(data, 0 + Length + Len2);
                        Split2 = new byte[Len3];
                        Array.Copy(data, Length + Len2, Split2, 0, Len3);

                        if (data.Length > Length + Len2 + Len3)
                        {
                            int Len4 = BitConverter.ToInt16(data, 0 + Length + Len2 + Len3);
                            Split3 = new byte[Len4];
                            Array.Copy(data, Length + Len2 + Len3, Split3, 0, Len4);

                            if (data.Length > Length + Len2 + Len3 + Len4)
                            {
                                int Len5 = BitConverter.ToInt16(data, 0 + Length + Len2 + Len3 + Len4);
                                Split4 = new byte[Len5];
                                Array.Copy(data, Length + Len2 + Len3 + Len4, Split4, 0, Len5);

                                if (data.Length > Length + Len2 + Len3 + Len4 + Len5)
                                {
                                    int Len6 = BitConverter.ToInt16(data, 0 + Length + Len2 + Len3 + Len4 + Len5);
                                    Split5 = new byte[Len6];
                                    Array.Copy(data, Length + Len2 + Len3 + Len4 + Len5, Split5, 0, Len6);

                                    if (data.Length > Length + Len2 + Len3 + Len4 + Len5 + Len6)
                                    {
                                        Console.WriteLine("WARNING: Too many bulk packets");
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[PacketLog] New Packet Received, Type: " + Type);
                Console.ResetColor();
                Console.WriteLine(Dump(data));
                switch (Type)
                {
                    #region Begin Cleint Auth (1052)
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

                                CSocket.Send(ConquerPacket.General(0, 2024554494, 0, 0, 0, 2024554494, Struct.DataType.UnkownLogin)); // test byte in proper packet form - sets client to send 1010 packet
                                if (CSocket.Client == null)
                                {                                               
                                    Console.WriteLine("[" + Nano.AuthenticatedLogins[Keys].Key + "] Making account");
                                        
                                    CSocket.Send(ConquerPacket.Chat(5, "SYSTEM", "ALLUSERS", "NEW_ROLE", Struct.ChatType.LoginInformation));
                                    return;
                                }

                                Calculation.Vitals(CSocket, true);
                                Database.Database.GetItems(CSocket);

                                Calculation.BP(CSocket.Client);
                                if (CSocket.Client.First)
                                {
                                    CSocket.Client.CurrentMP = CSocket.Client.MaxMP;
                                    CSocket.Client.CurrentHP = CSocket.Client.MaxHP;
                                }
                                if (Nano.ClientPool.ContainsKey(CSocket.Client.ID))
                                {
                                    ClientSocket C = Nano.ClientPool[CSocket.Client.ID];
                                    C.Send(ConquerPacket.Chat(0, "SYSTEM", C.Client.Name, "[ERROR] Your character has logged in from another location, you're being booted.", Struct.ChatType.Talk));
                                    C.Disconnect();
                                }
                                try
                                {
                                    Monitor.Enter(Nano.ClientPool);
                                    Nano.ClientPool.Add(CSocket.Client.ID, CSocket);
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.ToString());
                                }
                                finally
                                {
                                    Monitor.Exit(Nano.ClientPool);
                                }
                                CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", "ALLUSERS", "ANSWER_OK", Struct.ChatType.LoginInformation));
                                CSocket.Send(ConquerPacket.CharacterInfo(CSocket));
                                //CSocket.Send(ConquerPacket.MiniMap(true));
                                //PLEASE DO NOT REMOVE THIS CODE!
                                //CSocket.Send(ConquerPacket.General(CSocket.Client.ID, 0, 0, 0, 0, 0, Struct.DataType.ConfirmLoginComplete));
                                //CSocket.Send(ConquerPacket.Status(CSocket, 2, 0, Struct.StatusTypes.StatusEffect));
                                //CSocket.Send(String_To_Bytes("3900F003000000007D494C5EA4410600B60BB80B01000400000000000000000000000000000000000000000000000000000000000000000100"));
                                //CSocket.Send(ConquerPacket.ItemInfo(0,410020 ,50 , 0, 0, 2998, 3000, 4, 0, 0, 0));
                                //CSocket.Send(ConquerPacket.General(0, CSocket.Client.ID, 40, 1, 0, 0, Struct.DataType.testingloginitemcount));
                                foreach (KeyValuePair<int, Struct.ItemInfo> Item in CSocket.Client.Inventory)
                                {
                                    //CSocket.Send(ConquerPacket.ItemInfo(Item.Value.UID, Item.Value.ItemID, Item.Value.Plus, Item.Value.Bless, Item.Value.Enchant, Item.Value.Soc1, Item.Value.Soc2, Item.Value.Dura, Item.Value.MaxDura, Item.Value.Position, Item.Value.Color));
                                    CSocket.Send(ConquerPacket.ItemInfo(Item.Value.UID, Item.Value.ItemID, Item.Value.Plus, Item.Value.Soc1, Item.Value.Soc2, Item.Value.Dura, Item.Value.MaxDura, Item.Value.Position, 0, 0, 0));
                                }
                                                               
                                //CSocket.Send(ConquerPacket.EudemonTopIndicator(2024553253, 1071210));
                                //CSocket.Send(ConquerPacket.General(0, 2024553253, 0, 0, 0, 2024553253, Struct.DataType.eudtype));
                                //CSocket.Send(String_To_Bytes("B800F50701000000253BAC7815000000060000001400000007000000140000000A000000010000000800000064000000090000000000000037000000000000000C000000040000000E000000000000000F0000000000000010000000000000001100000000000000120000000000000013000000020000001500000008000000170000000100000019000000780900001A0000005D0400001B000000450800001C0000001400000032000000000000003300000000000000"));
                                


                                //CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "Welcome to EO Emu, " + CSocket.Client.Name, Struct.ChatType.Talk));
                                //CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "There are currently " + Nano.ClientPool.Count + " players online.", Struct.ChatType.Talk));
                                //CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "Be kind to your fellow player and have a good time.", Struct.ChatType.Talk));
                                if (CSocket.Client.First)
                                {
                                    Database.Database.SaveCharacter(CSocket.Client);
                                    Handler.Text("Welcome to the WONDERFUL world of EOEmu!", CSocket);
                                    Handler.Text("\n Use of this server BINDS YOU to the terms of service.", CSocket);
                                    Handler.End(CSocket);
                                }
                                //ConquerPacket.ToServer(ConquerPacket.Chat(0, "SYSTEM", "ALLUSERS", CSocket.Client.Name + " has come online.", Struct.ChatType.Top), 0);
                                
                            }
                            else
                            {
                                Console.WriteLine("[GameServer] Unauthenticated Login.");
                                CSocket.Disconnect();
                            }
                            break;
                        }
                    #endregion
                    #region CreateCharacter (1001)
                    case 1001://Create Character
                        {
                            Handler.NewCharacter(data, CSocket);
                            break;
                        }
                    #endregion
                    #region Item Packet (1009)
                    case 1009:
                        {
                            int Subtype = data[12];
                            

                            if (CSocket.Client.Dead && Subtype != 27)
                            {
                                CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "You are dead.", Struct.ChatType.System));
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
                            
                            switch ((Struct.ItemUsage)Subtype)
                            {
                                case Struct.ItemUsage.BuyItem://Buy Item
                                    {
                                        CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Not implemented packet 1009 Subtype: " + Subtype, Struct.ChatType.Talk));
                                        break;
                                    }
                                case Struct.ItemUsage.SellItem://Sell Item
                                    {
                                        CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Not implemented packet 1009 Subtype: " + Subtype, Struct.ChatType.Talk));
                                        break;
                                    }
                                case Struct.ItemUsage.RemoveDropItem://Drop item
                                    {
                                        int UID = ReadLong(data, 4);
                                        Handler.DropItem(UID, CSocket);
                                        break;
                                    }
                                case Struct.ItemUsage.EquipUseItem://Use Item
                                    {
                                        int UID = ReadLong(data, 4);
                                        int location = data[8];
                                        Handler.ItemEquip(location, UID, CSocket);
                                        break;

                                    }
                                case Struct.ItemUsage.UnequipItem://Unequip Item
                                    {
                                        int UID = ReadLong(data, 4);
                                        int location = data[8];
                                        Handler.ItemUnequip(location, UID, CSocket);
                                        break;
                                    }
                                case Struct.ItemUsage.SplitItem://Split Item
                                    {
                                        CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Not implemented packet 1009 Subtype: " + Subtype, Struct.ChatType.Talk));
                                        break;
                                    }
                                case Struct.ItemUsage.CombineItem://Combie Item
                                    {
                                        CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Not implemented packet 1009 Subtype: " + Subtype, Struct.ChatType.Talk));
                                        break;
                                    }
                                case Struct.ItemUsage.ViewWarehouse://Request Money in Warehouse
                                    {

                                        int NPCID = (BitConverter.ToInt16(data, 4));
                                        CSocket.Send(ConquerPacket.ItemUsage(NPCID, CSocket.Client.WHMoney, Struct.ItemUsage.ViewWarehouse));
                                        break;
                                    }
                                case Struct.ItemUsage.DepositCash://Deposit money to Warehouse
                                    {
                                        int NPCID = (BitConverter.ToInt16(data, 4));
                                        int Money = (BitConverter.ToInt16(data, 8));
                                        if (CSocket.Client.Money >= Money)
                                        {
                                            CSocket.Client.Money -= Money;
                                            CSocket.Client.WHMoney += Money;
                                            CSocket.Send(ConquerPacket.Status(CSocket, Struct.StatusTypes.InvMoney, CSocket.Client.Money));
                                        }
                                        break;
                                    }
                                case Struct.ItemUsage.WithdrawCash://Withdraw Money from Warehouse
                                    {
                                        int NPCID = (BitConverter.ToInt16(data, 4));
                                        int Amount = (BitConverter.ToInt16(data, 8));
                                        if (CSocket.Client.WHMoney >= Amount)
                                        {
                                            CSocket.Client.WHMoney -= Amount;
                                            CSocket.Client.Money += Amount;
                                            CSocket.Send(ConquerPacket.Status(CSocket, Struct.StatusTypes.InvMoney, CSocket.Client.Money));
                                        }
                                        break;
                                    }
                                case Struct.ItemUsage.DropMoney://Drop money
                                    {
                                        CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Not implemented packet 1009 Subtype: " + Subtype, Struct.ChatType.Talk));
                                        break;
                                    }
                                case Struct.ItemUsage.Repair://repair item
                                    {
                                        CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Not implemented packet 1009 Subtype: " + Subtype, Struct.ChatType.Talk));
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
                    #region 2036 (function keys - excape)
                    case 1123:
                        {
                            PacketBuilder Packet = new PacketBuilder(1123, 16);
                            Packet.Long(0);
                            Packet.Long(ConquerPacket.Timer);
                            Packet.Long(25200);
                            CSocket.Send(Packet.getFinal());
                            break;
                        }
                    case 2036:// excape key its seems
                        {
                            int Action = BitConverter.ToInt16(data, 4);
                            switch (Action)
                            {
                                case 473: //Game Exit
                                    {
                                        CSocket.Send(ConquerPacket.ExitPacket());
                                        break;
                                    }

                                default:
                                    {
                                        CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Please report: Unknown 2036 Actiontype: " + Action, Struct.ChatType.Talk));
                                        break;
                                    }

                            }
                            break;
                        }
                    case 1032:
                        {
                            int Subtype = data[22];
                            switch (Subtype)
                            {
                                case 107:
                                    {
                                        CSocket.Send(ConquerPacket.Dialog(898));
                                        break;
                                    }
                                case 29:
                                    {
                                        CSocket.Send(ConquerPacket.Dialog(473));
                                        break;
                                    }
                                default:
                                    {
                                        CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Please report: Unknown 1032 Subtype: " + Subtype, Struct.ChatType.Talk));
                                        break;
                                    }

                            }
                            //CSocket.Send(ConquerPacket.ExitPacket());
                            break;
                        }

                    #endregion
                    #region 0x3ED(1005) Walk Packet
                    case 3005: //Walk packet
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
                            int RX = (BitConverter.ToInt16(data, 12));
                            int RY = (BitConverter.ToInt16(data, 14));
                            //@TODO: check last cords vs current.
                            //CSocket.Client.X = RX;
                            //CSocket.Client.Y = RY;
                            int Direction = (BitConverter.ToInt16(data, 16)) % 8;
                            //CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "X: " + RX + " Y: " + RY + " ukwn: " + ukwn, Struct.ChatType.Talk));
                            Handler.Walk(Direction, CSocket);
                            break;
                        }
                    #endregion
                    #region 0x3f2(1010) Multi-Function Packet
                    case 1010: // 0x3f2, Multi-Function Packet
                        {
                            /*if (data.Length < 0x29)
                                                    break;*/
                            int SubType = BitConverter.ToInt16(data, 24);
                            switch (SubType)
                            {
                                case 9541: //Start login sequence.
                                    {
                                        Console.WriteLine("[GameServer] Login Sequence started for " + CSocket.Client.Name);
                                        CSocket.Send(ConquerPacket.General(1, (int)CSocket.Client.Map, CSocket.Client.X, CSocket.Client.Y, 0, (int)CSocket.Client.Map, Structs.Struct.DataType.MapShow));
                                        ConquerPacket.ToLocal(ConquerPacket.SpawnCharacter(CSocket), CSocket.Client.X, CSocket.Client.Y, (int)CSocket.Client.Map, 0, CSocket.Client.ID);
                                        Spawn.All(CSocket);
                                        CSocket.Client.Save = new System.Timers.Timer();
                                        CSocket.Client.Save.Elapsed += delegate {
                                            Database.Database.SaveCharacter(CSocket.Client);
                                            CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "Saved " + CSocket.Client.Name, Struct.ChatType.System));
                                        };
                                        CSocket.Client.Save.Interval = 200000;
                                        CSocket.Client.Save.Start();
                                        CSocket.Client.UpStam = new System.Timers.Timer();
                                        CSocket.Client.UpStam.Interval = 1000;
                                        CSocket.Client.UpStam.Elapsed += delegate { 
                                            CSocket.AddStam();
                                        };
                                        CSocket.Client.UpStam.Start();
                                        //CSocket.Send(ConquerPacket.Status(CSocket, 2, CSocket.Client.CurrentStam, Struct.StatusTypes.Stamina));
                                        CSocket.Client.LastAttack = System.Environment.TickCount;
                                        break;
                                    }
                                case 9542:
                                    {
                                        CSocket.Send(ConquerPacket.General(0, CSocket.Client.ID, 0, 0, 0, 1, Struct.DataType.PkmodeAndUi));

                                        CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "Class: " + CSocket.Client.Class, Struct.ChatType.Talk));
                                        /*CSocket.Send(ConquerPacket.Chat(2357, "SYSTEM", CSocket.Client.Name, "PK mode has been set to Peace Mode!", Struct.ChatType.Top));
                                        PacketBuilder Packet1 = new PacketBuilder(2036, 12);
                                        Packet1.Long(473);
                                        Packet1.Long(410);
                                        //CSocket.Send(Packet1.getFinal());

                                        //CSocket.Send(ConquerPacket.General(0, CSocket.Client.ID, 0, 0, 0, 0, Struct.DataType.UiMaybe2));
                                        PacketBuilder Packet2 = new PacketBuilder(1044, 12);
                                        Packet2.Long(1);
                                        Packet2.Long(0);
                                        //CSocket.Send(Packet2.getFinal());*/
                                        //CSocket.Send(String_To_Bytes("1c00f20330709948c602e400000000000000000001000000542500005400ec030000ff00d507000035090000ffffffff00000000040653595354454d0b48616c6f3737395b504d5d0023504b206d6f646520686173206265656e2073657420746f205065616365204d6f6465210000000c00f407d90100009a0100001c00f20330709948c602e400000000000000000000000000522500000c0014040100000000000000"));
                                        break;
                                    }
                                case 9556: //PK Mode
                                    {
                                        int PkType = data[20];
                                        if (PkType > 4)
                                            break;
                                        CSocket.Client.PKMode = (Struct.PkType)PkType;
                                        CSocket.Send(ConquerPacket.General(0, CSocket.Client.ID, 0, 0, 0, PkType, Struct.DataType.PkmodeAndUi));
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
                    #region NPC Talks (2031 & 2032)
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
                    #region ItemPickup
                    case 1101: //Item pickup
                        {
                            data[4] = (byte)(data[4] ^ 0x37);
                            data[5] = (byte)(data[5] ^ 0x25);
                            int UID = BitConverter.ToInt32(data, 4);
                            Handler.PickupItem(UID, CSocket);
                            break;
                        }
                    #endregion

                    #region Team - NEEDS WORK
                    case 10283:
                        {
                            Console.WriteLine("team Packet");
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
                                            CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You are already in a team!", Struct.ChatType.System));
                                        }
                                        break;
                                    }
                                case 1://Request to join
                                    {
                                        if (CSocket.Client.Team != null)
                                        {
                                            CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You are already in a team!", Struct.ChatType.System));
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
                                                            CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[Team] Request to join team sent to " + TeamLeader.Client.Name, Struct.ChatType.System));
                                                        }
                                                        else
                                                        {
                                                            CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] " + TeamLeader.Client.Name + "'s team is full.", Struct.ChatType.System));
                                                        }
                                                    }
                                                    else
                                                    {
                                                        CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] " + TeamLeader.Client.Name + "'s team forbids new members.", Struct.ChatType.System));
                                                    }
                                                }
                                                else
                                                {
                                                    CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] " + TeamLeader.Client.Name + " is not the team leader.", Struct.ChatType.System));
                                                }
                                            }
                                            else
                                            {
                                                CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] " + TeamLeader.Client.Name + " has not created a team.", Struct.ChatType.System));
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
                                                    Member.Value.Send(ConquerPacket.Chat(0, "SYSTEM", Member.Value.Client.Name, "[Team] " + CSocket.Client.Name + " has just left the team!", Struct.ChatType.System));
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
                                            CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You are not in a team!", Struct.ChatType.System));
                                        }
                                        break;
                                    }
                                case 3: //Accept Invite
                                    {
                                        if (CSocket.Client.Team != null)
                                        {
                                            CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You are already in a team!", Struct.ChatType.System));
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
                                                        CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Inviter's team does not accept new members.", Struct.ChatType.System));
                                                    }
                                                }
                                                else
                                                {
                                                    CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Inviter's team is full.", Struct.ChatType.System));
                                                }
                                            }
                                            else
                                            {
                                                CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Inviter no longer has a team.", Struct.ChatType.System));
                                            }
                                        }
                                        else
                                        {
                                            CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Inviter is no longer online.", Struct.ChatType.System));
                                        }
                                        break;
                                    }
                                case 4: //Invite to join
                                    {
                                        if (CSocket.Client.Team == null)
                                        {
                                            CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You are not in a team!", Struct.ChatType.System));
                                            break;
                                        }
                                        else
                                        {
                                            if (CSocket.Client.Team.LeaderID != CSocket.Client.ID)
                                            {
                                                CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You are not the leader and cannot invite new members!", Struct.ChatType.System));
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
                                                    CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[Team] Invited " + InvitedCSocket.Client.Name + " to join your team.", Struct.ChatType.System));
                                                    InvitedCSocket.Send(ConquerPacket.Team(CSocket.Client.ID, Struct.TeamOption.Invite));
                                                }
                                                else
                                                {
                                                    CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Your team forbids new members from joining.", Struct.ChatType.System));
                                                }
                                            }
                                            else
                                            {
                                                CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Target is already in a team.", Struct.ChatType.System));
                                            }
                                        }
                                        else
                                        {
                                            CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Target does not exist.", Struct.ChatType.System));
                                        }
                                        break;
                                    }
                                case 5://Accept join request
                                    {
                                        if (CSocket.Client.Team == null)
                                        {
                                            CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You are not in a team!", Struct.ChatType.System));
                                            break;
                                        }
                                        else
                                        {
                                            if (CSocket.Client.Team.LeaderID != CSocket.Client.ID)
                                            {
                                                CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You are not the leader and cannot accept join requests.", Struct.ChatType.System));
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
                                                    CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Your team forbids new members from joining.", Struct.ChatType.System));
                                                }
                                            }
                                            else
                                            {
                                                CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Target is already in a team.", Struct.ChatType.System));
                                            }
                                        }
                                        else
                                        {
                                            CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Target does not exist.", Struct.ChatType.System));
                                        }
                                        break;
                                    }
                                case 6: //Dismiss
                                    {
                                        if (CSocket.Client.Team == null)
                                        {
                                            CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You are not in a team!", Struct.ChatType.System));
                                            break;
                                        }
                                        else
                                        {
                                            if (CSocket.Client.Team.LeaderID != CSocket.Client.ID)
                                            {
                                                CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You are not the leader and cannot dismiss the team.", Struct.ChatType.System));
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
                                            CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You are not in a team!", Struct.ChatType.System));
                                            break;
                                        }
                                        else
                                        {
                                            if (CSocket.Client.Team.LeaderID != CSocket.Client.ID)
                                            {
                                                CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You are not the leader and cannot kick team members.", Struct.ChatType.System));
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
                                                CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Target not in team.", Struct.ChatType.System));
                                            }
                                        }
                                        else
                                        {
                                            CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Target does not exist.", Struct.ChatType.System));
                                        }
                                        break;
                                    }
                                case 8://Forbid
                                    {
                                        if (CSocket.Client.Team == null)
                                        {
                                            CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You are not in a team!", Struct.ChatType.System));
                                            break;
                                        }
                                        else
                                        {
                                            if (CSocket.Client.Team.LeaderID != CSocket.Client.ID)
                                            {
                                                CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You are not the leader and cannot forbid new joins!", Struct.ChatType.System));
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
                                            CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You are not in a team!", Struct.ChatType.System));
                                            break;
                                        }
                                        else
                                        {
                                            if (CSocket.Client.Team.LeaderID != CSocket.Client.ID)
                                            {
                                                CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You are not the leader and cannot forbid new joins!", Struct.ChatType.System));
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
                                            CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You are not in a team!", Struct.ChatType.System));
                                            break;
                                        }
                                        else
                                        {
                                            if (CSocket.Client.Team.LeaderID != CSocket.Client.ID)
                                            {
                                                CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You are not the leader and cannot forbid new joins!", Struct.ChatType.System));
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
                                            CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You are not in a team!", Struct.ChatType.System));
                                            break;
                                        }
                                        else
                                        {
                                            if (CSocket.Client.Team.LeaderID != CSocket.Client.ID)
                                            {
                                                CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You are not the leader and cannot forbid new joins!", Struct.ChatType.System));
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
                                            CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You are not in a team!", Struct.ChatType.System));
                                            break;
                                        }
                                        else
                                        {
                                            if (CSocket.Client.Team.LeaderID != CSocket.Client.ID)
                                            {
                                                CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You are not the leader and cannot forbid new joins!", Struct.ChatType.System));
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
                                            CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You are not in a team!", Struct.ChatType.System));
                                            break;
                                        }
                                        else
                                        {
                                            if (CSocket.Client.Team.LeaderID != CSocket.Client.ID)
                                            {
                                                CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You are not the leader and cannot forbid new joins!", Struct.ChatType.System));
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
                                        CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Please report: Unknown team subtype: " + subtype, Struct.ChatType.System));
                                        break;
                                    }
                            }
                            break;
                        }
                    #endregion
                    #region Attack
                    case 1022:
                        {
                            int AType = data[20];
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
                                        int Target = BitConverter.ToInt32(data, 12);
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
                                        CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[Handler-Error] Please report: Unable to handle 1002 subtype " + AType, Struct.ChatType.System));
                                        break;
                                    }
                            }
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
                    default:
                        {
                            Console.WriteLine("[GameServer] Unknown packet type: " + Type);
                            CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[Handler-Error] Please report: Unable to handle packet type " + Type, Struct.ChatType.System));
                            break;
                        }
                }
                if (Split1 != null)
                    ProcessPacket(Split1, CSocket);
                if (Split2 != null)
                    ProcessPacket(Split2, CSocket);
                if (Split3 != null)
                    ProcessPacket(Split3, CSocket);
                if (Split4 != null)
                    ProcessPacket(Split4, CSocket);
                if (Split5 != null)
                    ProcessPacket(Split5, CSocket);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return;
            }
        }
    }
}
