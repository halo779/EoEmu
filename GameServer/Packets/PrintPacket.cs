using System;


namespace GameServer.Packets
{
    /// <summary>
    /// A string to print packet datas
    /// </summary>
    public partial class EudemonPacket
    {
        public static void PrintPacket(byte[] Data)
        {
            string Packet = "";
            foreach (byte d in Data)
                Packet += Convert.ToString(d, 16).PadLeft(2, '0') + " ";
            Console.WriteLine(Packet);
        }
    }
}
