using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.Game.Conditions
{
    class NPCsConditions
    {
        public int CondiID { get; set; }
        public string Args { get; set; }

        public bool HasCondition(Game.Characters.Character character)
        {
            return true;
        }
    }
}
