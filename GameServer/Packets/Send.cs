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
    public partial class EudemonPacket
    {
        /// <summary>
        /// Sends Packet To all players within the given ranges
        /// </summary>
        /// <param name="Data">Packet to send</param>
        /// <param name="X">X coordinates of the object</param>
        /// <param name="Y">Y coordinates of the object</param>
        /// <param name="Map">Map Object is in</param>
        /// <param name="MapInstance">MapInsantce, not currently used</param>
        /// <param name="ForbidID">Player ID to not send any information to</param>
        public static void ToLocal(byte[] Data, int X, int Y, int Map, int MapInstance, int ForbidID)
        {
            //lock(MainGS.ClientPool)
            //{
            try
            {
                Monitor.Enter(MainGS.ClientPool);
                foreach (KeyValuePair<int, ClientSocket> Tests in MainGS.ClientPool)
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
                Monitor.Exit(MainGS.ClientPool);
            }
        }

        /// <summary>
        /// Sends Packet to everyone on the server
        /// </summary>
        /// <param name="Data">Packet to Send</param>
        /// <param name="ForbidID">User ID to not send any packet to</param>
        public static void ToServer(byte[] Data, int ForbidID)
        {
            //	lock(MainGS.ClientPool)
            //{
            try
            {
                Monitor.Enter(MainGS.ClientPool);
                foreach (KeyValuePair<int, ClientSocket> Tests in MainGS.ClientPool)
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
                Monitor.Exit(MainGS.ClientPool);
            }
        }
    }
}
