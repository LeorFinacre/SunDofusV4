using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.Game.Characters.Stats
{
    class GenericStat
    {
        public int Base { get; set; }
        public int Equipped { get; set; }
        public int Given { get; set; }
        public int Bonus { get; set; }

        public StatEnum Type { get; set; }
        public EffectEnum[] AddEffects { get; set; }
        public EffectEnum[] SubEffects { get; set; }

        public StatTotalFormule Formule { get; set; }

        public GenericStat(StatEnum type, EffectEnum[] addEffects, EffectEnum[] subEffects, int baseValue = 0, StatTotalFormule formule = null)
        {
            Type = type;
            AddEffects = addEffects;
            SubEffects = subEffects;

            Formule = (formule == null ? GenericStats.FormuleTotal : formule);

            Base = baseValue;
            Equipped = 0;
            Given = 0;
            Bonus = 0;
        }

        public int Total
        {
            get { return Formule(Base, Equipped, Given, Bonus); }
        }

        public override string ToString()
        {
            return string.Concat(Base, ',', Equipped, ',', Given, ',', Bonus);
        }
    }
}
