using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyCore;

namespace SunDofus.DataRecords
{
    [OTable("accounts")]
    class DB_Account : ORecord
    {
        [OProperty("ID", OProperty.TinyPropertyType.INT, true)]
        public int ID { get; protected set; }

        [OProperty("username", OProperty.TinyPropertyType.STRING)]
        public string Username { get; protected set; }

        [OProperty("password", OProperty.TinyPropertyType.STRING)]
        public string Password { get; protected set; }

        [OProperty("pseudo", OProperty.TinyPropertyType.STRING)]
        public string Pseudo { get; protected set; }

        [OProperty("com", OProperty.TinyPropertyType.INT)]
        public int Communauty { get; protected set; }

        [OProperty("lvl", OProperty.TinyPropertyType.INT)]
        public int GMLevel { get; set; }

        [OProperty("banned", OProperty.TinyPropertyType.INT)]
        public int IsBanned { get; set; }

        [OProperty("question", OProperty.TinyPropertyType.STRING)]
        public string Question { get; protected set; }

        [OProperty("answer", OProperty.TinyPropertyType.STRING)]
        public string Answer { get; protected set; }

        [OProperty("characterCnt", OProperty.TinyPropertyType.INT)]
        public int CharacterCnt { get; set; }

        [OProperty("connected", OProperty.TinyPropertyType.INT)]
        public int IsConnected { get; set; }

        [OProperty("subTime", OProperty.TinyPropertyType.DATETIME)]
        public DateTime SubTime { get; protected set; }

        public long SubscriptionTime
        {
            get
            {
                var time = SubTime.Subtract(DateTime.Now).TotalMilliseconds;

                if (SubTime.Subtract(DateTime.Now).TotalMilliseconds <= 1)
                    return 0;

                return (long)time;
            }
        }

        internal void ParseCharacters()
        {
            throw new NotImplementedException();
        }
    }
}
