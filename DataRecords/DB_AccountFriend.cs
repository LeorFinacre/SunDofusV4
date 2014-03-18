using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyCore;

namespace SunDofus.DataRecords
{
    [OTable("accounts_friends")]
    class DB_AccountFriend : ORecord
    {
        [OProperty("ID", OProperty.TinyPropertyType.LONG, true)]
        public long ID { get; set; }

        [OProperty("accountID", OProperty.TinyPropertyType.INT)]
        public int AccountID { get; set; }

        [OProperty("targetID", OProperty.TinyPropertyType.INT)]
        public int TargetID { get; set; }

        [OProperty("targetPseudo", OProperty.TinyPropertyType.STRING)]
        public string TargetPseudo { get; set; }
    }
}
