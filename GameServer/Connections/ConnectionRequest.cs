using System;
using System.Timers;

namespace GameServer.Connections
{
    /// <summary>
    /// Description of ConnectionRequest.
    /// </summary>
    public class ConnectionRequest
    {
        public ulong Key;
        public string Account;
        protected Timer Timeout;

        public ConnectionRequest(uint ExpireTime, ulong _Key, string User)
        {
            Key = _Key;
            Account = User;
            if (!Nano.AuthenticatedLogins.ContainsKey(Key))
            {
                Nano.AuthenticatedLogins.Add(Key, this);
            }
            Console.WriteLine("[" + Key + "] " + Account + " authenticated from LoginServer.");
            Timeout = new Timer();
            Timeout.Interval = ExpireTime;
            Timeout.Elapsed += delegate { Expire(true); };
            Timeout.Start();
        }
        public void Expire(bool TimedOut)
        {
            Timeout.Stop();
            Timeout.Dispose();
            if (TimedOut)
                Console.WriteLine(Account + "'s connection timed out.");
            if (Nano.AuthenticatedLogins.ContainsKey(Key))
            {
                Nano.AuthenticatedLogins.Remove(Key);
            }
        }
    }
}
