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
        public static void Teleport(int Map, int X, int Y, int Instance, ClientSocket CSocket)
        {
            if (Map > 0 && X > 0 && Y > 0 && CSocket != null)
            {
                ConquerPacket.ToLocal(ConquerPacket.General(CSocket.Client.ID, CSocket.Client.X, CSocket.Client.Y, 0, 0, 0, Struct.DataType.EntityRemove), CSocket.Client.X, CSocket.Client.Y, (int)CSocket.Client.Map, 0, CSocket.Client.ID);
                CSocket.Client.Map = (Struct.Maps)Map;
                CSocket.Client.X = X;
                CSocket.Client.Y = Y;
                //TODO: Instance pairing.
                CSocket.Send(ConquerPacket.General(CSocket.Client.ID, (int)CSocket.Client.Map, 0, CSocket.Client.X, CSocket.Client.Y, 0, Struct.DataType.ChangeMap));
                CSocket.Send(ConquerPacket.General(CSocket.Client.ID, Nano.TintR, Nano.TintG, 0, 0, 0, Struct.DataType.CompleteMapChange));
                CSocket.Send(ConquerPacket.NewMap((int)CSocket.Client.Map));
                CSocket.Send(ConquerPacket.General(CSocket.Client.ID, 2, 0, CSocket.Client.X, CSocket.Client.Y, 0, Struct.DataType.MapShow3));
                ConquerPacket.ToLocal(ConquerPacket.SpawnCharacter(CSocket), CSocket.Client.X, CSocket.Client.Y, (int)CSocket.Client.Map, 0, CSocket.Client.ID);
                CSocket.Client.PrevX = 0;
                CSocket.Client.PrevY = 0;
                Spawn.All(CSocket);
            }
        }
    }
}
