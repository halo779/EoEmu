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
    /// Calculate maximum mana, hp, etc
    /// </summary>
    public partial class Calculation
    {
        public static void Vitals(ClientSocket CSocket, bool first)
        {
            if (!first)
            {
                CSocket.Client.MaxHP -= CSocket.Client.BaseHP;
                CSocket.Client.MaxMP -= CSocket.Client.BaseMP;
            }
            //HP
            double HP = 0;
            double HpFactor = 24;

            switch (CSocket.Client.Class)
            {
                case Struct.ClassType.Mage:
                    {
                        HpFactor = 20;
                        break;
                    }
                case Struct.ClassType.Warrior:
                    {
                        HpFactor = 30;
                        break;
                    }
                case Struct.ClassType.Palidin:
                    {
                        HpFactor = 25;
                        break;
                    }
            }

            HP += (CSocket.Client.Vitality * HpFactor);
            HP += (CSocket.Client.Dexterity * 3);

            //CSocket.Client.MaxHP = (int)Math.Floor(HP);
            CSocket.Client.BaseHP = (int)Math.Floor(HP);
            CSocket.Client.MaxHP += CSocket.Client.BaseHP;

            //Mana
            int Mana = 0;
            int multiplier = 5;

            Mana = CSocket.Client.Power * multiplier;
            CSocket.Client.BaseMP = Mana;
            CSocket.Client.MaxMP += CSocket.Client.BaseMP;
        }
    }
}
