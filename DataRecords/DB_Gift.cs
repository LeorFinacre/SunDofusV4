using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyCore;

namespace SunDofus.DataRecords
{
    [OTable("gifts")]
    class DB_Gift : ORecord
    {
        [OProperty("ID", OProperty.TinyPropertyType.INT, true)]
        public int ID { get; set; }

        [OProperty("target", OProperty.TinyPropertyType.INT)]
        public int Target { get; set; }

        [OProperty("itemID", OProperty.TinyPropertyType.INT)]
        public int ItemID { get; set; }

        [OProperty("title", OProperty.TinyPropertyType.STRING)]
        public string Title { get; set; }

        [OProperty("message", OProperty.TinyPropertyType.STRING)]
        public string Message { get; set; }

        [OProperty("image", OProperty.TinyPropertyType.STRING)]
        public string Image { get; set; }

        public override string ToString()
        {
            return string.Format("{0}~{1}~{2}~{3}~{4}", ID, Title, Message, ItemID, Image);
        }
    }
}
