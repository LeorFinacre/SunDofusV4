using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using TinyCore;

namespace SunDofus.DataRecords
{
    [OTable("npcs")]
    class DB_NPC : ORecord
    {
        [OProperty("ID", OProperty.TinyPropertyType.INT, true)]
        public int ID { get; private set; }
        
        public int UniqueID { get; set; }

        public int GfxID { get; set; }
        public int Size { get; set; }
        public int Sex { get; set; }

        public int Color { get; set; }
        public int Color2 { get; set; }
        public int Color3 { get; set; }

        //public int ArtWork;
        //public int Bonus;

        public NPCsQuestion Question { get; set; }

        public string Name { get; set; }
        public string Items { get; set; }

        public List<int> SellingList { get; set; }

        public DB_NPC()
        {
            SellingList = new List<int>();
            Question = null;
        }

        public int MapID { get; set; }
        public int MapCell { get; set; }
        public int Dir { get; set; }

        public bool MustMove { get; set; }

        private Timer timer;

        public void StartMove()
        {
            if (MustMove == false || !Utilities.Config.GetBool("MUSTNPCSMOVE"))
                return;

            timer = new Timer();
            timer.Enabled = true;
            timer.Interval = Utilities.Basic.Rand(5000, 15000);
            timer.Elapsed += new ElapsedEventHandler(this.Move);
        }

        public string PatternOnMap()
        {
            var builder = new StringBuilder();
            {
                builder.Append(MapCell).Append(";").Append(Dir).Append(";0;");
                builder.Append(UniqueID).Append(";");
                builder.Append(ID).Append(";-4;");
                builder.Append(GfxID).Append("^").Append(Size).Append(";");
                builder.Append(Sex).Append(";").Append(Utilities.Basic.DeciToHex(Color)).Append(";");
                builder.Append(Utilities.Basic.DeciToHex(Color2)).Append(";").Append(Utilities.Basic.DeciToHex(Color3)).Append(";");
                builder.Append(Items).Append(";;");
            }

            return builder.ToString();
        }

        private void Move(object e, EventArgs e2)
        {
            timer.Interval = Utilities.Basic.Rand(5000, 15000);

            var map = Servers.Maps.First(x => x.ID == MapID);

            var path = new Game.Maps.Pathfinding("", map, MapCell, Dir);
            var newDir = Utilities.Basic.Rand(0, 3) * 2 + 1;
            var newCell = Game.Maps.Pathfinding.NextCell(map, MapCell, newDir);

            if (newCell <= 0)
                return;

            path.UpdatePath(Game.Maps.Pathfinding.GetDirChar(Dir) + Game.Maps.Pathfinding.GetCellChars(MapCell) + Game.Maps.Pathfinding.GetDirChar(newDir) +
                Game.Maps.Pathfinding.GetCellChars(newCell));

            var startpath = path.GetStartPath;
            var cellpath = path.RemakePath();

            if (!map.RushablesCells.Contains(newCell))
                return;

            if (cellpath != "")
            {
                MapCell = path.Destination;
                Dir = path.Direction;

                var packet = string.Format("GA0;1;{0};{1}", ID, startpath + cellpath);

                map.Send(packet);
            }
        }
    }
}
