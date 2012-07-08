using System;
using GameServer.Connections;
using GameServer.Entities;


namespace GameServer.Packets
{
    /// <summary>
    /// Description of CharacterInfo.
    /// </summary>
    public partial class ConquerPacket
    {
        public static byte[] CharacterInfo(ClientSocket CSocket)
        {
            PacketBuilder Packet = new PacketBuilder(1006, 162 + CSocket.Client.Name.Length + CSocket.Client.Spouse.Length);
            Packet.Long(CSocket.Client.ID);
            Packet.Long(CSocket.Client.Model);
            Packet.Short(CSocket.Client.Hair); //Hairstyle
            Packet.Short(0);//placeholder
            Packet.Long(CSocket.Client.Money); // Money
            Packet.Long(CSocket.Client.EPs); // EPs
            Packet.Long(CSocket.Client.Exp); // Exp
            Packet.Long(0);//placeholder
            Packet.Long(CSocket.Client.MentorExp);//TODO Mentor Exp
            Packet.Short(CSocket.Client.MercenaryExp);//TODO Merc Exp
            Packet.Short(0);//placeholder
            Packet.Short(CSocket.Client.BP);
            Packet.Short(0);//placeholder
            Packet.Short(CSocket.Client.Power);//power aka. force
            Packet.Short(CSocket.Client.Constitution);//Constitution
            Packet.Short(CSocket.Client.Dexterity);//Dexterity
            Packet.Short(CSocket.Client.Speed);//Speed
            Packet.Short(CSocket.Client.Vitality);//TODO Vitality
            Packet.Short(CSocket.Client.Soul);//soul
            Packet.Short(CSocket.Client.AdditionalPoint);//TODO Additional Point
            Packet.Short(CSocket.Client.CurrentHP);
            Packet.Short(CSocket.Client.MaxHP);
            Packet.Short(CSocket.Client.CurrentMP);//mana
            Packet.Short(CSocket.Client.MaxMP);
            Packet.Short(0);//placeholder
            Packet.Long(0);//placeholder
            Packet.Short(CSocket.Client.PkPoints);
            Packet.Int(CSocket.Client.Level); // Level
            Packet.Int((int)CSocket.Client.Class);//Class
            Packet.Int(CSocket.Client.Nobility);//TODO Nobility
            Packet.Int(CSocket.Client.Metempsychosis);//TODO Metempsychosis
            Packet.Int(CSocket.Client.AutoAllocate);//TODO Auto Allocate
            Packet.Int(CSocket.Client.MentorLevel);
            Packet.Int(CSocket.Client.MercenaryRank);//TODO Merc Rank
            Packet.Int(CSocket.Client.NobilityRank);//TODO Nobility Rank
            Packet.Short(CSocket.Client.MaxSummons);//TODO Max Summons
            Packet.Short(CSocket.Client.Exploit);//TODO Exploit
            Packet.Short(0);//placeholder
            Packet.Long(CSocket.Client.TokenPoints);//TODO Token Points
            Packet.Short(CSocket.Client.EudBagSize);
            Packet.Short(0);//placeholder
            Packet.Short(CSocket.Client.MuteFlag);//TODO Mute Flag
            Packet.Short(0);//placeholder
            Packet.Long(0);//placeholder
            Packet.Long(0);//placeholder
            Packet.Long(0);//placeholder
            Packet.Long(0);//placeholder
            Packet.Long(0);//placeholder
            Packet.Long(0);//placeholder
            Packet.Long(0);//placeholder
            Packet.Long(0);//placeholder
            Packet.Long(0);//placeholder
            Packet.Long(0);//placeholder
            Packet.Short(CSocket.Client.Vip);
            Packet.Short(0);
            Packet.Long(CSocket.Client.Wood);
            Packet.Short(CSocket.Client.Business);//TODO Business
            Packet.Short(0);
            Packet.Long(CSocket.Client.PPs);
            Packet.Int(2);//amount of strings
            Packet.Int(CSocket.Client.Name.Length);
            Packet.Text(CSocket.Client.Name);
            Packet.Int(CSocket.Client.Spouse.Length);
            Packet.Text(CSocket.Client.Spouse);
            Packet.Int(0);
            Packet.Short(0);
            /*byte[] D = Packet.getFinal();
            string packet = "";
            foreach(byte d in D)
            	packet += Convert.ToString(d, 16).PadLeft(2, '0') + " ";
            Console.WriteLine(packet);
            return D;*/
            return Packet.getFinal();
        }
    }
}
