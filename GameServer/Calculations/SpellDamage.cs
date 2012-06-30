using System;
using System.Collections;
using System.Collections.Generic;
using GameServer.Connections;
using GameServer.Entities;
using GameServer.Structs;
using GameServer.Packets;
using GameServer.Calculations;
using GameServer.Database;

namespace GameServer.Calculations
{
    public partial class Calculation
    {
        /// <summary>
        /// Calculates magical damage per spell
        /// </summary>
        /// <param name="ID">SpellID</param>
        /// <param name="Level">Spell Level</param>
        /// <returns>Base Magic Damage for that spell/level, or 0 if unknown</returns>
        public static int AddedMagicDamage(int ID, int Level)
        {
            switch (ID)
            {
                case 1002://Tornado
                    {
                        if (Level == 0)
                            return 505;
                        if (Level == 1)
                            return 666;
                        if (Level == 2)
                            return 882;
                        if (Level == 3)
                            return 1166;
                        break;
                    }
                case 1000://Thunder
                    {
                        if (Level == 0)
                            return 7;
                        if (Level == 1)
                            return 16;
                        if (Level == 2)
                            return 32;
                        if (Level == 3)
                            return 57;
                        if (Level == 4)
                            return 86;
                        break;
                    }
                case 1001://Fire
                    {
                        if (Level == 0)
                            return 130;
                        if (Level == 1)
                            return 189;
                        if (Level == 2)
                            return 275;
                        if (Level == 3)
                            return 380;
                        break;
                    }
                case 1150://Fire Ring/Ball
                    {
                        if (Level == 0)
                            return 378;
                        if (Level == 1)
                            return 550;
                        if (Level == 2)
                            return 760;
                        if (Level == 3)
                            return 1010;
                        if (Level == 4)
                            return 1332;
                        if (Level == 5)
                            return 1764;
                        if (Level == 6)
                            return 2332;
                        if (Level == 7)
                            return 2800;
                        break;
                    }
                case 1180://Fire Meteor
                    {
                        if (Level == 0)
                            return 760;
                        if (Level == 1)
                            return 1040;
                        if (Level == 2)
                            return 1250;
                        if (Level == 3)
                            return 1480;
                        if (Level == 4)
                            return 1810;
                        if (Level == 5)
                            return 2210;
                        if (Level == 6)
                            return 2700;
                        if (Level == 7)
                            return 3250;
                        break;
                    }
                case 1160: //Bomb
                    {
                        if (Level == 0)
                            return 855;
                        if (Level == 1)
                            return 1498;
                        if (Level == 2)
                            return 1985;
                        if (Level == 3)
                            return 2623;
                        break;
                    }
                default:
                    {
                        return 0;
                    }
            }
            return 0;
        }
    }
}
