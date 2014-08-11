using System;
using System.Collections.Generic;
using System.Collections;
using System.Threading;
using GameServer.Calculations;
using GameServer.Connections;
using GameServer.Entities;
using GameServer.Structs;
using GameServer.Packets;

namespace GameServer.Entities
{
    /// <summary>
    /// Spawns all entities in a client's new area, assuming he/she hasn't seen them before
    /// </summary>
    public class Spawn
    {
        public static void All(ClientSocket CSocket)
        {
            try
            {
                Monitor.Enter(MainGS.ClientPool);
                foreach (KeyValuePair<int, ClientSocket> Locals in MainGS.ClientPool)
                {
                    ClientSocket C = Locals.Value;
                    if ((int)C.Client.Map == (int)CSocket.Client.Map && CSocket.Client.ID != C.Client.ID)
                    {
                        if (!Calculation.CanSee(CSocket.Client.PrevX, CSocket.Client.PrevY, C.Client.X, C.Client.Y))
                        {
                            if (!Calculation.CanSee(CSocket.Client.X, CSocket.Client.Y, C.Client.X, C.Client.Y))
                                CSocket.Send(EudemonPacket.SpawnCharacter(C));
                        }
                        //@TODO: Send guild string packet
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                Monitor.Exit(MainGS.ClientPool);
            }
            try
            {
                Monitor.Enter(MainGS.Monsters);
                foreach (KeyValuePair<int, Monster> Monsters in MainGS.Monsters)
                {
                    Monster Mob = Monsters.Value;
                    if ((int)CSocket.Client.Map == Mob.Map)
                    {
                        if (!Calculation.CanSee(CSocket.Client.PrevX, CSocket.Client.PrevY, Mob.X, Mob.Y))
                        {
                            if (Calculation.CanSee(CSocket.Client.X, CSocket.Client.Y, Mob.X, Mob.Y))
                            {
                                CSocket.Send(EudemonPacket.SpawnMonster(Mob.UID, Mob.Info.Mesh, Mob.X, Mob.Y, Mob.Info.Name, Mob.CurrentHP, Mob.Level, Mob.Direction));
                                //Mob.TriggerMove();
                            }
                        }
                        if (Calculation.InRange(CSocket.Client.X, CSocket.Client.Y, Mob.X, Mob.Y, Mob.Info.AggroRange))
                            Mob.TriggerMove();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                Monitor.Exit(MainGS.Monsters);
            }
            foreach (KeyValuePair<int, Struct.NPC> Npcs in MainGS.Npcs)
            {
                Struct.NPC Npc = Npcs.Value;
                if ((int)CSocket.Client.Map == Npc.Map)
                {
                    if(!(Calculation.GetDistance(CSocket.Client.PrevX,CSocket.Client.PrevY,Npc.X,Npc.Y) <= 15))
                    {
                        if (Calculation.CanSee(CSocket.Client.X, CSocket.Client.Y, Npc.X, Npc.Y))
                        {
                            CSocket.Send(EudemonPacket.SpawnNPC(Npc.ID, Npc.X, Npc.Y, Npc.SubType, Npc.Direction, Npc.Flag));
                        }
                    }
                }
            }
            try
            {
                Monitor.Enter(MainGS.ItemFloor);
                foreach (KeyValuePair<int, Struct.ItemGround> Ig in MainGS.ItemFloor)
                {
                    Struct.ItemGround IG = Ig.Value;
                    if ((int)CSocket.Client.Map == IG.Map)
                    {
                        if (!Calculation.CanSee(CSocket.Client.PrevX, CSocket.Client.PrevY, IG.X, IG.Y))
                        {
                            if (Calculation.CanSee(CSocket.Client.X, CSocket.Client.Y, IG.X, IG.Y))
                            {
                                CSocket.Send(EudemonPacket.DropItem(IG.UID, IG.ItemID, IG.X, IG.Y));
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
                Monitor.Exit(MainGS.ItemFloor);
            }
            foreach (KeyValuePair<int, Struct.TerrainNPC> Tnpcs in MainGS.TerrainNpcs)
            {
                Struct.TerrainNPC TNpc = Tnpcs.Value;
                if (TNpc.Map == (int)CSocket.Client.Map)
                {
                    if (!Calculation.CanSee(CSocket.Client.PrevX, CSocket.Client.PrevY, TNpc.X, TNpc.Y))
                    {
                        if (Calculation.CanSee(CSocket.Client.X, CSocket.Client.Y, TNpc.X, TNpc.Y))
                        {
                            if (TNpc.Map == 1038)
                            {
                                if (TNpc.UID == 6700)
                                {
                                    CSocket.Send(EudemonPacket.TerrainNPC(TNpc.UID, TNpc.MaximumHP, TNpc.CurrentHP, TNpc.X, TNpc.Y, TNpc.Type, MainGS.PoleHolder, TNpc.Flag));
                                }
                                else
                                {
                                    CSocket.Send(EudemonPacket.TerrainNPC(TNpc.UID, TNpc.MaximumHP, TNpc.CurrentHP, TNpc.X, TNpc.Y, TNpc.Type, "Gate", TNpc.Flag));
                                }
                            }
                            else
                            {
                                CSocket.Send(EudemonPacket.TerrainNPC(TNpc));
                            }
                        }
                    }
                }
            }
        }
    }
}
