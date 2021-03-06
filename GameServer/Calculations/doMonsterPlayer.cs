﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using GameServer.Connections;
using GameServer.Entities;
using GameServer.Structs;
using GameServer.Packets;
using GameServer.Calculations;
using GameServer.Handlers;

namespace GameServer.Calculations
{
    /// <summary>
    /// Deals instantiated damage to a player or monster
    /// </summary>
    public partial class Calculation
    {
        /// <summary>
        /// Does damage to a monster.
        /// </summary>
        /// <param name="AttackedMob">The monster being attacked</param>
        /// <param name="Damage">The damage being done</param>
        /// <param name="AType">The type of attack being done</param>
        /// <param name="CSocket">The ClientSocket Attacking</param>
        /// <returns>true if the monster dies, false if not.</returns>
        public static bool doMonster(Monster AttackedMob, int Damage, int AType, ClientSocket CSocket)
        {
            Character Attacker = CSocket.Client;
            if (Damage < AttackedMob.CurrentHP)
            {
                if (AType != 21)
                    EudemonPacket.ToLocal(EudemonPacket.Attack(Attacker.ID, AttackedMob.UID, Attacker.X, Attacker.Y, Damage, AType), AttackedMob.X, AttackedMob.Y, (int)AttackedMob.Map, 0, 0);
                Interlocked.Add(ref AttackedMob.CurrentHP, (Damage * -1));
                if (AttackedMob.Info.Name != "CoEmuGuard" && AttackedMob.Info.Name != "GuardReviver" && AttackedMob.Info.Name != "CoEmuPatrol" && AttackedMob.Info.Name != "GuildPatrol")
                {
                    GiveExp(CSocket, AttackedMob, Damage, false);
                }
                return false;
            }
            else
            {
                if (AType != 21)
                    EudemonPacket.ToLocal(EudemonPacket.Attack(Attacker.ID, AttackedMob.UID, Attacker.X, Attacker.Y, Damage, AType), AttackedMob.X, AttackedMob.Y, AttackedMob.Map, 0, 0);
                EudemonPacket.ToLocal(EudemonPacket.Attack(Attacker.ID, AttackedMob.UID, Attacker.X, Attacker.Y, Damage, 14), AttackedMob.X, AttackedMob.Y, (int)AttackedMob.Map, 0, 0);
                //lock(MainGS.Monsters)
                //{
                try
                {
                    Monitor.Enter(MainGS.Monsters);
                    MainGS.Monsters.Remove(AttackedMob.UID);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                finally
                {
                    Monitor.Exit(MainGS.Monsters);
                }
                //}
                AttackedMob.Die(CSocket.Client.ID);
                if (AttackedMob.Info.Name != "CoEmuGuard" && AttackedMob.Info.Name != "GuardReviver" && AttackedMob.Info.Name != "CoEmuPatrol" && AttackedMob.Info.Name != "GuildPatrol")
                {
                    GiveExp(CSocket, AttackedMob, Damage, true);
                }
                if (MainGS.MonsterSpawns.ContainsKey(AttackedMob.SpawnID))
                    Interlocked.Add(ref MainGS.MonsterSpawns[AttackedMob.SpawnID].SpawnNumber, -1);
                return true;
            }
        }
        /// <summary>
        /// Does damage to a player
        /// </summary>
        /// <param name="CSocket">Attacking ClientSocket</param>
        /// <param name="ASocket">Attacked ClientSocket</param>
        /// <param name="Damage">Damage to do</param>
        /// <param name="AType">Type of attack</param>
        /// <returns>True if the player dies, otherwise false.</returns>
        public static bool doPlayer(ClientSocket CSocket, ClientSocket ASocket, int Damage, int AType)
        {
            //TODO: Pk points, Flashing names, etc
            Character Attacker = CSocket.Client;
            Character AttackedChar = ASocket.Client;
            if (Damage < AttackedChar.CurrentHP)
            {
                CSocket.Client.Flashing = true;
                if (CSocket.Client.FlashTimer == null)
                {
                    CSocket.Client.FlashTimer = new System.Timers.Timer();
                    CSocket.Client.FlashTimer.Interval = 10000;
                    CSocket.Client.FlashTimer.AutoReset = false;
                    CSocket.Client.FlashTimer.Elapsed += delegate
                    {
                        CSocket.Client.Flashing = false;
                        CSocket.Client.FlashTimer.Stop();
                        CSocket.Client.FlashTimer.Dispose();
                        EudemonPacket.ToLocal(EudemonPacket.Status(CSocket, 2, 0, Struct.StatusTypes.StatusEffect), CSocket.Client.X, CSocket.Client.Y, (int)CSocket.Client.Map, 0, 0);
                    };
                    CSocket.Client.FlashTimer.Start();
                }
                else
                {
                    CSocket.Client.FlashTimer.Stop();
                    CSocket.Client.FlashTimer.Dispose();
                    CSocket.Client.FlashTimer = new System.Timers.Timer();
                    CSocket.Client.FlashTimer.Interval = 10000;
                    CSocket.Client.FlashTimer.AutoReset = false;
                    CSocket.Client.FlashTimer.Elapsed += delegate
                    {
                        CSocket.Client.Flashing = false;
                        CSocket.Client.FlashTimer.Stop();
                        CSocket.Client.FlashTimer.Dispose();
                        EudemonPacket.ToLocal(EudemonPacket.Status(CSocket, 2, 0, Struct.StatusTypes.StatusEffect), CSocket.Client.X, CSocket.Client.Y, (int)CSocket.Client.Map, 0, 0);
                    };
                    CSocket.Client.FlashTimer.Start();
                }
                if (AType != 21)
                    EudemonPacket.ToLocal(EudemonPacket.Attack(Attacker.ID, AttackedChar.ID, Attacker.X, Attacker.Y, Damage, AType), Attacker.X, Attacker.Y, (int)Attacker.Map, 0, 0);
                Interlocked.Add(ref AttackedChar.CurrentHP, (Damage * -1));
                ASocket.Send(EudemonPacket.Status(ASocket, 2, AttackedChar.CurrentHP, Struct.StatusTypes.Hp));
                EudemonPacket.ToLocal(EudemonPacket.Status(CSocket, 2, 0, Struct.StatusTypes.StatusEffect), CSocket.Client.X, CSocket.Client.Y, (int)CSocket.Client.Map, 0, 0);
                return false;
            }
            else
            {
                CSocket.Client.Flashing = true;
                if (CSocket.Client.FlashTimer == null)
                {
                    CSocket.Client.FlashTimer = new System.Timers.Timer();
                    CSocket.Client.FlashTimer.Interval = 60000;
                    CSocket.Client.FlashTimer.AutoReset = false;
                    CSocket.Client.FlashTimer.Elapsed += delegate
                    {
                        CSocket.Client.Flashing = false;
                        CSocket.Client.FlashTimer.Stop();
                        CSocket.Client.FlashTimer.Dispose();
                        EudemonPacket.ToLocal(EudemonPacket.Status(CSocket, 2, 0, Struct.StatusTypes.StatusEffect), CSocket.Client.X, CSocket.Client.Y, (int)CSocket.Client.Map, 0, 0);
                    };
                    CSocket.Client.FlashTimer.Start();
                }
                else
                {
                    CSocket.Client.FlashTimer.Stop();
                    CSocket.Client.FlashTimer.Dispose();
                    CSocket.Client.FlashTimer = new System.Timers.Timer();
                    CSocket.Client.FlashTimer.Interval = 60000;
                    CSocket.Client.FlashTimer.AutoReset = false;
                    CSocket.Client.FlashTimer.Elapsed += delegate
                    {
                        CSocket.Client.Flashing = false;
                        CSocket.Client.FlashTimer.Stop();
                        CSocket.Client.FlashTimer.Dispose();
                        EudemonPacket.ToLocal(EudemonPacket.Status(CSocket, 2, 0, Struct.StatusTypes.StatusEffect), CSocket.Client.X, CSocket.Client.Y, (int)CSocket.Client.Map, 0, 0);
                    };
                    CSocket.Client.FlashTimer.Start();
                }
                if (AType != 21)
                    EudemonPacket.ToLocal(EudemonPacket.Attack(Attacker.ID, AttackedChar.ID, Attacker.X, Attacker.Y, Damage, AType), Attacker.X, Attacker.Y, (int)Attacker.Map, 0, 0);
                EudemonPacket.ToLocal(EudemonPacket.Attack(Attacker.ID, AttackedChar.ID, AttackedChar.X, AttackedChar.Y, Damage, 14), Attacker.X, Attacker.Y, (int)Attacker.Map, 0, 0);
                EudemonPacket.ToLocal(EudemonPacket.Status(ASocket, 2, AttackedChar.GhostModel, Struct.StatusTypes.Model), AttackedChar.X, AttackedChar.Y, (int)AttackedChar.Map, 0, 0);
                CSocket.Client.PkPoints += 10;
                if (ASocket.Client.Flashing)
                    ASocket.Client.Flashing = false;
                EudemonPacket.ToLocal(EudemonPacket.Status(ASocket, 2, 1024, Struct.StatusTypes.StatusEffect), AttackedChar.X, AttackedChar.Y, (int)AttackedChar.Map, 0, 0);
                EudemonPacket.ToLocal(EudemonPacket.Status(CSocket, 2, 0, Struct.StatusTypes.StatusEffect), CSocket.Client.X, CSocket.Client.Y, (int)CSocket.Client.Map, 0, 0);
                CSocket.Send(EudemonPacket.Status(CSocket, 2, CSocket.Client.PkPoints, Struct.StatusTypes.PKPoints));
                //TODO: Guild PKPs
                ASocket.Client.Dead = true;
                return true;
            }
        }
        public static bool doPlayer(Monster Attacker, ClientSocket ASocket, int Damage, int AType)
        {
            Character AttackedChar = ASocket.Client;
            if (Damage < AttackedChar.CurrentHP)
            {
                if (AType != 21)
                    EudemonPacket.ToLocal(EudemonPacket.Attack(Attacker.UID, AttackedChar.ID, Attacker.X, Attacker.Y, Damage, AType), Attacker.X, Attacker.Y, (int)Attacker.Map, 0, 0);
                Interlocked.Add(ref AttackedChar.CurrentHP, (Damage * -1));
                ASocket.Send(EudemonPacket.Status(ASocket, 2, AttackedChar.CurrentHP, Struct.StatusTypes.Hp));
                return false;
            }
            else
            {
                if (AType != 21)
                    EudemonPacket.ToLocal(EudemonPacket.Attack(Attacker.UID, AttackedChar.ID, Attacker.X, Attacker.Y, Damage, AType), Attacker.X, Attacker.Y, (int)Attacker.Map, 0, 0);
                EudemonPacket.ToLocal(EudemonPacket.Attack(Attacker.UID, AttackedChar.ID, AttackedChar.X, AttackedChar.Y, Damage, 14), Attacker.X, Attacker.Y, (int)Attacker.Map, 0, 0);
                EudemonPacket.ToLocal(EudemonPacket.Status(ASocket, 2, AttackedChar.GhostModel, Struct.StatusTypes.Model), AttackedChar.X, AttackedChar.Y, (int)AttackedChar.Map, 0, 0);
                EudemonPacket.ToLocal(EudemonPacket.Status(ASocket, 2, 1024, Struct.StatusTypes.StatusEffect), AttackedChar.X, AttackedChar.Y, (int)AttackedChar.Map, 0, 0);
                ASocket.Client.Dead = true;
                return true;
            }
        }
        /// <summary>
        /// Does damage to a conquer online terrain NPC.
        /// </summary>
        /// <param name="CSocket">Attacking client socket.</param>
        /// <param name="AttackedTNpc">Attacked TNpc</param>
        /// <param name="Damage">Damage done</param>
        /// <param name="AType">Type of attack</param>
        public static void doTNpc(ClientSocket CSocket, Struct.TerrainNPC AttackedTNpc, int Damage, int AType)
        {
            if (AttackedTNpc.UID >= 6700)
            {
                CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] GuildWar is not ready yet.", Struct.ChatType.System));
            }
            else
            {
                GiveExp(CSocket, AttackedTNpc, Damage);
                if (AType != 21)
                    EudemonPacket.ToLocal(EudemonPacket.Attack(CSocket.Client.ID, AttackedTNpc.UID, AttackedTNpc.X, AttackedTNpc.Y, Damage, AType), CSocket.Client.X, CSocket.Client.Y, (int)CSocket.Client.Map, 0, 0);
            }
        }
    }
}
