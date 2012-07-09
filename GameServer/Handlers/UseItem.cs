using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using GameServer.Connections;
using GameServer.Entities;
using GameServer.Structs;
using GameServer.Packets;
using GameServer.Calculations;
using GameServer.Database;

namespace GameServer.Handlers
{
    /// <summary>
    /// Handles the use of all non-equipment items
    /// </summary>
    public partial class Handler
    {
        public static void UseItem(Struct.ItemInfo Item, ClientSocket CSocket)
        {
            bool Delete = true;
            switch (Item.ItemID)
            {
                case 1000000://Stancher
                    {
                        int AddHP = 70;
                        if (CSocket.Client.CurrentHP >= CSocket.Client.MaxHP)
                        {
                            Delete = false;
                            break;
                        }
                        else
                        {
                            if (CSocket.Client.CurrentHP + AddHP > CSocket.Client.MaxHP)
                            {
                                Interlocked.Add(ref CSocket.Client.CurrentHP, (CSocket.Client.MaxHP - CSocket.Client.CurrentHP));
                                CSocket.Send(ConquerPacket.Status(CSocket, 2, CSocket.Client.CurrentHP, Struct.StatusTypes.Hp));
                            }
                            else
                            {
                                //Interlocked.Add(ref CSocket.Client.CurrentHP, AddHP);
                                Interlocked.Add(ref CSocket.Client.CurrentHP, AddHP);
                                CSocket.Send(ConquerPacket.Status(CSocket, 2, CSocket.Client.CurrentHP, Struct.StatusTypes.Hp));
                            }
                        }
                        break;
                    }
                case 1000010://Stancher
                    {
                        int AddHP = 100;
                        if (CSocket.Client.CurrentHP >= CSocket.Client.MaxHP)
                        {
                            Delete = false;
                            break;
                        }
                        else
                        {
                            if (CSocket.Client.CurrentHP + AddHP > CSocket.Client.MaxHP)
                            {

                                Interlocked.Add(ref CSocket.Client.CurrentHP, (CSocket.Client.MaxHP - CSocket.Client.CurrentHP));
                                CSocket.Send(ConquerPacket.Status(CSocket, 2, CSocket.Client.CurrentHP, Struct.StatusTypes.Hp));
                            }
                            else
                            {
                                Interlocked.Add(ref CSocket.Client.CurrentHP, AddHP);
                                CSocket.Send(ConquerPacket.Status(CSocket, 2, CSocket.Client.CurrentHP, Struct.StatusTypes.Hp));
                            }
                        }
                        break;
                    }
                case 1000020://Stancher
                    {
                        int AddHP = 250;
                        if (CSocket.Client.CurrentHP >= CSocket.Client.MaxHP)
                        {
                            Delete = false;
                            break;
                        }
                        else
                        {
                            if (CSocket.Client.CurrentHP + AddHP > CSocket.Client.MaxHP)
                            {
                                Interlocked.Add(ref CSocket.Client.CurrentHP, (CSocket.Client.MaxHP - CSocket.Client.CurrentHP));
                                CSocket.Send(ConquerPacket.Status(CSocket, 2, CSocket.Client.CurrentHP, Struct.StatusTypes.Hp));
                            }
                            else
                            {
                                Interlocked.Add(ref CSocket.Client.CurrentHP, AddHP);
                                CSocket.Send(ConquerPacket.Status(CSocket, 2, CSocket.Client.CurrentHP, Struct.StatusTypes.Hp));
                            }
                        }
                        break;
                    }
                case 1000030://Stancher
                    {
                        int AddHP = 500;
                        if (CSocket.Client.CurrentHP >= CSocket.Client.MaxHP)
                        {
                            Delete = false;
                            break;
                        }
                        else
                        {
                            if (CSocket.Client.CurrentHP + AddHP > CSocket.Client.MaxHP)
                            {
                                Interlocked.Add(ref CSocket.Client.CurrentHP, (CSocket.Client.MaxHP - CSocket.Client.CurrentHP));
                                CSocket.Send(ConquerPacket.Status(CSocket, 2, CSocket.Client.CurrentHP, Struct.StatusTypes.Hp));
                            }
                            else
                            {
                                Interlocked.Add(ref CSocket.Client.CurrentHP, AddHP);
                                CSocket.Send(ConquerPacket.Status(CSocket, 2, CSocket.Client.CurrentHP, Struct.StatusTypes.Hp));
                            }
                        }
                        break;
                    }
                case 1002000://Stancher
                    {
                        int AddHP = 800;
                        if (CSocket.Client.CurrentHP >= CSocket.Client.MaxHP)
                        {
                            Delete = false;
                            break;
                        }
                        else
                        {
                            if (CSocket.Client.CurrentHP + AddHP > CSocket.Client.MaxHP)
                            {
                                Interlocked.Add(ref CSocket.Client.CurrentHP, (CSocket.Client.MaxHP - CSocket.Client.CurrentHP));
                                CSocket.Send(ConquerPacket.Status(CSocket, 2, CSocket.Client.CurrentHP, Struct.StatusTypes.Hp));
                            }
                            else
                            {
                                Interlocked.Add(ref CSocket.Client.CurrentHP, AddHP);
                                CSocket.Send(ConquerPacket.Status(CSocket, 2, CSocket.Client.CurrentHP, Struct.StatusTypes.Hp));
                            }
                        }
                        break;
                    }
                case 1002010://Stancher
                    {
                        int AddHP = 1200;
                        if (CSocket.Client.CurrentHP >= CSocket.Client.MaxHP)
                        {
                            Delete = false;
                            break;
                        }
                        else
                        {
                            if (CSocket.Client.CurrentHP + AddHP > CSocket.Client.MaxHP)
                            {
                                Interlocked.Add(ref CSocket.Client.CurrentHP, (CSocket.Client.MaxHP - CSocket.Client.CurrentHP));
                                CSocket.Send(ConquerPacket.Status(CSocket, 2, CSocket.Client.CurrentHP, Struct.StatusTypes.Hp));
                            }
                            else
                            {
                                Interlocked.Add(ref CSocket.Client.CurrentHP, AddHP);
                                CSocket.Send(ConquerPacket.Status(CSocket, 2, CSocket.Client.CurrentHP, Struct.StatusTypes.Hp));
                            }
                        }
                        break;
                    }
                case 1002020://Stancher
                    {
                        int AddHP = 2000;
                        if (CSocket.Client.CurrentHP >= CSocket.Client.MaxHP)
                        {
                            Delete = false;
                            break;
                        }
                        else
                        {
                            if (CSocket.Client.CurrentHP + AddHP > CSocket.Client.MaxHP)
                            {
                                Interlocked.Add(ref CSocket.Client.CurrentHP, (CSocket.Client.MaxHP - CSocket.Client.CurrentHP));
                                CSocket.Send(ConquerPacket.Status(CSocket, 2, CSocket.Client.CurrentHP, Struct.StatusTypes.Hp));
                            }
                            else
                            {
                                Interlocked.Add(ref CSocket.Client.CurrentHP, AddHP);
                                CSocket.Send(ConquerPacket.Status(CSocket, 2, CSocket.Client.CurrentHP, Struct.StatusTypes.Hp));
                            }
                        }
                        break;
                    }
                case 1002050://Stancher
                    {
                        int AddHP = 3000;
                        if (CSocket.Client.CurrentHP >= CSocket.Client.MaxHP)
                        {
                            Delete = false;
                            break;
                        }
                        else
                        {
                            if (CSocket.Client.CurrentHP + AddHP > CSocket.Client.MaxHP)
                            {
                                Interlocked.Add(ref CSocket.Client.CurrentHP, (CSocket.Client.MaxHP - CSocket.Client.CurrentHP));
                                CSocket.Send(ConquerPacket.Status(CSocket, 2, CSocket.Client.CurrentHP, Struct.StatusTypes.Hp));
                            }
                            else
                            {
                                Interlocked.Add(ref CSocket.Client.CurrentHP, AddHP);
                                CSocket.Send(ConquerPacket.Status(CSocket, 2, CSocket.Client.CurrentHP, Struct.StatusTypes.Hp));
                            }
                        }
                        break;
                    }
                case 1001000://Stancher
                    {
                        int AddMP = 70;
                        if (CSocket.Client.CurrentMP >= CSocket.Client.MaxMP)
                        {
                            Delete = false;
                            break;
                        }
                        else
                        {
                            if (CSocket.Client.CurrentMP + AddMP > CSocket.Client.MaxMP)
                            {
                                Interlocked.Add(ref CSocket.Client.CurrentMP, (CSocket.Client.MaxMP - CSocket.Client.CurrentMP));
                                CSocket.Send(ConquerPacket.Status(CSocket, 2, CSocket.Client.CurrentMP, Struct.StatusTypes.Mp));
                            }
                            else
                            {
                                Interlocked.Add(ref CSocket.Client.CurrentMP, AddMP);
                                CSocket.Send(ConquerPacket.Status(CSocket, 2, CSocket.Client.CurrentMP, Struct.StatusTypes.Mp));
                            }
                        }
                        break;
                    }
                case 1001010://Stancher
                    {
                        int AddMP = 200;
                        if (CSocket.Client.CurrentMP >= CSocket.Client.MaxMP)
                        {
                            Delete = false;
                            break;
                        }
                        else
                        {
                            if (CSocket.Client.CurrentMP + AddMP > CSocket.Client.MaxMP)
                            {
                                Interlocked.Add(ref CSocket.Client.CurrentMP, (CSocket.Client.MaxMP - CSocket.Client.CurrentMP));
                                CSocket.Send(ConquerPacket.Status(CSocket, 2, CSocket.Client.CurrentMP, Struct.StatusTypes.Mp));
                            }
                            else
                            {
                                Interlocked.Add(ref CSocket.Client.CurrentMP, AddMP);
                                CSocket.Send(ConquerPacket.Status(CSocket, 2, CSocket.Client.CurrentMP, Struct.StatusTypes.Mp));
                            }
                        }
                        break;
                    }
                case 1001020://Stancher
                    {
                        int AddMP = 450;
                        if (CSocket.Client.CurrentMP >= CSocket.Client.MaxMP)
                        {
                            Delete = false;
                            break;
                        }
                        else
                        {
                            if (CSocket.Client.CurrentMP + AddMP > CSocket.Client.MaxMP)
                            {
                                Interlocked.Add(ref CSocket.Client.CurrentMP, (CSocket.Client.MaxMP - CSocket.Client.CurrentMP));
                                CSocket.Send(ConquerPacket.Status(CSocket, 2, CSocket.Client.CurrentMP, Struct.StatusTypes.Mp));
                            }
                            else
                            {
                                Interlocked.Add(ref CSocket.Client.CurrentMP, AddMP);
                                CSocket.Send(ConquerPacket.Status(CSocket, 2, CSocket.Client.CurrentMP, Struct.StatusTypes.Mp));
                            }
                        }
                        break;
                    }
                case 1001030://Stancher
                    {
                        int AddMP = 1000;
                        if (CSocket.Client.CurrentMP >= CSocket.Client.MaxMP)
                        {
                            Delete = false;
                            break;
                        }
                        else
                        {
                            if (CSocket.Client.CurrentMP + AddMP > CSocket.Client.MaxMP)
                            {
                                Interlocked.Add(ref CSocket.Client.CurrentMP, (CSocket.Client.MaxMP - CSocket.Client.CurrentMP));
                                CSocket.Send(ConquerPacket.Status(CSocket, 2, CSocket.Client.CurrentMP, Struct.StatusTypes.Mp));
                            }
                            else
                            {
                                Interlocked.Add(ref CSocket.Client.CurrentMP, AddMP);
                                CSocket.Send(ConquerPacket.Status(CSocket, 2, CSocket.Client.CurrentMP, Struct.StatusTypes.Mp));
                            }
                        }
                        break;
                    }
                case 1001040://Stancher
                    {
                        int AddMP = 2000;
                        if (CSocket.Client.CurrentMP >= CSocket.Client.MaxMP)
                        {
                            Delete = false;
                            break;
                        }
                        else
                        {
                            if (CSocket.Client.CurrentMP + AddMP > CSocket.Client.MaxMP)
                            {
                                Interlocked.Add(ref CSocket.Client.CurrentMP, (CSocket.Client.MaxMP - CSocket.Client.CurrentMP));
                                CSocket.Send(ConquerPacket.Status(CSocket, 2, CSocket.Client.CurrentMP, Struct.StatusTypes.Mp));
                            }
                            else
                            {
                                Interlocked.Add(ref CSocket.Client.CurrentMP, AddMP);
                                CSocket.Send(ConquerPacket.Status(CSocket, 2, CSocket.Client.CurrentMP, Struct.StatusTypes.Mp));
                            }
                        }
                        break;
                    }
                case 1002030://Stancher
                    {
                        int AddMP = 3000;
                        if (CSocket.Client.CurrentMP >= CSocket.Client.MaxMP)
                        {
                            Delete = false;
                            break;
                        }
                        else
                        {
                            if (CSocket.Client.CurrentMP + AddMP > CSocket.Client.MaxMP)
                            {
                                Interlocked.Add(ref CSocket.Client.CurrentMP, (CSocket.Client.MaxMP - CSocket.Client.CurrentMP));
                                CSocket.Send(ConquerPacket.Status(CSocket, 2, CSocket.Client.CurrentMP, Struct.StatusTypes.Mp));
                            }
                            else
                            {
                                Interlocked.Add(ref CSocket.Client.CurrentMP, AddMP);
                                CSocket.Send(ConquerPacket.Status(CSocket, 2, CSocket.Client.CurrentMP, Struct.StatusTypes.Mp));
                            }
                        }
                        break;
                    }
                case 1002040://Stancher
                    {
                        int AddMP = 4500;
                        if (CSocket.Client.CurrentMP >= CSocket.Client.MaxMP)
                        {
                            Delete = false;
                            break;
                        }
                        else
                        {
                            if (CSocket.Client.CurrentMP + AddMP > CSocket.Client.MaxMP)
                            {
                                Interlocked.Add(ref CSocket.Client.CurrentMP, (CSocket.Client.MaxMP - CSocket.Client.CurrentMP));
                                CSocket.Send(ConquerPacket.Status(CSocket, 2, CSocket.Client.CurrentMP, Struct.StatusTypes.Mp));
                            }
                            else
                            {
                                Interlocked.Add(ref CSocket.Client.CurrentMP, AddMP);
                                CSocket.Send(ConquerPacket.Status(CSocket, 2, CSocket.Client.CurrentMP, Struct.StatusTypes.Mp));
                            }
                        }
                        break;
                    }
                default:
                    {
                        CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Please report: Unable to handle item ID: " + Item.ItemID, Struct.ChatType.System));
                        Delete = false;
                        break;
                    }
            }
            if (Delete)
            {
                CSocket.Client.Inventory.Remove(Item.UID);
                CSocket.Send(ConquerPacket.ItemUsage(Item.UID, 255, Struct.ItemUsage.RemoveDropItem));
                Database.Database.DeleteItem(Item.UID);
            }
        }
    }
}
