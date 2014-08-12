using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EO_Proxy.Connections.Packets
{
    public class Packets
    {
        public enum PacketsEnum
        {
            ServerResponse = 1055,
            GeneralDataAkaActions = 1010,
            Message = 1004,
            Walk = 3005,
            CreateCharacter = 1001,
            Chat = 1004,
            CharacterInfo = 1006,
            String = 1015,
            LoginRequest = 1060,
            AuthorizationSeed = 1059,
            SpawnEudemonEntity = 1116,
            ItemInfo = 1008,
            ItemUsage = 1009,
            NPCTalk = 2032,
            SpawnEntityChar = 1014,
            SpawnNPC = 2030,
            Tick = 1012,
        }
    }
}
