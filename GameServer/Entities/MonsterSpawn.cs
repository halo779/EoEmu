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
            if (Nano.BaseMonsters.ContainsKey(MobID))
            {
                MonsterInfo MobInfo = Nano.BaseMonsters[MobID];
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
                        //Mon.X = Nano.Rand.Next(X, XStop);
                        //Mon.Y = Nano.Rand.Next(Y, YStop);
                        //Mon.X = Mon.SpawnX;
                        //Mon.Y = Mon.SpawnY;
                        MobX = Nano.Rand.Next(X, (X + XStop));
                        MobY = Nano.Rand.Next(Y, (Y + YStop));
                        if (Nano.Maps.ContainsKey(Map))
                        {
                            Structs.Struct.DmapData Mapping = Nano.Maps[Map];
                            while (!Mapping.CheckLoc((ushort)MobX, (ushort)MobY))
                            {
                                MobX = Nano.Rand.Next(X, (X + XStop));
                                MobY = Nano.Rand.Next(Y, (Y + YStop));
                            }
                        }
                        UID = Nano.Rand.Next(200000, 600000);
                        while (Nano.Monsters.ContainsKey(UID))
                        {
                            UID = Nano.Rand.Next(200000, 600000);
                        }
                        Mon.CurrentHP = MobInfo.MaxHP;
                        Mon.MaxHP = MobInfo.MaxHP;
                        Mon.UID = UID;
                        Mon.Direction = 0;
                        Mon.ID = MobInfo.ID;
                        Mon.Info = MobInfo;
                        Mon.Level = MobInfo.Level;
                        Mon.Map = Map;
                        Mon.X = MobX;
                        Mon.SpawnX = MobX;
                        Mon.Y = MobY;
                        Mon.SpawnY = MobY;
                        Mon.SpawnID = SpawnID;
                        if (!Nano.Monsters.ContainsKey(Mon.UID))
                        {
                            //lock(Nano.Monsters)
                            //{
                            try
                            {
                                Monitor.Enter(Nano.Monsters);
                                Nano.Monsters.Add(Mon.UID, Mon);
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
                        }
                        else
                        {
                            while (Nano.Monsters.ContainsKey(Mon.UID))
                            {
                                Mon.UID = Nano.Rand.Next(200000, 600000);
                            }
                            //lock(Nano.Monsters)
                            //{
                            try
                            {
                                Monitor.Enter(Nano.Monsters);
                                Nano.Monsters.Add(Mon.UID, Mon);
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
                        }
                        ConquerPacket.ToLocal(ConquerPacket.SpawnMonster(Mon.UID, MobInfo.Mesh, Mon.X, Mon.Y, MobInfo.Name, Mon.CurrentHP, Mon.Level, Mon.Direction), Mon.X, Mon.Y, Mon.Map, 0, 0);
                        ConquerPacket.ToLocal(ConquerPacket.MobSpawnEffect(Mon.UID, Mon.X, Mon.Y, Mon.Direction, 131), Mon.X, Mon.Y, Mon.Map, 0, 0);
                        Interlocked.Add(ref SpawnNumber, 1);
                    }
                }
            }
        }
    }
}
