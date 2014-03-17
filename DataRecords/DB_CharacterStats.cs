using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyCore;

namespace SunDofus.DataRecords
{
    [OTable("characters_stats")]
    class DB_CharacterStats : ORecord
    {
        [OProperty("CharacterID", OProperty.TinyPropertyType.INT)]
        public int CharacterID { get; set; }

        [OProperty("life", OProperty.TinyPropertyType.INT)]
        public int Life { get; set; }

        [OProperty("energy", OProperty.TinyPropertyType.INT)]
        public int Energy { get; set; }

        [OProperty("vitality", OProperty.TinyPropertyType.INT)]
        public int Vitality { get; set; }

        [OProperty("wisdom", OProperty.TinyPropertyType.INT)]
        public int Wisdom { get; set; }

        [OProperty("strength", OProperty.TinyPropertyType.INT)]
        public int Strength { get; set; }

        [OProperty("intelligence", OProperty.TinyPropertyType.INT)]
        public int Intelligence { get; set; }

        [OProperty("luck", OProperty.TinyPropertyType.INT)]
        public int Luck { get; set; }

        [OProperty("agility", OProperty.TinyPropertyType.INT)]
        public int Agility { get; set; }
    }
}
