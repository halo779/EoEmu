using System;
using System.Collections;
using System.Collections.Generic;
using GameServer.Structs;

namespace GameServer.Calculations
{
    /// <summary>
    /// Generates a new item from a list of variables.
    /// Partial Credit: LOTF Core
    /// </summary>
    public partial class Calculation
    {
        public static int Item(int Level, int Quality1)
        {
            int FinalID = -1;
            int tries;
            /*if (PercentSuccess(35))
                tries = Nano.Rand.Next(1, 8);
            else if (PercentSuccess(35))
                tries = Nano.Rand.Next(1, 16);
            else
            	tries = Nano.Rand.Next(1, 32);*/
            tries = Nano.Rand.Next(1, 7);
            int count = 0;
            int ItemType1 = 0;
            int ItemType0 = 0;
            int nr = Nano.Rand.Next(1, 7);
            if (nr == 1)
                ItemType1 = 11;
            else if (nr == 2)
                ItemType1 = 12;
            else if (nr == 3)
                ItemType1 = 13;
            else if (nr == 4)
                ItemType1 = 4;
            else if (nr == 5)
                ItemType1 = 5;
            else if (nr == 6)
                ItemType1 = 15;
            else if (nr == 7)
                ItemType1 = 16;

            nr = Nano.Rand.Next(1, 17);
            if (nr == 1)
                ItemType0 = 410;
            else if (nr == 2)
                ItemType0 = 420;
            else if (nr == 3)
                ItemType0 = 421;
            else if (nr == 4)
                ItemType0 = 430;
            else if (nr == 5)
                ItemType0 = 440;
            else if (nr == 6)
                ItemType0 = 450;
            else if (nr == 7)
                ItemType0 = 460;
            else if (nr == 8)
                ItemType0 = 480;
            else if (nr == 9)
                ItemType0 = 481;
            else if (nr == 10)
                ItemType0 = 490;
            else if (nr == 11)
                ItemType0 = 500;
            else if (nr == 12)
                ItemType0 = 510;
            else if (nr == 13)
                ItemType0 = 530;
            else if (nr == 14)
                ItemType0 = 540;
            else if (nr == 15)
                ItemType0 = 560;
            else if (nr == 16)
                ItemType0 = 561;
            else if (nr == 17)
                ItemType0 = 580;
            foreach (KeyValuePair<int, Struct.ItemData> Items in Nano.Items)
            {
                Struct.ItemData Item = Items.Value;
                if ((Item.Level - 10 < Level && Item.Level + 30 > Level) && Item.Level != 0)
                {
                    if (Quality(Item.ID.ToString()) == Quality1)
                    {
                        if (Type1(Item.ID.ToString()) == 1 || Type1(Item.ID.ToString()) == 4 || Type1(Item.ID.ToString()) == 5 || Type1(Item.ID.ToString()) == 9)
                        {
                            if (Type2(Item.ID.ToString()) == ItemType1 || (ItemType1 == 4 && WeaponType(Item.ID.ToString()) == ItemType0) || (ItemType1 == 5 && WeaponType(Item.ID.ToString()) == ItemType0))
                            {
                                FinalID = Item.ID;
                                count++;
                                if (count >= tries)
                                    break;
                            }
                        }
                    }
                }
            }
            return FinalID;
        }
    }
}
