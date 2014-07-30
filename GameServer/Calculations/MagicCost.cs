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
        /// Provides checks for mana costs for all spells / levels
        /// </summary>
        /// <param name="MagicID">The spell ID to check for</param>
        /// <param name="MagicLevel">The level of MagicID to check for</param>
        /// <param name="CSocket">The Attacking ClientSocket</param>
        /// <returns></returns>
        public static bool MagicCost(int MagicID, int MagicLevel, ClientSocket CSocket)
        {
            if ((CSocket.Client.isPM || CSocket.Client.isGM) && CSocket.Client.Invincible)
                return true;
            switch (MagicID)
            {
                case 1000://Thunder - MP Check
                    {
                        int ManaCost = 0;
                        if (MagicLevel == 0)
                            ManaCost = 1;
                        else if (MagicLevel == 1)
                            ManaCost = 6;
                        else if (MagicLevel == 2)
                            ManaCost = 10;
                        else if (MagicLevel == 3)
                            ManaCost = 11;
                        else if (MagicLevel == 4)
                            ManaCost = 17;
                        if (CSocket.Client.CurrentMP < ManaCost)
                        {
                            CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Not enough mana!", Struct.ChatType.System));
                            return false;
                        }
                        else
                        {
                            CSocket.Client.CurrentMP -= ManaCost;
                            CSocket.Send(EudemonPacket.Status(CSocket, 2, CSocket.Client.CurrentMP, Struct.StatusTypes.Mp));
                            return true;
                        }
                    }
                case 1001://Fire - MP Check
                    {
                        int ManaCost = 0;
                        if (MagicLevel == 0)
                            ManaCost = 21;
                        else if (MagicLevel == 1)
                            ManaCost = 21;
                        else if (MagicLevel == 2)
                            ManaCost = 28;
                        else if (MagicLevel == 3)
                            ManaCost = 38;
                        if (CSocket.Client.CurrentMP < ManaCost)
                        {
                            CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Not enough mana!", Struct.ChatType.System));
                            return false;
                        }
                        else
                        {
                            CSocket.Client.CurrentMP -= ManaCost;
                            CSocket.Send(EudemonPacket.Status(CSocket, 2, CSocket.Client.CurrentMP, Struct.StatusTypes.Mp));
                            return true;
                        }
                    }
                case 1150://FireRing - MP Check
                    {
                        int ManaCost = 0;
                        if (MagicLevel == 0)
                            ManaCost = 33;
                        else if (MagicLevel == 1)
                            ManaCost = 33;
                        else if (MagicLevel == 2)
                            ManaCost = 46;
                        else if (MagicLevel == 3)
                            ManaCost = 53;
                        else if (MagicLevel == 4)
                            ManaCost = 53;
                        else if (MagicLevel == 5)
                            ManaCost = 60;
                        else if (MagicLevel == 6)
                            ManaCost = 82;
                        else if (MagicLevel == 7)
                            ManaCost = 105;
                        if (CSocket.Client.CurrentMP < ManaCost)
                        {
                            CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Not enough mana!", Struct.ChatType.System));
                            return false;
                        }
                        else
                        {
                            CSocket.Client.CurrentMP -= ManaCost;
                            CSocket.Send(EudemonPacket.Status(CSocket, 2, CSocket.Client.CurrentMP, Struct.StatusTypes.Mp));
                            return true;
                        }
                    }
                case 1180://FireMet - MP Check
                    {
                        int ManaCost = 0;
                        if (MagicLevel == 0)
                            ManaCost = 62;
                        else if (MagicLevel == 1)
                            ManaCost = 74;
                        else if (MagicLevel == 2)
                            ManaCost = 115;
                        else if (MagicLevel == 3)
                            ManaCost = 130;
                        else if (MagicLevel == 4)
                            ManaCost = 150;
                        else if (MagicLevel == 5)
                            ManaCost = 215;
                        else if (MagicLevel == 6)
                            ManaCost = 285;
                        else if (MagicLevel == 7)
                            ManaCost = 300;
                        if (CSocket.Client.CurrentMP < ManaCost)
                        {
                            CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Not enough mana!", Struct.ChatType.System));
                            return false;
                        }
                        else
                        {
                            CSocket.Client.CurrentMP -= ManaCost;
                            CSocket.Send(EudemonPacket.Status(CSocket, 2, CSocket.Client.CurrentMP, Struct.StatusTypes.Mp));
                            return true;
                        }
                    }
                case 1120://FireCircle - MP Check
                    {
                        int ManaCost = 0;
                        if (MagicLevel == 0)
                            ManaCost = 150;
                        else if (MagicLevel == 1)
                            ManaCost = 170;
                        else if (MagicLevel == 2)
                            ManaCost = 190;
                        else if (MagicLevel == 3)
                            ManaCost = 210;
                        if (CSocket.Client.CurrentMP < ManaCost)
                        {
                            CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Not enough mana!", Struct.ChatType.System));
                            return false;
                        }
                        else
                        {
                            CSocket.Client.CurrentMP -= ManaCost;
                            CSocket.Send(EudemonPacket.Status(CSocket, 2, CSocket.Client.CurrentMP, Struct.StatusTypes.Mp));
                            return true;
                        }
                    }
                case 1002: //Tornado - MP Check
                    {
                        int ManaCost = 0;
                        if (MagicLevel == 0)
                            ManaCost = 32;
                        else if (MagicLevel == 1)
                            ManaCost = 36;
                        else if (MagicLevel == 2)
                            ManaCost = 50;
                        else if (MagicLevel == 3)
                            ManaCost = 64;
                        if (CSocket.Client.CurrentMP < ManaCost)
                        {
                            CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Not enough mana!", Struct.ChatType.System));
                            return false;
                        }
                        else
                        {
                            CSocket.Client.CurrentMP -= ManaCost;
                            CSocket.Send(EudemonPacket.Status(CSocket, 2, CSocket.Client.CurrentMP, Struct.StatusTypes.Mp));
                            return true;
                        }
                    }
                case 1160://Bomb - MP Check
                    {
                        int ManaCost = 0;
                        if (MagicLevel == 0)
                            ManaCost = 53;
                        else if (MagicLevel == 1)
                            ManaCost = 60;
                        else if (MagicLevel == 2)
                            ManaCost = 82;
                        else if (MagicLevel == 3)
                            ManaCost = 105;
                        if (CSocket.Client.CurrentMP < ManaCost)
                        {
                            CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Not enough mana!", Struct.ChatType.System));
                            return false;
                        }
                        else
                        {
                            CSocket.Client.CurrentMP -= ManaCost;
                            CSocket.Send(EudemonPacket.Status(CSocket, 2, CSocket.Client.CurrentMP, Struct.StatusTypes.Mp));
                            return true;
                        }
                    }
                case 1055: //Healing Rain - MP Check
                    {
                        int ManaCost = 0;
                        if (MagicLevel == 0)
                            ManaCost = 150;
                        else if (MagicLevel == 1)
                            ManaCost = 270;
                        else if (MagicLevel == 2)
                            ManaCost = 375;
                        else if (MagicLevel == 3)
                            ManaCost = 440;
                        else if (MagicLevel == 4)
                            ManaCost = 500;
                        if (CSocket.Client.CurrentMP < ManaCost)
                        {
                            CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Not enough mana!", Struct.ChatType.System));
                            return false;
                        }
                        else
                        {
                            CSocket.Client.CurrentMP -= ManaCost;
                            CSocket.Send(EudemonPacket.Status(CSocket, 2, CSocket.Client.CurrentMP, Struct.StatusTypes.Mp));
                            return true;
                        }
                    }
                case 1085://SoA - MP Check
                    {
                        int ManaCost = 0;
                        if (MagicLevel == 0)
                            ManaCost = 200;
                        else if (MagicLevel == 1)
                            ManaCost = 250;
                        else if (MagicLevel == 2)
                            ManaCost = 300;
                        else if (MagicLevel == 3)
                            ManaCost = 350;
                        else if (MagicLevel == 4)
                            ManaCost = 400;
                        if (CSocket.Client.CurrentMP < ManaCost)
                        {
                            CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Not enough mana!", Struct.ChatType.System));
                            return false;
                        }
                        else
                        {
                            CSocket.Client.CurrentMP -= ManaCost;
                            CSocket.Send(EudemonPacket.Status(CSocket, 2, CSocket.Client.CurrentMP, Struct.StatusTypes.Mp));
                            return true;
                        }
                    }
                case 1090://Magic Shield - MP Check
                    {
                        int ManaCost = 0;
                        if (MagicLevel == 0)
                            ManaCost = 200;
                        else if (MagicLevel == 1)
                            ManaCost = 250;
                        else if (MagicLevel == 2)
                            ManaCost = 300;
                        else if (MagicLevel == 3)
                            ManaCost = 350;
                        else if (MagicLevel == 4)
                            ManaCost = 400;
                        if (CSocket.Client.CurrentMP < ManaCost)
                        {
                            CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Not enough mana!", Struct.ChatType.System));
                            return false;
                        }
                        else
                        {
                            CSocket.Client.CurrentMP -= ManaCost;
                            CSocket.Send(EudemonPacket.Status(CSocket, 2, CSocket.Client.CurrentMP, Struct.StatusTypes.Mp));
                            return true;
                        }
                    }
                case 1075: // Invis - MP Check
                    {
                        int ManaCost = 0;
                        if (MagicLevel == 0)
                            ManaCost = 200;
                        else if (MagicLevel == 1)
                            ManaCost = 250;
                        else if (MagicLevel == 2)
                            ManaCost = 300;
                        else if (MagicLevel == 3)
                            ManaCost = 330;
                        else if (MagicLevel == 4)
                            ManaCost = 360;
                        if (CSocket.Client.CurrentMP < ManaCost)
                        {
                            CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Not enough mana!", Struct.ChatType.System));
                            return false;
                        }
                        else
                        {
                            CSocket.Client.CurrentMP -= ManaCost;
                            CSocket.Send(EudemonPacket.Status(CSocket, 2, CSocket.Client.CurrentMP, Struct.StatusTypes.Mp));
                            return true;
                        }
                    }
                case 1100:
                    {
                        int ManaCost = 1000;
                        if (CSocket.Client.CurrentMP < ManaCost)
                        {
                            CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Not enough mana!", Struct.ChatType.System));
                            return false;
                        }
                        else
                        {
                            CSocket.Client.CurrentMP -= ManaCost;
                            CSocket.Send(EudemonPacket.Status(CSocket, 2, CSocket.Client.CurrentMP, Struct.StatusTypes.Mp));
                            return true;
                        }
                    }
                case 1005:
                    {
                        int ManaCost = 0;
                        switch (MagicLevel)
                        {
                            case 0:
                                {
                                    ManaCost = 10;
                                    break;
                                }
                            case 1:
                                {
                                    ManaCost = 30;
                                    break;
                                }
                            case 2:
                                {
                                    ManaCost = 60;
                                    break;
                                }
                            case 3:
                                {
                                    ManaCost = 100;
                                    break;
                                }
                            case 4:
                                {
                                    ManaCost = 130;
                                    break;
                                }
                            default:
                                {
                                    break;
                                }
                        }
                        if (CSocket.Client.CurrentMP < ManaCost)
                        {
                            CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Not enough mana!", Struct.ChatType.System));
                            return false;
                        }
                        else
                        {
                            CSocket.Client.CurrentMP -= ManaCost;
                            CSocket.Send(EudemonPacket.Status(CSocket, 2, CSocket.Client.CurrentMP, Struct.StatusTypes.Mp));
                            return true;
                        }
                    }
                case 1170:
                    {
                        int ManaCost = 0;
                        switch (MagicLevel)
                        {
                            case 0:
                                {
                                    ManaCost = 600;
                                    break;
                                }
                            case 1:
                                {
                                    ManaCost = 660;
                                    break;
                                }
                            case 2:
                                {
                                    ManaCost = 720;
                                    break;
                                }
                            case 3:
                                {
                                    ManaCost = 770;
                                    break;
                                }
                            case 4:
                                {
                                    ManaCost = 820;
                                    break;
                                }
                            default:
                                {
                                    break;
                                }
                        }
                        if (CSocket.Client.CurrentMP < ManaCost)
                        {
                            CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Not enough mana!", Struct.ChatType.System));
                            return false;
                        }
                        else
                        {
                            CSocket.Client.CurrentMP -= ManaCost;
                            CSocket.Send(EudemonPacket.Status(CSocket, 2, CSocket.Client.CurrentMP, Struct.StatusTypes.Mp));
                            return true;
                        }
                    }
                case 1175:
                    {
                        int ManaCost = 0;
                        switch (MagicLevel)
                        {
                            case 0:
                                {
                                    ManaCost = 160;
                                    break;
                                }
                            case 1:
                                {
                                    ManaCost = 190;
                                    break;
                                }
                            case 2:
                                {
                                    ManaCost = 215;
                                    break;
                                }
                            case 3:
                                {
                                    ManaCost = 235;
                                    break;
                                }
                            case 4:
                                {
                                    ManaCost = 255;
                                    break;
                                }
                            default:
                                {
                                    break;
                                }
                        }
                        if (CSocket.Client.CurrentMP < ManaCost)
                        {
                            CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Not enough mana!", Struct.ChatType.System));
                            return false;
                        }
                        else
                        {
                            CSocket.Client.CurrentMP -= ManaCost;
                            CSocket.Send(EudemonPacket.Status(CSocket, 2, CSocket.Client.CurrentMP, Struct.StatusTypes.Mp));
                            return true;
                        }
                    }
                case 1095:
                    {
                        int ManaCost = 0;
                        if (MagicLevel == 0)
                            ManaCost = 200;
                        if (MagicLevel == 1)
                            ManaCost = 250;
                        if (MagicLevel == 2)
                            ManaCost = 300;
                        if (MagicLevel == 3)
                            ManaCost = 350;
                        if (MagicLevel == 4)
                            ManaCost = 400;
                        if (CSocket.Client.CurrentMP < ManaCost)
                        {
                            CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Not enough mana!", Struct.ChatType.System));
                            return false;
                        }
                        else
                        {
                            CSocket.Client.CurrentMP -= ManaCost;
                            CSocket.Send(EudemonPacket.Status(CSocket, 2, CSocket.Client.CurrentMP, Struct.StatusTypes.Mp));
                            return true;
                        }
                    }
                case 6000:
                    {
                        int StamCost = 30;
                        if (CSocket.Client.CurrentStam < StamCost)
                        {
                            CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Not enough stamina!", Struct.ChatType.System));
                            return false;
                        }
                        else
                        {
                            CSocket.Client.CurrentStam -= StamCost;
                            CSocket.Send(EudemonPacket.Status(CSocket, 2, CSocket.Client.CurrentStam, Struct.StatusTypes.Stamina));
                            return true;
                        }
                    }
                case 1165:
                    {
                        int ManaCost = 0;
                        if (MagicLevel == 0)
                        {
                            ManaCost = 120;
                        }
                        if (MagicLevel == 1)
                        {
                            ManaCost = 150;
                        }
                        if (MagicLevel == 2)
                        {
                            ManaCost = 180;
                        }
                        if (MagicLevel == 3)
                        {
                            ManaCost = 210;
                        }
                        if (CSocket.Client.CurrentMP < ManaCost)
                        {
                            CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Not enough mana!", Struct.ChatType.System));
                            return false;
                        }
                        else
                        {
                            CSocket.Client.CurrentMP -= ManaCost;
                            CSocket.Send(EudemonPacket.Status(CSocket, 2, CSocket.Client.CurrentMP, Struct.StatusTypes.Mp));
                            return true;
                        }
                    }
                case 1045:
                    {
                        int StamCost = 22;
                        if (CSocket.Client.CurrentStam < StamCost)
                        {
                            CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Not enough stamina!", Struct.ChatType.System));
                            return false;
                        }
                        else
                        {
                            CSocket.Client.CurrentStam -= StamCost;
                            CSocket.Send(EudemonPacket.Status(CSocket, 2, CSocket.Client.CurrentStam, Struct.StatusTypes.Stamina));
                            return true;
                        }
                    }
                case 1046:
                    {
                        int StamCost = 22;
                        if (CSocket.Client.CurrentStam < StamCost)
                        {
                            CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Not enough stamina!", Struct.ChatType.System));
                            return false;
                        }
                        else
                        {
                            CSocket.Client.CurrentStam -= StamCost;
                            CSocket.Send(EudemonPacket.Status(CSocket, 2, CSocket.Client.CurrentStam, Struct.StatusTypes.Stamina));
                            return true;
                        }
                    }
                case 1115:
                    {
                        int StamCost = 33;
                        if (CSocket.Client.CurrentStam < StamCost)
                        {
                            CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Not enough stamina!", Struct.ChatType.System));
                            return false;
                        }
                        else
                        {
                            CSocket.Client.CurrentStam -= StamCost;
                            CSocket.Send(EudemonPacket.Status(CSocket, 2, CSocket.Client.CurrentStam, Struct.StatusTypes.Stamina));
                            return true;
                        }
                    }

            }
            return true;
        }
    }
}
