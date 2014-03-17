using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyCore;

namespace SunDofus.DataRecords
{
    [OTable("banks")]
    class DB_Bank : ORecord
    {
        [OProperty("AccountID", OProperty.TinyPropertyType.INT, true)]
        public int AccountID { get; set; }

        [OProperty("kamas", OProperty.TinyPropertyType.INT)]
        public long Kamas { get; set; }

        public List<DB_BankItem> Items { get; set; }

        public DB_Bank()
        {
            Items = new List<DB_BankItem>();
        }

        public string GetItems()
        {
            return string.Join("|", Items); ;
        }

        public string GetExchangeItems()
        {
            return string.Join(";", from i in Items select string.Concat("O", i.ToString()));
        }
    }
}
