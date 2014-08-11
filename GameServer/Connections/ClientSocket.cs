using System;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using GameServer.Packets;
using GameServer.Entities;
using GameServer.Structs;

namespace GameServer.Connections
{
    /// <summary>
    /// The Client connection class.
    /// </summary>
    public class ClientSocket
    {
        #region Protected Members
        protected Socket CSocket;
        protected bool Continue = true;
        protected byte[] CSocketBuffer = new byte[1024]; //Maximum data size of 1024 bytes...Quite a bit :)
        protected bool ServerShake = false;
        protected bool ClientShake = false;
        protected object SyncRecv;
        protected object SyncSend;
        #endregion
        #region Public Members
        public Character Client;
        public string AccountName;
        #endregion

        public ClientSocket(Socket Sock)
        {
            CSocket = Sock;
            CSocket.NoDelay = true;
            SyncRecv = new object();
            SyncSend = new object();
        }

        public void Run()
        {
            while (Continue & MainGS.Continue)
            {
                if (CSocket.Connected)
                {
                    lock (SyncRecv)
                    {
                        byte[] Data = null;
                        try
                        {
                            int size = CSocket.Receive(CSocketBuffer, SocketFlags.None);
                            if (size == 0)
                            {
                                Disconnect();
                                Continue = false;
                                break;
                            }
                            if (size < 1000)
                            {
                                Data = new byte[size];
                                Array.Copy(CSocketBuffer, Data, size);
                            }
                            else
                            {
                                Console.WriteLine("[GameServer] Packet too large for client to send.");
                                Disconnect();
                                Continue = false;
                                break;
                            }
                        }
                        catch
                        {
                            Console.WriteLine("[GameServer] Unable to accept data from a client.");
                            Continue = false;
                            Disconnect();
                            break;
                        }
                        if (Data != null && Data.Length > 3)
                        {
                            ClientShake = true;
                            if (!ClientShake)
                            {
                                ClientShake = true;
                                Console.WriteLine("Client replying to handshake.");
                            }
                            else
                            {
                                PacketProcessor.ProcessPacket(Data, this);
                            }
                        }
                    }
                }
                else
                {
                    Disconnect();
                    break;
                }
            }
            Disconnect();
        }
        public bool Send(byte[] Data)
        {
            lock (SyncSend)
            {
                if (CSocket.Connected)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("[Packetlogger] Sending Packet to Client [" + Client.ID + " - " + Client.Name +"]: ");
                    Console.ResetColor();
                    Console.WriteLine(PacketProcessor.Dump(Data));
                    try
                    {
                        int sent = CSocket.Send(Data, Data.Length, SocketFlags.None);
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
                else
                {
                    Disconnect();
                    Continue = false;
                    return false;
                }
            }
        }
        public bool Disconnect()
        {
            try
            {
                Continue = false;
                if (Client != null)
                {
                    Client.Equipment.Clear();
                    Client.Skills.Clear();
                    Client.TCWhs.Clear();
                    Monitor.Enter(MainGS.ClientPool);
                    MainGS.ClientPool.Remove(Client.ID);
                    if (MainGS.Continue)
                    {
                        if (Client.Team != null)
                        {
                            if (Client.Team.LeaderID == Client.ID)
                            {
                                foreach (KeyValuePair<int, ClientSocket> Member in Client.Team.Members)
                                {
                                    if (Member.Value.Client.ID != Client.ID)
                                    {
                                        Member.Value.Send(EudemonPacket.Team(Member.Value.Client.ID, Struct.TeamOption.DismissTeam));
                                        Member.Value.Client.Team = null;
                                    }
                                }
                                Client.Team = null;
                            }
                            else
                            {
                                ClientSocket Leader = MainGS.ClientPool[Client.Team.LeaderID];
                                Leader.Client.Team.Members.Remove(Client.ID);
                                foreach (KeyValuePair<int, ClientSocket> Member in Leader.Client.Team.Members)
                                {
                                    if (Member.Value.Client.ID != Client.ID)
                                    {
                                        Member.Value.Send(EudemonPacket.Team(Client.ID, Struct.TeamOption.LeaveTeam));
                                    }
                                }
                                Leader.Client.Team.Members.Remove(Client.ID);
                                Client.Team = null;
                            }
                        }
                        EudemonPacket.ToLocal(EudemonPacket.General(Client.ID, Client.X, Client.Y, Client.Direction, Struct.DataType.EntityRemove, 0), Client.X, Client.Y, (int)Client.Map, 0, Client.ID);
                    }
                    Database.Database.SaveCharacter(Client);
                    if (Client.UpStam != null)
                    {
                        Client.UpStam.Stop();
                        Client.UpStam.Close();
                    }
                    if (Client.Attack != null)
                    {
                        Client.Attack.Stop();
                        Client.Attack.Close();
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
            finally
            {
                if (Client != null)
                    Monitor.Exit(MainGS.ClientPool);
                this.CSocket.Close();
            }
        }
        public void AddStam()
        {
            if (!Client.Dead && Client.Attack == null)
            {
                if (Client.CurrentStam < 100)
                {
                    if (Client.CurrentStam != 99)
                    {
                        Client.CurrentStam += 10;

                        Send(EudemonPacket.Status(this, Struct.StatusTypes.Stamina, Client.CurrentStam));
                    }
                    else
                    {
                        Client.CurrentStam += 1;
                        Send(EudemonPacket.Status(this, Struct.StatusTypes.Stamina, Client.CurrentStam));
                    }
                }
            }
        }


    }
}
