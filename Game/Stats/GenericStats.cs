using SunDofus.DataRecords;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.Game.Characters.Stats
{
    public delegate int StatTotalFormule(int baseValue, int equippedValue, int givenValue, int bonusValue);

    class GenericStats
    {
        private DB_Character character;
        private List<GenericStat> stats = new List<GenericStat>();

        public GenericStats(DB_Character character = null)
        {
            this.character = character;

            stats.Add(new GenericStat(StatEnum.Initiative, new EffectEnum[] { EffectEnum.AddInitiative }, new EffectEnum[] { EffectEnum.SubInitiative }, 100, FormuleInitiative));
            stats.Add(new GenericStat(StatEnum.Prospection, new EffectEnum[] { EffectEnum.AddProspection }, new EffectEnum[] { EffectEnum.SubProspection }, 100, FormuleProspection));
            stats.Add(new GenericStat(StatEnum.MaxLife, new EffectEnum[] { }, new EffectEnum[] { }, 50, FormuleMaxLife));
            stats.Add(new GenericStat(StatEnum.MaxPods, new EffectEnum[] { }, new EffectEnum[] { }, 1000, FormuleMaxPods));

            stats.Add(new GenericStat(StatEnum.Vitality, new EffectEnum[] { EffectEnum.AddVitality, EffectEnum.AddLife }, new EffectEnum[] { EffectEnum.SubVitality }));
            stats.Add(new GenericStat(StatEnum.Wisdom, new EffectEnum[] { EffectEnum.AddWisdom }, new EffectEnum[] { EffectEnum.SubWisdom }));
            stats.Add(new GenericStat(StatEnum.Strength, new EffectEnum[] { EffectEnum.AddStrength }, new EffectEnum[] { EffectEnum.SubStrength }));
            stats.Add(new GenericStat(StatEnum.Intelligence, new EffectEnum[] { EffectEnum.AddIntelligence }, new EffectEnum[] { EffectEnum.SubIntelligence }));
            stats.Add(new GenericStat(StatEnum.Luck, new EffectEnum[] { EffectEnum.AddLuck }, new EffectEnum[] { EffectEnum.SubLuck }));
            stats.Add(new GenericStat(StatEnum.Agility, new EffectEnum[] { EffectEnum.AddAgility }, new EffectEnum[] { EffectEnum.SubAgility }));
            stats.Add(new GenericStat(StatEnum.MaxAP, new EffectEnum[] { EffectEnum.AddAP, EffectEnum.AddAPBis }, new EffectEnum[] { EffectEnum.SubAP, EffectEnum.SubAPStrength }, 6, FormuleMaxPA));
            stats.Add(new GenericStat(StatEnum.MaxMP, new EffectEnum[] { EffectEnum.AddMP, EffectEnum.BonusMP }, new EffectEnum[] { EffectEnum.SubMP, EffectEnum.SubMPStrength }, 3));
            stats.Add(new GenericStat(StatEnum.RP, new EffectEnum[] { EffectEnum.AddRP }, new EffectEnum[] { EffectEnum.SubRP }));
            stats.Add(new GenericStat(StatEnum.InvocationMax, new EffectEnum[] { EffectEnum.AddMaxMonstersNb }, new EffectEnum[] { }, 1));

            stats.Add(new GenericStat(StatEnum.Damage, new EffectEnum[] { EffectEnum.AddDamage }, new EffectEnum[] { EffectEnum.SubDamage, EffectEnum.SubDamageBis }));
            stats.Add(new GenericStat(StatEnum.DamagePhysic, new EffectEnum[] { EffectEnum.AddDamagePhysic }, new EffectEnum[] { EffectEnum.SubDamagePhysic }));
            stats.Add(new GenericStat(StatEnum.DamageMagic, new EffectEnum[] { EffectEnum.AddDamageMagic }, new EffectEnum[] { EffectEnum.SubDamageMagic }));
            stats.Add(new GenericStat(StatEnum.DamagePercent, new EffectEnum[] { EffectEnum.AddDamagePercent }, new EffectEnum[] { EffectEnum.SubDamagePercent }));
            stats.Add(new GenericStat(StatEnum.DamagePiege, new EffectEnum[] { EffectEnum.AddDamagePiege }, new EffectEnum[] { }));
            stats.Add(new GenericStat(StatEnum.DamagePiegePercent, new EffectEnum[] { }, new EffectEnum[] { }));
            stats.Add(new GenericStat(StatEnum.Heal, new EffectEnum[] { EffectEnum.AddHeal }, new EffectEnum[] { EffectEnum.SubHeal }));
            stats.Add(new GenericStat(StatEnum.ReflectDamage, new EffectEnum[] { EffectEnum.AddRenvoiDamage, EffectEnum.AddRenvoiDamageItem }, new EffectEnum[] { }));
            stats.Add(new GenericStat(StatEnum.DamageCritic, new EffectEnum[] { EffectEnum.AddDamageCritic }, new EffectEnum[] { EffectEnum.SubDamageCritic }));
            stats.Add(new GenericStat(StatEnum.EchecCritic, new EffectEnum[] { EffectEnum.AddEchecCritic }, new EffectEnum[] { }));
            stats.Add(new GenericStat(StatEnum.StrengthAP, new EffectEnum[] { EffectEnum.AddStrengthMP }, new EffectEnum[] { EffectEnum.SubStrengthAP }, 0, FormuleEsquive));
            stats.Add(new GenericStat(StatEnum.StrengthMP, new EffectEnum[] { EffectEnum.AddStrengthMP }, new EffectEnum[] { EffectEnum.SubStrengthMP }, 0, FormuleEsquive));

            stats.Add(new GenericStat(StatEnum.ReduceDamagePhysic, new EffectEnum[] { EffectEnum.AddReduceDamagePhysic }, new EffectEnum[] { EffectEnum.SubReduceDamagePhysic }));
            stats.Add(new GenericStat(StatEnum.ReduceDamageMagic, new EffectEnum[] { EffectEnum.AddReduceDamageMagic }, new EffectEnum[] { EffectEnum.SubReduceDamageMagic }));

            stats.Add(new GenericStat(StatEnum.ReduceDamageNeutral, new EffectEnum[] { EffectEnum.AddReduceDamageNeutral }, new EffectEnum[] { EffectEnum.SubReduceDamageNeutral }));
            stats.Add(new GenericStat(StatEnum.ReduceDamagePercentNeutral, new EffectEnum[] { EffectEnum.AddReduceDamagePourcentNeutral }, new EffectEnum[] { EffectEnum.SubReduceDamagePourcentNeutral }));
            stats.Add(new GenericStat(StatEnum.ReduceDamageEarth, new EffectEnum[] { EffectEnum.AddReduceDamageStrength }, new EffectEnum[] { EffectEnum.SubReduceDamageStrength }));
            stats.Add(new GenericStat(StatEnum.ReduceDamagePercentEarth, new EffectEnum[] { EffectEnum.AddReduceDamagePourcentStrength }, new EffectEnum[] { EffectEnum.SubReduceDamagePourcentStrength }));
            stats.Add(new GenericStat(StatEnum.ReduceDamageFire, new EffectEnum[] { EffectEnum.AddReduceDamageFire }, new EffectEnum[] { EffectEnum.SubReduceDamageFire }));
            stats.Add(new GenericStat(StatEnum.ReduceDamagePercentFire, new EffectEnum[] { EffectEnum.AddReduceDamagePourcentFire }, new EffectEnum[] { EffectEnum.SubReduceDamagePourcentFire }));
            stats.Add(new GenericStat(StatEnum.ReduceDamageWater, new EffectEnum[] { EffectEnum.AddReduceDamageWater }, new EffectEnum[] { EffectEnum.SubReduceDamageWater }));
            stats.Add(new GenericStat(StatEnum.ReduceDamagePercentWater, new EffectEnum[] { EffectEnum.AddReduceDamagePourcentWater }, new EffectEnum[] { EffectEnum.SubReduceDamagePourcentWater }));
            stats.Add(new GenericStat(StatEnum.ReduceDamageAir, new EffectEnum[] { EffectEnum.AddReduceDamageAir }, new EffectEnum[] { EffectEnum.SubReduceDamageAir }));
            stats.Add(new GenericStat(StatEnum.ReduceDamagePercentAir, new EffectEnum[] { EffectEnum.AddReduceDamagePourcentAir }, new EffectEnum[] { EffectEnum.SubReduceDamagePourcentAir }));

            stats.Add(new GenericStat(StatEnum.ReduceDamagePvPNeutral, new EffectEnum[] { EffectEnum.AddReduceDamagePvPNeutral }, new EffectEnum[] { }));
            stats.Add(new GenericStat(StatEnum.ReduceDamagePercentPvPNeutral, new EffectEnum[] { EffectEnum.AddReduceDamagePourcentPvPNeutral }, new EffectEnum[] { }));
            stats.Add(new GenericStat(StatEnum.ReduceDamagePvPEarth, new EffectEnum[] { EffectEnum.AddReduceDamagePvPStrength }, new EffectEnum[] { }));
            stats.Add(new GenericStat(StatEnum.ReduceDamagePercentPvPEarth, new EffectEnum[] { EffectEnum.AddReduceDamagePourcentPvPStrength }, new EffectEnum[] { EffectEnum.SubReduceDamagePourcentPvPStrength }));
            stats.Add(new GenericStat(StatEnum.ReduceDamagePvPFire, new EffectEnum[] { EffectEnum.AddReduceDamagePvPFire }, new EffectEnum[] { }));
            stats.Add(new GenericStat(StatEnum.ReduceDamagePercentPvPFire, new EffectEnum[] { EffectEnum.AddReduceDamagePourcentPvPFire }, new EffectEnum[] { EffectEnum.SubReduceDamagePourcentPvPFire }));
            stats.Add(new GenericStat(StatEnum.ReduceDamagePvPWater, new EffectEnum[] { EffectEnum.AddReduceDamagePvPWater }, new EffectEnum[] { }));
            stats.Add(new GenericStat(StatEnum.ReduceDamagePercentPvPWater, new EffectEnum[] { EffectEnum.AddReduceDamagePourcentPvPWater }, new EffectEnum[] { EffectEnum.SubReduceDamagePourcentPvPWater }));
            stats.Add(new GenericStat(StatEnum.ReduceDamagePvPAir, new EffectEnum[] { EffectEnum.AddReduceDamagePvPAir }, new EffectEnum[] { }));
            stats.Add(new GenericStat(StatEnum.ReduceDamagePercentPvPAir, new EffectEnum[] { EffectEnum.AddReduceDamagePourcentPvPAir }, new EffectEnum[] { EffectEnum.SubReduceDamagePourcentPvPAir }));

            stats.Add(new GenericStat(StatEnum.Armor, new EffectEnum[] { EffectEnum.AddArmor, EffectEnum.AddArmorBis }, new EffectEnum[] { EffectEnum.SubArmor }));
            stats.Add(new GenericStat(StatEnum.ArmorNeutral, new EffectEnum[] { EffectEnum.AddArmorNeutral }, new EffectEnum[] { EffectEnum.SubArmorNeutral }));
            stats.Add(new GenericStat(StatEnum.ArmorEarth, new EffectEnum[] { EffectEnum.AddArmorStrength }, new EffectEnum[] { EffectEnum.SubArmorStrength }));
            stats.Add(new GenericStat(StatEnum.ArmorFire, new EffectEnum[] { EffectEnum.AddArmorFire }, new EffectEnum[] { EffectEnum.SubArmorFire }));
            stats.Add(new GenericStat(StatEnum.ArmorWater, new EffectEnum[] { EffectEnum.AddArmorWater }, new EffectEnum[] { EffectEnum.SubArmorWater }));
            stats.Add(new GenericStat(StatEnum.ArmorAir, new EffectEnum[] { EffectEnum.AddArmorAir }, new EffectEnum[] { EffectEnum.SubArmorAir }));
        }

        public static int FormuleTotal(int baseValue, int equippedValue, int givenValue, int bonusValue)
        {
            return baseValue + equippedValue + givenValue + bonusValue;
        }

        public int FormuleInitiative(int baseValue, int equippedValue, int givenValue, int bonusValue)
        {
            return (GetStat(StatEnum.Strength).Total + GetStat(StatEnum.Intelligence).Total + GetStat(StatEnum.Luck).Total + GetStat(StatEnum.Agility).Total + (baseValue + equippedValue + givenValue + bonusValue)) * (character.Life / GetStat(StatEnum.MaxLife).Total);
        }

        public int FormuleProspection(int baseValue, int equippedValue, int givenValue, int bonusValue)
        {
            return (baseValue + equippedValue + givenValue + bonusValue) + GetStat(StatEnum.Luck).Total / 10 + (character.Class == 3 ? 20 : 0);
        }

        public int FormuleMaxLife(int baseValue, int equippedValue, int givenValue, int bonusValue)
        {
            return (baseValue + equippedValue + givenValue + bonusValue) + GetStat(StatEnum.Vitality).Total + character.Level * 5;
        }

        public int FormuleMaxPods(int baseValue, int equippedValue, int givenValue, int bonusValue)
        {
            return (baseValue + equippedValue + givenValue + bonusValue) + GetStat(StatEnum.Strength).Total * 5;
        }

        public int FormuleMaxPA(int baseValue, int equippedValue, int givenValue, int bonusValue)
        {
            return (baseValue + equippedValue + givenValue + bonusValue) + (character.Level >= 100 ? 1 : 0);
        }

        public int FormuleEsquive(int baseValue, int equippedValue, int givenValue, int bonusValue)
        {
            return (baseValue + equippedValue + givenValue + bonusValue) + GetStat(StatEnum.Wisdom).Total / 4;
        }

        public GenericStat GetStat(StatEnum stat)
        {
            return stats.FirstOrDefault(x => x.Type == stat);
        }

        public void ModifyStatBase(EffectEnum effect, int value)
        {
            GenericStat stat = null;
            var addValue = false;

            foreach (var forStat in stats)
            {
                if (forStat.AddEffects.Contains(effect))
                {
                    stat = forStat;
                    addValue = true;
                    break;
                }

                if (forStat.SubEffects.Contains(effect))
                {
                    stat = forStat;
                    addValue = false;
                    break;
                }
            }

            if (stat == null)
                return;

            if (addValue)
                stat.Base += value;
            else
                stat.Base -= value;
        }

        public void ModifyStatEquipped(EffectEnum effect, int value)
        {
            GenericStat stat = null;
            var addValue = false;

            foreach (var forStat in stats)
            {
                if (forStat.AddEffects.Contains(effect))
                {
                    stat = forStat;
                    addValue = true;
                    break;
                }

                if (forStat.SubEffects.Contains(effect))
                {
                    stat = forStat;
                    addValue = false;
                    break;
                }
            }

            if (stat == null)
                return;

            if (addValue)
                stat.Equipped += value;
            else
                stat.Equipped -= value;
        }

        public void ModifyStatGiven(EffectEnum effect, int value)
        {
            GenericStat stat = null;
            var addValue = false;

            foreach (var forStat in stats)
            {
                if (forStat.AddEffects.Contains(effect))
                {
                    stat = forStat;
                    addValue = true;
                    break;
                }

                if (forStat.SubEffects.Contains(effect))
                {
                    stat = forStat;
                    addValue = false;
                    break;
                }
            }

            if (stat == null)
                return;

            if (addValue)
                stat.Given += value;
            else
                stat.Given -= value;
        }

        public void ModifyStatBonus(EffectEnum effect, int value, bool opposite = false)
        {
            GenericStat stat = null;
            var addValue = false;

            foreach (var forStat in stats)
            {
                if (forStat.AddEffects.Contains(effect))
                {
                    stat = forStat;
                    addValue = true;
                    break;
                }

                if (forStat.SubEffects.Contains(effect))
                {
                    stat = forStat;
                    addValue = false;
                    break;
                }
            }

            if (stat == null)
                return;

            if (opposite)
                addValue = !addValue;

            if (addValue)
                stat.Bonus += value;
            else
                stat.Bonus -= value;
        }

        public void ResetStatEquipped()
        {
            foreach (var stat in stats)
                stat.Equipped = 0;
        }

        public void ResetStatGiven()
        {
            foreach (var stat in stats)
                stat.Given = 0;
        }

        public void ResetStatBonus()
        {
            foreach (var stat in stats)
                stat.Bonus = 0;
        }
    }
}
