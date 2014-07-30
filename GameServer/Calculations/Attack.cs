using System;
using System.Collections;
using System.Collections.Generic;
using GameServer.Connections;
using GameServer.Entities;
using GameServer.Structs;
using GameServer.Packets;

namespace GameServer.Calculations
{
    /// <summary>
    /// Calculates min. and max. attack as well as magical attack.
    /// </summary>
    public partial class Calculation
    {
        public static void Attack(ClientSocket CSocket)
        {
            CSocket.Client.MaxAttack = CSocket.Client.BaseMaxAttack;//TODO: Revise Attack Calcucations!
            CSocket.Client.MinAttack = CSocket.Client.BaseMinAttack;
            CSocket.Client.MagicAttack = CSocket.Client.BaseMagicAttack;
            int MaxAdd = 0;
            int MinAdd = 0;
            int MagicAdd = 0;
            if (CSocket.Client.NCG > 0)
            {
                MaxAdd += (int)Math.Floor(CSocket.Client.MaxAttack * (CSocket.Client.NCG * .05));
                MinAdd += (int)Math.Floor(CSocket.Client.MinAttack * (CSocket.Client.NCG * .05));
            }
            if (CSocket.Client.RCG > 0)
            {
                MaxAdd += (int)Math.Floor(CSocket.Client.MaxAttack * (CSocket.Client.RCG * .10));
                MinAdd += (int)Math.Floor(CSocket.Client.MinAttack * (CSocket.Client.RCG * .10));
            }
            if (CSocket.Client.SCG > 0)
            {
                MaxAdd += (int)Math.Floor(CSocket.Client.MaxAttack * (CSocket.Client.SCG * .20));
                MinAdd += (int)Math.Floor(CSocket.Client.MinAttack * (CSocket.Client.SCG * .20));
            }
            CSocket.Client.MaxAttack += MaxAdd;
            CSocket.Client.MinAttack += MinAdd;

            CSocket.Client.MagicAttack += MagicAdd;
            CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "Max: " + CSocket.Client.MaxAttack + " Min: " + CSocket.Client.MinAttack + " Magic: " + CSocket.Client.MagicAttack + " / " + CSocket.Client.BonusMagicAttack + " Defense: " + CSocket.Client.Defense + " Bless: " + CSocket.Client.Bless + "% Dodge: " + CSocket.Client.Dodge + " % MaxHP/MP: " + CSocket.Client.MaxHP + "/" + CSocket.Client.MaxMP, Struct.ChatType.CenterGm));
        }
    }
}
