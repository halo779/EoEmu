using System;


namespace GameServer.Packets
{
    /// <summary>
    /// Description of MonsterSpawn.
    /// </summary>
    public partial class EudemonPacket
    {

        public static byte[] SpawnMonster(int UID, int Mesh, int X, int Y, string Name, int HP, int Level, int Direction)
        {
            PacketBuilder Packet = new PacketBuilder(1014, 89 + Name.Length);
            Packet.Long(UID);
            Packet.Long(Mesh);
            Packet.Long(0);//@TODO: Monster Status Mask
            
            Packet.Long(0);
            Packet.Long(0);
            Packet.Long(0);

            Packet.Long(HP);

            Packet.Long(0);
            Packet.Long(0);

            Packet.Long(HP);
            Packet.Long(Level);
            Packet.Short(X);
            Packet.Short(Y);

            Packet.Long(0);

            Packet.Long(Direction);
            Packet.Long(1010);//@TODO: Monstertype?

            Packet.Long(0);
            Packet.Long(0);
            Packet.Long(0);
            Packet.Long(0);
            Packet.Long(0);
            Packet.Short(0);
            Packet.Byte(0);
            Packet.Byte(1);//StringCount
            Packet.Byte(Name.Length);
            Packet.Text(Name);

            return Packet.getFinal();
        }

        /// <summary>
        /// Old. Dont use.
        /// </summary>
        public static byte[] SpawnMonsterOLD(int UID, int Mesh, int X, int Y, string Name, int HP, int Level, int Direction)
        {
            PacketBuilder Packet = new PacketBuilder(1014, 97 + Name.Length);
            Packet.Long(UID);
            Packet.Long(Mesh);
            Packet.Long(0); //TODO: Status
            Packet.Long(0); //Unknown
            Packet.Short(0); //TODO: Guilds
            Packet.Int(0); //Unknown
            Packet.Int(0); //GuildRank
            Packet.Long(0); //Unknown
            Packet.Long(0); //Headgear
            Packet.Long(0); //Armor or garment
            Packet.Long(0); //Right hand
            Packet.Long(0); //Left hand
            Packet.Long(0); //Unknown
            Packet.Short(HP);
            Packet.Short(Level); //Level
            Packet.Short(X);
            Packet.Short(Y);
            Packet.Short(330);
            Packet.Int(Direction);
            Packet.Int(100);//Action
            Packet.Short(0);
            Packet.Int(Level);
            Packet.Int(0); //Unknown
            Packet.Long(0); //Unknown
            Packet.Long(0); //TODO: Nobility
            Packet.Long(UID);
            Packet.Short(0); //Unknown
            Packet.Short(0); //Unknown
            Packet.Long(0); //Unknown
            Packet.Int(0); //Unknown
            Packet.Short(0); //Unknown, 0 for mobs
            Packet.Short(0); //Unknwon, 0 for mobs
            Packet.Long(0); //Unknown
            Packet.Short(0); //Unknown
            Packet.Int(1); //String count
            Packet.Int(Name.Length);
            Packet.Text(Name);
            return Packet.getFinal();
        }
    }
}
