using System;


namespace GameServer.Structs
{
    /// <summary>
    /// Description of PkType.
    /// </summary>
    public partial class Struct
    {
        public enum PkType : int
        {
            PK = 0,
            Peace = 1,
            Team = 2,
            Capture = 3,
            Legion = 4
        }
    }
}
