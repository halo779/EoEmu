
namespace EO_Proxy.Connections.Packets
{
    using System;
    using System.Text;

    public unsafe class ServerResponse
    {
        public ServerResponse(byte[] buffer, Asynchronous.AsyncWrapper client)
        {
            fixed (byte* arg = buffer)
            {
                client.Identity = BitConverter.ToUInt32(buffer, 4);
                if (BitConverter.ToUInt32(buffer, 8) > 1)
                {
                    if (buffer.Length > 32)
                    {
                        Program.GameAddress = Encoding.ASCII.GetString(buffer, 20, 16);
                        Program.GameAddress.Trim('\0');
                        for (short i = 1; i > -1; i--) //zerobyting packet, will cause error upon connection
                            *((byte*)arg + 12 + i) = 0;
                        for (short i = 1; i > -1; i--)
                            *((byte*)arg + 12 + i) = BitConverter.GetBytes(Program.WorldPort)[i];
                        for (ushort i = 0; i < 16; i++)
                            *((byte*)(arg + 20 + i)) = 0;
                        for (ushort i = 0; i < Program.ProxyAddress.Length; i++)
                            *((byte*)(arg + 20 + i)) = (byte)Program.ProxyAddress[i];
                        Kernel.InQueueConnections.TryAdd(client.Identity, client);
                    }
                }
            }
        }
    }
}
