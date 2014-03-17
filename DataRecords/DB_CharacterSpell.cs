using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyCore;

namespace SunDofus.DataRecords
{
    [OTable("characters_spells")]
    class DB_CharacterSpell : ORecord
    {
        [OProperty("ID", OProperty.TinyPropertyType.INT, true)]
        public int ID { get; set; }

        [OProperty("characterID", OProperty.TinyPropertyType.INT)]
        public int CharacterID { get; set; }

        [OProperty("spellID", OProperty.TinyPropertyType.INT)]
        public int SpellID { get; set; }

        [OProperty("spellLevel", OProperty.TinyPropertyType.INT)]
        public int SpellLevel { get; set; }

        [OProperty("spellPosition", OProperty.TinyPropertyType.INT)]
        public int SpellPosition { get; set; }

        public void ChangeLevel(int lvl)
        {
            SpellLevel = lvl;
            //LevelModel = Model.Levels.First(x => x.Level == Level);
        }

        //public SpellModel Model;
        //public SpellLevelModel LevelModel;

        public override string ToString()
        {
            return string.Format("{0},{1},{2}", ID, SpellLevel, SpellPosition);
        }
    }
}
