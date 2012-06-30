using System;


namespace GameServer.Structs
{
    /// <summary>
    /// Description of ActionType.
    /// </summary>
    public partial class Struct
    {
        public enum ActionType : int
        {
            Dance = 1,
            Happy = 150,
            Angry = 160,
            Sad = 170,
            Wave = 190,
            Bow = 200,
            Kneel = 210,
            Cool = 230,
            Sit = 250,
            Lie = 270,
            Pet = 100
            //Still missing (Skill Learnt) Dances 1-7
        }
    }
}
