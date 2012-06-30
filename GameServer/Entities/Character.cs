using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using GameServer.Connections;
using GameServer.Structs;
using GameServer.Database;

namespace GameServer.Entities
{
    /// <summary>
    /// A conquer online character. This is a PC(Player Controlled) character.
    /// </summary>
    public class Character
    {
        public bool Dead = false;
        public bool Transformed = false;
        public bool isGM = false;
        public bool isPM = false;
        public bool Invincible = false;
        public bool First = false;
        public bool Flashing = false;
        public bool Flying = false;
        public string Name = "";
        public string Spouse = "";
        public int ID = 0;
        public int EPs = 0;
        public int X = 438;
        public int Y = 377;
        public int Direction = 0;
        public int MentorExp = 0;
        public int MercenaryExp = 0;
        public int Action = 0;
        public int Model = 1;
        public int GhostModel = 1;
        public int CurrentHP = 0;
        public int MaxHP = 0;
        public int BaseHP = 0;
        public int BaseMP = 0;
        public int Dexterity = 0;
        public int Vitality = 0;
        public int CurrentMP = 0;
        public int CurrentStam = 0;
        public int MaxMP = 0;
        public int Money = 0;
        public int BP = 0;
        public int Constitution = 0;
        public int Speed = 100;
        public int MentorLevel = 1;
        public int Wood = 0;
        public byte Business = 0;
        public int PPs = 0;
        public int WHMoney = 0;
        public byte Nobility = 0;
        public byte Metempsychosis = 0;
        public byte AutoAllocate = 0;
        public byte MercenaryRank = 0;
        public byte NobilityRank = 0;
        public byte MaxSummons = 0;
        public byte Exploit = 0;
        public int TokenPoints = 0;
        public int Hair = 0;
        public byte MuteFlag = 0;
        public int Level = 0;
        public int LastNPC = 0;
        public ulong Exp = 0;
        public int PkPoints = 0;
        public int PrevX = 0;
        public int PrevY = 0;
        public int BaseMinAttack = 0;
        public int BaseMaxAttack = 0;
        public int BaseMagicAttack = 0;
        public int MinAttack = 0;
        public int MaxAttack = 0;
        public int MagicAttack = 0;
        public int BonusMagicAttack = 0;
        public int BonusMagicDefense = 0;
        public int Defense = 0;
        public int MagicDefense = 0;
        public int Power = 0;
        public int Soul = 0;
        public int AdditionalPoint = 0;
        public byte EudBagSize = 0;
        public byte Vip = 0;
        //Gem counts.
        //*CG = (*) Citrine
        //*AG = (*) Amethyst
        //*SG = (*) Sapphire
        //*BG = (*) Beryl
        //*AM = (*) Amber

        //#Citrine
        public int NCG = 0;
        public int RCG = 0;
        public int SCG = 0;
        //Amethyst
        public int NAG = 0;
        public int RAG = 0;
        public int SAG = 0;
        //Sapphire
        public int NSG = 0;
        public int RSG = 0;
        public int SSG = 0;
        //Beryl
        public int NBG = 0;
        public int RBG = 0;
        public int SBG = 0;
        //Amber
        public int NAM = 0;
        public int RAM = 0;
        public int SAM = 0;

        public int Dodge = 0;
        public int Bless = 0;
        public Struct.Maps Map = Struct.Maps.Cronus;
        public Struct.Team Team = null;
        public Struct.PkType PKMode = Struct.PkType.Peace;
        public Struct.ClassType Class = Struct.ClassType.Warrior;
        public Dictionary<int, Struct.ItemInfo> Inventory = new Dictionary<int, Struct.ItemInfo>();
        /// The equipment system is simple. Each key corresponds to a location in which an item can be equipped.
        public Dictionary<int, Struct.ItemInfo> Equipment = new Dictionary<int, Struct.ItemInfo>();
        public Dictionary<int, Struct.CharSkill> Skills = new Dictionary<int, Struct.CharSkill>();
        public Dictionary<int, Struct.ItemInfo> TCWhs = new Dictionary<int, Struct.ItemInfo>(20);
        public Timer Attack;
        public int LastAttack;
        public Timer UpStam;
        public Timer Save;
        public Timer FlashTimer;
    }
}
