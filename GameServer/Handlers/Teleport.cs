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
    /// Handles teleportations in the Conquer worls
    /// </summary>
    public partial class Handler
    {
        public static void Teleport(int Map, ushort X, ushort Y, int Instance, ClientSocket CSocket)
        {
            if (Map > 0 && X > 0 && Y > 0 && CSocket != null)
            {
                if (Instance == 0)
                    Instance = Map;
                //@TODO: Check Map Pos is Valid.
                EudemonPacket.ToLocal(EudemonPacket.General(CSocket.Client.ID, CSocket.Client.X, CSocket.Client.Y, 0, 0, 0, Struct.DataType.EntityRemove), CSocket.Client.X, CSocket.Client.Y, (int)CSocket.Client.Map, 0, CSocket.Client.ID);
                CSocket.Client.Map = (Struct.Maps)Map;
                CSocket.Client.X = X;
                CSocket.Client.Y = Y;
                //TODO: Instance pairing.
                CSocket.Send(EudemonPacket.General(CSocket.Client.ID, X, Y, CSocket.Client.Direction, Struct.DataType.actionFlyMap, Map));
                CSocket.Send(EudemonPacket.NewMap((int)CSocket.Client.Map, 2097152, Instance));
                EudemonPacket.ToLocal(EudemonPacket.SpawnCharacter(CSocket), CSocket.Client.X, CSocket.Client.Y, (int)CSocket.Client.Map, 0, CSocket.Client.ID);
                //@TODO: work out the point of ARGB.
                //CSocket.Send(EudemonPacket.General(CSocket.Client.ID, CSocket.Client.X, CSocket.Client.Y, CSocket.Client.Direction, Struct.DataType.actionMapARGB, 65535));



                EudemonPacket.ToLocal(EudemonPacket.String(CSocket, 10, "other05"), CSocket.Client.X, CSocket.Client.Y, (int)CSocket.Client.Map, 0, 0);
                CSocket.Client.PrevX = 0;
                CSocket.Client.PrevY = 0;
                Spawn.All(CSocket);
            }
        }
    }
}
