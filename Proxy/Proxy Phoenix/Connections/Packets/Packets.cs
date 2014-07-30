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
            ActionData = 1010,
            Message = 1004
        }
    }
}
