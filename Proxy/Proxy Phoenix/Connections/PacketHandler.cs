using System;
using EO_Proxy.Connections.Packets;
using EO_Proxy.Functions;

namespace EO_Proxy.Connections
{
    public class PacketHandler
    {
        public static void HandleAuthentication(Asynchronous.AsyncWrapper client, byte[] buffer)
        {
            //Console.WriteLine("handling" + BitConverter.ToUInt16(buffer, 2));
            if (BitConverter.ToUInt16(buffer, 2) == 1055)
                new Packets.ServerResponse(buffer, client);
        }
        public static void HandleBuffer(World.Client client, byte[] buffer, bool serverData)
        {
            try
            {
                if (buffer == null || client == null)
                    return;
                if (serverData)
                {
                    if (client.RemainingServerData != null && client.RemainingServerData.Length > 0)
                    {
                        byte[] combined = new byte[buffer.Length + client.RemainingServerData.Length];
                        Buffer.BlockCopy(client.RemainingServerData, 0, combined, 0, client.RemainingServerData.Length);
                        Buffer.BlockCopy(buffer, 0, combined, client.RemainingServerData.Length, buffer.Length);
                        client.RemainingServerData = null;
                        buffer = combined;
                    }
                rollAgain:
                    ushort length = (ushort)(BitConverter.ToUInt16(buffer, 0));
                    if (length == buffer.Length)
                        HandleServer(client, buffer);
                    else if (length < buffer.Length)
                    {
                        byte[] initial = new byte[length];
                        Buffer.BlockCopy(buffer, 0, initial, 0, length);
                        byte[] nextpacket = new byte[buffer.Length - (length)];
                        Buffer.BlockCopy(buffer, length, nextpacket, 0, buffer.Length - (length));
                        buffer = nextpacket;
                        HandleServer(client, initial);
                        goto rollAgain;
                    }
                    else
                    {
                        client.RemainingServerData = new byte[buffer.Length];
                        Buffer.BlockCopy(buffer, 0, client.RemainingServerData, 0, buffer.Length);
                        return;
                    }
                }
                else
                {
                    if (client.RemainingClientData != null && client.RemainingClientData.Length > 0)
                    {
                        byte[] combined = new byte[buffer.Length + client.RemainingClientData.Length];
                        Buffer.BlockCopy(client.RemainingClientData, 0, combined, 0, client.RemainingClientData.Length);
                        Buffer.BlockCopy(buffer, 0, combined, client.RemainingClientData.Length, buffer.Length);
                        client.RemainingClientData = null;
                        buffer = combined;
                    }
                rollAgain:
                    ushort length = (ushort)(BitConverter.ToUInt16(buffer, 0));
                    if (length == buffer.Length)
                        HandleClient(client, buffer);
                    else if (length + 8 < buffer.Length)
                    {
                        byte[] initial = new byte[length];
                        Buffer.BlockCopy(buffer, 0, initial, 0, length);
                        byte[] nextpacket = new byte[buffer.Length - (length)];
                        Buffer.BlockCopy(buffer, length, nextpacket, 0, buffer.Length - (length));
                        buffer = nextpacket;
                        HandleClient(client, initial);
                        goto rollAgain;
                    }
                    else
                    {
                        client.RemainingClientData = new byte[buffer.Length];
                        Buffer.BlockCopy(buffer, 0, client.RemainingClientData, 0, buffer.Length);
                        return;
                    }
                }
            }
            catch { }
        }
        public static void HandleServer(World.Client client, byte[] buffer)
        {
            if (buffer == null || client == null)
                return;
            if (PacketSniffing.Sniffing)
                PacketSniffing.Sniff(buffer, true);
            ushort length = BitConverter.ToUInt16(buffer, 0);
            ushort id = BitConverter.ToUInt16(buffer, 2);

            bool send = true;

            
            switch (id)
            {
                #region [1004] Chat
                case 1004: Handlers.Chat(client, new Message(buffer), out send); break;
                #endregion
                #region [1052] Login
                case 1052: client.Identity = BitConverter.ToUInt32(buffer, 4); break;
                #endregion
            }
            if (Program.ConsoleLogging && send)//hides any command to the proxy
            {
                Console.WriteLine("New packet passed from client -> server | Packet ID: " + id + " | Size: " + length);
                Console.ResetColor();
                Console.WriteLine(PacketOutput.Dump(buffer));
                Console.ForegroundColor = ConsoleColor.White;
            }
            if (send)
                client.SendToServer(buffer);
        }
        public static void HandleClient(World.Client client, byte[] buffer)
        {
            if (buffer == null || client == null)
                return;
            if (PacketSniffing.Sniffing)
                PacketSniffing.Sniff(buffer, false);
            ushort length = BitConverter.ToUInt16(buffer, 0);
            ushort id = BitConverter.ToUInt16(buffer, 2);

            bool send = true;

            if (Program.ConsoleLogging)
            {
                Console.WriteLine("New packet passed from server -> client | Packet ID: " + id + " | Size: " + length);
                Console.ResetColor();
                Console.WriteLine(PacketOutput.Dump(buffer));
                Console.ForegroundColor = ConsoleColor.White;
            }

            switch (id)
            {
                    //@TODO: add packet tasks here eg. copy npc data to files
                default:
                    {
                        if(Program.DebugMode)
                            Console.WriteLine("Packet unknown");
                        break;
                    }
            }
            if (send)
                client.SendToClient(buffer);
        }
    }
}
