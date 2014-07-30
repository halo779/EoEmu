using System;
using System.Collections;
using System.Collections.Generic;
using GameServer.Connections;
using GameServer.Entities;
using GameServer.Structs;
using GameServer.Packets;
using GameServer.Calculations;
using System.Threading;


namespace GameServer.Handlers
{
    /// <summary>
    /// Description of Attack.
    /// </summary>
    public partial class Handler
    {
        public static void Attack(int Target, int MagicID, int AType, int X, int Y, ClientSocket CSocket)
        {
            #region Checks
            Struct.CharSkill AttackSkill = null;
            if (MagicID != 0)
            {
                if (!CSocket.Client.Skills.ContainsKey(MagicID))
                {
                    CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You do not have the skill.", Struct.ChatType.System));
                    return;
                }
                else
                {
                    AttackSkill = CSocket.Client.Skills[MagicID];
                }
            }
            if (CSocket.Client.Dead)
            {
                if (CSocket.Client.Attack != null)
                {
                    CSocket.Client.Attack.Stop();
                    CSocket.Client.Attack.Dispose();
                }
                return;
            }
            #endregion
            Character Attacker = CSocket.Client;
            Character AttackedChar = null;
            ClientSocket ASocket = null;
            Monster AttackedMob = null;
            Struct.TerrainNPC AttackedTNpc = null;
            if (CSocket.Client.Equipment.ContainsKey(4))
            {
                if (CSocket.Client.Equipment[4].ItemID != 1050002 && CSocket.Client.Equipment[4].ItemID != 1050001 & CSocket.Client.Equipment[4].ItemID != 1050000)
                {
                    int ID = Calculation.WeaponType(Convert.ToString(CSocket.Client.Equipment[4].ItemID));
                }
            }
            if (CSocket.Client.Equipment.ContainsKey(5))
            {
                if (CSocket.Client.Equipment[5].ItemID != 1050002 && CSocket.Client.Equipment[5].ItemID != 1050001 & CSocket.Client.Equipment[5].ItemID != 1050000)
                {
                    int ID = Calculation.WeaponType(Convert.ToString(CSocket.Client.Equipment[5].ItemID));
                }
            }
            if (Nano.ClientPool.ContainsKey(Target))
            {
                ASocket = Nano.ClientPool[Target];
                AttackedChar = Nano.ClientPool[Target].Client;
            }
            else if (Nano.Monsters.ContainsKey(Target))
                AttackedMob = Nano.Monsters[Target];
            else if (Nano.TerrainNpcs.ContainsKey(Target))
                AttackedTNpc = Nano.TerrainNpcs[Target];
            else if (Target > 0)
            {
                //CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Target not found.", Struct.ChatType.Top));
                return;
            }
            CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "Target: " + ASocket.Client.Name + "Max HP: " + ASocket.Client.MaxHP.ToString() + "Current HP: " + ASocket.Client.CurrentHP , Struct.ChatType.System));
                
            if (AttackedChar != null)
            {
                if (!CheckMode(CSocket, ASocket))
                {
                    if (CSocket.Client.Attack != null)
                    {
                        CSocket.Client.Attack.Stop();
                        CSocket.Client.Attack.Dispose();
                    }
                    return;
                }
            }
            int Damage = 0;
            #region Melee
            if (AType == 2)
            {
                if (AttackedChar != null && !AttackedChar.Dead)
                {
                    if (!Calculation.InRange(AttackedChar.X, AttackedChar.Y, CSocket.Client.X, CSocket.Client.Y, 2))
                    {
                        if (CSocket.Client.Attack != null)
                        {
                            if (CSocket.Client.Attack.Enabled)
                            {
                                CSocket.Client.Attack.Stop();
                                CSocket.Client.Attack.Dispose();
                            }
                        }
                        return;
                    }
                    Damage = Calculation.Damage(Attacker, AttackedChar, AType, 0, 0);
                    Damage = 5;
                    bool killed = Calculation.doPlayer(CSocket, ASocket, Damage, AType);
                    if (!killed)
                    {
                        if (CSocket.Client.Attack != null)
                        {
                            if (!CSocket.Client.Attack.Enabled)
                            {
                                CSocket.Client.Attack = new System.Timers.Timer();
                                CSocket.Client.Attack.Interval = 900;
                                CSocket.Client.Attack.Elapsed += delegate { Attack(Target, MagicID, AType, X, Y, CSocket); };
                                CSocket.Client.Attack.Start();
                            }
                        }
                        else
                        {
                            CSocket.Client.Attack = new System.Timers.Timer();
                            CSocket.Client.Attack.Interval = 900;
                            CSocket.Client.Attack.Elapsed += delegate { Attack(Target, MagicID, AType, X, Y, CSocket); };
                            CSocket.Client.Attack.Start();
                        }
                    }
                }
                else if (AttackedMob != null)
                {
                    if (!Calculation.InRange(AttackedMob.X, AttackedMob.Y, CSocket.Client.X, CSocket.Client.Y, 2))
                    {
                        if (CSocket.Client.Attack != null)
                        {
                            if (CSocket.Client.Attack.Enabled)
                            {
                                CSocket.Client.Attack.Stop();
                                CSocket.Client.Attack.Dispose();
                            }
                        }
                        return;
                    }
                    int RemainingHP = AttackedMob.CurrentHP;
                    Damage = Calculation.Damage(Attacker, AttackedMob, AType, 0, 0);
                    bool killed = Calculation.doMonster(AttackedMob, Damage, AType, CSocket);
                    if (!killed)
                    {
                        if (CSocket.Client.Attack != null)
                        {
                            if (!CSocket.Client.Attack.Enabled)
                            {
                                CSocket.Client.Attack = new System.Timers.Timer();
                                CSocket.Client.Attack.Interval = 900;
                                CSocket.Client.Attack.Elapsed += delegate { Attack(Target, MagicID, AType, X, Y, CSocket); };
                                CSocket.Client.Attack.Start();
                            }
                        }
                        else
                        {
                            CSocket.Client.Attack = new System.Timers.Timer();
                            CSocket.Client.Attack.Interval = 900;
                            CSocket.Client.Attack.Elapsed += delegate { Attack(Target, MagicID, AType, X, Y, CSocket); };
                            CSocket.Client.Attack.Start();
                        }
                    }
                    else
                    {

                    }
                }
                else if (AttackedTNpc != null)
                {
                    if (!Calculation.InRange(AttackedTNpc.X, AttackedTNpc.Y, CSocket.Client.X, CSocket.Client.Y, 16))
                    {
                        if (CSocket.Client.Attack != null)
                        {
                            if (CSocket.Client.Attack.Enabled)
                            {
                                CSocket.Client.Attack.Stop();
                                CSocket.Client.Attack.Dispose();
                            }
                        }
                        return;
                    }
                    if (CSocket.Client.MinAttack < CSocket.Client.MaxAttack)
                        Damage = Nano.Rand.Next(CSocket.Client.MinAttack, CSocket.Client.MaxAttack);
                    Calculation.doTNpc(CSocket, AttackedTNpc, Damage, AType);

                    if (CSocket.Client.Attack != null)
                    {
                        if (!CSocket.Client.Attack.Enabled)
                        {
                            CSocket.Client.Attack = new System.Timers.Timer();
                            CSocket.Client.Attack.Interval = 900;
                            CSocket.Client.Attack.Elapsed += delegate { Attack(Target, MagicID, AType, X, Y, CSocket); };
                            CSocket.Client.Attack.Start();
                        }
                    }
                    else
                    {
                        CSocket.Client.Attack = new System.Timers.Timer();
                        CSocket.Client.Attack.Interval = 900;
                        CSocket.Client.Attack.Elapsed += delegate { Attack(Target, MagicID, AType, X, Y, CSocket); };
                        CSocket.Client.Attack.Start();
                    }
                }
            }
            #endregion
            #region Magic
            else if (AType == 21)
            {
                if (Target == 0 && MagicID != 0)
                {
                    //Non-targeting magical attack(scatter, fb, etc)
                    NoTargetMagic(CSocket, MagicID, X, Y);
                }
                else if (Target > 0 && MagicID != 0 && AttackedChar != null)
                {
                    if (AttackSkill != null)
                    {
                        bool OK = Calculation.MagicCost(AttackSkill.ID, AttackSkill.Level, CSocket);
                        if (!OK)
                        {
                            if (CSocket.Client.Attack != null)
                            {
                                if (CSocket.Client.Attack.Enabled)
                                {
                                    CSocket.Client.Attack.Stop();
                                    CSocket.Client.Attack.Dispose();
                                }
                            }
                            return;
                        }
                        switch (AttackSkill.ID)
                        {
                            //TODO: Special effects for spells like FireOfHell, Revive, etc here
                            case 1100://Revive!
                                {
                                    if (ASocket.Client.Dead)
                                    {
                                        ASocket.Client.CurrentHP = ASocket.Client.MaxHP;
                                        ASocket.Client.Dead = false;
                                        ASocket.Send(EudemonPacket.Status(ASocket, 2, 0, Struct.StatusTypes.StatusEffect));
                                        ASocket.Send(EudemonPacket.Status(ASocket, 2, ASocket.Client.Model, Struct.StatusTypes.Model));
                                        ASocket.Send(EudemonPacket.Status(ASocket, 2, ASocket.Client.CurrentHP, Struct.StatusTypes.Hp));
                                        EudemonPacket.ToLocal(EudemonPacket.General(ASocket.Client.ID, ASocket.Client.X, ASocket.Client.Y, 0, 0, 0, Struct.DataType.EntityRemove), ASocket.Client.X, ASocket.Client.Y, (int)ASocket.Client.Map, 0, ASocket.Client.ID);
                                        EudemonPacket.ToLocal(EudemonPacket.SpawnCharacter(ASocket), ASocket.Client.X, ASocket.Client.Y, (int)ASocket.Client.Map, 0, 0);
                                    }
                                    Damage = 0;
                                    Dictionary<int, int> Targets = new Dictionary<int, int>();
                                    Targets.Add(AttackedChar.ID, Damage);
                                    EudemonPacket.ToLocal(EudemonPacket.MagicAttack(CSocket.Client.ID, AttackSkill.ID, AttackSkill.Level, Targets, AttackedChar.X, AttackedChar.Y), CSocket.Client.X, CSocket.Client.Y, (int)CSocket.Client.Map, 0, 0);
                                    Targets.Clear();
                                    break;
                                }
                            default:
                                {
                                    Dictionary<int, int> Targets = new Dictionary<int, int>();
                                    Damage = Calculation.Damage(CSocket.Client, AttackedChar, AType, AttackSkill.ID, AttackSkill.Level);
                                    Calculation.doPlayer(CSocket, ASocket, Damage, AType);
                                    Targets.Add(AttackedChar.ID, Damage);
                                    EudemonPacket.ToLocal(EudemonPacket.MagicAttack(CSocket.Client.ID, AttackSkill.ID, AttackSkill.Level, Targets, AttackedChar.X, AttackedChar.Y), CSocket.Client.X, CSocket.Client.Y, (int)CSocket.Client.Map, 0, 0);
                                    Targets.Clear();
                                    break;
                                }
                        }
                    }
                }
                else if (Target > 0 && MagicID != 0 && AttackedMob != null)
                {
                    if (AttackSkill != null)
                    {
                        bool OK = Calculation.MagicCost(AttackSkill.ID, AttackSkill.Level, CSocket);
                        if (!OK)
                        {
                            if (CSocket.Client.Attack != null)
                            {
                                if (CSocket.Client.Attack.Enabled)
                                {
                                    CSocket.Client.Attack.Stop();
                                    CSocket.Client.Attack.Dispose();
                                }
                            }
                            return;
                        }
                        switch (AttackSkill.ID)
                        {
                            //TODO: Special effects for spells like FireOfHell, Revive, etc here
                            default:
                                {
                                    Dictionary<int, int> Targets = new Dictionary<int, int>();
                                    int RemainingHP = AttackedMob.CurrentHP;
                                    Damage = Calculation.Damage(CSocket.Client, AttackedMob, AType, AttackSkill.ID, AttackSkill.Level);
                                    bool killed = Calculation.doMonster(AttackedMob, Damage, AType, CSocket);
                                    Targets.Add(AttackedMob.UID, Damage);
                                    EudemonPacket.ToLocal(EudemonPacket.MagicAttack(CSocket.Client.ID, AttackSkill.ID, AttackSkill.Level, Targets, AttackedMob.X, AttackedMob.Y), CSocket.Client.X, CSocket.Client.Y, (int)CSocket.Client.Map, 0, 0);
                                    Targets.Clear();
                                    if (!killed)
                                    {
                                        if (CSocket.Client.Attack != null)
                                        {
                                            if (!CSocket.Client.Attack.Enabled)
                                            {
                                                CSocket.Client.Attack = new System.Timers.Timer();
                                                CSocket.Client.Attack.Interval = 900;
                                                CSocket.Client.Attack.Elapsed += delegate { Attack(Target, MagicID, AType, X, Y, CSocket); };
                                                CSocket.Client.Attack.Start();
                                            }
                                        }
                                        else
                                        {
                                            CSocket.Client.Attack = new System.Timers.Timer();
                                            CSocket.Client.Attack.Interval = 900;
                                            CSocket.Client.Attack.Elapsed += delegate { Attack(Target, MagicID, AType, X, Y, CSocket); };
                                            CSocket.Client.Attack.Start();
                                        }
                                        Calculation.SkillExp(AttackSkill.ID, CSocket, Damage);
                                    }
                                    else
                                    {
                                        Calculation.SkillExp(AttackSkill.ID, CSocket, RemainingHP);
                                    }
                                    break;
                                }
                        }
                    }
                }
                else if (Target > 0 && MagicID != 0 && AttackedTNpc != null)
                {
                    if (AttackSkill != null)
                    {
                        if (!Calculation.InRange(AttackedTNpc.X, AttackedTNpc.Y, CSocket.Client.X, CSocket.Client.Y, 16))
                        {
                            if (CSocket.Client.Attack != null)
                            {
                                if (CSocket.Client.Attack.Enabled)
                                {
                                    CSocket.Client.Attack.Stop();
                                    CSocket.Client.Attack.Dispose();
                                }
                            }
                            return;
                        }
                        Damage = Calculation.AddedMagicDamage(AttackSkill.ID, AttackSkill.Level);
                        Damage += CSocket.Client.MagicAttack + CSocket.Client.BonusMagicAttack;
                        Dictionary<int, int> Targets = new Dictionary<int, int>();
                        Targets.Add(AttackedTNpc.UID, Damage);
                        Calculation.doTNpc(CSocket, AttackedTNpc, Damage, AType);
                        EudemonPacket.ToLocal(EudemonPacket.MagicAttack(CSocket.Client.ID, AttackSkill.ID, AttackSkill.Level, Targets, X, Y), AttackedTNpc.X, AttackedTNpc.Y, AttackedTNpc.Map, 0, 0);
                        Targets.Clear();
                        Calculation.SkillExp(AttackSkill.ID, CSocket, Damage / 10);
                        if (CSocket.Client.Attack != null)
                        {
                            if (!CSocket.Client.Attack.Enabled)
                            {
                                CSocket.Client.Attack = new System.Timers.Timer();
                                CSocket.Client.Attack.Interval = 900;
                                CSocket.Client.Attack.Elapsed += delegate { Attack(Target, MagicID, AType, X, Y, CSocket); };
                                CSocket.Client.Attack.Start();
                            }
                        }
                        else
                        {
                            CSocket.Client.Attack = new System.Timers.Timer();
                            CSocket.Client.Attack.Interval = 900;
                            CSocket.Client.Attack.Elapsed += delegate { Attack(Target, MagicID, AType, X, Y, CSocket); };
                            CSocket.Client.Attack.Start();
                        }
                    }
                }
            }
            #endregion
            #region Bow
            else if (AType == 25)
            {
                if (AttackedChar != null && !AttackedChar.Dead)
                {
                    if (!Calculation.InRange(AttackedChar.X, AttackedChar.Y, CSocket.Client.X, CSocket.Client.Y, 16))
                    {
                        if (CSocket.Client.Attack != null)
                        {
                            if (CSocket.Client.Attack.Enabled)
                            {
                                CSocket.Client.Attack.Stop();
                                CSocket.Client.Attack.Dispose();
                            }
                        }
                        return;
                    }
                    Damage = Calculation.Damage(Attacker, AttackedChar, AType, 0, 0);
                    bool killed = Calculation.doPlayer(CSocket, ASocket, Damage, AType);
                    if (!killed)
                    {
                        if (CSocket.Client.Attack != null)
                        {
                            if (!CSocket.Client.Attack.Enabled)
                            {
                                CSocket.Client.Attack = new System.Timers.Timer();
                                CSocket.Client.Attack.Interval = 450;
                                CSocket.Client.Attack.Elapsed += delegate { Attack(Target, MagicID, AType, X, Y, CSocket); };
                                CSocket.Client.Attack.Start();
                            }
                        }
                        else
                        {
                            CSocket.Client.Attack = new System.Timers.Timer();
                            CSocket.Client.Attack.Interval = 450;
                            CSocket.Client.Attack.Elapsed += delegate { Attack(Target, MagicID, AType, X, Y, CSocket); };
                            CSocket.Client.Attack.Start();
                        }
                    }
                }
                else if (AttackedMob != null)
                {
                    if (!Calculation.InRange(AttackedMob.X, AttackedMob.Y, CSocket.Client.X, CSocket.Client.Y, 16))
                    {
                        if (CSocket.Client.Attack != null)
                        {
                            if (CSocket.Client.Attack.Enabled)
                            {
                                CSocket.Client.Attack.Stop();
                                CSocket.Client.Attack.Dispose();
                            }
                        }
                        return;
                    }
                    int RemainingHP = AttackedMob.CurrentHP;
                    Damage = Calculation.Damage(Attacker, AttackedMob, AType, 0, 0);
                    bool killed = Calculation.doMonster(AttackedMob, Damage, AType, CSocket);
                    if (!killed)
                    {
                        if (CSocket.Client.Attack != null)
                        {
                            if (!CSocket.Client.Attack.Enabled)
                            {
                                CSocket.Client.Attack = new System.Timers.Timer();
                                CSocket.Client.Attack.Interval = 450;
                                CSocket.Client.Attack.Elapsed += delegate { Attack(Target, MagicID, AType, X, Y, CSocket); };
                                CSocket.Client.Attack.Start();
                            }
                        }
                        else
                        {
                            CSocket.Client.Attack = new System.Timers.Timer();
                            CSocket.Client.Attack.Interval = 450;
                            CSocket.Client.Attack.Elapsed += delegate { Attack(Target, MagicID, AType, X, Y, CSocket); };
                            CSocket.Client.Attack.Start();
                        }
                    }
                }
                else if (AttackedTNpc != null)
                {
                    if (!Calculation.InRange(AttackedTNpc.X, AttackedTNpc.Y, CSocket.Client.X, CSocket.Client.Y, 16))
                    {
                        if (CSocket.Client.Attack != null)
                        {
                            if (CSocket.Client.Attack.Enabled)
                            {
                                CSocket.Client.Attack.Stop();
                                CSocket.Client.Attack.Dispose();
                            }
                        }
                        return;
                    }
                    if (CSocket.Client.MinAttack < CSocket.Client.MaxAttack)
                        Damage = Nano.Rand.Next(CSocket.Client.MinAttack, CSocket.Client.MaxAttack);
                    Calculation.doTNpc(CSocket, AttackedTNpc, Damage, AType);
                    if (CSocket.Client.Attack != null)
                    {
                        if (!CSocket.Client.Attack.Enabled)
                        {
                            CSocket.Client.Attack = new System.Timers.Timer();
                            CSocket.Client.Attack.Interval = 450;
                            CSocket.Client.Attack.Elapsed += delegate { Attack(Target, MagicID, AType, X, Y, CSocket); };
                            CSocket.Client.Attack.Start();
                        }
                    }
                    else
                    {
                        CSocket.Client.Attack = new System.Timers.Timer();
                        CSocket.Client.Attack.Interval = 450;
                        CSocket.Client.Attack.Elapsed += delegate { Attack(Target, MagicID, AType, X, Y, CSocket); };
                        CSocket.Client.Attack.Start();
                    }
                }
            }
            #endregion


        }
        public static void NoTargetMagic(ClientSocket CSocket, int MagicID, int X, int Y)
        {
            Struct.CharSkill Skill = CSocket.Client.Skills[MagicID];
            if ((int)CSocket.Client.Map != 1039)
            {
                bool OK = Calculation.MagicCost(Skill.ID, Skill.Level, CSocket);
                if (!OK)
                {
                    if (CSocket.Client.Attack != null)
                    {
                        if (CSocket.Client.Attack.Enabled)
                        {
                            CSocket.Client.Attack.Stop();
                            CSocket.Client.Attack.Dispose();
                        }
                    }
                    return;
                }
            }
            else
            {
                if (CSocket.Client.Attack != null)
                {
                    if (!CSocket.Client.Attack.Enabled)
                    {
                        CSocket.Client.Attack = new System.Timers.Timer();
                        CSocket.Client.Attack.Interval = 900;
                        CSocket.Client.Attack.Elapsed += delegate { Attack(0, MagicID, 21, X, Y, CSocket); };
                        CSocket.Client.Attack.Start();
                    }
                }
                else
                {
                    CSocket.Client.Attack = new System.Timers.Timer();
                    CSocket.Client.Attack.Interval = 900;
                    CSocket.Client.Attack.Elapsed += delegate { Attack(0, MagicID, 21, X, Y, CSocket); };
                    CSocket.Client.Attack.Start();
                }
            }
            switch (MagicID)
            {
                case 8001://Scatter
                    {
                        Dictionary<int, Monster> ToDo = new Dictionary<int, Monster>();
                        Dictionary<int, int> Targets = new Dictionary<int, int>();
                        //lock(Nano.Monsters)
                        //{
                        try
                        {
                            Monitor.Enter(Nano.Monsters);
                            foreach (KeyValuePair<int, Monster> Mob in Nano.Monsters)
                            {
                                if ((int)CSocket.Client.Map == Mob.Value.Map)
                                {
                                    if (Calculation.InRange(Mob.Value.X, Mob.Value.Y, CSocket.Client.X, CSocket.Client.Y, 12))
                                    {
                                        if (Mob.Value.Info.Name == "CoEmuGuard" || Mob.Value.Info.Name == "CoEmuPatrol" || Mob.Value.Info.Name == "GuardReviver")
                                        {
                                            if (CSocket.Client.PKMode == Struct.PkType.PK)
                                            {
                                                if (!ToDo.ContainsKey(Mob.Value.UID))
                                                    ToDo.Add(Mob.Key, Mob.Value);
                                            }
                                        }
                                        else
                                        {
                                            if (!ToDo.ContainsKey(Mob.Value.UID))
                                                ToDo.Add(Mob.Key, Mob.Value);
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                        }
                        finally
                        {
                            Monitor.Exit(Nano.Monsters);
                        }
                        //}
                        //lock(Nano.ClientPool)
                        //{
                        try
                        {
                            Monitor.Enter(Nano.ClientPool);
                            foreach (KeyValuePair<int, ClientSocket> Clients in Nano.ClientPool)
                            {
                                ClientSocket ASocket = Clients.Value;
                                if ((int)CSocket.Client.Map == (int)ASocket.Client.Map && CSocket.Client.ID != ASocket.Client.ID)
                                {
                                    if (Calculation.InRange(ASocket.Client.X, ASocket.Client.Y, CSocket.Client.X, CSocket.Client.Y, 12) && !ASocket.Client.Dead)
                                    {
                                        if (CheckMode(CSocket, ASocket))
                                        {
                                            int Damage = Calculation.Damage(CSocket.Client, ASocket.Client, 25, 0, 0);
                                            Calculation.doPlayer(CSocket, ASocket, Damage, 21);
                                            if (!Targets.ContainsKey(ASocket.Client.ID))
                                                Targets.Add(ASocket.Client.ID, Damage);
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                        }
                        finally
                        {
                            Monitor.Exit(Nano.ClientPool);
                        }
                        //}
                        foreach (KeyValuePair<int, Struct.TerrainNPC> TNPCS in Nano.TerrainNpcs)
                        {
                            Struct.TerrainNPC Tnpc = TNPCS.Value;
                            if ((int)CSocket.Client.Map == Tnpc.Map)
                            {
                                if (Calculation.InRange(Tnpc.X, Tnpc.Y, CSocket.Client.X, CSocket.Client.Y, 12))
                                {
                                    int Damage = 0;
                                    if (CSocket.Client.MinAttack < CSocket.Client.MaxAttack)
                                        Damage = Nano.Rand.Next(CSocket.Client.MinAttack, CSocket.Client.MaxAttack);
                                    Calculation.doTNpc(CSocket, Tnpc, Damage, 21);
                                    if (!Targets.ContainsKey(Tnpc.UID))
                                        Targets.Add(Tnpc.UID, Damage);
                                }
                            }
                        }
                        //TODO: GW, TG
                        foreach (KeyValuePair<int, Monster> Mob in ToDo)
                        {
                            int Damage = Calculation.Damage(CSocket.Client, Mob.Value, 25, 0, 0);
                            Calculation.doMonster(Mob.Value, Damage, 21, CSocket);
                            if (!Targets.ContainsKey(Mob.Value.UID))
                                Targets.Add(Mob.Key, Damage);
                        }
                        ToDo.Clear();
                        EudemonPacket.ToLocal(EudemonPacket.MagicAttack(CSocket.Client.ID, Skill.ID, Skill.Level, Targets, CSocket.Client.X, CSocket.Client.Y), CSocket.Client.X, CSocket.Client.Y, (int)CSocket.Client.Map, 0, 0);
                        Targets.Clear();
                        break;
                    }
                case 1045:
                    {
                        int Distance = 0;
                        if (Skill.Level == 1)
                            Distance = 5;
                        if (Skill.Level == 2)
                            Distance = 6;
                        if (Skill.Level == 3)
                            Distance = 7;
                        if (Skill.Level == 4)
                            Distance = 8;
                        int[][] FB = fbCoords(CSocket.Client.X, CSocket.Client.Y, X, Y, Distance);
                        Dictionary<int, int> Targets = new Dictionary<int, int>();
                        foreach (int[] HitCoords in FB)
                        {
                            int HitX = HitCoords[0];
                            int HitY = HitCoords[1];
                            //lock(Nano.ClientPool)
                            //{
                            try
                            {
                                Monitor.Enter(Nano.ClientPool);
                                foreach (KeyValuePair<int, ClientSocket> Clients in Nano.ClientPool)
                                {
                                    ClientSocket ASocket = Clients.Value;
                                    if ((int)CSocket.Client.Map == (int)ASocket.Client.Map && CSocket.Client.ID != ASocket.Client.ID)
                                    {
                                        if (Calculation.InRange(ASocket.Client.X, ASocket.Client.Y, CSocket.Client.X, CSocket.Client.Y, Distance) && !ASocket.Client.Dead)
                                        {
                                            if (ASocket.Client.X == HitX && ASocket.Client.Y == HitY && !Targets.ContainsKey(ASocket.Client.ID))
                                            {
                                                if (CheckMode(CSocket, ASocket))
                                                {
                                                    int Damage = Calculation.Damage(CSocket.Client, ASocket.Client, 2, 0, 0);
                                                    Calculation.doPlayer(CSocket, ASocket, Damage, 21);
                                                    Targets.Add(ASocket.Client.ID, Damage);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.ToString());
                            }
                            finally
                            {
                                Monitor.Exit(Nano.ClientPool);
                            }
                            //}
                            Dictionary<int, Monster> ToDo = new Dictionary<int, Monster>();
                            //lock(Nano.Monsters)
                            //{
                            try
                            {
                                Monitor.Enter(Nano.Monsters);
                                foreach (KeyValuePair<int, Monster> Mob in Nano.Monsters)
                                {
                                    if ((int)CSocket.Client.Map == Mob.Value.Map)
                                    {
                                        if (Calculation.InRange(Mob.Value.X, Mob.Value.Y, CSocket.Client.X, CSocket.Client.Y, Distance))
                                        {
                                            if (Mob.Value.X == HitX && Mob.Value.Y == HitY && !Targets.ContainsKey(Mob.Value.UID))
                                            {
                                                if (Mob.Value.Info.Name == "CoEmuGuard" || Mob.Value.Info.Name == "CoEmuPatrol" || Mob.Value.Info.Name == "GuardReviver")
                                                {
                                                    if (CSocket.Client.PKMode == Struct.PkType.PK)
                                                    {
                                                        if (!ToDo.ContainsKey(Mob.Value.UID))
                                                            ToDo.Add(Mob.Key, Mob.Value);
                                                    }
                                                }
                                                else
                                                {
                                                    if (!ToDo.ContainsKey(Mob.Value.UID))
                                                        ToDo.Add(Mob.Key, Mob.Value);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.ToString());
                            }
                            finally
                            {
                                Monitor.Exit(Nano.Monsters);
                            }
                            //}
                            foreach (KeyValuePair<int, Struct.TerrainNPC> TNPCS in Nano.TerrainNpcs)
                            {
                                Struct.TerrainNPC Tnpc = TNPCS.Value;
                                if ((int)CSocket.Client.Map == Tnpc.Map)
                                {
                                    if (Calculation.InRange(Tnpc.X, Tnpc.Y, CSocket.Client.X, CSocket.Client.Y, 12))
                                    {
                                        if (HitX == Tnpc.X && HitY == Tnpc.Y)
                                        {
                                            int Damage = 0;
                                            if (CSocket.Client.MinAttack < CSocket.Client.MaxAttack)
                                                Damage = Nano.Rand.Next(CSocket.Client.MinAttack, CSocket.Client.MaxAttack);
                                            Calculation.doTNpc(CSocket, Tnpc, Damage, 21);
                                            if (!Targets.ContainsKey(Tnpc.UID))
                                                Targets.Add(Tnpc.UID, Damage);
                                        }
                                    }
                                }
                            }
                            if (ToDo.Count > 0)
                            {
                                foreach (KeyValuePair<int, Monster> Mob in ToDo)
                                {
                                    int Damage = Calculation.Damage(CSocket.Client, Mob.Value, 2, 0, 0);
                                    Calculation.doMonster(Mob.Value, Damage, 21, CSocket);
                                    if (!Targets.ContainsKey(Mob.Value.UID))
                                        Targets.Add(Mob.Key, Damage);
                                }
                            }
                            ToDo.Clear();
                        }
                        EudemonPacket.ToLocal(EudemonPacket.MagicAttack(CSocket.Client.ID, Skill.ID, Skill.Level, Targets, X, Y), CSocket.Client.X, CSocket.Client.Y, (int)CSocket.Client.Map, 0, 0);
                        Targets.Clear();
                        break;
                    }
                case 1046:
                    {
                        int Distance = 0;
                        if (Skill.Level == 1)
                            Distance = 5;
                        if (Skill.Level == 2)
                            Distance = 6;
                        if (Skill.Level == 3)
                            Distance = 7;
                        if (Skill.Level == 4)
                            Distance = 8;
                        int[][] FB = fbCoords(CSocket.Client.X, CSocket.Client.Y, X, Y, Distance);
                        Dictionary<int, int> Targets = new Dictionary<int, int>();
                        foreach (int[] HitCoords in FB)
                        {
                            int HitX = HitCoords[0];
                            int HitY = HitCoords[1];
                            //lock(Nano.ClientPool)
                            //{
                            try
                            {
                                Monitor.Enter(Nano.ClientPool);
                                foreach (KeyValuePair<int, ClientSocket> Clients in Nano.ClientPool)
                                {
                                    ClientSocket ASocket = Clients.Value;
                                    if ((int)CSocket.Client.Map == (int)ASocket.Client.Map)
                                    {
                                        if (Calculation.InRange(ASocket.Client.X, ASocket.Client.Y, CSocket.Client.X, CSocket.Client.Y, Distance) && !ASocket.Client.Dead)
                                        {
                                            if (ASocket.Client.X == HitX && ASocket.Client.Y == HitY && !Targets.ContainsKey(ASocket.Client.ID))
                                            {
                                                if (CheckMode(CSocket, ASocket))
                                                {
                                                    int Damage = Calculation.Damage(CSocket.Client, ASocket.Client, 2, 0, 0);
                                                    Calculation.doPlayer(CSocket, ASocket, Damage, 21);
                                                    Targets.Add(ASocket.Client.ID, Damage);
                                                }
                                            }
                                        }
                                    }
                                }
                                //}
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.ToString());
                            }
                            finally
                            {
                                Monitor.Exit(Nano.ClientPool);
                            }
                            Dictionary<int, Monster> ToDo = new Dictionary<int, Monster>();
                            //lock(Nano.Monsters)
                            //{
                            try
                            {
                                Monitor.Enter(Nano.Monsters);
                                foreach (KeyValuePair<int, Monster> Mob in Nano.Monsters)
                                {
                                    if ((int)CSocket.Client.Map == Mob.Value.Map)
                                    {
                                        if (Calculation.InRange(Mob.Value.X, Mob.Value.Y, CSocket.Client.X, CSocket.Client.Y, Distance))
                                        {
                                            if (Mob.Value.X == HitX && Mob.Value.Y == HitY && !Targets.ContainsKey(Mob.Value.UID))
                                            {
                                                if (Mob.Value.Info.Name == "CoEmuGuard" || Mob.Value.Info.Name == "CoEmuPatrol" || Mob.Value.Info.Name == "GuardReviver")
                                                {
                                                    if (CSocket.Client.PKMode == Struct.PkType.PK)
                                                    {
                                                        if (!ToDo.ContainsKey(Mob.Value.UID))
                                                            ToDo.Add(Mob.Key, Mob.Value);
                                                    }
                                                }
                                                else
                                                {
                                                    if (!ToDo.ContainsKey(Mob.Value.UID))
                                                        ToDo.Add(Mob.Key, Mob.Value);
                                                }
                                            }
                                        }
                                    }
                                }
                                //}
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.ToString());
                            }
                            finally
                            {
                                Monitor.Exit(Nano.Monsters);
                            }
                            foreach (KeyValuePair<int, Struct.TerrainNPC> TNPCS in Nano.TerrainNpcs)
                            {
                                Struct.TerrainNPC Tnpc = TNPCS.Value;
                                if ((int)CSocket.Client.Map == Tnpc.Map)
                                {
                                    if (Calculation.InRange(Tnpc.X, Tnpc.Y, CSocket.Client.X, CSocket.Client.Y, 12))
                                    {
                                        if (HitX == Tnpc.X && HitY == Tnpc.Y)
                                        {
                                            int Damage = 0;
                                            if (CSocket.Client.MinAttack < CSocket.Client.MaxAttack)
                                                Damage = Nano.Rand.Next(CSocket.Client.MinAttack, CSocket.Client.MaxAttack);
                                            Calculation.doTNpc(CSocket, Tnpc, Damage, 21);
                                            if (!Targets.ContainsKey(Tnpc.UID))
                                                Targets.Add(Tnpc.UID, Damage);
                                        }
                                    }
                                }
                            }
                            if (ToDo.Count > 0)
                            {
                                foreach (KeyValuePair<int, Monster> Mob in ToDo)
                                {
                                    int Damage = Calculation.Damage(CSocket.Client, Mob.Value, 2, 0, 0);
                                    Calculation.doMonster(Mob.Value, Damage, 21, CSocket);
                                    if (!Targets.ContainsKey(Mob.Value.UID))
                                        Targets.Add(Mob.Key, Damage);
                                }
                            }
                            ToDo.Clear();
                        }
                        EudemonPacket.ToLocal(EudemonPacket.MagicAttack(CSocket.Client.ID, Skill.ID, Skill.Level, Targets, X, Y), CSocket.Client.X, CSocket.Client.Y, (int)CSocket.Client.Map, 0, 0);
                        Targets.Clear();
                        break;
                    }
                case 1115: //Hercules
                    {
                        int Distance = 0;
                        if (Skill.Level == 0)
                            Distance = 1;
                        else if (Skill.Level == 1)
                            Distance = 1;
                        else if (Skill.Level == 2)
                            Distance = 1;
                        else if (Skill.Level == 3)
                            Distance = 2;
                        else if (Skill.Level == 4)
                            Distance = 3;
                        Dictionary<int, Monster> ToDo = new Dictionary<int, Monster>();
                        Dictionary<int, int> Targets = new Dictionary<int, int>();
                        //lock(Nano.Monsters)
                        //{
                        try
                        {
                            Monitor.Enter(Nano.Monsters);
                            foreach (KeyValuePair<int, Monster> Mob in Nano.Monsters)
                            {
                                if ((int)CSocket.Client.Map == Mob.Value.Map)
                                {
                                    if (Calculation.InRange(Mob.Value.X, Mob.Value.Y, CSocket.Client.X, CSocket.Client.Y, Distance))
                                    {
                                        if (Mob.Value.Info.Name == "CoEmuGuard" || Mob.Value.Info.Name == "CoEmuPatrol" || Mob.Value.Info.Name == "GuardReviver")
                                        {
                                            if (CSocket.Client.PKMode == Struct.PkType.PK)
                                            {
                                                if (!ToDo.ContainsKey(Mob.Value.UID))
                                                    ToDo.Add(Mob.Key, Mob.Value);
                                            }
                                        }
                                        else
                                        {
                                            if (!ToDo.ContainsKey(Mob.Value.UID))
                                                ToDo.Add(Mob.Key, Mob.Value);
                                        }
                                    }
                                }
                            }
                            //}
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                        }
                        finally
                        {
                            Monitor.Exit(Nano.Monsters);
                        }
                        //lock(Nano.ClientPool)
                        //{
                        try
                        {
                            Monitor.Enter(Nano.ClientPool);
                            foreach (KeyValuePair<int, ClientSocket> Clients in Nano.ClientPool)
                            {
                                ClientSocket ASocket = Clients.Value;
                                if ((int)CSocket.Client.Map == (int)ASocket.Client.Map && CSocket.Client.ID != ASocket.Client.ID)
                                {
                                    if (Calculation.InRange(ASocket.Client.X, ASocket.Client.Y, CSocket.Client.X, CSocket.Client.Y, Distance) && !ASocket.Client.Dead)
                                    {
                                        if (CheckMode(CSocket, ASocket))
                                        {
                                            int Damage = Calculation.Damage(CSocket.Client, ASocket.Client, 2, 0, 0);
                                            Damage = (Damage / 3);
                                            Calculation.doPlayer(CSocket, ASocket, Damage, 21);
                                            if (!Targets.ContainsKey(ASocket.Client.ID))
                                                Targets.Add(ASocket.Client.ID, Damage);
                                        }
                                    }
                                }
                            }
                            //}
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                        }
                        finally
                        {
                            Monitor.Exit(Nano.ClientPool);
                        }
                        foreach (KeyValuePair<int, Struct.TerrainNPC> TNPCS in Nano.TerrainNpcs)
                        {
                            Struct.TerrainNPC Tnpc = TNPCS.Value;
                            if ((int)CSocket.Client.Map == Tnpc.Map)
                            {
                                if (Calculation.InRange(Tnpc.X, Tnpc.Y, CSocket.Client.X, CSocket.Client.Y, Distance))
                                {
                                    int Damage = 0;
                                    if (CSocket.Client.MinAttack < CSocket.Client.MaxAttack)
                                    {
                                        Damage = Nano.Rand.Next(CSocket.Client.MinAttack, CSocket.Client.MaxAttack);
                                        Damage = (Damage / 3);
                                    }
                                    Calculation.doTNpc(CSocket, Tnpc, Damage, 21);
                                    if (!Targets.ContainsKey(Tnpc.UID))
                                        Targets.Add(Tnpc.UID, Damage);
                                }
                            }
                        }
                        foreach (KeyValuePair<int, Monster> Mob in ToDo)
                        {
                            int Damage = Calculation.Damage(CSocket.Client, Mob.Value, 2, 0, 0);
                            Damage = (Damage / 3);
                            Calculation.doMonster(Mob.Value, Damage, 21, CSocket);
                            if (!Targets.ContainsKey(Mob.Value.UID))
                                Targets.Add(Mob.Key, Damage);
                        }
                        ToDo.Clear();
                        EudemonPacket.ToLocal(EudemonPacket.MagicAttack(CSocket.Client.ID, Skill.ID, Skill.Level, Targets, CSocket.Client.X, CSocket.Client.Y), CSocket.Client.X, CSocket.Client.Y, (int)CSocket.Client.Map, 0, 0);
                        Targets.Clear();
                        break;
                    }
            }
        }
        public static int[][] fbCoords(int userx, int usery, int shotx, int shoty, int length)
        {
            int[][] fbcr = new int[length][];
            int nx = shotx - userx;
            int ny = shoty - usery;
            double dir = Math.Atan2(ny, nx);
            for (int i = 1; i <= length; i++)
            {
                double ax = i * Math.Cos(dir);
                double ay = i * Math.Sin(dir);
                int fbx = (int)Math.Round(userx + ax);
                int fby = (int)Math.Round(usery + ay);
                fbcr[i - 1] = new int[2] { fbx, fby };
            }
            return fbcr;
        }
        /// <summary>
        /// Checks the PK mode of a player and tells the attack system if it is okay to include that attacked player or not.
        /// </summary>
        /// <param name="CSocket">The attacking ClientSocket</param>
        /// <param name="ASocket">The attacked ClientSocket</param>
        /// <returns>True if the current mode is okay, otherwise false.</returns>
        public static bool CheckMode(ClientSocket CSocket, ClientSocket ASocket)
        {
            if ((int)CSocket.Client.Map == 1002 || (int)CSocket.Client.Map == 1011 || (int)CSocket.Client.Map == 1039)
                return false;
            //TODO: Teams, friends, guildies
            if (CSocket.Client.PKMode == Struct.PkType.Peace)
                return false;
            else if (CSocket.Client.PKMode == Struct.PkType.Capture)
                return false;
            else if (CSocket.Client.PKMode == Struct.PkType.Team)
            {
                if (CSocket.Client.Team != null)
                {
                    if (Nano.ClientPool.ContainsKey(CSocket.Client.Team.LeaderID))
                    {
                        ClientSocket Leader = Nano.ClientPool[CSocket.Client.Team.LeaderID];
                        if (Leader.Client.Team.Members.ContainsKey(ASocket.Client.ID))
                            return false;
                        else
                            return true;
                    }
                }
                return true;
            }
            else if (CSocket.Client.PKMode == Struct.PkType.PK)
                return true;
            return false;
        }
    }
}
