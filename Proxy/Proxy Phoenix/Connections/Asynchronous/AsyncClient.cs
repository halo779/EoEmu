
namespace EO_Proxy.Connections.Asynchronous
{
    using System.Net.Sockets;
    using ClientSocket;

    public class AsyncWrapper
    {
        public Socket IncomingSocket;
        public WinsockClient OutgoingSocket;

        public uint Identity;
        public byte[] Data = new byte[8192];
        public object Client;
        public ushort Port;

        public AsyncWrapper(ushort port)
        {
            this.Port = port;
        }
        public void StartSending()
        {
            this.OutgoingSocket = new WinsockClient(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.OutgoingSocket.OnReceive += new SocketEventCallback<WinsockClient, byte[]>(OutgoingSocket_OnReceive);
            this.OutgoingSocket.OnDisconnect += new SocketEventCallback<WinsockClient, object>(OutgoingSocket_OnDisconnect);
            this.OutgoingSocket.Enable(Program.AuthAddress, this.Port, this.Data);
        }

        private void OutgoingSocket_OnReceive(WinsockClient sender, byte[] data)
        {
            if (Functions.PacketSniffing.Sniffing)
                Functions.PacketSniffing.Sniff(data, true);
            Connections.PacketHandler.HandleAuthentication(this, data);
            this.IncomingSocket.Send(data);
        }

        private void OutgoingSocket_OnDisconnect(WinsockClient sender, object arg)
        {

        }
    }
}
