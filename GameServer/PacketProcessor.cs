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
                if(CSocket.Client != null)
                    Console.WriteLine("[PacketLog] New Packet Received From [" + CSocket.Client.ID + " - " + CSocket.Client.Name + "] Packet Type: " + Type + " Packet Size: " + Length);
                else
                    Console.WriteLine("[PacketLog] New Packet Received From [Unknown Client] Packet Type: " + Type + " Packet Size: " + Length);
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
                            
                            if (MainGS.AuthenticatedLogins.ContainsKey(Keys))
                            {
                                CSocket.AccountName = MainGS.AuthenticatedLogins[Keys].Account;
                                Console.WriteLine("[GameServer] Authenticated Login.");
                                ConnectionRequest User = MainGS.AuthenticatedLogins[Keys];
                                User.Expire(false);
                                CSocket.Client = Database.Database.GetCharacter(User.Account);
                                if (CSocket.Client == null)
                                {                                               
                                    Console.WriteLine("[" + MainGS.AuthenticatedLogins[Keys].Key + "] Making account");
                                        
                                    CSocket.Send(EudemonPacket.Chat(5, "SYSTEM", "ALLUSERS", "NEW_ROLE", Struct.ChatType.LoginInformation));
                                    return;
                                }

                                Console.WriteLine("[GameServer] Login Sequence started for " + CSocket.Client.Name);
                                Calculation.Vitals(CSocket, true);
                                Database.Database.GetItems(CSocket);

                                Calculation.BP(CSocket.Client);
                                if (CSocket.Client.First)
                                {
                                    CSocket.Client.CurrentMP = CSocket.Client.MaxMP;
                                    CSocket.Client.CurrentHP = CSocket.Client.MaxHP;
                                }
                                if (MainGS.ClientPool.ContainsKey(CSocket.Client.ID))
                                {
                                    ClientSocket C = MainGS.ClientPool[CSocket.Client.ID];
                                    C.Send(EudemonPacket.Chat(0, "SYSTEM", C.Client.Name, "[ERROR] Your character has logged in from another location, you're being booted.", Struct.ChatType.Talk));
                                    C.Disconnect();
                                }
                                try
                                {
                                    Monitor.Enter(MainGS.ClientPool);
                                    MainGS.ClientPool.Add(CSocket.Client.ID, CSocket);
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.ToString());
                                }
                                finally
                                {
                                    Monitor.Exit(MainGS.ClientPool);
                                }
                                CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", "ALLUSERS", "ANSWER_OK", Struct.ChatType.LoginInformation));
                                CSocket.Send(EudemonPacket.CharacterInfo(CSocket));
                                CSocket.Send(EudemonPacket.Status(CSocket, Struct.StatusTypes.MAXMANA, CSocket.Client.MaxMP));
                                

                                CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "Welcome to EO Emu, " + CSocket.Client.Name, Struct.ChatType.Talk));
                                CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "There are currently " + MainGS.ClientPool.Count + " players online.", Struct.ChatType.Talk));
                                
                                if (CSocket.Client.First)
                                {
                                    Database.Database.SaveCharacter(CSocket.Client);
                                    Handler.Text("Welcome to the world of EOEmu!", CSocket);
                                    Handler.End(CSocket);
                                }
                                EudemonPacket.ToServer(EudemonPacket.Chat(0, "SYSTEM", "ALLUSERS", CSocket.Client.Name + " has come online.", Struct.ChatType.System), 0);

                                CSocket.Client.Save = new System.Timers.Timer();
                                CSocket.Client.Save.Elapsed += delegate
                                {
                                    Database.Database.SaveCharacter(CSocket.Client);
                                    CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "Saved " + CSocket.Client.Name, Struct.ChatType.System));
                                };
                                CSocket.Client.Save.Interval = 200000;
                                CSocket.Client.Save.Start();
                                CSocket.Client.UpStam = new System.Timers.Timer();
                                CSocket.Client.UpStam.Interval = 1000;
                                CSocket.Client.UpStam.Elapsed += delegate
                                {
                                    CSocket.AddStam();
                                };
                                CSocket.Client.UpStam.Start();
                                CSocket.Client.LastAttack = System.Environment.TickCount;

                                CSocket.Client.Tick = new System.Timers.Timer();
                                CSocket.Client.Tick.Interval = 10000;
                                CSocket.Client.Tick.Elapsed += delegate
                                {
                                    CSocket.Send(EudemonPacket.Tick(CSocket.Client.ID));
                                };
                                CSocket.Client.Tick.Start();
                                CSocket.Send(EudemonPacket.Tick(CSocket.Client.ID));
                                
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
                                CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "You are dead.", Struct.ChatType.System));
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
                                        CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Not implemented packet 1009 Subtype: " + Subtype, Struct.ChatType.Talk));
                                        break;
                                    }
                                case Struct.ItemUsage.SellItem://Sell Item
                                    {
                                        CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Not implemented packet 1009 Subtype: " + Subtype, Struct.ChatType.Talk));
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
                                        CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Not implemented packet 1009 Subtype: " + Subtype, Struct.ChatType.Talk));
                                        break;
                                    }
                                case Struct.ItemUsage.CombineItem://Combie Item
                                    {
                                        CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Not implemented packet 1009 Subtype: " + Subtype, Struct.ChatType.Talk));
                                        break;
                                    }
                                case Struct.ItemUsage.ViewWarehouse://Request Money in Warehouse
                                    {

                                        int NPCID = (BitConverter.ToInt16(data, 4));
                                        CSocket.Send(EudemonPacket.ItemUsage(NPCID, CSocket.Client.WHMoney, Struct.ItemUsage.ViewWarehouse));
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
                                            CSocket.Send(EudemonPacket.Status(CSocket, Struct.StatusTypes.InvMoney, CSocket.Client.Money));
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
                                            CSocket.Send(EudemonPacket.Status(CSocket, Struct.StatusTypes.InvMoney, CSocket.Client.Money));
                                        }
                                        break;
                                    }
                                case Struct.ItemUsage.DropMoney://Drop money
                                    {
                                        CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Not implemented packet 1009 Subtype: " + Subtype, Struct.ChatType.Talk));
                                        break;
                                    }
                                case Struct.ItemUsage.Repair://repair item
                                    {
                                        CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Not implemented packet 1009 Subtype: " + Subtype, Struct.ChatType.Talk));
                                        break;
                                    }
                                default:
                                    {
                                        CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Please report: Unknown 1009 Subtype: " + Subtype, Struct.ChatType.Talk));
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
                            Packet.Long(EudemonPacket.Timer);
                            Packet.Long(25200);
                            CSocket.Send(Packet.getFinal());
                            break;
                        }
                    #endregion
                    #region GameActions
                    case 2036:// excape key its seems
                        {
                            int Action = BitConverter.ToInt16(data, 4);
                            switch (Action)
                            {
                                case 473: //Game Exit
                                    {
                                        CSocket.Send(EudemonPacket.ExitPacket());
                                        break;
                                    }
                                case 515:
                                case 518:
                                    {
                                        bool Upgraded = false;

                                        int ItemId = BitConverter.ToInt32(data, 8);
                                        int GemID = BitConverter.ToInt32(data, 12);
                                        if (CSocket.Client.Inventory.ContainsKey(GemID) && CSocket.Client.Inventory.ContainsKey(ItemId))
                                        {
                                            Struct.ItemInfo Gem = CSocket.Client.Inventory[GemID];
                                            Struct.ItemInfo Item = CSocket.Client.Inventory[ItemId];
                                                
                                            List<int> PossibleGems = new List<int>(new int[]{ 1037150, 1037159 });
                                            //if (Gem.ItemID != 1037150 || Gem.ItemID != 1037159)
                                            if(!PossibleGems.Contains(Gem.ItemID))
                                            {
                                                CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "You Need to use a Yellow Stone Or A Super Yellow Stone!", Struct.ChatType.Talk));
                                                //@TODO: Log Uses of Non YellowStones
                                                break; //Gem isnt a YellowStone!
                                            }
                                            if (Item.Plus >= 9 && Gem.ItemID == 1037159)
                                            {
                                                CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "You Need to use a Yellow Stone for bonuses above 9!", Struct.ChatType.Talk));
                                                break; //cant be a super yellow!
                                            }

                                            Struct.ItemData iData = MainGS.Items[Item.ItemID];

                                            double LessChance = iData.Level / 3;

                                            switch (Item.Plus)
                                            {
                                                case 0:
                                                    {
                                                        Upgraded = Calculation.PercentSuccess(90 - LessChance);
                                                        break;
                                                    }
                                                case 1:
                                                case 2:
                                                case 3:
                                                    {
                                                        Upgraded = Calculation.PercentSuccess(85 - LessChance);
                                                        break;
                                                    }
                                                case 4:
                                                    {
                                                        Upgraded = Calculation.PercentSuccess(80 - LessChance);
                                                        break;
                                                    }
                                                case 5:
                                                case 6:
                                                    {
                                                        Upgraded = Calculation.PercentSuccess(75 - LessChance);
                                                        break;
                                                    }
                                                case 7:
                                                case 8:
                                                    {
                                                        Upgraded = Calculation.PercentSuccess(70 - LessChance);
                                                        break;
                                                    }
                                                case 9:
                                                    {
                                                        Upgraded = Calculation.PercentSuccess(60 - LessChance);
                                                        break;
                                                    }
                                                case 10:
                                                case 11:
                                                    {
                                                        Upgraded = Calculation.PercentSuccess(50 - LessChance);
                                                        break;
                                                    }
                                                case 12:
                                                    {
                                                        Upgraded = Calculation.PercentSuccess(45 - LessChance);
                                                        break;
                                                    }
                                            }

                                            if (Gem.ItemID == 1037159)
                                                Upgraded = true;
                                            

                                            if (Upgraded)
                                            {
                                                Item.Plus += 1;
                                            }
                                            else
                                            {
                                                if (Item.Plus > 0)
                                                {
                                                    Item.Plus -= 1;
                                                }
                                            }

                                            CSocket.Send(EudemonPacket.ItemUsage(GemID, 0, Struct.ItemUsage.RemoveDropItem));
                                            Database.Database.DeleteItem(GemID);
                                            Database.Database.UpdateItem(Item);
                                            CSocket.Send(EudemonPacket.ItemInfo(Item.UID, Item.ItemID, Item.Plus, Item.Soc1, Item.Soc2, Item.Dura, Item.MaxDura, Item.Position, 0, 0, 0));
                                        }
                                        else
                                        {
                                            if (CSocket.Client.Inventory.ContainsKey(GemID))
                                                CSocket.Send(EudemonPacket.Chat(0, "ItemUpgrader", CSocket.Client.Name, "[ERROR] Gem Doesnt exist", Struct.ChatType.Talk));
                                            if (CSocket.Client.Inventory.ContainsKey(ItemId))
                                                CSocket.Send(EudemonPacket.Chat(0, "ItemUpgrader", CSocket.Client.Name, "[ERROR] Item Doesnt exist", Struct.ChatType.Talk));
                                        }

                                        break;
                                    }

                                default:
                                    {
                                        CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Please report: Unknown 2036 Actiontype: " + Action, Struct.ChatType.Talk));
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
                                        CSocket.Send(EudemonPacket.Dialog(898));
                                        break;
                                    }
                                case 29:
                                    {
                                        CSocket.Send(EudemonPacket.Dialog(473));
                                        break;
                                    }
                                default:
                                    {
                                        CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Please report: Unknown 1032 Subtype: " + Subtype, Struct.ChatType.Talk));
                                        break;
                                    }

                            }
                            //CSocket.Send(EudemonPacket.ExitPacket());
                            break;
                        }

                    #endregion
                    #region WalkRun Packet (3005)
                    case 3005: //WalkRun packet
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

                            int ucDir = BitConverter.ToInt16(data, 16);

                            Handler.Walk(ucDir, CSocket);
                            break;
                        }
                    #endregion
                    #region Tick Packet (1012)
                    case 1012:
                        {
                            //@TODO: actually process and get the 1012
                            break;
                        }
                    #endregion

                    #region Multi-Function Packet 0x3f2(1010)
                    case 1010: // 0x3f2, Multi-Function Packet
                        {
                            int SubType = BitConverter.ToInt16(data, 24) - 9527;
                            ushort Dir = BitConverter.ToUInt16(data, 16);
                            int idTarget = BitConverter.ToInt32(data, 20);
                            uint dwData = BitConverter.ToUInt32(data, 20);

                            switch ((Struct.DataType)SubType)
                            {
                                case Struct.DataType.actionEnterMap: //Start login sequence. //actionEnterMap //14
                                    {
                                        Console.WriteLine("[GameServer] "+ CSocket.Client.Name + " Is Entering the Map " + CSocket.Client.Map.ToString());
                                        CSocket.Send(EudemonPacket.GeneralOld(1, (int)CSocket.Client.Map, CSocket.Client.X, CSocket.Client.Y, 0, (int)CSocket.Client.Map, Structs.Struct.DataType.actionEnterMap));
                                        CSocket.Send(EudemonPacket.NewMap((int)CSocket.Client.Map, 2097152, (int)CSocket.Client.Map)); //@TODO: Check for Instancing and Load map type from database.
                                        EudemonPacket.ToLocal(EudemonPacket.SpawnCharacter(CSocket), CSocket.Client.X, CSocket.Client.Y, (int)CSocket.Client.Map, 0, CSocket.Client.ID);
                                        Spawn.All(CSocket);
                                        
                                        break;
                                    }
                                case Struct.DataType.actionQueryPlayer:
                                    {
                                        ClientSocket targetCS = null;
                                        try
                                        {
                                            Monitor.Enter(MainGS.ClientPool);
                                            foreach (KeyValuePair<int, ClientSocket> Tests in MainGS.ClientPool)
                                            {
                                                ClientSocket C = Tests.Value;
                                                if (C.Client.ID == idTarget)
                                                    targetCS = C;
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
                                        //@TODO: Move this to dedicated function.
                                        if (targetCS != null)
                                        {
                                            if (Calculation.GetDistance(CSocket.Client.X, CSocket.Client.Y, targetCS.Client.X, targetCS.Client.Y) <= 18) //CELLS_PER_VIEW
                                            {
                                                CSocket.Send(EudemonPacket.SpawnCharacter(targetCS));
                                                CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[Info] Querying Player.", Struct.ChatType.System));//@TODO: remove debug message.
                                            }
                                            else
                                            {
                                                CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Too Far from Player.", Struct.ChatType.System));//@TODO: remove debug message.
                                            }
                                        }
                                        else
                                        {
                                            CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Invalid Player ID.", Struct.ChatType.System));
                                        }
                                        break;
                                    }

                                case Struct.DataType.actionChgDir:
                                    {
                                        CSocket.Client.Direction = Dir;
                                        EudemonPacket.ToLocal(EudemonPacket.General(CSocket.Client.ID, CSocket.Client.X, CSocket.Client.Y, CSocket.Client.Direction, Struct.DataType.actionChgDir, 0), CSocket.Client.X, CSocket.Client.Y, (int)CSocket.Client.Map, 0, CSocket.Client.ID);
                                        break;
                                    }
                                case Struct.DataType.actionChgMap:
                                    {
                                        if (CSocket.Client.Dead)
                                        {
                                            //@TODO: handle making sure dead is dead.
                                            break;
                                        }
                                        //@TODO: Make sure Player has stopped all actions.

                                        break;
                                    }
                                case Struct.DataType.actionGetItemSet: //actionGetItemSet //15
                                    {
                                        foreach (KeyValuePair<int, Struct.ItemInfo> Item in CSocket.Client.Inventory)
                                        {
                                            CSocket.Send(EudemonPacket.ItemInfo(Item.Value.UID, Item.Value.ItemID, Item.Value.Plus, Item.Value.Soc1, Item.Value.Soc2, Item.Value.Dura, Item.Value.MaxDura, Item.Value.Position, 0, 0, 0));
                                        }
                                        foreach (KeyValuePair<int, Struct.ItemInfo> Item in CSocket.Client.Equipment)
                                        {
                                            CSocket.Send(EudemonPacket.ItemInfo(Item.Value.UID, Item.Value.ItemID, Item.Value.Plus, Item.Value.Soc1, Item.Value.Soc2, Item.Value.Dura, Item.Value.MaxDura, Item.Value.Position, 0, 0, 0));
                                        }
                                        
                                        CSocket.Send(EudemonPacket.General(0, 0, 0, 0, Struct.DataType.actionGetItemSet, 0)); //@NOTE: DO NOT SEND CLIENT ID HERE. - @TODO: Work out why this causes odd issues.
                                        CSocket.Send(EudemonPacket.GeneralOld(0, CSocket.Client.ID, 0, 0, 0, 1, Struct.DataType.actionSetPkMode)); //Make sure Client is in sync.
                                       break;                                       
                                    }
                                case Struct.DataType.actionGetGoodFriend:
                                    {
                                        CSocket.Send(EudemonPacket.General(CSocket.Client.ID, 0, 0, 0, Struct.DataType.actionGetGoodFriend, 0));
                                        //@TODO: actually handle this.
                                        break;
                                    }
                                case Struct.DataType.actionGetWeaponSkillSet:
                                    {
                                        CSocket.Send(EudemonPacket.General(CSocket.Client.ID, 0, 0, 0, Struct.DataType.actionGetWeaponSkillSet, 0));
                                        //@TODO: actually handle this.
                                        break;
                                    }
                                case Struct.DataType.actionSetPkMode: //PK Mode //29
                                    {
                                        uint PkType = dwData;
                                        if (PkType > 4)
                                            break; //spot of error checking
                                        CSocket.Client.PKMode = (Struct.PkType)PkType;
                                        CSocket.Send(EudemonPacket.GeneralOld(0, CSocket.Client.ID, 0, 0, 0, (int)PkType, Struct.DataType.actionSetPkMode));
                                        CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "PK Mode Changed to " + CSocket.Client.PKMode.ToString() + " Mode.", Struct.ChatType.System));
                                        break;
                                    }
   
                                default:
                                    {
                                        CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[Handler-Error] Please report: Unable to handle packet 0x3F2 Sub type " + SubType, Struct.ChatType.System));
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
                            data[4] = (byte)(data[4] ^ 0x37);//removing TQs half assed encryption.
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
                                            CSocket.Send(EudemonPacket.Team(CSocket.Client.ID, Struct.TeamOption.MakeTeam));
                                        }
                                        else
                                        {
                                            CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You are already in a team!", Struct.ChatType.System));
                                        }
                                        break;
                                    }
                                case 1://Request to join
                                    {
                                        if (CSocket.Client.Team != null)
                                        {
                                            CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You are already in a team!", Struct.ChatType.System));
                                            break;
                                        }
                                        int Leader = ReadLong(data, 8);
                                        if (MainGS.ClientPool.ContainsKey(Leader))
                                        {
                                            ClientSocket TeamLeader = MainGS.ClientPool[Leader];
                                            if (TeamLeader.Client.Team != null)
                                            {
                                                if (TeamLeader.Client.Team.LeaderID == TeamLeader.Client.ID)
                                                {
                                                    if (!TeamLeader.Client.Team.Forbid)
                                                    {
                                                        if (TeamLeader.Client.Team.Members.Count < 5)
                                                        {
                                                            TeamLeader.Send(EudemonPacket.Team(CSocket.Client.ID, Struct.TeamOption.JoinTeam));
                                                            CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[Team] Request to join team sent to " + TeamLeader.Client.Name, Struct.ChatType.System));
                                                        }
                                                        else
                                                        {
                                                            CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] " + TeamLeader.Client.Name + "'s team is full.", Struct.ChatType.System));
                                                        }
                                                    }
                                                    else
                                                    {
                                                        CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] " + TeamLeader.Client.Name + "'s team forbids new members.", Struct.ChatType.System));
                                                    }
                                                }
                                                else
                                                {
                                                    CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] " + TeamLeader.Client.Name + " is not the team leader.", Struct.ChatType.System));
                                                }
                                            }
                                            else
                                            {
                                                CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] " + TeamLeader.Client.Name + " has not created a team.", Struct.ChatType.System));
                                            }
                                        }
                                        break;
                                    }
                                case 2://Exit Team
                                    {
                                        if (CSocket.Client.Team != null)
                                        {
                                            ClientSocket Leader = MainGS.ClientPool[CSocket.Client.Team.LeaderID];
                                            foreach (KeyValuePair<int, ClientSocket> Member in Leader.Client.Team.Members)
                                            {
                                                if (Member.Value.Client.ID != CSocket.Client.ID)
                                                {
                                                    Member.Value.Send(EudemonPacket.Chat(0, "SYSTEM", Member.Value.Client.Name, "[Team] " + CSocket.Client.Name + " has just left the team!", Struct.ChatType.System));
                                                    Member.Value.Send(EudemonPacket.Team(CSocket.Client.ID, Struct.TeamOption.LeaveTeam));
                                                }
                                            }
                                            Leader.Client.Team.Members.Remove(CSocket.Client.ID);
                                            CSocket.Client.Team = null;
                                            //CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[Team] You have left the team.", Struct.ChatType.Top));
                                            CSocket.Send(EudemonPacket.Team(CSocket.Client.ID, Struct.TeamOption.LeaveTeam));
                                        }
                                        else
                                        {
                                            CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You are not in a team!", Struct.ChatType.System));
                                        }
                                        break;
                                    }
                                case 3: //Accept Invite
                                    {
                                        if (CSocket.Client.Team != null)
                                        {
                                            CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You are already in a team!", Struct.ChatType.System));
                                            break;
                                        }
                                        int Inviter = ReadLong(data, 8);
                                        if (MainGS.ClientPool.ContainsKey(Inviter))
                                        {
                                            ClientSocket TeamLeader = MainGS.ClientPool[Inviter];
                                            if (TeamLeader.Client.Team != null)
                                            {
                                                if (TeamLeader.Client.Team.Members.Count < 5)
                                                {
                                                    if (!TeamLeader.Client.Team.Forbid)
                                                    {
                                                        foreach (KeyValuePair<int, ClientSocket> Member in TeamLeader.Client.Team.Members)
                                                        {
                                                            Member.Value.Send(EudemonPacket.TeamMember(CSocket.Client));
                                                            CSocket.Send(EudemonPacket.TeamMember(Member.Value.Client));
                                                        }
                                                        TeamLeader.Client.Team.Members.Add(CSocket.Client.ID, CSocket);
                                                        CSocket.Client.Team = TeamLeader.Client.Team;
                                                    }
                                                    else
                                                    {
                                                        CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Inviter's team does not accept new members.", Struct.ChatType.System));
                                                    }
                                                }
                                                else
                                                {
                                                    CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Inviter's team is full.", Struct.ChatType.System));
                                                }
                                            }
                                            else
                                            {
                                                CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Inviter no longer has a team.", Struct.ChatType.System));
                                            }
                                        }
                                        else
                                        {
                                            CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Inviter is no longer online.", Struct.ChatType.System));
                                        }
                                        break;
                                    }
                                case 4: //Invite to join
                                    {
                                        if (CSocket.Client.Team == null)
                                        {
                                            CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You are not in a team!", Struct.ChatType.System));
                                            break;
                                        }
                                        else
                                        {
                                            if (CSocket.Client.Team.LeaderID != CSocket.Client.ID)
                                            {
                                                CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You are not the leader and cannot invite new members!", Struct.ChatType.System));
                                                break;
                                            }
                                        }
                                        int Invited = ReadLong(data, 8);
                                        if (MainGS.ClientPool.ContainsKey(Invited))
                                        {
                                            ClientSocket InvitedCSocket = MainGS.ClientPool[Invited];
                                            if (InvitedCSocket.Client.Team == null)
                                            {
                                                if (!CSocket.Client.Team.Forbid)
                                                {
                                                    CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[Team] Invited " + InvitedCSocket.Client.Name + " to join your team.", Struct.ChatType.System));
                                                    InvitedCSocket.Send(EudemonPacket.Team(CSocket.Client.ID, Struct.TeamOption.Invite));
                                                }
                                                else
                                                {
                                                    CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Your team forbids new members from joining.", Struct.ChatType.System));
                                                }
                                            }
                                            else
                                            {
                                                CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Target is already in a team.", Struct.ChatType.System));
                                            }
                                        }
                                        else
                                        {
                                            CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Target does not exist.", Struct.ChatType.System));
                                        }
                                        break;
                                    }
                                case 5://Accept join request
                                    {
                                        if (CSocket.Client.Team == null)
                                        {
                                            CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You are not in a team!", Struct.ChatType.System));
                                            break;
                                        }
                                        else
                                        {
                                            if (CSocket.Client.Team.LeaderID != CSocket.Client.ID)
                                            {
                                                CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You are not the leader and cannot accept join requests.", Struct.ChatType.System));
                                                break;
                                            }
                                        }
                                        int ToJoin = ReadLong(data, 8);
                                        if (MainGS.ClientPool.ContainsKey(ToJoin))
                                        {
                                            ClientSocket Invited = MainGS.ClientPool[ToJoin];
                                            if (Invited.Client.Team == null)
                                            {
                                                if (!CSocket.Client.Team.Forbid)
                                                {
                                                    foreach (KeyValuePair<int, ClientSocket> Member in CSocket.Client.Team.Members)
                                                    {
                                                        Member.Value.Send(EudemonPacket.TeamMember(Invited.Client));
                                                        Invited.Send(EudemonPacket.TeamMember(Member.Value.Client));
                                                    }
                                                    CSocket.Client.Team.Members.Add(Invited.Client.ID, Invited);
                                                    Invited.Client.Team = CSocket.Client.Team;
                                                }
                                                else
                                                {
                                                    CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Your team forbids new members from joining.", Struct.ChatType.System));
                                                }
                                            }
                                            else
                                            {
                                                CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Target is already in a team.", Struct.ChatType.System));
                                            }
                                        }
                                        else
                                        {
                                            CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Target does not exist.", Struct.ChatType.System));
                                        }
                                        break;
                                    }
                                case 6: //Dismiss
                                    {
                                        if (CSocket.Client.Team == null)
                                        {
                                            CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You are not in a team!", Struct.ChatType.System));
                                            break;
                                        }
                                        else
                                        {
                                            if (CSocket.Client.Team.LeaderID != CSocket.Client.ID)
                                            {
                                                CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You are not the leader and cannot dismiss the team.", Struct.ChatType.System));
                                                break;
                                            }
                                        }
                                        foreach (KeyValuePair<int, ClientSocket> Member in CSocket.Client.Team.Members)
                                        {
                                            if (Member.Value.Client.ID != CSocket.Client.ID)
                                            {
                                                Member.Value.Send(EudemonPacket.Team(Member.Value.Client.ID, Struct.TeamOption.DismissTeam));
                                                Member.Value.Client.Team = null;
                                            }
                                        }
                                        CSocket.Client.Team = null;
                                        CSocket.Send(EudemonPacket.Team(CSocket.Client.ID, Struct.TeamOption.DismissTeam));
                                        break;
                                    }
                                case 7: //Kick from team
                                    {
                                        if (CSocket.Client.Team == null)
                                        {
                                            CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You are not in a team!", Struct.ChatType.System));
                                            break;
                                        }
                                        else
                                        {
                                            if (CSocket.Client.Team.LeaderID != CSocket.Client.ID)
                                            {
                                                CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You are not the leader and cannot kick team members.", Struct.ChatType.System));
                                                break;
                                            }
                                        }
                                        int Kick = ReadLong(data, 8);
                                        if (MainGS.ClientPool.ContainsKey(Kick))
                                        {
                                            if (CSocket.Client.Team.Members.ContainsKey(Kick))
                                            {
                                                foreach (KeyValuePair<int, ClientSocket> Member in CSocket.Client.Team.Members)
                                                {
                                                    Member.Value.Send(EudemonPacket.Team(Kick, Struct.TeamOption.Kick));
                                                    if (Member.Value.Client.ID == Kick)
                                                    {
                                                        Member.Value.Client.Team = null;
                                                    }
                                                }
                                                CSocket.Client.Team.Members.Remove(Kick);
                                            }
                                            else
                                            {
                                                CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Target not in team.", Struct.ChatType.System));
                                            }
                                        }
                                        else
                                        {
                                            CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Target does not exist.", Struct.ChatType.System));
                                        }
                                        break;
                                    }
                                case 8://Forbid
                                    {
                                        if (CSocket.Client.Team == null)
                                        {
                                            CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You are not in a team!", Struct.ChatType.System));
                                            break;
                                        }
                                        else
                                        {
                                            if (CSocket.Client.Team.LeaderID != CSocket.Client.ID)
                                            {
                                                CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You are not the leader and cannot forbid new joins!", Struct.ChatType.System));
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
                                            CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You are not in a team!", Struct.ChatType.System));
                                            break;
                                        }
                                        else
                                        {
                                            if (CSocket.Client.Team.LeaderID != CSocket.Client.ID)
                                            {
                                                CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You are not the leader and cannot forbid new joins!", Struct.ChatType.System));
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
                                            CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You are not in a team!", Struct.ChatType.System));
                                            break;
                                        }
                                        else
                                        {
                                            if (CSocket.Client.Team.LeaderID != CSocket.Client.ID)
                                            {
                                                CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You are not the leader and cannot forbid new joins!", Struct.ChatType.System));
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
                                            CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You are not in a team!", Struct.ChatType.System));
                                            break;
                                        }
                                        else
                                        {
                                            if (CSocket.Client.Team.LeaderID != CSocket.Client.ID)
                                            {
                                                CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You are not the leader and cannot forbid new joins!", Struct.ChatType.System));
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
                                            CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You are not in a team!", Struct.ChatType.System));
                                            break;
                                        }
                                        else
                                        {
                                            if (CSocket.Client.Team.LeaderID != CSocket.Client.ID)
                                            {
                                                CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You are not the leader and cannot forbid new joins!", Struct.ChatType.System));
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
                                            CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You are not in a team!", Struct.ChatType.System));
                                            break;
                                        }
                                        else
                                        {
                                            if (CSocket.Client.Team.LeaderID != CSocket.Client.ID)
                                            {
                                                CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You are not the leader and cannot forbid new joins!", Struct.ChatType.System));
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
                                        CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Please report: Unknown team subtype: " + subtype, Struct.ChatType.System));
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
                                        CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[Handler-Error] Please report: Unable to handle 1002 subtype " + AType, Struct.ChatType.System));
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
                            CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[Handler-Error] Please report: Unable to handle packet type " + Type, Struct.ChatType.System));
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
