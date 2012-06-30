using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using GameServer.Calculations;
using GameServer.Entities;
using GameServer.Structs;
using GameServer.Connections;


namespace GameServer.Packets
{
    /// <summary>
    /// Contains methods for sending packets to Player, Monster, and item locals as well as globally to the server, a guild, or entire maps.
    /// </summary>
    public partial class ConquerPacket
    {
        public static void ToLocal(byte[] Data, int X, int Y, int Map, int MapInstance, int ForbidID)
        {
            //lock(Nano.ClientPool)
            //{
            try
            {
                Monitor.Enter(Nano.ClientPool);
                foreach (KeyValuePair<int, ClientSocket> Tests in Nano.ClientPool)
                {
                    ClientSocket C = Tests.Value;
                    if (Calculation.CanSee(X, Y, C.Client.X, C.Client.Y) && C.Client.ID != ForbidID && (int)C.Client.Map == Map)
                    {
                        C.Send(Data);
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
        }
        public static void ToServer(byte[] Data, int ForbidID)
        {
            //	lock(Nano.ClientPool)
            //{
            try
            {
                Monitor.Enter(Nano.ClientPool);
                foreach (KeyValuePair<int, ClientSocket> Tests in Nano.ClientPool)
                {
                    if (Tests.Value.Client.ID != ForbidID)
                        Tests.Value.Send(Data);
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
        }
    }
}
