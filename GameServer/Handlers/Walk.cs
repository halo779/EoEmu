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

        public static void Walk(int Direction, MovementModes MMode, ClientSocket CSocket)
        {
            short DX = 0;
            short DY = 0;
            if(Direction > MAX_DIRSIZE)
                {
                    //Unknown Walking direction.
                    CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Unknown walking direction.", Struct.ChatType.Talk));
                    CSocket.Disconnect();
                    return;
                }

            DX += _DELTA_X[Direction];
            DY += _DELTA_Y[Direction];
            CSocket.Client.PrevX = CSocket.Client.X;
            CSocket.Client.PrevY = CSocket.Client.Y;

            if (Nano.Maps.ContainsKey((int)CSocket.Client.Map))
            {
                Struct.DmapData Map = Nano.Maps[(int)CSocket.Client.Map];
                if (!Map.CheckLoc(CSocket.Client.X + DX, CSocket.Client.Y + DY))
                {
                    CSocket.Send(EudemonPacket.General(CSocket.Client.ID, CSocket.Client.PrevX, CSocket.Client.PrevY, CSocket.Client.Direction, Struct.DataType.actionKickBack, CSocket.Client.PrevX, CSocket.Client.PrevY));
                    CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Invalid coordinates.", Struct.ChatType.System));
                    return;
                }
            }

            
            CSocket.Client.X = (ushort)(CSocket.Client.X + DX);
            CSocket.Client.Y = (ushort)(CSocket.Client.Y + DY);

            if (MMode >= MovementModes.RUN_DIR0 && MMode <= MovementModes.RUN_DIR7)
            {
                DX = 0; DY = 0;

                DX += _DELTA_X[MMode - MovementModes.RUN_DIR0];
                DY += _DELTA_Y[MMode - MovementModes.RUN_DIR0];

                if (Nano.Maps.ContainsKey((int)CSocket.Client.Map))
                {
                    Struct.DmapData Map = Nano.Maps[(int)CSocket.Client.Map];
                    if (!Map.CheckLoc(CSocket.Client.X + DX, CSocket.Client.Y + DY))
                    {
                       CSocket.Send(EudemonPacket.General(CSocket.Client.ID, CSocket.Client.PrevX, CSocket.Client.PrevY, CSocket.Client.Direction, Struct.DataType.actionKickBack, CSocket.Client.PrevX, CSocket.Client.PrevY));
                        CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Invalid coordinates.", Struct.ChatType.System));
                        return;
                    }
                }

                CSocket.Client.PrevX = CSocket.Client.X;
                CSocket.Client.PrevY = CSocket.Client.Y;
                CSocket.Client.X = (ushort)(CSocket.Client.X + DX);
                CSocket.Client.Y = (ushort)(CSocket.Client.Y + DY);
            }
            
            //TODO: Map / Guild wall / other checks
            byte[] WalkPacket = EudemonPacket.Walk(Direction, CSocket.Client.ID, CSocket.Client.X,CSocket.Client.Y);
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
                                C.Send(EudemonPacket.Chat(0, "SYSTEM", C.Client.Name, "Player Moved to ." + CSocket.Client.X + ", " + CSocket.Client.Y, Struct.ChatType.System));
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
