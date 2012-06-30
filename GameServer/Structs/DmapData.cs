using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using IniParser;

namespace GameServer.Structs
{
    /// <summary>
    /// Provides mapping data for Dmaps
    /// </summary>
    public partial class Struct
    {
        public class DmapData
        {
            public ushort[,] Tiles;
            public DmapData(string File)
            {
                FileStream dmFile = new FileStream(File, FileMode.Open);
                BinaryReader dmReader = new BinaryReader(dmFile);

                dmReader.ReadBytes(8);
                dmReader.ReadBytes(260);
                ushort XCount = Convert.ToUInt16(dmReader.ReadUInt32());
                ushort YCount = Convert.ToUInt16(dmReader.ReadUInt32());
                Tiles = new ushort[XCount, YCount];
                for (ushort y = 0; y < YCount; y++)
                {
                    for (ushort x = 0; x < XCount; x++)
                    {
                        ushort Access = dmReader.ReadUInt16();
                        dmReader.ReadUInt16();
                        dmReader.ReadUInt16();
                        if (Access != 1)
                        {
                            /*Tiles.Add(Count, new DmapTile(x, y));
                            Count++;*/
                            Tiles[x, y] = 1;
                        }
                        else
                        {
                            Tiles[x, y] = 0;
                        }
                    }
                    dmReader.ReadInt32();
                }
                dmFile.Close();
                dmReader.Close();
                dmFile.Dispose();
            }
            public bool CheckLoc(ushort x, ushort y)
            {
                try
                {
                    if (Tiles[x, y] == 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch
                {
                    return false;
                }
            }
        }
        public class DmapTile
        {
            public ushort X;
            public ushort Y;
            public DmapTile(ushort _x, ushort _y)
            {
                X = _x;
                Y = _y;
            }
        }
        public class DMap
        {
            public UInt32 MapId;
            public string MapFile;

            public DMap(UInt32 pmapid, string pmapfile)
            {
                MapId = pmapid;
                MapFile = pmapfile;
            }
        }

        public class GameMap
        {
            private UInt32 m_mapCount;
            private DMap[] m_maps;

            public GameMap(string filename)
            {
                /*FileStream gmFile = new FileStream(filename, FileMode.Open);
                BinaryReader gmReader = new BinaryReader(gmFile);

                m_mapCount = gmReader.ReadUInt32();
                m_maps = new DMap[m_mapCount];

                for (int i = 0; i < m_mapCount; i++)
                {
                    UInt32 mapId = gmReader.ReadUInt32();
                    int mapnamelen = gmReader.ReadInt32();

                    m_maps[i] = new DMap(mapId, Encoding.ASCII.GetString(gmReader.ReadBytes(mapnamelen)).Remove(0, 8), gmReader.ReadInt32());
                    //Console.WriteLine("Data/maps/" + m_maps[i].MapFile);

                }
                gmFile.Dispose();
                gmFile.Close();
                gmReader.Close();*/
                FileIniDataParser praser = new FileIniDataParser();
                IniData data = praser.LoadFile(filename);
                m_maps = new DMap[data.Sections.Count];
                foreach (SectionData map in data.Sections)
                {
                    UInt32 mapid = Convert.ToUInt32(map.SectionName.Remove(0, 3));
                    string MapFile = map.Keys.GetKeyData("File").Value;
                    MapFile = MapFile.Remove(0, MapFile.LastIndexOf('/') + 1);
                    m_maps[m_mapCount] = new DMap(mapid, MapFile);
                    m_mapCount++;
                }
            }

            public UInt32 MapCount
            {
                get
                {
                    return m_mapCount;
                }
            }

            public DMap[] Maps
            {
                get
                {
                    return m_maps;
                }
            }

        }
    }
}
