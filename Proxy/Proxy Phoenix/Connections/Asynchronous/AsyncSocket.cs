
namespace EO_Proxy.Connections.Asynchronous
{
    using System;
    using System.Net;
    using System.Net.Sockets;
using ClientSocket;

    public class AsyncSocket
    {
        public event Action<AsyncWrapper> ClientConnect;
        public event Action<AsyncWrapper, byte[]> ClientReceive;
        public event Action<object> ClientDisconnect;
        public bool AcceptConnections = true;
        private ushort _port;

        protected Socket pConnection = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        public AsyncSocket(ushort port, ushort externalport)
        {
            _port = externalport;
            pConnection.Bind(new IPEndPoint(IPAddress.Any, port));
            pConnection.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
            pConnection.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);
            pConnection.SetIPProtectionLevel(IPProtectionLevel.Unrestricted);
        }
        public void Listen()
        {
            pConnection.Listen(500);
            pConnection.BeginAccept(new AsyncCallback(Connect), null);
        }

        public void Connect(IAsyncResult result)
        {
            if (AcceptConnections)
            {
                AsyncWrapper obj = new AsyncWrapper(_port);
                try
                {
                    obj.IncomingSocket = pConnection.EndAccept(result);
                    if (obj.IncomingSocket == null)
                    {
                        obj = null;
                        pConnection.BeginAccept(new AsyncCallback(Connect), null);
                        return;
                    }
                    ClientConnect.Invoke(obj);
                    obj.IncomingSocket.BeginReceive(obj.Data, 0, 8192, SocketFlags.None, Receive, obj);
                }
                catch (Exception e) { obj = null; Console.WriteLine(e); }
                pConnection.BeginAccept(new AsyncCallback(Connect), null);
            }
        }
        public void Receive(IAsyncResult result)
        {
            bool received = false;
            AsyncWrapper obj = null;
            try
            {
                obj = result.AsyncState as AsyncWrapper;
                if (obj.IncomingSocket != null)
                {
                    SocketError status;
                    int size = obj.IncomingSocket.EndReceive(result, out status);
                    if (status == SocketError.Success && size > 0)
                    {
                        received = true;
                        byte[] buffer = new byte[size];
                        Buffer.BlockCopy(obj.Data, 0, buffer, 0, size);
                        if (obj.IncomingSocket != null)
                            obj.IncomingSocket.BeginReceive(obj.Data, 0, obj.Data.Length, SocketFlags.None, new AsyncCallback(Receive), obj);
                        ClientReceive(obj, buffer);
                    }
                    else
                        Disconnect(obj);
                }
                else
                    Disconnect(obj);
            }
            catch (Exception e)
            {
                if (received)
                    obj.IncomingSocket.BeginReceive(obj.Data, 0, obj.Data.Length, SocketFlags.None, new AsyncCallback(Receive), obj);
                obj = null;
                if (e.Message != "Cannot access a disposed object.\r\nObject name: 'System.Net.Sockets.Socket'.") //removing error when processing ending dc data after connection has been closed - @TODO: a proper way of dealing with this.
                    Console.WriteLine(e);
            }
        }
        public void Disconnect(AsyncWrapper obj)
        {
            if (obj != null)
                try
                {
                    if (obj.IncomingSocket != null)
                    {
                        obj.IncomingSocket.Shutdown(System.Net.Sockets.SocketShutdown.Both);
                        obj.IncomingSocket.Close();
                    }
                    obj.IncomingSocket = null;
                    if (obj.Client != null)
                        ClientDisconnect(obj);
                }
                catch (ObjectDisposedException e) { Console.WriteLine(e); }
            obj = null;
        }
    }
}
