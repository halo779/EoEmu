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
    /// Description of WalkRun.
    /// </summary>
    public partial class Handler
    {
        public enum MovementModes
        {
            WALK=0,					
		    RUN,
		    SHIFT,						// to server only
		    JUMP,
		    TRANS,
		    CHGMAP,
		    JUMPMAGICATTCK,
		    COLLIDE,
		    SYNCHRO,					// to server only
		    TRACK,
		    RUN_DIR0 = 20,
		    RUN_DIR7 = 27,
        };

        static readonly short[] _DELTA_X = { 0, -1, -1, -1, 0, 1, 1, 1, 0 };
        static readonly short[] _DELTA_Y = { 1, 1, 0, -1, -1, -1, 0, 1, 0 };

        const int MAX_DIRSIZE = 8;

        public static void Walk(int ucModeDir, ClientSocket CSocket)
        {
            short newXPos = 0;
            short newYPos = 0;

            int Direction = ((ucModeDir >> 0) & 0xff) % 8;
            MovementModes MMode = (MovementModes)((ucModeDir >> 8) & 0xff);

            if(Direction > MAX_DIRSIZE)
                {
                    //Unknown Walking direction.
                    CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Unknown walking direction.", Struct.ChatType.Talk));
                    CSocket.Disconnect();
                    return;
                }
            newXPos = (short) (CSocket.Client.X + _DELTA_X[Direction]);
            newYPos = (short) (CSocket.Client.Y + _DELTA_Y[Direction]);



            if (Nano.Maps.ContainsKey((int)CSocket.Client.Map))
            {
                Struct.DmapData Map = Nano.Maps[(int)CSocket.Client.Map];
                if (!Map.CheckLoc(newXPos, newYPos))
                {
                    CSocket.Send(EudemonPacket.General(CSocket.Client.ID, CSocket.Client.PrevX, CSocket.Client.PrevY, CSocket.Client.Direction, Struct.DataType.actionKickBack, CSocket.Client.PrevX, CSocket.Client.PrevY));
                    CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Invalid coordinates.", Struct.ChatType.System));
                    return;
                }
            }

            if (MMode >= MovementModes.RUN_DIR0 && MMode <= MovementModes.RUN_DIR7)
            {
                newXPos += _DELTA_X[MMode - MovementModes.RUN_DIR0];
                newYPos += _DELTA_Y[MMode - MovementModes.RUN_DIR0];

                if (Nano.Maps.ContainsKey((int)CSocket.Client.Map))
                {
                    Struct.DmapData Map = Nano.Maps[(int)CSocket.Client.Map];
                    if (!Map.CheckLoc(newXPos, newYPos))
                    {
                       CSocket.Send(EudemonPacket.General(CSocket.Client.ID, CSocket.Client.PrevX, CSocket.Client.PrevY, CSocket.Client.Direction, Struct.DataType.actionKickBack, CSocket.Client.PrevX, CSocket.Client.PrevY));
                        CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Invalid coordinates.", Struct.ChatType.System));
                        return;
                    }
                }
            }

            if (newXPos < 0 || newYPos < 0)
            {
                CSocket.Send(EudemonPacket.General(CSocket.Client.ID, CSocket.Client.PrevX, CSocket.Client.PrevY, CSocket.Client.Direction, Struct.DataType.actionKickBack, CSocket.Client.PrevX, CSocket.Client.PrevY));
                CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Negetive coordinates.", Struct.ChatType.System));
                return;
            }
            //Only set Previous values If dmap check has passed.
            CSocket.Client.PrevX = CSocket.Client.X;
            CSocket.Client.PrevY = CSocket.Client.Y;

            CSocket.Client.X = Convert.ToUInt16(newXPos);
            CSocket.Client.Y = Convert.ToUInt16(newYPos);

            //TODO: Map / Guild wall / other checks
            byte[] WalkPacket = EudemonPacket.WalkRun(ucModeDir, CSocket.Client.ID, CSocket.Client.X, CSocket.Client.Y);
            try
            {
                Monitor.Enter(Nano.ClientPool);
                foreach (KeyValuePair<int, ClientSocket> Tests in Nano.ClientPool)
                {
                    ClientSocket C = Tests.Value;
                    if ((int)C.Client.Map == (int)CSocket.Client.Map && C.Client.ID != CSocket.Client.ID)
                    {
                         if (!Calculation.CanSee(CSocket.Client.PrevX, CSocket.Client.PrevY, C.Client.X, C.Client.Y))
                        {
                            if (Calculation.CanSee(CSocket.Client.X, CSocket.Client.Y, C.Client.X, C.Client.Y))
                            {
                                C.Send(EudemonPacket.SpawnCharacter(CSocket));
                            }
                        }
                        else
                        {
                            if (Calculation.CanSee(CSocket.Client.X, CSocket.Client.Y, C.Client.X, C.Client.Y))
                            {
                                C.Send(WalkPacket);
                            }
                            else
                                C.Send(EudemonPacket.GeneralOld(CSocket.Client.ID, CSocket.Client.PrevX, CSocket.Client.PrevY, 0, 0, 0, Struct.DataType.EntityRemove));
                        }
                    }
                }
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
