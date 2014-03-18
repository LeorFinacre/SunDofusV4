using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyCore;

namespace SunDofus.DataRecords
{
    [OTable("zaapis")]
    class DB_Zaapis : ORecord
    {
        [OProperty("ID", OProperty.TinyPropertyType.INT, true)]
        public int ID { get; protected set; }

        [OProperty("mapID", OProperty.TinyPropertyType.INT)]
        public int MapID { get; set; }

        [OProperty("cellID", OProperty.TinyPropertyType.INT)]
        public int CellID { get; set; }

        [OProperty("zone", OProperty.TinyPropertyType.INT)]
        public int Faction { get; set; }

        public DB_Map Map { get; set; }
    }
}
