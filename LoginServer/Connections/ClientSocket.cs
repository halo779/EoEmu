using System;
using System.Net;
using System.Net.Sockets;
using LoginServer.Encryption;

namespace LoginServer.Connections
{
    /// <summary>
    /// The Client connection class.
    /// </summary>
    public class ClientSocket
    {
        protected Socket CSocket;
        protected bool Continue = true;
        protected byte[] CSocketBuffer = new byte[1024]; //Maximum data size of 1024 bytes...Quite a bit :)
        //protected LoginEncryption Crypt;
        protected object Sync = 0;
        //protected Encryption Crypt;

        public ClientSocket(Socket Sock)
        {
            CSocket = Sock;
            //Crypt = new LoginEncryption();
        }

        public void Run()
        {
            while (Continue)
            {
                byte[] Data = null;
                try
                {
                    CSocket.Send(Packets.LoginSeed());
                    int size = CSocket.Receive(CSocketBuffer, SocketFlags.None);
                    if (size < 1000)
                    {
                        Data = new byte[size];
                        Array.Copy(CSocketBuffer, Data, size);
                    }
                    else
                    {
                        Console.WriteLine("[LoginServer] Packet too large for client to send.");
                        Disconnect();
                        Continue = false;
                        break;
                    }
                }
                catch
                {
                    Console.WriteLine("[LoginServer] Unable to accept data from a client.");
                    Continue = false;
                    Disconnect();
                    break;
                }
                if (Data != null && Data.Length > 3)
                {
                    //Crypt.Decrypt(ref Data); //decryption removed but here incase there is encryption added in a later version ... that way we are able to add an encryption key and uncomment and it works perfectly 
                    PacketProcessor.ProcessPacket(Data, this);
                }
                else
                {
                    Console.WriteLine("[LoginServer] Packet sent by client is to small or empty");
                }
                System.Threading.Thread.Sleep(200);
                Continue = false;
            }
        }
        public bool Send(byte[] Data)
        {
            lock (this.Sync)
            {
                //Crypt.Encrypt(ref Data);//encryption removed but here incase there is encryption added in a later version ... that way we are able to add an encryption key and uncomment and it works perfectly 
                try
                {
                    CSocket.Send(Data, Data.Length, SocketFlags.None);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    Disconnect();
                    Continue = false;
                    return false;
                }
                return true;
            }
        }
        public bool Disconnect()
        {
            Console.WriteLine("[LoginServer] Client Disconnect!");
            try
            {
                this.CSocket.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
            return true;
        }

    }
}
