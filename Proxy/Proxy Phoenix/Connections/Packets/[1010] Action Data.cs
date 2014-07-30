
namespace EO_Proxy.Connections.Packets
{
    using System;

    public unsafe class GeneralAction
    {
        public byte[] Buffer;
        public GeneralAction(byte[] buffer)
        {
            Buffer = buffer;
        }
        public GeneralAction()
        {
            Buffer = new byte[45];
            fixed (byte* arg = Buffer)
            {
                *((ushort*)(arg)) = 37;
                *((ushort*)(arg + 2)) = 10010;
            }
        }
        /// <summary> Offset [4] UInt32 </summary>
        public uint Identity
        {
            get { return BitConverter.ToUInt32(Buffer, 4); }
            set { fixed (byte* arg = Buffer) *((uint*)(arg + 4)) = value; }
        }
        /// <summary> Offset [8] UInt32 </summary>
        public uint Parameter
        {
            get { return BitConverter.ToUInt32(Buffer, 8); }
            set { fixed (byte* arg = Buffer) *((uint*)(arg + 8)) = value; }
        }
        /// <summary> Offset [8] UInt16</summary>
        public ushort Value1
        {
            get { return BitConverter.ToUInt16(Buffer, 8); }
            set { fixed (byte* arg = Buffer) *((ushort*)(arg + 8)) = value; }
        }
        /// <summary> Offset [10] UInt16</summary>
        public ushort Value2
        {
            get { return BitConverter.ToUInt16(Buffer, 10); }
            set { fixed (byte* arg = Buffer) *((ushort*)(arg + 10)) = value; }
        }
        /// <summary> Offset [12] UInt16</summary>
        public ushort Value3
        {
            get { return BitConverter.ToUInt16(Buffer, 12); }
            set { fixed (byte* arg = Buffer) *((ushort*)(arg + 12)) = value; }
        }
        /// <summary> Offset [14] UInt16</summary>
        public ushort Value4
        {
            get { return BitConverter.ToUInt16(Buffer, 14); }
            set { fixed (byte* arg = Buffer) *((ushort*)(arg + 14)) = value; }
        }
        /// <summary> Offset [16] UInt16</summary>
        public ushort Value5
        {
            get { return BitConverter.ToUInt16(Buffer, 16); }
            set { fixed (byte* arg = Buffer) *((ushort*)(arg + 16)) = value; }
        }
        /// <summary> Offset [18] UInt16</summary>
        public ushort Value6
        {
            get { return BitConverter.ToUInt16(Buffer, 18); }
            set { fixed (byte* arg = Buffer) *((ushort*)(arg + 18)) = value; }
        }
        /// <summary> Offset [20] UInt16</summary>
        public ushort Type
        {
            get { return BitConverter.ToUInt16(Buffer, 20); }
            set { fixed (byte* arg = Buffer) *((ushort*)(arg + 20)) = value; }
        }
        /// <summary> Offset [22] UInt16</summary>
        public ushort Direction
        {
            get { return BitConverter.ToUInt16(Buffer, 22); }
            set { fixed (byte* arg = Buffer) *((ushort*)(arg + 16)) = value; }
        }
        /// <summary> Offset [24] UInt16</summary>
        public ushort X
        {
            get { return BitConverter.ToUInt16(Buffer, 24); }
            set { fixed (byte* arg = Buffer) *((ushort*)(arg + 24)) = value; }
        }
        /// <summary> Offset [26] UInt16</summary>
        public ushort Y
        {
            get { return BitConverter.ToUInt16(Buffer, 26); }
            set { fixed (byte* arg = Buffer) *((ushort*)(arg + 26)) = value; }
        }
        /// <summary> Offset [28] UInt32</summary>
        public uint Map
        {
            get { return BitConverter.ToUInt32(Buffer, 28); }
            set { fixed (byte* arg = Buffer) *((uint*)(arg + 28)) = value; }
        }
    }
}
