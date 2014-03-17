using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunDofus.Game
{
    class JobOptions
    {
        public JobOptionParams Params { get; set; }
        public int MinSlots { get; set; }
        public int MaxSlots { get; set; }

        public JobOptions(JobOptionParams p, int min, int max)
        {
            Params = p;
            MinSlots = min;
            MaxSlots = max;
        }

        public override string ToString()
        {
            return string.Format("{0}|{1}|{2}", (int)Params, MinSlots, MaxSlots);
        }
    }
}
