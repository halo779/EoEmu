using System;


namespace LoginServer
{
    /// <summary>
    /// Basic Login Server Packets.
    /// </summary>
    public static class Packets
    {
        /// <summary>
        /// Sends an Response to the server containing the data to which game server to connect to
        /// </summary>
        /// <param name="LocalIP">IP address of the game server</param>
        /// <param name="Key1">unknown(in co it is the token number</param>
        /// <param name="Key2">random number but is the acc id in binarys</param>
        /// <returns>the packet is built then returned for sending in byte form</returns>
        public static byte[] AuthResponse(string LocalIP, byte[] Key1, byte[] Key2)
        {
            byte[] PacketData = new byte[52];
            PacketData[0] = 0x34;//packet length
            PacketData[1] = 0x00;//packet length
            PacketData[2] = 0x1f;//packet ID (1055)
            PacketData[3] = 0x04;//packet ID (1055)
            PacketData[4] = Key2[3];//acc id in binary
            PacketData[5] = Key2[2];//acc id in binary
            PacketData[6] = Key2[1];//acc id in binary
            PacketData[7] = Key2[0];//acc id in binary
            PacketData[8] = Key1[3];//unknown (could be random token number)
            PacketData[9] = Key1[2];//unknown (could be random token number)
            PacketData[10] = Key1[1];//unknown (could be random token number)
            PacketData[11] = Key1[0];//unknown (could be random token number)
            PacketData[12] = 0xb8;//port
            PacketData[13] = 0x16;//port
            for (int x = 0; x < LocalIP.Length; x++)
            {
                PacketData[20 + x] = (byte)LocalIP[x];
            }
            //any packetdata left in the array after this point will be zeroed
            return PacketData;
        }
        /// <summary>
        /// Sends an responce to the client telling it that they have the wrong password
        /// </summary>
        /// <returns>the packet is built then returned for sending in byte form</returns>
        public static byte[] WrongPass()
        {
            string error = "Account name or password wrong";
            byte[] PacketData = new byte[0x34];
            PacketData[0] = 0x34;//packet lentgh
            PacketData[1] = 0x00;//packet lentgh
            PacketData[2] = 0x1f;//packet ID (1055)
            PacketData[3] = 0x04;//packet ID (1055)
            PacketData[8] = 0x01;//
            for (int x = 0; x < error.Length; x++)
            {
                PacketData[20 + x] = (byte)error[x];
            }
            return PacketData;
        }

        public static byte[] ErrorMessage(string Message)
        {
            byte[] PacketData = new byte[52];
            PacketData[0] = 0x34;
            PacketData[1] = 0x00;//packet lentgh
            PacketData[2] = 0x1f;//packet ID (1055)
            PacketData[3] = 0x04;//packet ID (1055)
            for (int x = 0; x < Message.Length; x++)
            {
                PacketData[20 + x] = (byte)Message[x];
            }
            return PacketData;
        }
        /// <summary>
        /// Sends a Packet needed to start the auth process, it is assumed to be a login seed packet id 1059
        /// </summary>
        /// <returns>login seed packet</returns>
        public static byte[] LoginSeed()
        {
            byte[] PacketData = new byte[8];
            PacketData[0] = 0x08;
            PacketData[1] = 0x00;
            PacketData[2] = 0x23;
            PacketData[3] = 0x04;
            PacketData[4] = 0xcb;
            PacketData[5] = 0x5c;
            PacketData[6] = 0x00;
            PacketData[7] = 0x00;
            return PacketData;

        }
    }
}
