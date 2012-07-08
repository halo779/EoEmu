using System;
using System.Collections;
using System.Collections.Generic;
using GameServer.Connections;
using GameServer.Entities;
using GameServer.Structs;
using GameServer.Packets;
using GameServer.Calculations;

namespace GameServer.Handlers
{
    /// <summary>
    /// Handles porting through world portals.
    /// </summary>
    public partial class Handler
    {
        public static void Portal(int X, int Y, ClientSocket CSocket)
        {
            string PID = X + "," + Y + "," + (int)CSocket.Client.Map + "," + 0;
            if (Nano.Portals.ContainsKey(PID))
            {
                Struct.Portal Port = Nano.Portals[PID];
                Handler.Teleport(Port.EndMap, Port.EndX, Port.EndY, Port.EndInstance, CSocket);
            }
            else
            {
                Handler.Teleport((int)CSocket.Client.Map, CSocket.Client.PrevX, CSocket.Client.PrevY, 0, CSocket);
                CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Please report: Unknown portal: " + PID, Struct.ChatType.System));
            }
        }
    }
}
