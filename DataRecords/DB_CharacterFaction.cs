using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyCore;

namespace SunDofus.DataRecords
{
    [OTable("characters_factions")]
    class DB_CharacterFaction : ORecord
    {
        [OProperty("CharacterID", OProperty.TinyPropertyType.INT, true)]
        public int CharacterID { get; set; }

        [OProperty("factionID", OProperty.TinyPropertyType.INT)]
        public int FactionID { get; set; }

        [OProperty("level", OProperty.TinyPropertyType.INT)]
        public int Level { get; set; }

        [OProperty("honor", OProperty.TinyPropertyType.INT)]
        public int Honor { get; set; }

        [OProperty("deshonor", OProperty.TinyPropertyType.INT)]
        public int Deshonor { get; set; }

        public bool IsEnabled { get; set; }

        private DB_Character m_Character;

        public void SetCharacter(DB_Character character)
        {
            m_Character = character;
        }

        public void AddExp(int exp)
        {
            this.Honor += exp;

            if (this.Level != 10)
                this.Level = Servers.Levels.First(x => x.Alignment <= this.Honor && Servers.Levels.First(y => y.ID == x.ID + 1).Alignment > this.Honor).ID;
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

            this.Level = Servers.Levels.First(x => x.Alignment <= this.Honor && Servers.Levels.First(y => y.ID == x.ID + 1).Alignment > this.Honor).ID;
        }

        public string AlignementInfos
        {
            get
            {
                return string.Format("{0},{1},{2},{3}", FactionID, FactionID, (IsEnabled ? Level.ToString() : "0"), (m_Character.Level + m_Character.ID));
            }
        }

        public override string ToString()
        {
            return string.Format("{0}~2,{1},{2},{3},{4},{5}", FactionID, Level, Level, Honor, Deshonor, (IsEnabled ? "1" : "0"));
        }
    }
}
