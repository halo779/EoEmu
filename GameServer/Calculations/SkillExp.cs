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
        /// Calculates and adds experience to skills in conquer online.
        /// </summary>
        /// <param name="SkillID">The skill being leveled.</param>
        /// <param name="CSocket">The clientsocket using the skill</param>
        /// <param name="AddExp">The exp to add.</param>
        public static void SkillExp(int SkillID, ClientSocket CSocket, int AddExp)
        {
            if (CSocket.Client.Skills.ContainsKey(SkillID))
            {
                Struct.CharSkill Skill = CSocket.Client.Skills[SkillID];
                if (Nano.ServerSkills.ContainsKey(SkillID))
                {
                    Struct.ServerSkill ServerSkill = Nano.ServerSkills[SkillID];
                    if (Skill.Level < ServerSkill.MaxLevel)
                    {
                        if (ServerSkill.RequiredExp.ContainsKey(Skill.Level))
                        {
                            int[] LevelExp = ServerSkill.RequiredExp[Skill.Level];
                            int ReqExp = LevelExp[0];
                            int ReqLevel = LevelExp[1];
                            if (CSocket.Client.Level >= ReqLevel)
                            {
                                AddExp *= Nano.SKILL_MULTIPLER;
                                if (AddExp > 0)
                                {
                                    Skill.Exp += (uint)AddExp;
                                    if (Skill.Exp >= ReqExp)
                                    {
                                        Skill.Level++;
                                        Skill.Exp -= (uint)ReqExp;
                                        CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "Congratulations! One of your skill's level just increased!", Struct.ChatType.Top));
                                        CSocket.Send(ConquerPacket.Skill(Skill.ID, Skill.Level, Skill.Exp));
                                        Database.Database.SetSkill(Skill.ID, Skill.Level, Skill.Exp, CSocket.Client.ID, true);
                                    }
                                    else
                                    {
                                        CSocket.Send(ConquerPacket.Skill(Skill.ID, Skill.Level, Skill.Exp));
                                        Database.Database.SetSkill(Skill.ID, Skill.Level, Skill.Exp, CSocket.Client.ID, true);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
