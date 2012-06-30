using System;
using GameServer.Entities;
using System.Collections.Generic;
using GameServer.Structs;

namespace GameServer.Calculations
{
    public partial class Calculation
    {
        public static void BP(Character CSocket)
        {
            int BP = CSocket.Level;

            foreach (KeyValuePair<int, Struct.ItemInfo> Item in CSocket.Equipment)
            {
                byte ItemQuality = (byte)(Item.Value.ItemID % 10);
                switch (ItemQuality)
                {
                    case 0:
                        {
                            BP += 1;
                            break;
                        }
                    case 1:
                        {
                            BP += 2;
                            break;
                        }
                    case 2:
                        {
                            BP += 3;
                            break;
                        }
                    case 3:
                        {
                            BP += 4;
                            break;
                        }
                    case 4:
                        {
                            BP += 5;
                            break;
                        }
                }
                if (Item.Value.Soc1 != 0)
                {
                    BP += 1;
                }
                if (Item.Value.Soc2 != 0)
                {
                    BP += 1;
                }
            }
            BP += (CSocket.NAM * 0);
            BP += (CSocket.RAM * 2);
            BP += (CSocket.SAM * 4);

            CSocket.BP = BP;
        }
    }
}
