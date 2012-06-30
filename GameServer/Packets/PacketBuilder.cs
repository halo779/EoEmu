using System;
using System.Text;

namespace GameServer.Packets
{
    /// <summary>
    /// Conquer packet construction.
    /// </summary>
    /// <param name="T">The Type of the Conquer packet</param>
    /// <param name="L">The Length of the Conquer</param>
    public class PacketBuilder
    {
        protected byte[] _buffer = new byte[1024];
        protected int Position = 0;
        protected byte[] TQ_SERVER = Encoding.ASCII.GetBytes("TQServer");

        public PacketBuilder(int T, int L)
        {
            Length(L);
            Type(T);
        }

        public void Short(int value)
        {
            _buffer[Position] = ((byte)(value & 0xff));
            Position++;
            _buffer[Position] = ((byte)((value >> 8) & 0xff));
            Position++;
        }
        public void Length(int value)
        {
            _buffer[Position] = ((byte)(value & 0xff));
            Position++;
            _buffer[Position] = ((byte)((value >> 8) & 0xff));
            Position++;
        }
        public void Type(int value)
        {
            _buffer[Position] = ((byte)(value & 0xff));
            Position++;
            _buffer[Position] = ((byte)((value >> 8) & 0xff));
            Position++;
        }
        public void Long(int value)
        {
            _buffer[Position] = ((byte)(value & 0xff));
            Position++;
            _buffer[Position] = ((byte)(value >> 8 & 0xff));
            Position++;
            _buffer[Position] = (byte)(value >> 16 & 0xff);
            Position++;
            _buffer[Position] = ((byte)(value >> 24 & 0xff));
            Position++;
        }
        public void Long(ulong value)
        {
            _buffer[Position] = ((byte)((ulong)value & 0xffL));
            Position++;
            _buffer[Position] = ((byte)(value >> 8 & 0xff));
            Position++;
            _buffer[Position] = (byte)(value >> 16 & 0xff);
            Position++;
            _buffer[Position] = ((byte)(value >> 24 & 0xff));
            Position++;
        }
        public void ULong(ulong value)
        {
            _buffer[Position] = (byte)(((ulong)value & 0xff00000000000000L) >> 56);
            Position++;
            _buffer[Position] = (byte)((value & 0xff000000000000) >> 48);
            Position++;
            _buffer[Position] = (byte)((value & 0xff0000000000) >> 40);
            Position++;
            _buffer[Position] = (byte)((value & 0xff00000000) >> 32);
            Position++;
            _buffer[Position] = (byte)((value & 0xff000000) >> 24);
            Position++;
            _buffer[Position] = (byte)((value & 0xff0000) >> 16);
            Position++;
            _buffer[Position] = (byte)((value & 0xff00) >> 8);
            Position++;
            _buffer[Position] = (byte)(value & 0xff);
            Position++;
        }
        public void Int(int value)
        {
            _buffer[Position] = (Convert.ToByte(value & 0xff));
            Position++;
        }
        public void Long(uint value)
        {
            _buffer[Position] = ((byte)(value & 0xff));
            Position++;
            _buffer[Position] = ((byte)(value >> 8 & 0xff));
            Position++;
            _buffer[Position] = (byte)(value >> 16 & 0xff);
            Position++;
            _buffer[Position] = ((byte)(value >> 24 & 0xff));
            Position++;
        }

        public void Text(string value)
        {
            byte[] nvalue = Encoding.ASCII.GetBytes(value);
            Array.Copy(nvalue, 0, _buffer, Position, nvalue.Length);
            Position += nvalue.Length;
        }
        protected void Seal()
        {
            //Array.Copy(TQ_SERVER, 0, _buffer, Position, TQ_SERVER.Length);
            //Position += TQ_SERVER.Length + 1;
            //byte[] x = new byte[Position - 1];
            //Array.Copy(_buffer, x, Position - 1);
            //_buffer = new byte[x.Length];
            //Array.Copy(x, _buffer, x.Length);
            //x = null;
            byte[] x = new byte[Position];
            Array.Copy(_buffer, x, Position);
            _buffer = new byte[x.Length];
            Array.Copy(x, _buffer, x.Length);
            x = null;
        }
        public byte[] getFinal()
        {
            Seal();
            return _buffer;
        }
    }
}
