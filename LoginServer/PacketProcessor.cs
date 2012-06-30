using System;
using LoginServer.Connections;
using LoginServer.Database;

namespace LoginServer
{
    /// <summary>
    /// Allows for handling of packets from the client->loginserver or client->gameserver.
    /// </summary>
    public static class PacketProcessor
    {
        /// <summary>
        /// used to produce simple byte arrays from hex strings
        /// </summary>
        /// <param name="strInput">hex string (text string may also work)</param>
        /// <returns>returns the string in byte array form</returns>
        public static byte[] String_To_Bytes(string strInput)
        {
            // i variable used to hold position in string  
            int i = 0;
            // x variable used to hold byte array element position  
            int x = 0;
            // allocate byte array based on half of string length  
            byte[] bytes = new byte[(strInput.Length) / 2];
            // loop through the string - 2 bytes at a time converting  
            //  it to decimal equivalent and store in byte array  
            while (strInput.Length > i + 1)
            {
                long lngDecimal = Convert.ToInt32(strInput.Substring(i, 2), 16);
                bytes[x] = Convert.ToByte(lngDecimal);
                i = i + 2;
                ++x;
            }
            // return the finished byte array of decimal values  
            return bytes;
        }
        /// <summary>
        /// void to prcess all packets sent to the login server
        /// </summary>
        /// <param name="Data">packet byte array for handling</param>
        /// <param name="CSocket">client socket</param>
        public static void ProcessPacket(byte[] Data, ClientSocket CSocket)
        {
            int Type = (BitConverter.ToInt16(Data, 2));
            switch (Type)
            {
                case 1060://login request
                    {
                        RequestLogin(Data, CSocket);
                        break;
                    }
                default:
                    {
                        Console.WriteLine("[LoginServer] Unknown packet type: " + Type);
                        break;
                    }
            }
        }

        /// <summary>
        /// Method to handle Login Requests
        /// </summary>
        /// <param name="Data">Packet in byte array</param>
        /// <param name="CSocket">Client Socket</param>
        public static void RequestLogin(byte[] Data, ClientSocket CSocket)
        {
            string AccountName = "";
            string Password = "";
            string ServerName = "";
            if (Data.Length >= 276)
            {
                for (int i = 4; i < 0x114; i++)
                {
                    if (i >= 0x14 && i < 0xf9)
                    {
                        if (Data[i] != 0x00)
                            Password += Convert.ToChar(Data[i]);
                    }
                    if (i < 0x14)
                        if (Data[i] != 0x00)
                            AccountName += Convert.ToChar(Data[i]);
                    if (i > 0xfa)
                        if (Data[i] != 0x00)
                            ServerName += Convert.ToChar(Data[i]);
                }
            }
            else
            {
                return;
            }
            System.Random Random = new System.Random();
            Console.WriteLine("[LoginServer] " + AccountName + " logging in to " + ServerName);
            string DBPass = Database.Database.Password(AccountName);
            if (DBPass != "ERROR")
            {
                if (DBPass == Password)
                {
                    //A ban Check could be put here
                    uint Key = (uint)(Random.Next(10000000));
                    Key = Key << 32;
                    Key = Key << 32;
                    Key = (uint)(Key | (uint)Random.Next(10000000));
                    byte[] Key1 = new byte[4];
                    byte[] Key2 = new byte[4];
                    Key1[0] = (byte)(((ulong)Key & 0xff00000000000000L) >> 56);
                    Key1[1] = (byte)((Key & 0xff000000000000) >> 48);
                    Key1[2] = (byte)((Key & 0xff0000000000) >> 40);
                    Key1[3] = (byte)((Key & 0xff00000000) >> 32);
                    Key2[0] = (byte)((Key & 0xff000000) >> 24);
                    Key2[1] = (byte)((Key & 0xff0000) >> 16);
                    Key2[2] = (byte)((Key & 0xff00) >> 8);
                    Key2[3] = (byte)(Key & 0xff);
                    if (ServerName == "ghost")
                    {
                        if (AuthSocket.Authorize(AccountName, Key, false))//checks if the sending to the game server is successful (no error check atm so if it fails to send there will be an error)
                        {
                            CSocket.Send(Packets.AuthResponse("127.0.0.1", Key1, Key2));
                        }
                        else
                        {
                            Console.WriteLine("Unable to send client data to gameserver");
                            CSocket.Send(Packets.ErrorMessage("Game Server is down"));
                            Console.WriteLine("Disconnecting client");
                            CSocket.Disconnect();
                        }
                    }
                    else
                    {
                        Console.WriteLine("Client used the server name \"" + ServerName + "\" but this server is not on the connectable list");
                        Console.WriteLine("Disconnecting client");
                        CSocket.Disconnect();
                    }
                }
                    //This has not been set for EO yet but is kept from the base source as it could be used in the future
                    //This will set the Password for the account when it logs in for the first time to the password which is entered
                /*else if (DBPass == "")
                {
                    Console.WriteLine("[LoginServer](Diagnostic) Set password for " + AccountName);
                    Database.Database.SetPass(AccountName, Password);
                    //OKAY to login!
                    uint Key = (uint)(Random.Next(10000000) << 32);
                    Key = Key << 32;
                    Key = (uint)(Key | (uint)Random.Next(10000000));
                    byte[] Key1 = new byte[4];
                    byte[] Key2 = new byte[4];
                    Key1[0] = (byte)(((ulong)Key & 0xff00000000000000L) >> 56);
                    Key1[1] = (byte)((Key & 0xff000000000000) >> 48);
                    Key1[2] = (byte)((Key & 0xff0000000000) >> 40);
                    Key1[3] = (byte)((Key & 0xff00000000) >> 32);
                    Key2[0] = (byte)((Key & 0xff000000) >> 24);
                    Key2[1] = (byte)((Key & 0xff0000) >> 16);
                    Key2[2] = (byte)((Key & 0xff00) >> 8);
                    Key2[3] = (byte)(Key & 0xff);
                    if (ServerName == "ghost")
                    {
                        if (AuthSocket.Authorize(AccountName, Key, false))//checks if the sending to the game server is successful (no error check atm so if it fails to send there will be an error)
                        {
                            CSocket.Send(Packets.AuthResponse("127.0.0.1", Key1, Key2));
                        }
                        else
                        {
                            Console.WriteLine("Unable to send client data to gameserver");
                            Console.WriteLine("Disconnecting client");
                        }
                    }

                    else
                    {
                        Console.WriteLine("Client used the server name \"" + ServerName + "\" but this server is not on the connectable list");
                        Console.WriteLine("Disconnecting client");
                        CSocket.Disconnect();
                    }
                }*/
                else
                {
                    Console.WriteLine("Client entered the wrong password");
                    CSocket.Send(Packets.WrongPass());
                    CSocket.Disconnect();
                }

            }
            else
            {
                Console.WriteLine("DBPass equals ERROR: " + DBPass);
                CSocket.Disconnect();
            }
        }
    }
}
