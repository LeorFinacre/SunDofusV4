using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SunDofus.World.Game.Characters.Stats;

namespace SunDofus.Game.Effects
{
    class EffectItem
    {
        public int ID { get; set; }
        public int Value { get; set; }
        public int Value2 { get; set; }
        public int Value3 { get; set; }
        public string Effect { get; set; }

        public Characters.Character Client { get; set; }

        public EffectItem()
        {
            Value = 0;
            Value2 = 0;
            Value3 = 0;

            Effect = "0d0+0";
        }

        public EffectItem(EffectItem x)
        {
            ID = x.ID;

            Value = x.Value;
            Value2 = x.Value2;
            Value3 = x.Value3;

            Effect = x.Effect;
        }

        public override string ToString()
        {
            return string.Format("{0}#{1}#{2}#{3}#{4}", Utilities.Basic.DeciToHex(ID), (Value <= 0 ? "" : Utilities.Basic.DeciToHex(Value)),
                (Value2 <= 0 ? "" : Utilities.Basic.DeciToHex(Value2)), (Value3 <= 0 ? "0" : Utilities.Basic.DeciToHex(Value3)), Effect);
        }

        public string SetString()
        {
            return string.Format("{0}#{1}#{2}", Utilities.Basic.DeciToHex(ID), (Value <= 0 ? "" : Utilities.Basic.DeciToHex(Value)),
                (Value2 <= 0 ? "" : Utilities.Basic.DeciToHex(Value2)));
        }

        public void ParseEffect(Characters.Character client)
        {
            Client = client;

            client.Stats.ModifyStatEquipped((EffectEnum)ID, Value);
        }

        public bool IsWeaponEffect()
        {
            return ID >= 91 & ID <= 101;
        }

        public EffectSpell CopyToSpellEffect()
        {
            EffectSpell effect = new EffectSpell();

            effect.ID = ID;
            effect.Value = Value;
            effect.Value2 = Value2;
            effect.Value3 = Value3;
            effect.Effect = Effect;
            effect.Target = new EffectSpellTarget(19);

            return effect;
        }
    }
}
