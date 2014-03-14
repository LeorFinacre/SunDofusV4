using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunDofus.Utilities
{
    class Formulas
    {
        public static double ExpPvp(int level, int levelTeam, int levelRivalTeam)
        {
            var baseexp = Math.Sqrt(level) * 10;
            var coef = ((double)levelRivalTeam / (double)levelTeam);

            return Math.Floor(((baseexp * coef) * (level * level)) * Config.GetInt32("RatePvP"));
        }

        public static double ExpHonor(int level, int levelTeam, int levelRivalTeam)
        {
            var baseexp = Math.Sqrt(level) * 10;
            var coef = ((double)levelRivalTeam / (double)levelTeam);

            return Math.Floor((baseexp * coef) * Config.GetInt32("RateHonor"));
        }
    }
}
