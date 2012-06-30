using System;


namespace GameServer.Packets
{
    /// <summary>
    /// Description of ItemInfo.
    /// </summary>
    public partial class ConquerPacket
    {
       /* public static byte[] ItemInfo(int UID, int itemid, int Plus, int Minus, int Enchant, int soc1, int soc2, int currentdura, int maxdura, int Location, int Color)
        {
            PacketBuilder Packet = new PacketBuilder(1008, 56);
            Packet.Long(UID);
            Packet.Long(itemid);
            //Packet.Int(Math.Abs(200-currentdura) & 0xff);
            //Packet.Int((byte)(currentdura / 2.56) & 0xff);
            //Packet.Int(Math.Abs(200-maxdura) & 0xff);
            //Packet.Int((byte)(maxdura / 2.56) & 0xff);
            //Packet.Long(0);
            Packet.Int(1);
            Packet.Int(1);
            Packet.Int(1);
            Packet.Int(1);
            Packet.Short(1);
            Packet.Int(Location);
            Packet.Long(0);
            Packet.Int(0);
            Packet.Int(soc1);
            Packet.Int(soc2);
            Packet.Short(2);
            Packet.Int(Plus);
            Packet.Int(Minus);
            Packet.Int(0); // Free / Unfree
            Packet.Long(Enchant);
            Packet.Int(0);
            Packet.Short(0); // Suspicious item??
            Packet.Short(0); //Boolean 1- yes 0 - no (locked)
            Packet.Long(Color); //Item color. 2-9(headgear is 3-9)
            Packet.Long(0);
            return Packet.getFinal();
        }*/
        public static byte[] ItemInfo(int UID, int itemid, int Plus, int soc1, int soc2, int currentdura, int maxdura, int Location, int magic1, int magic2, int data)
        {
            PacketBuilder Packet = new PacketBuilder(1008, 57);
            Packet.Long(0);
            Packet.Long(UID);
            Packet.Long(itemid);
            Packet.Short(currentdura);
            Packet.Short(maxdura);
            Packet.Short(1);//creation type
            Packet.Int(Location);
            Packet.Int(soc1);
            Packet.Int(soc2);
            Packet.Int(magic1);
            Packet.Int(magic2);
            Packet.Int(Plus);
            Packet.Short(data);
            Packet.Short(0);
            Packet.Long(0);
            Packet.Short(0); //warghostexp
            Packet.Short(0);
            Packet.Long(0);
            Packet.Long(0);
            Packet.Int(0);//Earth Attack
            Packet.Int(0);//Water Attack
            Packet.Int(0);//Fire Attack
            Packet.Int(0);//Air Attack
            Packet.Int(0);//Special Attack
            Packet.Int(0);
            Packet.Int(0);
            Packet.Int(1);//string Count
            Packet.Int(0);//String Length
            return Packet.getFinal();
            
        }
    }
}
