using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using GameServer.Connections;
using GameServer.Entities;
using GameServer.Structs;
using GameServer.Packets;
using GameServer.Calculations;

namespace GameServer.Handlers
{
    /// <summary>
    /// Description of Walk.
    /// </summary>
    public partial class Handler
    {
        public static void Walk(int Direction, ClientSocket CSocket)
        {
            int DX = 0;
            int DY = 0;
            switch (Direction)
            {
                case 0:
                    {
                        DY = 1;
                        break;
                    }
                case 1:
                    {
                        DX = -1;
                        DY = 1;
                        break;
                    }
                case 2:
                    {
                        DX = -1;
                        break;
                    }
                case 3:
                    {
                        DX = -1;
                        DY = -1;
                        break;
                    }
                case 4:
                    {
                        DY = -1;
                        break;
                    }
                case 5:
                    {
                        DX = 1;
                        DY = -1;
                        break;
                    }
                case 6:
                    {
                        DX = 1;
                        break;
                    }
                case 7:
                    {
                        DY = 1;
                        DX = 1;
                        break;
                    }
                default:
                    {
                        //Unknown Walking direction.
                        CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Unknown walking direction.", Struct.ChatType.Talk));
                        CSocket.Disconnect();
                        break;
                    }
            }
            if (Nano.Maps.ContainsKey((int)CSocket.Client.Map))
            {
                Struct.DmapData Map = Nano.Maps[(int)CSocket.Client.Map];
                //if(!Map.Tiles.ContainsValue(new Struct.DmapTile((ushort)(CSocket.Client.X+DX), (ushort)(CSocket.Client.Y+DY))))
                if (!Map.CheckLoc((ushort)(CSocket.Client.X), (ushort)(CSocket.Client.Y)))
                {
                    CSocket.Send(ConquerPacket.General(CSocket.Client.ID, 0, 0, CSocket.Client.X, CSocket.Client.Y, 0, Struct.DataType.CorrectCords));
                    CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Invalid coordinates.", Struct.ChatType.Top));
                    return;
                }
            }
            CSocket.Client.PrevX = CSocket.Client.X;
            CSocket.Client.PrevY = CSocket.Client.Y;
            //TODO: Map / Guild wall / other checks
            byte[] WalkPacket = ConquerPacket.Walk(Direction, CSocket.Client.ID, CSocket.Client.X,CSocket.Client.Y);
            //lock(Nano.ClientPool)
            //{
            try
            {
                Monitor.Enter(Nano.ClientPool);
                foreach (KeyValuePair<int, ClientSocket> Tests in Nano.ClientPool)
                {
                    ClientSocket C = Tests.Value;
                    if ((int)C.Client.Map == (int)CSocket.Client.Map)
                    {
                        bool seen = Calculation.CanSee(CSocket.Client.PrevX, CSocket.Client.PrevY, C.Client.X, C.Client.Y);
                        //C.Send(ConquerPacket.SpawnCharacter(CSocket)); //testing
                        //Console.WriteLine("seen: " + seen.ToString());
                         if (!seen)
                        {
                            //CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "Char was false", Struct.ChatType.Talk));
                            if (Calculation.CanSee(CSocket.Client.X, CSocket.Client.Y, C.Client.X, C.Client.Y))
                            {
                                C.Send(ConquerPacket.SpawnCharacter(CSocket));
                                //CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "CharSpawn Packet sent!!!",Struct.ChatType.Talk));
                            }
                        }
                        else
                        {
                            if (Calculation.CanSee(CSocket.Client.X, CSocket.Client.Y, C.Client.X, C.Client.Y))
                                C.Send(WalkPacket);
                            else
                                C.Send(ConquerPacket.General(CSocket.Client.ID, CSocket.Client.PrevX, CSocket.Client.PrevY, 0, 0, 0, Struct.DataType.EntityRemove));
                        }
                    }
                }
                //}
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
            Spawn.All(CSocket);
        }
    }
}
