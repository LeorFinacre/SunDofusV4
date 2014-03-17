using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunDofus.DataRecords
{
    class DB_JobSkill
    {
        public int Id;
        public int[] Params;

        public JobSkill(int id, params int[] args)
        {
            Id = id;
            Params = args;
        }

        public override string ToString()
        {
            return string.Format("{0}~{1}~{2}~{3}~{4}", Id, Params[0], Params[1], Params[2], Params[3]);
        }
    }
}
