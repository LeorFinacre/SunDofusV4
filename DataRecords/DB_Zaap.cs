using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunDofus.DataRecords
{
    class DB_Zaap
    {
        public int MapID { get; set; }
        public int CellID { get; set; }

        public Map Map { get; set; }
    }
}
