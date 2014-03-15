using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunDofus.Utilities;
using TinyCore;

namespace SunDofus.DataRecords
{
    [OTable("characters")]
    class DB_Character : ORecord
    {
        [OProperty("ID", OProperty.TinyPropertyType.INT, true)]
        public int ID { get; set; }
        [OProperty("name", OProperty.TinyPropertyType.STRING)]
        public string Name { get; set; }
        [OProperty("accountID", OProperty.TinyPropertyType.INT)]
        public int AccountID { get; set; }

        [OProperty("color", OProperty.TinyPropertyType.INT)]
        public int Color { get; set; }
        [OProperty("color2", OProperty.TinyPropertyType.INT)]
        public int Color2 { get; set; }
        [OProperty("color3", OProperty.TinyPropertyType.INT)]
        public int Color3 { get; set; }

        [OProperty("class", OProperty.TinyPropertyType.INT)]
        public int Class { get; set; }
        [OProperty("sex", OProperty.TinyPropertyType.INT)]
        public int Sex { get; set; }
        [OProperty("skin", OProperty.TinyPropertyType.INT)]
        public int Skin { get; set; }
        [OProperty("size", OProperty.TinyPropertyType.INT)]
        public int Size { get; set; }
        [OProperty("lvl", OProperty.TinyPropertyType.INT)]
        public int Level { get; set; }

        [OProperty("mapid", OProperty.TinyPropertyType.INT)]
        public int MapID { get; set; }
        [OProperty("mapcell", OProperty.TinyPropertyType.INT)]
        public int MapCell { get; set; }
        [OProperty("mapdir", OProperty.TinyPropertyType.INT)]
        public int MapDir { get; set; }

        [OProperty("statsPoint", OProperty.TinyPropertyType.INT)]
        public int CharactPoint { get; set; }
        [OProperty("spellPoint", OProperty.TinyPropertyType.INT)]
        public int SpellPoint { get; set; }

        [OProperty("energy", OProperty.TinyPropertyType.INT)]
        public int Energy { get; set; }

        [OProperty("life", OProperty.TinyPropertyType.INT)]
        public int Life { get; set; }
        public int Pods { get; set; }

        [OProperty("savemap", OProperty.TinyPropertyType.INT)]
        public int SaveMap { get; set; }
        [OProperty("savecell", OProperty.TinyPropertyType.INT)]
        public int SaveCell { get; set; }

        [OProperty("exp", OProperty.TinyPropertyType.LONG)]
        public long Exp { get; set; }
        [OProperty("kamas", OProperty.TinyPropertyType.LONG)]
        public long Kamas { get; set; }

        public DB_Character() { }

        #region Pattern

        public string PatternOnList()
        {
            StringBuilder builder = new StringBuilder();
            {
                builder.Append(ID).Append(";");
                builder.Append(Name).Append(";");
                builder.Append(Level).Append(";");
                builder.Append(Skin).Append(";");
                builder.Append(Basic.DeciToHex(Color)).Append(";");
                builder.Append(Basic.DeciToHex(Color2)).Append(";");
                builder.Append(Basic.DeciToHex(Color3)).Append(";");

                //builder.Append(GetItemsPos()).Append(";");
                builder.Append(",,,,"); //Items

                builder.Append("0;").Append(Servers.GAMESERVER_ID).Append(";;;");
            }

            return builder.ToString();
        }

        public string PatternOnSelect()
        {
            StringBuilder builder = new StringBuilder();
            {
                builder.Append("|").Append(ID).Append("|");
                builder.Append(Name).Append("|");
                builder.Append(Level).Append("|");
                builder.Append(Class).Append("|");
                builder.Append(Skin).Append("|");
                builder.Append(Utilities.Basic.DeciToHex(Color)).Append("|");
                builder.Append(Utilities.Basic.DeciToHex(Color2)).Append("|");
                builder.Append(Utilities.Basic.DeciToHex(Color3)).Append("||");
                //builder.Append(GetItems()).Append("|");         Items ToString separate with ;
                builder.Append("|");
            }

            return builder.ToString();
        }

        #endregion
    }
}
