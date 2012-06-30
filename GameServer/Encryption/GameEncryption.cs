using System;
using System.IO;
using System.Text;
using OpenSSL;

namespace GameServer.Encryption
{
    public class GameEncryption
    {
        OpenSSL.Blowfish _blowfish;
        string _key;

        public GameEncryption(string key)
        {
            _blowfish = new OpenSSL.Blowfish(OpenSSL.BlowfishAlgorithm.CFB64);
            _blowfish.SetKey(Encoding.ASCII.GetBytes(key));
            _key = key;
        }

        public byte[] Decrypt(byte[] packet)
        {
            byte[] buffer = _blowfish.Decrypt(packet);
            System.Buffer.BlockCopy(buffer, 0, packet, 0, buffer.Length);
            return buffer;
        }

        public byte[] Encrypt(byte[] packet)
        {
            byte[] buffer = _blowfish.Encrypt(packet);
            //System.Buffer.BlockCopy(buffer, buffer.Length, packet, 0, buffer.Length);
            return buffer;
        }

        public OpenSSL.Blowfish Blowfish
        {
            get { return _blowfish; }
        }
        public void SetKey(byte[] k)
        {
            _blowfish.SetKey(k);
        }
        public void SetIvs(byte[] i1, byte[] i2)
        {
            _blowfish.EncryptIV = i1;
            _blowfish.DecryptIV = i2;
        }
        public string Key
        {
            get { return _key; }
        }
    }
    public class ServerKeyExchange
    {
        OpenSSL.DH _keyExchange;
        byte[] _serverIv;
        byte[] _clientIv;

        public byte[] CreateServerKeyPacket()
        {
            _clientIv = new byte[8];
            _serverIv = new byte[8];
            string P = "E7A69EBDF105F2A6BBDEAD7E798F76A209AD73FB466431E2E7352ED262F8C558F10BEFEA977DE9E21DCEE9B04D245F300ECCBBA03E72630556D011023F9E857F";
            string G = "05";
            _keyExchange = new OpenSSL.DH(OpenSSL.BigNumber.FromHexString(P), OpenSSL.BigNumber.FromHexString(G));
            _keyExchange.GenerateKeys();
            Console.WriteLine("P, Private Key, Public Key: " + P + ", " + _keyExchange.PublicKey.ToHexString() + ", " + _keyExchange.PrivateKey.ToHexString());
            return GeneratePacket(_serverIv, _clientIv, P, G, _keyExchange.PublicKey.ToHexString());
        }

        public GameEncryption HandleClientKeyPacket(string PublicKey, GameEncryption cryptographer)
        {
            byte[] key = _keyExchange.ComputeKey(OpenSSL.BigNumber.FromHexString(PublicKey));
            //Console.WriteLine("Client's public key: " + PublicKey);
            /*string DatatoOutput = "";
            foreach (byte D in key)
            DatatoOutput += Convert.ToString(D, 16).PadLeft(2, '0') + " ";*/
            //DataHolder.ConsoleWriteQueue.Enqueue("\n{<White>}[Finalized Key] {Length: " + key.Length + "} {<Yellow>}" + DatatoOutput);
            // cryptographer.Blowfish.SetKey(_keyExchange.ComputeKey(OpenSSL.BigNumber.FromHexString(PublicKey)));
            cryptographer.SetKey(key);
            cryptographer.SetIvs(_clientIv, _serverIv);
            /* cryptographer.Blowfish.EncryptIV = _clientIv;
             cryptographer.Blowfish.DecryptIV = _serverIv;*/
            return cryptographer;
        }
        public byte[] GeneratePacket(byte[] ServerIV1, byte[] ServerIV2, string P, string G, string ServerPublicKey)
        {
            int PAD_LEN = 11;
            int _junk_len = 12;
            string tqs = "TQServer";
            MemoryStream ms = new MemoryStream();
            byte[] pad = new byte[PAD_LEN];
            Nano.Rand.NextBytes(pad);
            byte[] junk = new byte[_junk_len];
            Nano.Rand.NextBytes(junk);
            int size = 47 + P.Length + G.Length + ServerPublicKey.Length + 12 + 8 + 8;
            BinaryWriter bw = new BinaryWriter(ms);
            bw.Write(pad);
            bw.Write(size - PAD_LEN);
            bw.Write((UInt32)_junk_len);
            bw.Write(junk);
            bw.Write((UInt32)ServerIV2.Length);
            bw.Write(ServerIV2);
            bw.Write((UInt32)ServerIV1.Length);
            bw.Write(ServerIV1);
            bw.Write((UInt32)P.ToCharArray().Length);
            foreach (char fP in P.ToCharArray())
            {
                bw.BaseStream.WriteByte((byte)fP);
            }
            bw.Write((UInt32)G.ToCharArray().Length);
            foreach (char fG in G.ToCharArray())
            {
                bw.BaseStream.WriteByte((byte)fG);
            }
            bw.Write((UInt32)ServerPublicKey.ToCharArray().Length);
            foreach (char SPK in ServerPublicKey.ToCharArray())
            {
                bw.BaseStream.WriteByte((byte)SPK);
            }
            //Console.WriteLine("Size : " + size);
            foreach (char tq in tqs.ToCharArray())
            {
                bw.BaseStream.WriteByte((byte)tq);
            }
            byte[] Packet = new byte[ms.Length];
            Packet = ms.ToArray();
            ms.Close();
            return Packet;
        }
    }
    public class ClientKeyPacket
    {
        private static int PAD_LEN = 7;
        private uint _junk_len;
        string _publicKey;

        public ClientKeyPacket(byte[] buffer)
        {
            //_buffer = buffer;
            MemoryStream ms = new MemoryStream(buffer);
            BinaryReader br = new BinaryReader(ms);
            br.BaseStream.Seek(PAD_LEN, SeekOrigin.Begin); //ignore padding
            uint len = br.ReadUInt32(); //read packet length. ignore
            _junk_len = br.ReadUInt32();
            //br.BaseStream.Seek(_junk_len, SeekOrigin.Current); //ignore junk but grab size
            //Console.WriteLine("junk len " + _junk_len);
            byte[] junk = br.ReadBytes((int)_junk_len);
            _publicKey = Encoding.ASCII.GetString(br.ReadBytes(br.ReadInt32()));
        }

        public string PublicKey
        {
            get { return _publicKey; }
        }

    }
}
