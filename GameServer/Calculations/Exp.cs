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
    /// <summary>
    /// Calculates an amount of EXP to give to a player.
    /// </summary>
    public partial class Calculation
    {
        public static void GiveExp(ClientSocket CSocket, Monster Attacked, int Damage, bool kill)
        {
            int ExpToGive = 0;
            if (Damage > Attacked.MaxHP)
                ExpToGive = Attacked.MaxHP;
            else
                ExpToGive = Damage;
            if (kill)
            {
                if (CSocket.Client.Team != null)
                    TeamExp(Attacked, CSocket);
            }
            if (CSocket.Client.Level < 135)
            {
                if (Attacked.Level <= (CSocket.Client.Level - 5))
                {
                    int Lev = CSocket.Client.Level - Attacked.Level;
                    switch (Lev)
                    {
                        case 5:
                            {
                                ExpToGive = Convert.ToInt32(ExpToGive * 0.31);
                                break;
                            }
                        case 6:
                            {
                                ExpToGive = Convert.ToInt32(ExpToGive * 0.18);
                                break;
                            }
                        default:
                            {
                                ExpToGive = Convert.ToInt32(ExpToGive * 0.10);
                                break;
                            }

                    }
                }
                if (kill)
                {
                    ExpToGive += (Attacked.MaxHP / 20);
                }
                if (ExpToGive > 0)
                {
                    ExpToGive *= MainGS.EXP_MULTIPLER;
                    CSocket.Client.Exp += (ulong)ExpToGive;
                    if (NeededExp(CSocket.Client.Level) <= CSocket.Client.Exp)
                    {
                        GiveLevel(CSocket);
                    }
                    else
                    {
                        //CSocket.Send(EudemonPacket.Exp(CSocket.Client.ID, 5, CSocket.Client.Exp));
                        CSocket.Send(EudemonPacket.Status(CSocket, 2, CSocket.Client.Exp, Struct.StatusTypes.Exp));
                    }
                }
            }
        }
        public static void TeamExp(Monster Attacked, ClientSocket CSocket)
        {
            bool noobexp = false;
            if (MainGS.ClientPool.ContainsKey(CSocket.Client.Team.LeaderID))
            {
                ClientSocket Leader = MainGS.ClientPool[CSocket.Client.Team.LeaderID];
                foreach (KeyValuePair<int, ClientSocket> Member in Leader.Client.Team.Members)
                {
                    if ((Member.Value.Client.Level - Attacked.Level) < -20)
                    {
                        noobexp = true;
                        break;
                    }
                }
                foreach (KeyValuePair<int, ClientSocket> Member in Leader.Client.Team.Members)
                {
                    if (CanSee(Member.Value.Client.X, Member.Value.Client.Y, CSocket.Client.X, CSocket.Client.Y) && Member.Value.Client.Level < 135 && Member.Value.Client.ID != CSocket.Client.ID)
                    {
                        int LevelDiff = Member.Value.Client.Level - Attacked.Level;
                        int ExpToGive = 0;
                        if (LevelDiff <= -20)
                            ExpToGive = Attacked.Level * 30;
                        else
                            ExpToGive = (int)Math.Floor((double)Attacked.MaxHP / 20);
                        if (LevelDiff <= 10)
                            ExpToGive = (int)Math.Floor(ExpToGive * 1.3);
                        else if (LevelDiff <= -5)
                            ExpToGive = (int)Math.Floor(ExpToGive * 1.2);
                        if (noobexp)
                            ExpToGive *= 2;
                        if (CSocket.Client.Spouse == Member.Value.Client.Name)
                            ExpToGive *= 2;
                        if ((int)Member.Value.Client.Class >= 133 && (int)Member.Value.Client.Class <= 135)
                            ExpToGive *= 2;
                        if (ExpToGive > 0)
                        {
                            ExpToGive *= MainGS.EXP_MULTIPLER;
                            if (noobexp && ((int)Member.Value.Client.Class >= 133 && (int)Member.Value.Client.Class <= 135))
                                Member.Value.Send(EudemonPacket.Chat(0, "SYSTEM", Member.Value.Client.Name, "You gained " + ExpToGive + " exp from team killings, with additional exp for low level team-mates, and for being a water tao.", Struct.ChatType.System));
                            else if (!noobexp && ((int)Member.Value.Client.Class >= 133 && (int)Member.Value.Client.Class <= 135))
                                Member.Value.Send(EudemonPacket.Chat(0, "SYSTEM", Member.Value.Client.Name, "You gained " + ExpToGive + " exp from team killings, with additional exp for low level team-mate.", Struct.ChatType.System));
                            else
                                Member.Value.Send(EudemonPacket.Chat(0, "SYSTEM", Member.Value.Client.Name, "You gained " + ExpToGive + " exp from team killings.", Struct.ChatType.System));
                            Member.Value.Client.Exp += (ulong)ExpToGive;
                            if (NeededExp(Member.Value.Client.Level) <= Member.Value.Client.Exp)
                            {
                                GiveLevel(Member.Value);
                            }
                            else
                            {
                                Member.Value.Send(EudemonPacket.Status(Member.Value, 2, Member.Value.Client.Exp, Struct.StatusTypes.Exp));
                            }
                        }
                    }
                }
            }
        }
        public static void GiveExp(ClientSocket CSocket, Struct.TerrainNPC Attacked, int Damage)
        {
            int ExpToGive = Damage;
            if (CSocket.Client.Level < 135)
            {
                if (Attacked.Level <= (CSocket.Client.Level - 5))
                {
                    int Lev = CSocket.Client.Level - Attacked.Level;
                    switch (Lev)
                    {
                        case 5:
                            {
                                ExpToGive = Convert.ToInt32(ExpToGive * 0.31);
                                break;
                            }
                        case 6:
                            {
                                ExpToGive = Convert.ToInt32(ExpToGive * 0.18);
                                break;
                            }
                        default:
                            {
                                ExpToGive = Convert.ToInt32(ExpToGive * 0.10);
                                break;
                            }

                    }
                }
                if (ExpToGive > 0)
                {
                    ExpToGive = (ExpToGive / 3);
                    CSocket.Client.Exp += (ulong)ExpToGive;
                    if (NeededExp(CSocket.Client.Level) <= CSocket.Client.Exp)
                    {
                        GiveLevel(CSocket);
                    }
                    else
                    {
                        //CSocket.Send(EudemonPacket.Exp(CSocket.Client.ID, 5, CSocket.Client.Exp));
                        CSocket.Send(EudemonPacket.Status(CSocket, 2, CSocket.Client.Exp, Struct.StatusTypes.Exp));
                    }
                }
            }
        }
        public static void GiveLevel(ClientSocket CSocket)
        {
            CSocket.Client.Exp -= NeededExp(CSocket.Client.Level);
            CSocket.Client.Level++;
            CSocket.Client.CurrentHP = CSocket.Client.MaxHP;
            CSocket.Send(EudemonPacket.Status(CSocket, 2, CSocket.Client.CurrentHP, Struct.StatusTypes.Hp));
            EudemonPacket.ToLocal(EudemonPacket.Effect(CSocket.Client.ID, "LevelUp"), CSocket.Client.X, CSocket.Client.Y, (int)CSocket.Client.Map, 0, 0);
            CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "Congratulations! You have attained level " + CSocket.Client.Level + ". Keep going!", Struct.ChatType.System));
            CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "You have gained three attribute points! Use them wiesely!", Struct.ChatType.System));
            if (CSocket.Client.Level == 135)
                EudemonPacket.ToServer(EudemonPacket.Chat(0, "SYSTEM", "ALLUSERS", "CONGRATULATIONS! " + CSocket.Client.Name + " has just achieved level 135! Great job!", Struct.ChatType.NewBroadcast), 0);
            if (CSocket.Client.Level < 135)
            {
                if (CSocket.Client.Exp >= NeededExp(CSocket.Client.Level))
                {
                    //Multiple levels
                    GiveLevel(CSocket);
                }
                else
                {
                    CSocket.Send(EudemonPacket.Status(CSocket, 2, CSocket.Client.Level, Struct.StatusTypes.Level));
                    CSocket.Send(EudemonPacket.Status(CSocket, 2, CSocket.Client.Exp, Struct.StatusTypes.Exp));
                    //CSocket.Send(EudemonPacket.Exp(CSocket.Client.ID, 5, CSocket.Client.Exp));
                }
            }
        }
        public static ulong NeededExp(int Level)
        {
            if (Level == 1)
                return 39;
            else if (Level == 2)
                return 165;
            else if (Level == 3)
                return 165;
            else if (Level == 4)
                return 347;
            else if (Level == 5)
                return 627;
            else if (Level == 6)
                return 990;
            else if (Level == 7)
                return 1183;
            else if (Level == 8)
                return 2407;
            else if (Level == 9)
                return 3679;
            else if (Level == 10)
                return 8341;
            else if (Level == 11)
                return 11996;
            else if (Level == 12)
                return 14429;
            else if (Level == 13)
                return 18043;
            else if (Level == 14)
                return 21612;
            else if (Level == 15)
                return 22596;
            else if (Level == 16)
                return 32217;
            else if (Level == 17)
                return 37480;
            else if (Level == 18)
                return 47573;
            else if (Level == 19)
                return 56704;
            else if (Level == 20)
                return 68789;
            else if (Level == 21)
                return 70451;
            else if (Level == 22)
                return 75923;
            else if (Level == 23)
                return 97776;
            else if (Level == 24)
                return 114826;
            else if (Level == 25)
                return 120892;
            else if (Level == 26)
                return 123980;
            else if (Level == 27)
                return 126799;
            else if (Level == 28)
                return 145811;
            else if (Level == 29)
                return 173384;
            else if (Level == 30)
                return 197651;
            else if (Level == 31)
                return 202490;
            else if (Level == 32)
                return 212172;
            else if (Level == 33)
                return 244204;
            else if (Level == 34)
                return 285805;
            else if (Level == 35)
                return 305949;
            else if (Level == 36)
                return 312881;
            else if (Level == 37)
                return 324575;
            else if (Level == 38)
                return 366153;
            else if (Level == 39)
                return 434023;
            else if (Level == 40)
                return 460573;
            else if (Level == 41)
                return 506713;
            else if (Level == 42)
                return 570008;
            else if (Level == 43)
                return 728546;
            else if (Level == 44)
                return 850828;
            else if (Level == 45)
                return 916402;
            else if (Level == 46)
                return 935051;
            else if (Level == 47)
                return 940860;
            else if (Level == 48)
                return 1076590;
            else if (Level == 49)
                return 1272807;
            else if (Level == 50)
                return 1357986;
            else if (Level == 51)
                return 1384873;
            else if (Level == 52)
                return 1478420;
            else if (Level == 53)
                return 1632489;
            else if (Level == 54)
                return 1903121;
            else if (Level == 55)
                return 2065957;
            else if (Level == 56)
                return 2104909;
            else if (Level == 57)
                return 1921149;
            else if (Level == 58)
                return 2417153;
            else if (Level == 59)
                return 2853501;
            else if (Level == 60)
                return 3054580;
            else if (Level == 61)
                return 3111200;
            else if (Level == 62)
                return 3225607;
            else if (Level == 63)
                return 3811037;
            else if (Level == 64)
                return 4437965;
            else if (Level == 65)
                return 4880615;
            else if (Level == 66)
                return 4970959;
            else if (Level == 67)
                return 5107243;
            else if (Level == 68)
                return 5652526;
            else if (Level == 69)
                return 6579184;
            else if (Level == 70)
                return 6878005;
            else if (Level == 71)
                return 7100739;
            else if (Level == 72)
                return 7157642;
            else if (Level == 73)
                return 9106931;
            else if (Level == 74)
                return 10596415;
            else if (Level == 75)
                return 11220485;
            else if (Level == 76)
                return 11409179;
            else if (Level == 77)
                return 11424043;
            else if (Level == 78)
                return 12882966;
            else if (Level == 79)
                return 15172842;
            else if (Level == 80)
                return 15896985;
            else if (Level == 81)
                return 16163738;
            else if (Level == 82)
                return 16800069;
            else if (Level == 83)
                return 19230324;
            else if (Level == 84)
                return 22365189;
            else if (Level == 85)
                return 23819291;
            else if (Level == 86)
                return 24219524;
            else if (Level == 87)
                return 24864054;
            else if (Level == 88)
                return 27200095;
            else if (Level == 89)
                return 32033236;
            else if (Level == 90)
                return 33723786;
            else if (Level == 91)
                return 34291244;
            else if (Level == 92)
                return 34944017;
            else if (Level == 93)
                return 39463459;
            else if (Level == 94)
                return 45878550;
            else if (Level == 95)
                return 48924263;
            else if (Level == 96)
                return 49729242;
            else if (Level == 97)
                return 51072047;
            else if (Level == 98)
                return 55808382;
            else if (Level == 99)
                return 64870117;
            else if (Level == 100)
                return 68391872;
            else if (Level == 101)
                return 69537082;
            else if (Level == 102)
                return 76422949;
            else if (Level == 103)
                return 96950832;
            else if (Level == 104)
                return 112676761;
            else if (Level == 105)
                return 120090440;
            else if (Level == 106)
                return 121798300;
            else if (Level == 107)
                return 127680095;
            else if (Level == 108)
                return 137446904;
            else if (Level == 109)
                return 193716061;
            else if (Level == 110)
                return 408832135;
            else if (Level == 111)
                return 454674621;
            else if (Level == 112)
                return 461125840;
            else if (Level == 113)
                return 469189848;
            else if (Level == 114)
                return 477253857;
            else if (Level == 115)
                return 480479444;
            else if (Level == 116)
                return 485317884;
            else if (Level == 117)
                return 493381812;
            else if (Level == 118)
                return 580579979;
            else if (Level == 119)
                return 717424993;
            else if (Level == 120)
                return 282274071;
            else if (Level == 121)
                return 338728845;
            else if (Level == 122)
                return 406474656;
            else if (Level == 123)
                return 487769554;
            else if (Level == 124)
                return 585323469;
            else if (Level == 125)
                return 702388103;
            else if (Level == 126)
                return 842865806;
            else if (Level == 127)
                return 1011439064;
            else if (Level == 128)
                return 1073741808;
            else if (Level == 129)
                return 1073741759;
            else if (Level == 130)
                return 8589134588;
            else if (Level == 131)
                return 25767403764;
            else if (Level == 132)
                return 77302211292;
            else if (Level == 133)
                return 231906633876;
            else if (Level == 134)
                return 347859950814;
            else
                return 1;
        }
    }
}
