using EO_Proxy.Connections.Packets;
using System.Drawing;

namespace EO_Proxy.Connections
{
    public partial class Handlers
    {
        public static void Chat(World.Client client, Message message, out bool send)
        {
            if (message.Text.StartsWith("|"))
            {
                send = false;
                string[] data = message.Text.Split(' ');
                switch (data[0].Replace("|", ""))
                {
                    case "sniffpackets":
                        {
                            Functions.PacketSniffing.Sniffing = !Functions.PacketSniffing.Sniffing;
                            client.SendToClient(new Message(0, "Packet Sniffing: " + Functions.PacketSniffing.Sniffing, "ALLUSERS", "SYSTEM", Color.Aqua, Message.Center).Buffer);
                            System.Console.WriteLine("Packet sniffing has been set to: " + Functions.PacketSniffing.Sniffing);
                            return;
                        }
                    case "consolelog":
                        {
                            Program.ConsoleLogging = !Program.ConsoleLogging;
                            client.SendToClient(new Message(0, "Console Logging: " + Program.ConsoleLogging, "ALLUSERS", "SYSTEM", Color.Aqua, Message.Center).Buffer);
                            System.Console.WriteLine("Console logging has been set to: " + Program.ConsoleLogging);
                            return;

                        }
                    case "snifftobytes":
                        {
                            if (Functions.PacketSniffing.Sniffing)
                            {
                                Program.WriteToBytes = !Program.WriteToBytes;
                                client.SendToClient(new Message(0, "Byte Packet Output: " + Program.ConsoleLogging, "ALLUSERS", "SYSTEM", Color.Aqua, Message.Center).Buffer);
                                System.Console.WriteLine("Byte packet output has been set to: " + Program.ConsoleLogging);
                                return;
                            }
                            else
                            {
                                client.SendToClient(new Message(0, "Please enable packet sniffing before toggling byte output", "ALLUSERS", "SYSTEM", Color.Aqua, Message.Center).Buffer);
                                return;
                            }
                        }
                }
            }
            send = true;
        }
    }
}
