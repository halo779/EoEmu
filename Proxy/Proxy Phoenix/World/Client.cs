using System.Net.Sockets;
using ClientSocket;
using System;

namespace EO_Proxy.World
{
    public class Client
    {
        public uint Identity = 0;
        
        public Socket ClientSocket;
        public WinsockClient ServerSocket;
        public byte[] Data = new byte[8192];
        public byte[] RemainingServerData;
        public byte[] RemainingClientData;

        public bool Exchange = false, Continue = false;

        public Client(Socket incoming, ushort port)
        {
            this.ClientSocket = incoming;

            this.ServerSocket = new WinsockClient(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.ServerSocket.OnReceive += new SocketEventCallback<WinsockClient, byte[]>(OutgoingSocket_OnReceive);
            this.ServerSocket.OnDisconnect += new SocketEventCallback<WinsockClient, object>(OutgoingSocket_OnDisconnect);
            this.ServerSocket.Enable(Program.GameAddress, port, Data);
        }

        private void OutgoingSocket_OnReceive(WinsockClient sender, byte[] buffer)
        {
            if (this.Exchange && buffer.Length > 36)
            {
                this.SendToClient(buffer);
            }
            else
                Connections.PacketHandler.HandleBuffer(this, buffer, false);
        }
        private void OutgoingSocket_OnDisconnect(WinsockClient sender, object obj)
        {
            if (ClientSocket.Connected)
                ClientSocket.Disconnect(false);
            ClientSocket.Dispose();
            Console.WriteLine("Client Disconnected");
        }
        public void SendToClient(byte[] buffer)
        {
            byte[] packet = new byte[buffer.Length];
            Buffer.BlockCopy(buffer, 0, packet, 0, buffer.Length);
            if (this.ClientSocket.Connected)
                this.ClientSocket.Send(packet);
        }
        public void SendToServer(byte[] buffer)
        {
            byte[] packet = new byte[buffer.Length];
            Buffer.BlockCopy(buffer, 0, packet, 0, buffer.Length);
            this.ServerSocket.Send(buffer);
        }
    }
}
