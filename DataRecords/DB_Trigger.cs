using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyCore;

namespace SunDofus.DataRecords
{
    [OTable("triggers")]
    class DB_Trigger : ORecord
    {
        [OProperty("ID", OProperty.TinyPropertyType.INT, true)]
        public int ID { get; protected set; }

        [OProperty("mapID", OProperty.TinyPropertyType.INT)]
        public int MapID { get; protected set; }

        [OProperty("cellID", OProperty.TinyPropertyType.INT)]
        public int CellID { get; protected set; }

        [OProperty("actionID", OProperty.TinyPropertyType.INT)]
        public int ActionID { get; protected set; }

        [OProperty("args", OProperty.TinyPropertyType.STRING)]
        public string Args { get; protected set; }

        [OProperty("conditions", OProperty.TinyPropertyType.STRING)]
        public string Conditions { get; protected set; }
    }
}
