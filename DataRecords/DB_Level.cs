using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyCore;

namespace SunDofus.DataRecords
{
    [OTable("levels")]
    class DB_Level : ORecord
    {
        [OProperty("ID", OProperty.TinyPropertyType.INT, true)]
        public int ID { get; protected set; }

        [OProperty("character", OProperty.TinyPropertyType.LONG)]
        public long Character { get; protected set; }
        [OProperty("job", OProperty.TinyPropertyType.LONG)]
        public long Job { get; protected set; }
        [OProperty("pvp", OProperty.TinyPropertyType.LONG)]
        public long Alignment { get; protected set; }
        [OProperty("guild", OProperty.TinyPropertyType.LONG)]
        public long Guild { get; protected set; }
        [OProperty("mount", OProperty.TinyPropertyType.LONG)]
        public long Mount { get; protected set; }

        public DB_Level()
        {
            Character = 0;
            Job = 0;
            Mount = 0;
            Alignment = 0;
            Guild = 0;
        }
    }
}
