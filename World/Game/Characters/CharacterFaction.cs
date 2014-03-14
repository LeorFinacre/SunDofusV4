using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Game.Characters
{
    class CharacterFaction
    {
        private Character character;

        public CharacterFaction(Character character)
        {
            this.character = character;
        }

        public bool IsEnabled { get; set; }

        public int ID { get; set; }
        public int Honor { get; set; }
        public int Deshonor { get; set; }
        public int Level { get; set; }

        public void AddExp(int exp)
        {
            this.Honor += exp;

            if(this.Level != 10)
                this.Level = Entities.Requests.LevelsRequests.LevelsList.First(x => x.Alignment <= this.Honor && Entities.Requests.LevelsRequests.ReturnLevel(x.ID + 1).Alignment > this.Honor).ID;
        }

        public void RemoveExp(int exp)
        {
            this.Honor -= exp;

            if (Honor <= 0)
            {
                this.Level = 1;
                this.Honor = 0;

                return;
            }
            
            this.Level = Entities.Requests.LevelsRequests.LevelsList.First(x => x.Alignment <= this.Honor && Entities.Requests.LevelsRequests.ReturnLevel(x.ID + 1).Alignment > this.Honor).ID;
        }

        public string AlignementInfos
        {
            get
            {
                return string.Format("{0},{1},{2},{3}", ID, ID, (IsEnabled ? Level.ToString() : "0"), (character.Level + character.ID));
            }
        }

        public override string ToString()
        {
            return string.Format("{0}~2,{1},{2},{3},{4},{5}", ID, Level, Level, Honor, Deshonor, (IsEnabled ? "1" : "0"));
        }
    }
}
