﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunDofus.DataRecords
{
    class DB_CharacterJob
    {
        public long Experience { get; set; }
        public int ID { get; set; }
        public int Level { get; set; }
        public List<JobSkill> Skills { get; set; }

        public Job(int id, int level, long experience)
        {
            ID = id;
            Level = level;
            Experience = experience;
            Skills = new List<JobSkill>();
        }

        public void ParseSkills(string skills)
        {
            if (skills.Length == 0)
                return;

            foreach (var ski in skills.Split(',').Select(sk => sk.Split('~')))
                Skills.Add(new JobSkill(int.Parse(ski[0]), int.Parse(ski[1]), int.Parse(ski[2]), int.Parse(ski[3]), int.Parse(ski[4])));
        }

        public string SaveSkills()
        {
            var data = Skills.Aggregate("",
                                           (current, sk) =>
                                           current +
                                           string.Format(",{0}~{1}~{2}~{3}~{4}", sk.Id, sk.Params[0], sk.Params[1],
                                                         sk.Params[2], sk.Params[3]));
            return data.Length > 0 ? data.Substring(1) : "";
        }

        public string GetSkills()
        {
            return string.Join(",", Skills);
        }

        public long GetMinExperience()
        {
            return Servers.Levels.First(x => x.ID == Level).Job;
        }

        public long GetMaxExperience()
        {
            var l = Level + 1;
            if (Level == 100)
                l = Level;

            return Servers.Levels.First(x => x.ID == l).Job;
        }
    }
}
