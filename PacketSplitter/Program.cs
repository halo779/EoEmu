using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;

namespace PacketSplitter
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Eudemons C# Packet Splitter © Hio77";
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("---------------------------------");
            Console.WriteLine("---Eudemons C# Packet Splitter---");
            Console.WriteLine("---------------------------------");
            Console.ResetColor();
            Console.WriteLine("");
            Console.WriteLine("Packet Splitter Created by Hio77");
            Console.WriteLine("This will Split Packets to each file as to how eo packets are designed");
            Console.WriteLine("");
            byte[] data;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("This tool will output up to 100 packets split from the orignal!");
            Console.WriteLine("Please enter the file name of your packet dump(eg test.pk)");
            Console.ResetColor();
            readline:
            Console.WriteLine("");
            string Filename = Console.ReadLine();
            //byte[] data = File.ReadAllBytes("test");
            Console.WriteLine("");
            if (File.Exists(Filename))
            {
                data = File.ReadAllBytes(Filename);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[Error] File does not exist");
                Console.WriteLine("Please Try again");
                Console.ResetColor();
                goto readline;
            }
            Console.WriteLine("Begining on analysing packets");
            byte[][] Split = new byte[200][];
            int LoopCount = 0;
            byte Counter = 0;
            int Total = 0;
            int Type = (BitConverter.ToInt16(data, 2));
            int Length = (BitConverter.ToInt16(data, 0));
            if (data.Length > 0)
                Counter++;
            if (data.Length > Length)
            {
            top:
                if (data.Length > (Total + Length))
                {
                    int LenTest = (BitConverter.ToInt16(data, 0 + Length + Total));
                    Split[LoopCount] = new byte[LenTest];
                    Array.Copy(data, Length + Total, Split[LoopCount], 0, LenTest);
                    Counter++;
                    Total = Total + LenTest;
                    LoopCount++;
                    goto top;
                }
            }
            Console.WriteLine("Packet Split into " + (LoopCount - 1).ToString() + " Packets");
            byte[] Split0 = new byte[Length];
            Array.Copy(data, Split0, Length);
            if (data.Length > (Total + Length))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("There is " + (data.Length - (Total + Length)) + " more data than splits avalible");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("All Packets have been Split!!!");
                Console.ResetColor();
            }
            Console.WriteLine("Now outputting Packets");
            Console.WriteLine("");
            for (int i = 0; i < LoopCount; i++)
            {
                Console.WriteLine(Dump(Split[i],i,Filename));
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Packet Splitting Finished Press enter to exit");
            Console.ReadLine();
        }
        public static void SaveDump(byte[] Bytes, int Count, string FileName)
        {
            Console.Write("Writing To File - Packet Length: ");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(Bytes.Length.ToString());
            Console.ResetColor();
            Console.Write(" Packet Count: ");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(Count.ToString());
            Console.ResetColor();
            if (Directory.Exists("output"))
            {
                
            }
            else
            {
                Directory.CreateDirectory("output");
            }
            FileName = FileName + "//";
            if (Directory.Exists(FileName))
            {

            }
            else
            {
                Directory.CreateDirectory("output//" + FileName);
            }
            File.WriteAllBytes("output//" + FileName + Count.ToString(), Bytes);
        }
        public static object Dump(byte[] Bytes, int Count, string FileName)
        {
            int Type = (BitConverter.ToInt16(Bytes, 2));
            //Console.WriteLine("Packet Length: " + Bytes.Length.ToString() + " Packet Count: " + Count.ToString() + " Packet Type: " + Type.ToString());
            Console.Write("Packet Length: ");
            Console.ForegroundColor = ConsoleColor.DarkGray; 
            Console.Write(Bytes.Length.ToString()); 
            Console.ResetColor(); 
            Console.Write(" Packet Count: ");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(Count.ToString());
            Console.ResetColor();
            Console.Write(" Packet Type: ");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(Type.ToString());
            Console.ResetColor();
            SaveDump(Bytes, Count, FileName);
            string Hex = "";
            foreach (byte b in Bytes)
            {
                Hex = Hex + b.ToString("X2") + " ";
            }
            string Out = "";
            while (Hex.Length != 0)
            {
                int SubLength = 0;
                if (Hex.Length >= 48)
                {
                    SubLength = 48;
                }
                else
                {
                    SubLength = Hex.Length;
                }
                string SubString = Hex.Substring(0, SubLength);
                int Remove = SubString.Length;
                SubString = SubString.PadRight(60, ' ') + StrHexToAnsi(SubString);
                Hex = Hex.Remove(0, Remove);
                Out = Out + SubString + "\r\n";
            }
            return Out;
        }
        private static string StrHexToAnsi(string StrHex)
        {
            string[] Data = StrHex.Split(new char[] { ' ' });
            string Ansi = "";
            foreach (string tmpHex in Data)
            {
                if (tmpHex != "")
                {
                    byte ByteData = byte.Parse(tmpHex, NumberStyles.HexNumber);
                    if ((ByteData >= 32) & (ByteData <= 126))
                    {
                        Ansi = Ansi + ((char)(ByteData)).ToString();
                    }
                    else
                    {
                        Ansi = Ansi + ".";
                    }
                }
            }
            return Ansi;
        }
    }
}
