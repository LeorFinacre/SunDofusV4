using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyCore;

namespace SunDofus.DataRecords
{
    [OTable("characters_items")]
    class DB_CharacterItem : ORecord
    {
        [OProperty("ID", OProperty.TinyPropertyType.INT, true)]
        public int ID { get; set; }

        [OProperty("characterID", OProperty.TinyPropertyType.INT)]
        public int CharacterID { get; set; }

        [OProperty("quantity", OProperty.TinyPropertyType.INT)]
        public int Quantity { get; set; }

        [OProperty("position", OProperty.TinyPropertyType.INT)]
        public int Position { get; set; }

        [OProperty("effects", OProperty.TinyPropertyType.STRING)]
        public string Effects { get; set; }
    }
}
