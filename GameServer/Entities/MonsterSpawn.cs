using System;
using System.Timers;
using System.Threading;
using GameServer.Packets;

namespace GameServer.Entities
{
    /// <summary>
    /// Contains information for base monster spawns.
    /// </summary>
    public class MonsterSpawn
    {
        public int X;
        public int Y;
        public int XStop;
        public int YStop;
        public int MobID;
        public int SpawnID;
        public int Map;
        public int SpawnNumber;
        public int MaxSpawnNumber;
        public System.Timers.Timer RespawnTimer;
        public void Respawn()
        {
            if (MainGS.BaseMonsters.ContainsKey(MobID))
            {
                MonsterInfo MobInfo = MainGS.BaseMonsters[MobID];
                if (SpawnNumber != MaxSpawnNumber)
                {
                    while (SpawnNumber < MaxSpawnNumber)
                    {
                        //Console.WriteLine("[Monster-Respawns] Respawning " + SpawnID);
                        Monster Mon = new Monster();
                        int MobX = 0;
                        int MobY = 0;
                        int UID = 0;
                        //TODO: Dmaps
                        //Mon.X = MainGS.Rand.Next(X, XStop);
                        //Mon.Y = MainGS.Rand.Next(Y, YStop);
                        //Mon.X = Mon.SpawnX;
                        //Mon.Y = Mon.SpawnY;
                        MobX = MainGS.Rand.Next(X, (X + XStop));
                        MobY = MainGS.Rand.Next(Y, (Y + YStop));
                        if (MainGS.Maps.ContainsKey(Map))
                        {
                            Structs.Struct.DmapData Mapping = MainGS.Maps[Map];
                            while (!Mapping.CheckLoc((ushort)MobX, (ushort)MobY))
                            {
                                MobX = MainGS.Rand.Next(X, (X + XStop));
                                MobY = MainGS.Rand.Next(Y, (Y + YStop));
                            }
                        }
                        UID = MainGS.Rand.Next(400000, 600000);
                        while (MainGS.Monsters.ContainsKey(UID))
                        {
                            UID = MainGS.Rand.Next(400000, 600000);
                        }
                        Mon.CurrentHP = MobInfo.MaxHP;
                        Mon.MaxHP = MobInfo.MaxHP;
                        Mon.UID = UID;
                        //Mon.Direction = MainGS.Rand.Next(1,7); //@TODO: Fix Monster Directions..
                        Mon.Direction = 2;
                        Mon.ID = MobInfo.ID;
                        Mon.Info = MobInfo;
                        Mon.Level = MobInfo.Level;
                        Mon.Map = Map;
                        Mon.X = MobX;
                        Mon.SpawnX = MobX;
                        Mon.Y = MobY;
                        Mon.SpawnY = MobY;
                        Mon.SpawnID = SpawnID;
                        if (!MainGS.Monsters.ContainsKey(Mon.UID))
                        {
                            //lock(MainGS.Monsters)
                            //{
                            try
                            {
                                Monitor.Enter(MainGS.Monsters);
                                MainGS.Monsters.Add(Mon.UID, Mon);
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
                        }
                        else
                        {
                            while (MainGS.Monsters.ContainsKey(Mon.UID))
                            {
                                Mon.UID = MainGS.Rand.Next(400000, 600000);
                            }
                            //lock(MainGS.Monsters)
                            //{
                            try
                            {
                                Monitor.Enter(MainGS.Monsters);
                                MainGS.Monsters.Add(Mon.UID, Mon);
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
                        }
                        EudemonPacket.ToLocal(EudemonPacket.SpawnMonster(Mon.UID, MobInfo.Mesh, Mon.X, Mon.Y, MobInfo.Name, Mon.CurrentHP, Mon.Level, Mon.Direction), Mon.X, Mon.Y, Mon.Map, 0, 0);
                        EudemonPacket.ToLocal(EudemonPacket.General(Mon.UID, (ushort)Mon.X, (ushort)Mon.Y, (ushort)Mon.Direction, Structs.Struct.DataType.actionSoundEffect, Mon.ID), Mon.X, Mon.Y, Mon.Map, 0, 0);//Spawns Monster Effect.
                        Interlocked.Add(ref SpawnNumber, 1);
                    }
                }
            }
        }
    }
}
