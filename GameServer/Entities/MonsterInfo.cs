using System;


namespace GameServer.Entities
{
    /// <summary>
    /// Stores base, non-unique information for monsters ( attack, defense, etc )
    /// </summary>
    public class MonsterInfo
    {
        public int ID;
        public string Name = "";
        public int Mesh;
        public int Level;
        public int MaxHP;
        public int MinAttack;
        public int MaxAttack;
        public int Defense;
        public int MDefense;
        public int AggroRange;
        public int AttackRange;
        public int Dodge;
        public int Speed;
        public int MagicID;
        public int AttackSpeed = 5000;
        public bool IsGuard
        {
            get
            {
                if (Mesh == 900)
                {
                    return true;
                }
                else
                    return false;
            }
        }
    }
}
