using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunDofus.World.Game.Maps.Monsters;
using SunDofus.World.Game.Characters.Stats;
using SunDofus.World.Game.Characters;

namespace SunDofus.World.Game.Maps.Fights
{
    class MonsterFighter : Fighter
    {
        private Monster player;
        private Character character;
        private Entities.Models.Monsters.MonsterLevelModel level;

        public MonsterFighter(Monster player, Fight fight)
            : base(FighterType.MONSTER, fight)
        {
            this.player = player;
            this.level = player.Model.Levels[player.Level];

            this.Skin = this.player.Model.GfxID;

            character = player.GetCharacter();

            AP = level.AP;
            MP = level.MP;
        }

        public override GenericStats Stats
        {
            get { return character.Stats; }
        }

        public override Character Character
        {
            get { return character; }
        }

        public override int ID
        {
            //TOMAKE
            get { return 1;  }
        }

        public override string Name
        {
            get { return player.Model.Name; }
        }

        public override int Level
        {
            get { return player.Level; }
        }

        public override int Life
        {
            get { return player.Life; }
            set { player.Life = value; }
        }

        public override int Skin
        {
            get { return player.Skin; }
            set { player.Skin = value; }
        }

        public override string GetPattern()
        {
            //TOMAKE
            return "";
        }
    }
}
