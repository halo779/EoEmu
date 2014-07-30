
namespace EO_Proxy
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    public class Kernel
    {
        public static ConcurrentDictionary<uint, Connections.Asynchronous.AsyncWrapper> InQueueConnections = new ConcurrentDictionary<uint, Connections.Asynchronous.AsyncWrapper>(15, 15);
        public static ConcurrentDictionary<uint, World.Client> WorldClients = new ConcurrentDictionary<uint, World.Client>();
    }
}
