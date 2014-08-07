using System;
using System.Collections;
using System.Collections.Generic;
using GameServer.Connections;
using GameServer.Entities;
using GameServer.Structs;
using GameServer.Packets;
using GameServer.Calculations;
using Lua511;
using LuaInterface;

namespace GameServer.Handlers
{
    /// <summary>
    /// Description of NpcTalk.
    /// </summary>
    public partial class Handler
    {
        public static void NpcTalk(ClientSocket CSocket, int ID, int LinkBack)
        {
            if (ID > 0)
                CSocket.Client.LastNPC = ID;
            switch (ID)
            {
                case 12040:
                    {
                        if (LinkBack == 0)
                        {
                            Text("Hello! The Composer of market.", CSocket);
                            Text("What would you like to do?", CSocket);
                            Link("Main Composing", 1, CSocket);
                            Link("Nevermind!", 255, CSocket);
                            End(CSocket);
                        }
                        else if (LinkBack == 1)
                        {
                            CSocket.Send(EudemonPacket.GeneralOld(0, CSocket.Client.ID, 319, 445, 4, 37, Struct.DataType.Dialog));
                        }
                        break;
                    }
                case 1504://high Bonus person
                    {
                        if (LinkBack == 0)
                        {
                            Text("Hello! How can I help?.", CSocket);
                            Link("[Add a high Bonus to Gear]", 1, CSocket);
                            Link("[Nevermind]", 255, CSocket);
                            End(CSocket);
                        }
                        else if (LinkBack == 1)
                        {
                            CSocket.Send(EudemonPacket.GeneralOld(0, CSocket.Client.ID, CSocket.Client.X, CSocket.Client.Y, 1, (int)Struct.Dialog.ItemHighBonus, Struct.DataType.Dialog));
                        }

                        break;
                    }
                case 741://Low Bonus person
                    {
                        if (LinkBack == 0)
                        {
                            Text("Hello! How can I help?.", CSocket);
                            Link("[Add Bonus to Gear]", 1, CSocket);
                            Link("[Nevermind]", 255, CSocket);
                            End(CSocket);
                        }
                        else if (LinkBack == 1)
                        {
                            CSocket.Send(EudemonPacket.GeneralOld(0, CSocket.Client.ID, CSocket.Client.X, CSocket.Client.Y, 1, (int)Struct.Dialog.ItemBonus, Struct.DataType.Dialog));
                        }
                        
                        break;
                    }
                case 10012://PC - WHS
                    {
                        CSocket.Send(EudemonPacket.GeneralOld(CSocket.Client.ID, 4, 0, 0, 0, 0, Struct.DataType.Dialog));
                        break;
                    }
                case 10011://DC - WHS
                    {
                        CSocket.Send(EudemonPacket.GeneralOld(CSocket.Client.ID, 4, 0, 0, 0, 0, Struct.DataType.Dialog));
                        break;
                    }
                case 10028://AM - WHS
                    {
                        CSocket.Send(EudemonPacket.GeneralOld(CSocket.Client.ID, 4, 0, 0, 0, 0, Struct.DataType.Dialog));
                        break;
                    }
                case 10027://BI - WHS
                    {
                        CSocket.Send(EudemonPacket.GeneralOld(CSocket.Client.ID, 4, 0, 0, 0, 0, Struct.DataType.Dialog));
                        break;
                    }
                case 44://MA - WHS
                    {
                        CSocket.Send(EudemonPacket.GeneralOld(CSocket.Client.ID, 4, 0, 0, 0, 0, Struct.DataType.Dialog));
                        break;
                    }
                case 10050: //TC - Conductress
                    {
                        if (LinkBack == 0)
                        {
                            Text("Hello! I am Twin City's Conductress. I can teleport you to a variety of places, for free!", CSocket);
                            Text("What kind of person charges money for teleportaion?", CSocket);
                            Link("Ape City", 1, CSocket);
                            Link("Bird Island", 2, CSocket);
                            Link("Phoenix Castle", 3, CSocket);
                            Link("Desert City", 4, CSocket);
                            Link("Market", 5, CSocket);
                            Link("Nevermind!", 255, CSocket);
                            End(CSocket);
                        }
                        else if (LinkBack == 1) //Ape City
                        {
                            Teleport(1002, 555, 958, 0, CSocket);
                        }
                        else if (LinkBack == 2) //Bird Island
                        {
                            Teleport(1002, 229, 197, 0, CSocket);
                        }
                        else if (LinkBack == 3)//Phoenix Castle
                        {
                            Teleport(1002, 956, 555, 0, CSocket);
                        }
                        else if (LinkBack == 4)//Desert City
                        {
                            Teleport(1002, 67, 463, 0, CSocket);
                        }
                        else if (LinkBack == 5)//Market
                        {
                            Teleport(1036, 211, 196, 0, CSocket);
                        }
                        break;
                    }
                case 390:   //Market - LoveStone
                    {
                        if (LinkBack == 0)
                        {
                            // Text("Do you wish to propose to your sweetheart?")); Needs to be
                            Text("Do you wish to propose to your sweetheart?", CSocket); //Remove one ) and add, CSocket
                            //Text("Remember that marriage is a serious commitment, do not enter into it lightly.")); Needs to be
                            Text("Remember that marriage is a serious commitment, do not enter into it lightly.", CSocket);// Remove one ) and add CSocket
                            Link("Yes I want to propose", 1, CSocket);
                            Link("No, I am not ready yet", 255, CSocket);//Add 255 
                            End(CSocket);
                        }//<-- was a {, needs to be }
                        else if (LinkBack == 1)
                        {

                        }
                        break;
                    }
                case 1152: // Simon the lab teleporter
                    {
                        if (LinkBack == 0)
                        {
                            Text("Hello My name is Simon, And I can help you train after you reach Level 70.\n Although I am willing to help you out I have one requirement:\n you pay me 100,000 gold for my services.", CSocket);
                            Link("I am prepared to pay you for your\n wonderful service!", 1, CSocket);
                            Link("I'm sorry I can't agree with your\n demand, But Thank you anyway", 255, CSocket);
                            End(CSocket);
                        }
                        else if (LinkBack == 1)
                        {
                            //Teleport(lab, 000, 000, 0, CSocket); //Don't Know Lab map id
                            if (CSocket.Client.Level >= 70)
                            {
                                if (CSocket.Client.Money >= 100000)
                                {
                                    Money(-100000, CSocket);
                                    Teleport(1351, 20, 130, 0, CSocket);
                                }
                                else
                                {
                                    Text("Dare you try to rip me off?! Be gone, fool!", CSocket);
                                    Link("Sorry sir.", 255, CSocket);
                                    End(CSocket);
                                }
                            }
                            else
                            {
                                Text("You are not yet level 70. Train harder.", CSocket);
                                Link("Oh, sorry. Thanks!", 255, CSocket);
                                End(CSocket);
                            }
                        }
                        break;
                    }
                case 104839: // BoxerLi PC- TG teleporter
                    {
                        if (LinkBack == 0)
                        {
                            Text("Hello I can help you train after you reach level 20,\n But I will charge you 1,000 gold.\n Would you like to go to the Training Grounds?", CSocket);
                            Link("Yes Please, Here is the money", 1, CSocket);
                            Link("No Thank you I cannot afford it.", 255, CSocket);
                            End(CSocket);
                        }
                        else if (LinkBack == 1)
                        {
                            if (CSocket.Client.Money >= 1000)
                            {
                                Teleport(1039, 219, 215, 0, CSocket);
                                Money(-1000, CSocket);
                            }
                            else
                            {
                                Text("How dare you try to rip me off! Get lost, Or get my money!", CSocket);
                                Link("I'm sorry, I didn't realize.", 255, CSocket);
                                End(CSocket);
                            }
                        }
                        break;
                    }
                case 45: // Mark. Controller
                    {
                        if (LinkBack == 0)
                        {
                            Text("Hello I can teleport You outside of the market for free! Do you want to leave?", CSocket);
                            Link("Why would you be so kind to do so?", 1, CSocket);
                            Link("No, Thank you anyway.", 255, CSocket);
                            End(CSocket);
                        }
                        else if (LinkBack == 1)
                        {
                            Teleport(1002, 439, 390, 0, CSocket);
                        }
                        break;
                    }
                case 104827:// Guild Map-GC 1
                    {
                        if (LinkBack == 0)
                        {
                            Text("Hello I can teleport you to The Advanced Zone, \nnFor a tiny fee of 500 gold!", CSocket);
                            Link("Sure here ya are.", 1, CSocket);
                            Link("No Thanks!", 255, CSocket);
                            End(CSocket);
                        }
                        else if (LinkBack == 1)
                        {
                            if (CSocket.Client.Money >= 500)
                            {
                                Teleport(1017, 518, 557, 0, CSocket);
                                Money(-500, CSocket);
                            }
                            else
                            {
                                Text("I'm sorry you do not have the required Gold.", CSocket);
                                Link("Okay, I will be back when I have 500 Gold.", 255, CSocket);
                                End(CSocket);
                            }
                        }
                        break;
                    }
                case 104821:// Guild Map-GC 2
                    {
                        if (LinkBack == 0)
                        {
                            Text("Hello I can teleport you to Mystic Castle, \nFor a tiny fee of 500 gold!", CSocket);
                            Link("Sure here ya are.", 1, CSocket);
                            Link("No Thanks!", 255, CSocket);
                            End(CSocket);
                        }
                        else if (LinkBack == 1)
                        {
                            if (CSocket.Client.Money >= 500)
                            {
                                Teleport(1001, 272, 181, 0, CSocket);
                                Money(-500, CSocket);
                            }
                            else
                            {
                                Text("I'm sorry you do not have the required Gold.", CSocket);
                                Link("Okay, I will be back when i have 500 Gold.", 255, CSocket);
                                End(CSocket);
                            }
                        }
                        break;
                    }
                case 104815:// Guild Map-GC 3
                    {
                        if (LinkBack == 0)
                        {
                            Text("Hello I can teleport you to Ape Mountain, \nFor a tiny fee of 500 gold!", CSocket);
                            Link("Sure here ya are.", 1, CSocket);
                            Link("No Thanks!", 255, CSocket);
                            End(CSocket);
                        }
                        else if (LinkBack == 1)
                        {
                            if (CSocket.Client.Money >= 500)
                            {
                                Teleport(1020, 539, 534, 0, CSocket);
                                Money(-500, CSocket);
                            }
                            else
                            {
                                Text("I'm sorry you do not have the required Gold.", CSocket);
                                Link("Okay, I will be back when i have 500 Gold.", 255, CSocket);
                                End(CSocket);
                            }
                        }
                        break;
                    }
                case 104809:// Guild Map-GC 4
                    {
                        if (LinkBack == 0)
                        {
                            Text("Hello I can teleport you to Bird Island, \nFor a tiny fee of 500 gold!", CSocket);
                            Link("Sure here ya are.", 1, CSocket);
                            Link("No Thanks!", 255, CSocket);
                            End(CSocket);
                        }
                        else if (LinkBack == 1)
                        {
                            if (CSocket.Client.Money >= 500)
                            {
                                Teleport(1015, 705, 564, 0, CSocket);
                                Money(-500, CSocket);
                            }
                            else
                            {
                                Text("I'm sorry you do not have the required Gold.", CSocket);
                                Link("Okay, I will be back when i have 500 Gold.", 255, CSocket);
                                End(CSocket);
                            }
                        }
                        break;
                    }
                case 10056: //BI - Conductress
                    {
                        if (LinkBack == 0)
                        {
                            Text("Hello! I am Bird Islands' Conductress. I can teleport you to Twin City, or the market for free!", CSocket);
                            Text("What kind of person charges money for teleportation?", CSocket);
                            Link("Twin City", 1, CSocket);
                            Link("Market", 2, CSocket);
                            Link("Nevermind!", 255, CSocket);
                            End(CSocket);
                        }
                        else if (LinkBack == 1) //Twin City
                        {
                            Teleport(1015, 1010, 710, 0, CSocket);
                        }
                        else if (LinkBack == 2)//Market
                        {
                            Teleport(1036, 211, 196, 0, CSocket);
                        }
                        break;
                    }
                case 10051: //DC - Conductress
                    {
                        if (LinkBack == 0)
                        {
                            Text("Hello! I am Desert Citys' Conductress. I can teleport you to Twin City, or the market for free!", CSocket);
                            Text("What kind of person charges money for teleportation?", CSocket);
                            Link("Twin City", 1, CSocket);
                            Link("Market", 2, CSocket);
                            Link("Mystic Castle", 3, CSocket);
                            Link("Nevermind!", 255, CSocket);
                            End(CSocket);
                        }
                        else if (LinkBack == 1) //Twin City
                        {
                            Teleport(1000, 970, 666, 0, CSocket);
                        }
                        else if (LinkBack == 2)//Market
                        {
                            Teleport(1036, 211, 196, 0, CSocket);
                        }
                        else if (LinkBack == 3)//Mystic Castle
                        {
                            Teleport(1000, 80, 320, 0, CSocket);
                        }
                        break;
                    }
                case 10052: //PC - Conductress
                    {
                        if (LinkBack == 0)
                        {
                            Text("Hello! I am Phoenix Castles' Conductress. I can teleport you to Twin City, or the market for free!", CSocket);
                            Text("What kind of person charges money for teleportaion?", CSocket);
                            Link("Twin City", 1, CSocket);
                            Link("Market", 2, CSocket);
                            Link("Nevermind!", 255, CSocket);
                            End(CSocket);
                        }
                        else if (LinkBack == 1) //Twin City
                        {
                            Teleport(1011, 11, 376, 0, CSocket);
                        }
                        else if (LinkBack == 2)//Market
                        {
                            Teleport(1036, 211, 196, 0, CSocket);
                        }
                        break;
                    }
                case 10053: //AC - Conductress
                    {
                        if (LinkBack == 0)
                        {
                            Text("Hello! I am Ape Citys' Conductress. I can teleport you to Twin City, or the market for free!", CSocket);
                            Text("What kind of person charges money for teleportaion?", CSocket);
                            Link("Twin City", 1, CSocket);
                            Link("Market", 2, CSocket);
                            Link("Nevermind!", 255, CSocket);
                            End(CSocket);
                        }
                        else if (LinkBack == 1) //Twin City
                        {
                            Teleport(1020, 381, 20, 0, CSocket);
                        }
                        else if (LinkBack == 2)//Market
                        {
                            Teleport(1036, 211, 196, 0, CSocket);
                        }
                        break;
                    }
                case 10021: // ArenaGuard
                    {
                        if (LinkBack == 0)
                        {
                            Text("The arena is open, you are welcome to challenge others.\n The admission fee is 1000 silver.", CSocket);
                            Link("Enter the arena.", 1, CSocket);
                            Link("Just passing by.", 255, CSocket);
                            End(CSocket);
                        }
                        else if (LinkBack == 1)
                        {
                            if (CSocket.Client.Money >= 1000)
                            {
                                Teleport(1005, 051, 068, 0, CSocket);
                                Money(-1000, CSocket);
                            }
                        }
                        break;
                    }
                case 30084: // GeneralQing
                    {
                        if (LinkBack == 0)
                        {
                            Text("I have sent my soldiers out to fetch the monthly provisions with my army token, but I have received no news since they left.", CSocket);
                            Link("They must have troubles", 255, CSocket);
                            Link("All will be fine", 255, CSocket);
                            End(CSocket);
                        }
                        break;
                    }
                default:
                    {
                        Text("NPC " + ID + "'s dialog is not coded.", CSocket);
                        Link("Aw! That's too bad!", 255, CSocket);
                        End(CSocket);
                        break;
                    }
            }
        }
        public static void Text(string value, ClientSocket CSocket)
        {
            CSocket.Send(EudemonPacket.NPCTalk(255, 1, value));
        }
        public static void Money(int value, ClientSocket CSocket)
        {
            CSocket.Client.Money += value;
            CSocket.Send(EudemonPacket.Status(CSocket, 2, CSocket.Client.Money, Struct.StatusTypes.InvMoney));
        }
        public static void CPs(int value, ClientSocket CSocket)
        {
            CSocket.Client.EPs += value;
            CSocket.Send(EudemonPacket.Status(CSocket, 2, CSocket.Client.EPs, Struct.StatusTypes.InvEPoints));
        }
        public static void Link(string value, int LinkBack, ClientSocket CSocket)
        {
            CSocket.Send(EudemonPacket.NPCTalk(LinkBack, 2, value));
        }
        public static void Input(int LinkBack, ClientSocket CSocket)
        {
            CSocket.Send(EudemonPacket.NPCTalk(LinkBack, 3, ""));
        }
        public static void End(ClientSocket CSocket)
        {
            CSocket.Send(EudemonPacket.NPCTalk(0, 0, 255, 100));
        }
    }
}
