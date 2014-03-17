using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SunDofus.World.Game.Characters.Spells;

namespace SunDofus.World.Game.Maps.Fights
{
    class FighterSpell
    {
        private Dictionary<int, SpellCooldown> cooldown = new Dictionary<int, SpellCooldown>();
        private Dictionary<int, List<SpellTarget>> targets = new Dictionary<int, List<SpellTarget>>();

        public bool CanLaunchSpell(CharacterSpell spell, long target)
        {
            if (spell.LevelModel.TurnNumber > 0)
            {
                if (cooldown.ContainsKey(spell.ID) && cooldown[spell.ID].Cooldown > 0)
                    return false;
            }

            if (spell.LevelModel.MaxPerTurn > 0)
            {
                if (targets.ContainsKey(spell.ID) && targets[spell.ID].Count >= spell.LevelModel.MaxPerTurn)
                    return false;
            }

            if (spell.LevelModel.MaxPerPlayer > 0)
            {
                if (targets.ContainsKey(spell.ID) && targets[spell.ID].Count(x => x.Target == target) >= spell.LevelModel.MaxPerPlayer)
                    return false;
            }

            return true;
        }

        public void OnLaunchSpell(CharacterSpell spell, long target)
        {
            if (spell.LevelModel.TurnNumber > 0)
            {
                if (cooldown.ContainsKey(spell.ID))
                    cooldown[spell.ID].Cooldown = spell.LevelModel.TurnNumber;
                else
                    cooldown.Add(spell.ID, new SpellCooldown(spell.LevelModel.TurnNumber));
            }

            if (spell.LevelModel.MaxPerTurn > 0 || spell.LevelModel.MaxPerPlayer > 0)
            {
                if (targets.ContainsKey(spell.ID))
                    targets[spell.ID].Add(new SpellTarget(target));
                else
                    targets.Add(spell.ID, new List<SpellTarget> { new SpellTarget(target) });
            }
        }

        public void OnTurnEnd()
        {
            foreach (List<SpellTarget> targetsChild in targets.Values)
                targetsChild.Clear();

            foreach (SpellCooldown cooldownChild in cooldown.Values)
                cooldownChild.Decrement();
        }

        private class SpellCooldown
        {
            public SpellCooldown(int cooldown)
            {
                this.Cooldown = cooldown;
            }

            public int Cooldown { get; set; }

            public void Decrement()
            {
                this.Cooldown--;
            }
        }

        private class SpellTarget
        {
            public SpellTarget(long target)
            {
                this.Target = target;
            }

            public long Target { get; set; }
        }
    }
}