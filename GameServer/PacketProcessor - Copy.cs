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
        public void WritePacketToFile(byte[] data, int startid, bool Recv)
        {
            
        }
        public static void ProcessPacket(byte[] data, ClientSocket CSocket)
        {
            try
            {
                byte[] Split1 = null;
                byte[] Split2 = null;
                byte[] Split3 = null;
                byte[] Split4 = null;
                byte[] Split5 = null;
                int Type = (BitConverter.ToInt16(data, 2));
                int Length = (BitConverter.ToInt16(data, 0)); 
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
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[PacketLog] New Packet Recived, Type: " + Type);
                Console.ResetColor();
                Console.WriteLine(Dump(data));
                switch (Type)
                {
                    #region Begin Cleint Auth
                    case 1052888:
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
                            Console.WriteLine("SENDING DUMMY PACKET");
                            byte[] test;
                            //byte[] test = String_To_Bytes("1400f903fe3fac780100000023000000320000001400f103fe3fac78000000003e000000fe3fac781c00f2036096c600fe3fac780000000000000000fe3fac78092600001c00f203fe3fac78c602e400280001000000000000000000252600001400f9034567ac780100000023000000320000001400f1034567ac78000000003e0000004567ac781c00f2036096c6004567ac7800000000000000004567ac78092600001c00f2034567ac78c602e400280001000000000000000000252600001400f9031375ac780100000023000000320000001400f1031375ac78000000003e0000001375ac781c00f2036096c6001375ac7800000000000000001375ac78092600001c00f2031375ac78c602e4000e0001000000000000000000252600001400f9034367ac780100000023000000320000001400f1034367ac78000000003e0000004367ac781c00f2036096c6004367ac7800000000000000004367ac78092600001c00f2034367ac78c602e400280001000000000000000000252600001400f9030f75ac780100000023000000320000001400f1030f75ac78000000003e0000000f75ac781c00f2036096c6000f75ac7800000000000000000f75ac78092600001c00f2030f75ac78c602e4000e0001000000000000000000252600001400f9031475ac780100000023000000320000001400f1031475ac78000000003e0000001475ac781c00f2036096c6001475ac7800000000000000001475ac78092600001c00f2031475ac78c602e4000e0004000000000000000000252600001400f9031175ac780100000023000000320000001400f1031175ac78000000003e0000001175ac781c00f2036096c6001175ac7800000000000000001175ac78092600001c00f2031175ac78c602e40016000200000000000000000025260000af00ee03c602e400f149020065000000faf1cc1d0c11466b00000000000000000000000000000000110200000f0000000200640005000400000032003200500000000000000000000000ff0a0000010000000200000000000000000006000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000007000000000000000000000000000000020b48616c6f3737395b504d5d02cede0000002400f903c602e400030000001a0000000000000028000000e803000003000000500000002400fb030000000000000000000000000000000048616c6f3737395b504d5d00000000001000f403c602e40000000000000000001400f903c602e40001000000090000000a000000");
                            //test = String_To_Bytes("1400F103FE3FAC78000000003E000000FE3FAC78");
                            //CSocket.Send(test);
                            test = String_To_Bytes("1C00F2036096C600FE3FAC780000000000000000FE3FAC7809260000");
                            CSocket.Send(test);
                            //test = String_To_Bytes("1C00F203FE3FAC78C602E40028000100000000000000000025260000");
                            //CSocket.Send(test);
                            //test = String_To_Bytes("1400F9034567AC78010000002300000032000000");
                            //CSocket.Send(test);
                            test = String_To_Bytes("AF00EE03C602E400F149020065000000FAF1CC1D0C11466B00000000000000000000000000000000110200000F0000000200640005000400000032003200500000000000000000000000FF0A0000010000000200000000000000000006000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000007000000000000000000000000000000020B48616C6F3737395B504D5D02CEDE000000");
                            CSocket.Send(test);
                            ConnectionRequest User = Nano.AuthenticatedLogins[Keys];
                            CSocket.Client = Database.Database.GetCharacter(User.Account);
                            break;

                        }
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
                                CSocket.Client.PrevX = 0;
                                CSocket.Client.PrevY = 0;
                                byte[] test;
                                test = String_To_Bytes("1C00F2036096C600FE3FAC780000000000000000FE3FAC7809260000");
                                CSocket.Send(test);
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
                                //CSocket.Send(ConquerPacket.MiniMap(true));
                                //PLEASE DO NOT REMOVE THIS CODE!
                               // CSocket.Send(ConquerPacket.General(CSocket.Client.ID, 0, 0, 0, 0, 0, Struct.DataType.ConfirmLoginComplete));
                               // CSocket.Send(ConquerPacket.Status(CSocket, 2, 0, Struct.StatusTypes.StatusEffect));
                                //CSocket.Send(String_To_Bytes("3900F003000000007D494C5EA4410600B60BB80B01000400000000000000000000000000000000000000000000000000000000000000000100"));
                                //CSocket.Send(ConquerPacket.ItemInfo(0,410020 ,50 , 0, 0, 2998, 3000, 4, 0, 0, 0));
                                foreach (KeyValuePair<int, Struct.ItemInfo> Item in CSocket.Client.Inventory)
                                {
                                    //CSocket.Send(ConquerPacket.ItemInfo(Item.Value.UID, Item.Value.ItemID, Item.Value.Plus, Item.Value.Bless, Item.Value.Enchant, Item.Value.Soc1, Item.Value.Soc2, Item.Value.Dura, Item.Value.MaxDura, Item.Value.Position, Item.Value.Color));
                                    CSocket.Send(ConquerPacket.ItemInfo(0, Item.Value.ItemID, Item.Value.Plus, Item.Value.Soc1, Item.Value.Soc2, Item.Value.Dura, Item.Value.MaxDura, Item.Value.Position, 0, 0, 6));
                                }
                                CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "Welcome to EO Emu, " + CSocket.Client.Name, Struct.ChatType.Talk));
                                CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "There are currently " + Nano.ClientPool.Count + " players online.", Struct.ChatType.Talk));
                                //CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "Use of this server binds you to the Terms of Service (ToS) located on http://www.coemu.org", Struct.ChatType.Talk));
                                //CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "Current server rates: Prof: " + Nano.PROF_MULTIPLER + "x, Skill: " + Nano.SKILL_MULTIPLER + "x, Exp: " + Nano.EXP_MULTIPLER + "x.", Struct.ChatType.Talk));
                                //CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "Be kind to your fellow player and have a good time.", Struct.ChatType.Talk));
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
                    #endregion
                    #region Testing 1010 packet (disabled)
                    case 10103423://disabled (1010 packet)
                        {
                            byte[] test = String_To_Bytes("1000f403c602e4000000000000000000");
                            CSocket.Send(test);
                            break;
                        }
                    #endregion
                    #region CreateCharacter
                    case 1001://Create Character
                        {
                            Handler.NewCharacter(data, CSocket);
                            break;
                        }
                    #endregion
                    #region 1123 (unkonwn atm)
                    case 112300:
                        {
                            byte[] test = String_To_Bytes("2400FB030000000000000000000000000000000048616C6F3737395B504D5D0000000000");
                            CSocket.Send(test);
                            break;
                        }
                    #endregion
                    #region 1009 (possibly requestion map)
                    case 1009://request map i think
                        {
                            //Console.WriteLine("SENDING DUMMY PACKET");
                            //byte[] test = String_To_Bytes("1400f903c602e4000100000009000000140000001400f10301000000000000003e000000fe3fac781400f10301000000000000003e0000004567ac781400f10301000000000000003e0000001375ac781400f10301000000000000003e0000004367ac781400f10301000000000000003e0000000f75ac781400f10301000000000000003e0000001475ac781400f10301000000000000003e0000001175ac78100063040000000019dd2a4c706200001c00f203f1b2c600e803000026019c0100000000e8030000452500001c00f203f1b2c600c602e40026019c0100000000ffffffff5f25000010005604e8030000e8030000000020002000ee071b0700001801820115470000020001000000000000000000000000002000ee07230400001c019b015e2900007b0001000000000000000000000000002000ee071b0400001b01a40114290000020001000000000000000000000000002000ee07220400001c01a101fb4c00001d0001000000000001000000000000002000ee07400600001c019e01873e0000010001000000000000000000000000002000ee07ce0400001e01ae0112300000020001000000000000000000000000002000ee07a08601001f01a4012a440000020001000000000000000000000000002000ee071d0400002d01830122290000010001000000000000000000000000002000ee071f0400002901a4013c280000010001000000000000000000000000002000ee074a8801002401a401f75500006f00010000000000e8030000000000002000ee074c8801002401ae01f75500006f00010000000000e8030000000000002000ee0721040000390181014a290000010001000000000000000000000000002000ee071c0400003601a0011f290000010001000000000000000000000000001400f903c602e40001000000240000000004000020006704c602e40028dd2a4c00000000000000000004000000000000000000000c00f70301000000800000001400f407df0300000db63201d6980300765c370d120011080100c90d33000b0e00009801c30b");
                            //CSocket.Send(test);
                            //byte[] test = String_To_Bytes("1C00F203F1B2C600E803000026019C0100000000E803000045250000");
                            CSocket.Send(Packets.ConquerPacket.General(1, (int)CSocket.Client.Map, CSocket.Client.X, CSocket.Client.Y, 0, (int)CSocket.Client.Map, Structs.Struct.DataType.MapShow));
                            ConquerPacket.ToLocal(ConquerPacket.SpawnCharacter(CSocket), CSocket.Client.X, CSocket.Client.Y, (int)CSocket.Client.Map, 0, CSocket.Client.ID);
                            Spawn.All(CSocket);
                            //CSocket.Send(test);
                            //CSocket.Send(ConquerPacket.NewMap(1000));
                            //test = String_To_Bytes("10004f044567ac7800000000e903000010004f044567ac7800000000b90b000010004f044567ac7800000000d507000010004f044567ac7800000000e60700001c00f203f7bec6001375ac7800000000000000001375ac78092600000001f507010000001375ac781e00000006000000c007000007000000c00700000a000000500000000800000096000000090000001ae4270037000000000000000c000000050000000e000000000000000f0000000000000010000000000000001100000000000000120000000000000013000000070000001500000008000000170000000100000019000000580600001a000000000000001b000000d30000001c00000009000000320000000000000033000000000000001400000000000000180000007f0100000d00000032000000010000005803000000000000660500000300000000000000020000000000000004000000bd040000050000009701000010004f041375ac7800000000e903000010004f041375ac7800000000b90b000010004f041375ac7800000000e607000010004f041375ac7800000000d50700001c00f203f7bec6004367ac7800000000000000004367ac78092600000001f507010000004367ac781e00000006000000280d000007000000280d00000a00000051000000080000009600000009000000d386330037000000000000000c000000020000000e000000d70000000f00000000000000100000000000000011000000000000001200000000000000130000000300000015000000080000001700000001000000190000000f2700001a0000000f2700001b0000000f2700001c000000b002000032000000000000003300000000000000140000000000000018000000190200000d0000003400000001000000df03000000000000e10500000300000019030000020000007105000004000000fb020000050000009702000010004f044367ac7800000000d507000010004f044367ac7800000000e607000010004f044367ac7800000000e903000010004f044367ac7800000000b90b00001c00f203f7bec6000f75ac7800000000000000000f75ac78092600000001f507010000000f75ac781e00000006000000e804000007000000e80400000a000000380000000800000096000000090000003c72020037000000000000000c000000050000000e000000000000000f0000000000000010000000000000001100000000000000120000000000000013000000070000001500000008000000170000000100000019000000cd0000001a000000050400001b000000630200001c0000000f00000032000000000000003300000000000000140000000000000018000000760100000d0000002a000000010000002d00000000000000b600000003000000e0000000020000003201000004000000e4020000050000008001000010004f040f75ac7800000000e903000010004f040f75ac7800000000b90b000010004f040f75ac7800000000e607000010004f040f75ac7800000000d50700001c00f203f7bec6001475ac7800000000000000001475ac78092600000001f507010000001475ac781e000000060000006f0a0000070000006f0a00000a000000500000000800000096000000090000001ae4270037000000000000000c000000050000000e000000000000000f0000000000000010000000000000001100000000000000120000000000000013000000070000001500000008000000170000000100000019000000570600001a000000000000001b0000008f0300001c0000001e000000320000000000000033000000000000001400000000000000180000007d0100000d0000003200000001000000e40200000000000049030000");
                            //CSocket.Send(test);
                            
                            /*PacketBuilder Packet = new PacketBuilder(1019, 36);
                            Packet.Long(0);
                            Packet.Long(0);
                            Packet.Long(0);
                            Packet.Long(0);
                            Packet.Text(CSocket.Client.Name.ToString());
                            CSocket.Send(Packet.getFinal());*/

                            
                            break;
                        }
                    case 2036:// excape key its seems
                        {
                           PacketBuilder Packet = new PacketBuilder(2036,12);
                           Packet.Short(473);
                           Packet.Short(0);
                           Packet.Long(0);
                            
                            CSocket.Send(Packet.getFinal());
                            CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", "ALLUSERS", "Excape Key (or any other hotkey) May not work currently", Struct.ChatType.Top));
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
                            CSocket.Client.X = RX;
                            CSocket.Client.Y = RY;
                            int ukwn = (BitConverter.ToInt16(data, 16));
                            //CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "Dirrection: " + dir.ToString(), Struct.ChatType.Talk));
                            Handler.Walk(ukwn, CSocket);
                            break;
                        }
                    #endregion
                    #region 0x3f2(1010) Multi-Function Packet
                    case 9991010: // 0x3f2, Multi-Function Packet
                        {
                            if (data.Length < 0x29)
                                break;
                            //int SubType = data[0x18];
                            int SubType = data[24];
                            //int SubType = BitConverter.ToInt16(data, 24);
                            switch (SubType)
                            {
                                case 70: //Start login sequence.
                                    {
                                        Console.WriteLine("[GameServer] Login Sequence started for " + CSocket.Client.Name);
                                        //CSocket.Send(ConquerPacket.General(CSocket.Client.ID, (int)CSocket.Client.Map, CSocket.Client.X, CSocket.Client.Y, 0, Struct.DataType.MapShow));
                                        CSocket.Send(ConquerPacket.General(CSocket.Client.ID, (int)CSocket.Client.Map, CSocket.Client.X, CSocket.Client.Y, 0, (int)CSocket.Client.Map, Struct.DataType.MapShow));
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
                                        //CSocket.Send(ConquerPacket.Status(CSocket, 2, CSocket.Client.CurrentStam, Struct.StatusTypes.Stamina));
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
                                
                                         
                                        
                                   
                                default:
                                    {
                                        Console.WriteLine("[GameServer] Unknown 0x3F2 Packet Subtype: " + SubType);
                                        break;
                                    }
                            }
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

                    #region Team
                    case 1023:
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
