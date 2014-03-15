using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyCore;

namespace SunDofus.DataRecords
{
    [OTable("bannedIp")]
    class DB_BannedIP : ORecord
    {
        [OProperty("ID", OProperty.TinyPropertyType.INT, true)]
        public int ID { get; protected set; }

        [OProperty("ip", OProperty.TinyPropertyType.STRING)]
        public string IP { get; protected set; }

        public DB_BannedIP() { }
        public DB_BannedIP(int id, string ip)
        {
            ID = id;
            IP = ip;
        }
    }
}
