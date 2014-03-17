using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunDofus.DataRecords
{
    class DB_CharacterChannel
    {
        public char Head { get; set; }
        public bool On { get; set; }

        public Channel(char head, bool on)
        {
            Head = head;
            On = on;
        }
    }
}
