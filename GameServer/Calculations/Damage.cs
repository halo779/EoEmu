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
    /// Calculates damage to a character or monster.
    /// </summary>
    public partial class Calculation
    {
        public static int Damage(Character Attacker, Character Attacked, int AType, int SkillID, int SkillLevel)
        {
            if (Attacker.MinAttack > Attacker.MaxAttack)
            {
                return 0;
            }
            if (AType == 2)
            {
                int Damage = MainGS.Rand.Next(Attacker.MinAttack, Attacker.MaxAttack);
                Damage -= Attacked.Defense;
                double Tort = 0;
                Tort += Attacked.NBG * 0.05;
                Tort += Attacked.RBG * 0.10;
                Tort += Attacked.SBG * 0.15;
                Damage = (int)Math.Floor(Damage * (1 - Tort));
                if (Attacked.Bless > 0)
                {
                    Damage = (int)Math.Floor(Damage * (1 - (Attacked.Bless * 0.01)));
                }
                //TODO: Superman
                if (Damage < 0)
                    Damage = 1;
                if ((Attacked.isGM || Attacked.isPM) && Attacked.Invincible)
                    Damage = 0;
                return Damage;
            }
            else if (AType == 21)
            {
                int Damage = AddedMagicDamage(SkillID, SkillLevel);
                Damage += Attacker.MagicAttack;
                if (Attacked.MagicDefense > 0)
                {
                    double MDef = 1;
                    if (Attacked.MagicDefense < 90)
                    {
                        MDef = (Attacked.MagicDefense * 0.01);
                    }
                    else
                    {
                        MDef = (90 * 0.01);
                    }
                    Damage = (int)Math.Floor(Damage - (Damage * MDef));
                }
                Damage = Convert.ToInt32(Damage * 0.75);
                Damage += Attacker.BonusMagicAttack;
                Damage -= Attacked.BonusMagicDefense;
                double Tort = 0;
                Tort += Attacked.NBG * 0.05;
                Tort += Attacked.RBG * 0.10;
                Tort += Attacked.SBG * 0.15;
                Damage = (int)Math.Floor(Damage * (1 - Tort));
                if (Attacked.Bless > 0)
                {
                    Damage = (int)Math.Floor(Damage * (1 - (Attacked.Bless * 0.01)));
                }
                if (Damage < 0)
                    Damage = 1;
                if ((Attacked.isGM || Attacked.isPM) && Attacked.Invincible)
                    Damage = 0;
                return Damage;
            }
            else if (AType == 25)
            {
                int Damage = MainGS.Rand.Next(Attacker.MinAttack, Attacker.MaxAttack);
                if (Attacked.Dodge > 0)
                {
                    double Dodge = 0;
                    if (Attacked.Dodge <= 94)
                        Dodge = Attacked.Dodge * 0.01;
                    else
                        Dodge = 94 * 0.01;
                    Damage = (int)Math.Floor(Damage - (Damage * Dodge));
                }
                Damage += Attacker.BonusMagicAttack;
                Damage -= Attacked.BonusMagicDefense;
                double Tort = 0;
                Tort += Attacked.NBG * 0.05;
                Tort += Attacked.RBG * 0.10;
                Tort += Attacked.SBG * 0.15;
                Damage = (int)Math.Floor(Damage * (1 - Tort));
                if (Attacked.Bless > 0)
                {
                    Damage = (int)Math.Floor(Damage * (1 - (Attacked.Bless * 0.01)));
                }
                if (Damage < 0)
                    Damage = 1;
                if ((Attacked.isGM || Attacked.isPM) && Attacked.Invincible)
                    Damage = 0;
                return Damage;
            }
            return 0;
        }
        public static int Damage(Character Attacker, Monster Attacked, int AType, int MagicID, int MagicLevel)
        {
            if (Attacker.MinAttack > Attacker.MaxAttack)
            {
                return 0;
            }
            //TODO: Magic, Archer
            if (AType == 2)
            {
                int Damage = MainGS.Rand.Next(Attacker.MinAttack, Attacker.MaxAttack);
                if (Attacked.Info.Name != "CoEmuGuard" && Attacked.Info.Name != "CoEmuPatrol" && Attacked.Info.Name != "GuildPatrol" && Attacked.Info.Name != "GuardReviver")
                {
                    int Leveldiff = (Attacker.Level + 2) - Attacked.Level;
                    int Damageadd = (int)Math.Floor(1 + (Leveldiff / 5) * 0.8);
                    if (Damageadd > 1)
                        Damage = Damageadd * Damage;
                }
                Damage -= Attacked.Info.Defense;
                if (Damage < 0)
                    Damage = 1;
                //TODO: Superman
                return Damage;
            }
            else if (AType == 21)
            {
                int Damage = AddedMagicDamage(MagicID, MagicLevel);
                Damage += Attacker.MagicAttack;
                if (Attacked.Info.MDefense > 0)
                {
                    double MDef = (Attacked.Info.MDefense * 0.01);
                    Damage = (int)Math.Floor(Damage - (Damage * MDef));
                }
                if (Attacked.Info.Name != "CoEmuGuard" && Attacked.Info.Name != "CoEmuPatrol" && Attacked.Info.Name != "GuildPatrol" && Attacked.Info.Name != "GuardReviver")
                {
                    int Leveldiff = (Attacker.Level + 2) - Attacked.Level;
                    int Damageadd = (int)Math.Floor(1 + (Leveldiff / 5) * 0.8);
                    if (Damageadd > 1)
                        Damage = Damageadd * Damage;
                }
                if (Damage < 0)
                    Damage = 1;
                return Damage;
            }
            else if (AType == 25)
            {
                int Damage = MainGS.Rand.Next(Attacker.MinAttack, Attacker.MaxAttack);
                if (Attacked.Info.Dodge > 0)
                {
                    double Dodge = Attacked.Info.Dodge * 0.01;
                    Damage = (int)Math.Floor(Damage - (Damage * Dodge));
                }
                if (Attacked.Info.Name != "CoEmuGuard" && Attacked.Info.Name != "CoEmuPatrol" && Attacked.Info.Name != "GuildPatrol" && Attacked.Info.Name != "GuardReviver")
                {
                    int Leveldiff = (Attacker.Level + 2) - Attacked.Level;
                    int Damageadd = (int)Math.Floor(1 + (Leveldiff / 5) * 0.8);
                    if (Damageadd > 1)
                        Damage = Damageadd * Damage;
                }
                if (Damage < 0)
                    Damage = 1;
                return Damage;
            }
            return 0;
        }
    }
}
