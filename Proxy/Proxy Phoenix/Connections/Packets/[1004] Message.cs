
namespace EO_Proxy.Connections.Packets
{
    using System.Text;
    using System;

    public unsafe class Message
    {
        public byte[] Buffer;
        public const uint Talk = 2000, Center = 2011;

        public string Text;
        public Message(byte[] buffer)
        {
            Buffer = buffer;
            Text = Encoding.ASCII.GetString(buffer, 29 + buffer[25] + buffer[26 + buffer[25]],
                buffer[28 + buffer[25] + buffer[26 + buffer[25]]]);
        }
        public Message(uint account, string message, string to, string from, System.Drawing.Color color, uint type)
        {
            Buffer = new byte[32 + message.Length + to.Length + from.Length];
            fixed (byte* arg = Buffer)
            {
                *((ushort*)(arg)) = (ushort)(Buffer.Length);
                *((ushort*)(arg + 2)) = 1004;
                *((uint*)(arg + 4)) = (uint)color.ToArgb();
                *((uint*)(arg + 8)) = type;
                *((uint*)(arg + 12)) = account;
                *((byte*)(arg + 24)) = 4;
                *((byte*)(arg + 25)) = (byte)from.Length;
                for (ushort i = 0; i < from.Length; i++)
                    *((byte*)(arg + 26 + i)) = (byte)from[i];
                *((byte*)(arg + 26) + from.Length) = (byte)to.Length;
                for (ushort i = 0; i < to.Length; i++)
                    *((byte*)(arg + 27 + from.Length + i)) = (byte)to[i];
                *((byte*)(arg + 28) + from.Length + to.Length) = (byte)message.Length;
                for (ushort i = 0; i < message.Length; i++)
                    *((byte*)(arg + 29 + from.Length + to.Length + i)) = (byte)message[i];
            }
            Text = message;
        }
        public uint ChatType
        {
            get { return BitConverter.ToUInt32(Buffer, 8); }
            set { fixed (byte* arg = Buffer) *((uint*)(arg + 8)) = value; }
        }
        public string From
        {
            get { return Encoding.ASCII.GetString(Buffer, 26, Buffer[25]); }
            set
            {
                fixed (byte* arg = Buffer)
                {
                    *((byte*)(arg + 25)) = (byte)value.Length;
                    for (ushort i = 0; i < value.Length; i++)
                        *((byte*)(arg + 26 + i)) = (byte)value[i];
                }
            }
        }
        public string To
        {
            get { return Encoding.ASCII.GetString(Buffer, 27 + Buffer[25], Buffer[26 + Buffer[25]]); }
            set
            {
                fixed (byte* arg = Buffer)
                {
                    *((byte*)(arg + 26 + Buffer[25])) = (byte)value.Length;
                    for (ushort i = 0; i < value.Length; i++)
                        *((byte*)(arg + 27 + Buffer[25] + i)) = (byte)value[i];
                }
            }
        }
    }
}
