﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.Game.Maps.Monsters
{
    class Monster
    {
        public SunDofus.World.Entities.Models.Monsters.MonsterModel Model { get; set; }

        public int Level { get; set; }

        public int Life { get; set; }
        public int Skin { get; set; }

        public Monster(Entities.Models.Monsters.MonsterModel model, int grade)
        {
            Model = model;
            Level = grade;
        }

        public Characters.Character GetCharacter()
        {
            //TOMAKE;
            return null;
        }
    }
}
