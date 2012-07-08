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
    /// Method for Jumping
    /// </summary>
    public partial class Handler
    {
        public static void Jump(int X, int Y, ClientSocket CSocket)
        {
            //Check if jump distance is too large!
            int Dis1 = 0;
            if (X < CSocket.Client.X)
                Dis1 = Math.Abs(X - CSocket.Client.X);
            else
                Dis1 = Math.Abs(CSocket.Client.X - X);
            int Dis2 = 0;
            if (Y < CSocket.Client.Y)
                Dis2 = Math.Abs(Y - CSocket.Client.Y);
            else
                Dis2 = Math.Abs(CSocket.Client.Y - Y);
            if (Dis1 > 18 || Dis2 > 18)
            {
                CSocket.Send(ConquerPacket.General(CSocket.Client.ID, 0, 0, CSocket.Client.X, CSocket.Client.Y, 0, Struct.DataType.CorrectCords));
                CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Jump too large!", Struct.ChatType.System));
                return;
            }
            if (Nano.Maps.ContainsKey((int)CSocket.Client.Map))
            {
                Struct.DmapData Map = Nano.Maps[(int)CSocket.Client.Map];
                if (!Map.CheckLoc((ushort)X, (ushort)Y))
                {
                    CSocket.Send(ConquerPacket.General(CSocket.Client.ID, 0, 0, CSocket.Client.X, CSocket.Client.Y, 0, Struct.DataType.CorrectCords));
                    CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Invalid coordinates.", Struct.ChatType.System));
                    return;
                }
            }
            else
            {
                CSocket.Send(ConquerPacket.General(CSocket.Client.ID, 0, 0, CSocket.Client.X, CSocket.Client.Y, 0, Struct.DataType.CorrectCords));
                CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Please report: Map[" + (int)CSocket.Client.Map + "] not loaded.", Struct.ChatType.System));
                return;
            }
            //TODO: Map / Guild wall / other checks
            CSocket.Client.PrevX = CSocket.Client.X;
            CSocket.Client.PrevY = CSocket.Client.Y;
            CSocket.Client.X = X;
            CSocket.Client.Y = Y;
            byte[] JumpPacket = ConquerPacket.General(CSocket.Client.ID, CSocket.Client.X, CSocket.Client.Y, CSocket.Client.PrevX, CSocket.Client.PrevY, 0, Struct.DataType.Jump);
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
                        if (!seen)
                        {
                            if (Calculation.CanSee(CSocket.Client.X, CSocket.Client.Y, C.Client.X, C.Client.Y))
                            {
                                C.Send(ConquerPacket.SpawnCharacter(CSocket));
                            }
                        }
                        else
                        {
                            if (Calculation.CanSee(CSocket.Client.X, CSocket.Client.Y, C.Client.X, C.Client.Y))
                                C.Send(JumpPacket);
                            else
                                C.Send(ConquerPacket.General(CSocket.Client.ID, CSocket.Client.PrevX, CSocket.Client.PrevY, 0, 0, 0, Struct.DataType.EntityRemove));
                        }
                    }
                }
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
