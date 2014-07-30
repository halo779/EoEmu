using System;
using System.Collections.Generic;
using System.IO;
    using System.Text;
namespace EO_Proxy.Functions
{
    public class PacketSniffing
    {
        private static uint _count = 0;
        private static DateTime _time = DateTime.Now;

        public static StreamWriter SW;
        public static StreamWriter SWPackets = new StreamWriter(Program.PacketSniffingPath + "PacketsTypes " + _time.ToString("yyyy MM dd [hhhh.mm.ss]") + ".txt", false);
        public static List<KeyValuePair<int, string>> PacketTypes = new List<KeyValuePair<int, string>>(); 
        private static bool _sniffing = true;
        public static bool Sniffing
        {
            get { return _sniffing; }
            set
            {
                _time = DateTime.Now;
                if (value)
                    SW = new StreamWriter(Program.PacketSniffingPath + _time.ToString("yyyy MM dd [hhhh.mm.ss]") + ".txt", true);
                else
                    SW.Dispose();
                _sniffing = value;
            }
        }
        private static Action<byte[], bool> SniffThis = new Action<byte[], bool>(_sniff);
        public static void Sniff(byte[] data, bool client)
        {
            _sniff(data, client);
        }
        private static void _sniff(byte[] data, bool client)
        {
            string dataString = "";
            string from = "";

            int PacketID = BitConverter.ToUInt16(data, 2);
            
            if (client)
                from = "Server";
            else
                from = "Client";
            dataString += "Packet " + _count + " -- To " + from + " -- Length: " + BitConverter.ToUInt16(data, 0) + " | Pack: "
                + data.Length + " -- Type: " + PacketID;

            if (Enum.IsDefined(typeof(Connections.Packets.Packets.PacketsEnum), PacketID))
                dataString += " (" + AddSpacesToSentence(Enum.GetName(typeof(Connections.Packets.Packets.PacketsEnum), PacketID)) + ")";

            dataString += Environment.NewLine;

            if (!PacketTypes.Contains(new KeyValuePair<int, string>(PacketID, from)))
            {
                SWPackets.WriteLine(PacketID + " - From: " + from);
                SWPackets.Flush();
                PacketTypes.Add(new KeyValuePair<int, string>(PacketID, from));
            }

            for (int i = 0; i < Math.Ceiling((double)data.Length / 16); i++)
            {
                int t = 16;
                if ((i + 1) * 16 > data.Length)
                    t = data.Length - (i * 16);
                for (int a = 0; a < t; a++)
                    dataString += data[i * 16 + a].ToString("X2") + " ";
                if (t < 16)
                    for (int a = t; a < 16; a++)
                        dataString += "   ";
                dataString += ";   ";
                for (int a = 0; a < t; a++)
                    dataString += Convert.ToChar(data[i * 16 + a]);
                dataString += Environment.NewLine;
            }
            dataString.Replace(Convert.ToChar(0), '.');
            dataString += Environment.NewLine;
            Write(dataString);
            if (Program.WriteToBytes)
                WriteToByteForm(data, client);
        }
        private static void Write(string data)
        {
        Again:
            bool written = false;
            if (SW == null)
                SW = new StreamWriter(Program.PacketSniffingPath + _time.ToString("yyyy MM dd [hhhh.mm.ss]") + ".txt", true);
            lock (SW)
            {
                try
                {
                    SW.WriteLine(data);
                    written = true;
                    SW.Flush();
                    _count++;
                }
                catch { goto Again; }
            }
            if (!written)
                goto Again;
        }

        private static void WriteToByteForm(byte[] data, bool client)
        {
            string Direction = "Client";
            if (client)
                Direction = "Server";
            if (!Directory.Exists(Program.PacketSniffingPath + @"SinglePackets\" + _time.ToString("yyyy MM dd [hhhh.mm.ss]") + @"\"))
                Directory.CreateDirectory(Program.PacketSniffingPath + @"SinglePackets\" + _time.ToString("yyyy MM dd [hhhh.mm.ss]") + @"\");
            try
            {
                File.WriteAllBytes(Program.PacketSniffingPath + @"SinglePackets\" + _time.ToString("yyyy MM dd [hhhh.mm.ss]") + @"\" + _count + "[" + BitConverter.ToUInt16(data, 2) + "] - To " + Direction + ".packet", data);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static string AddSpacesToSentence(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return "";
            StringBuilder newText = new StringBuilder(text.Length * 2);
            newText.Append(text[0]);
            for (int i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]) && text[i - 1] != ' ')
                    newText.Append(' ');
                newText.Append(text[i]);
            }
            return newText.ToString();
        }
    }
}
