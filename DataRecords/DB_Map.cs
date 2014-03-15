using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyCore;

namespace SunDofus.DataRecords
{
    [OTable("maps")]
    public class DB_Map : ORecord
    {
        [OProperty("ID", OProperty.TinyPropertyType.INT, true)]
        public int ID { get; set; }
        [OProperty("date", OProperty.TinyPropertyType.STRING)]
        public string Date { get; set; }
        [OProperty("width", OProperty.TinyPropertyType.INT)]
        public int Width { get; set; }
        [OProperty("height", OProperty.TinyPropertyType.INT)]
        public int Height { get; set; }
        [OProperty("capabilities", OProperty.TinyPropertyType.INT)]
        public int Capabilities { get; set; }

        public int PosX { get; set; }
        public int PosY { get; set; }
        public int SubArea { get; set; }

        [OProperty("mapData", OProperty.TinyPropertyType.STRING)]
        public string MapData { get; set; }
        [OProperty("key", OProperty.TinyPropertyType.STRING)]
        public string Key { get; set; }
        [OProperty("mappos", OProperty.TinyPropertyType.STRING)]
        public string Mappos { get; set; }

        [OProperty("numgroup", OProperty.TinyPropertyType.INT)]
        public int MaxMonstersGroup { get; set; }
        [OProperty("groupsize", OProperty.TinyPropertyType.INT)]
        public int MaxGroupSize { get; set; }

        //public Dictionary<int, List<int>> Monsters { get; set; }

        public DB_Map()
        {
            //Monsters = new Dictionary<int, List<int>>();
        }

        public void ParsePos()
        {
            try
            {
                var datas = Mappos.Split(',');

                if (datas.Length < 3)
                    return;

                PosX = int.Parse(datas[0]);
                PosY = int.Parse(datas[1]);
                SubArea = int.Parse(datas[2]);
            }
            catch { }
        }
    }
}
