using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SunDofus.World.Game.Characters;
using SunDofus.World.Game.Characters.Stats;

namespace SunDofus.Game.Maps.Fights
{
    class CharacterFighter : Fighter
    {
        private Character character;

        public CharacterFighter(Character player, Fight fight)
            : base(FighterType.CHARACTER, fight)
        {
            player.Fight = fight;
            player.Fighter = this;

            character = player;

            AP = Stats.GetStat(StatEnum.MaxPA).Total;
            MP = Stats.GetStat(StatEnum.MaxPM).Total;
        }

        public override int ID
        {
            get { return character.ID; }
        }

        public override string Name
        {
            get { return character.Name; }
        }

        public override int Level
        {
            get { return character.Level; }
        }

        public override int Life
        {
            get { return character.Life; }
            set { character.Life = value; }
        }

        public override int Skin
        {
            get { return character.Skin; }
            set { character.Skin = value; }
        }

        public override GenericStats Stats
        {
            get { return character.Stats; }
        }

        public override Character Character
        {
            get { return character; }
        }

        public override string GetPattern()
        {
            return character.PatternFightDisplayChar();
        }
    }
}
