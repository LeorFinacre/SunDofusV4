using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Game.Characters.Stats
{
    public delegate int StatTotalFormule(int baseValue, int equippedValue, int givenValue, int bonusValue);

    class GenericStats
    {
        private Character character;
        private List<GenericStat> stats = new List<GenericStat>();

        public GenericStats(Character character = null)
        {
            this.character = character;

            stats.Add(new GenericStat(StatEnum.Initiative, new EffectEnum[] { EffectEnum.AddInitiative }, new EffectEnum[] { EffectEnum.SubInitiative }, 100, FormuleInitiative));
            stats.Add(new GenericStat(StatEnum.Prospection, new EffectEnum[] { EffectEnum.AddProspection }, new EffectEnum[] { EffectEnum.SubProspection }, 100, FormuleProspection));
            stats.Add(new GenericStat(StatEnum.MaxLife, new EffectEnum[] { }, new EffectEnum[] { }, 50, FormuleMaxLife));
            stats.Add(new GenericStat(StatEnum.MaxPods, new EffectEnum[] { }, new EffectEnum[] { }, 1000, FormuleMaxPods));

            stats.Add(new GenericStat(StatEnum.Vitalite, new EffectEnum[] { EffectEnum.AddVitalite, EffectEnum.AddVie }, new EffectEnum[] { EffectEnum.SubVitalite }));
            stats.Add(new GenericStat(StatEnum.Sagesse, new EffectEnum[] { EffectEnum.AddSagesse }, new EffectEnum[] { EffectEnum.SubSagesse }));
            stats.Add(new GenericStat(StatEnum.Force, new EffectEnum[] { EffectEnum.AddForce }, new EffectEnum[] { EffectEnum.SubForce }));
            stats.Add(new GenericStat(StatEnum.Intelligence, new EffectEnum[] { EffectEnum.AddIntelligence }, new EffectEnum[] { EffectEnum.SubIntelligence }));
            stats.Add(new GenericStat(StatEnum.Chance, new EffectEnum[] { EffectEnum.AddChance }, new EffectEnum[] { EffectEnum.SubChance }));
            stats.Add(new GenericStat(StatEnum.Agilite, new EffectEnum[] { EffectEnum.AddAgilite }, new EffectEnum[] { EffectEnum.SubAgilite }));
            stats.Add(new GenericStat(StatEnum.MaxPA, new EffectEnum[] { EffectEnum.AddPA, EffectEnum.AddPABis }, new EffectEnum[] { EffectEnum.SubPA, EffectEnum.SubPAEsquive }, 6, FormuleMaxPA));
            stats.Add(new GenericStat(StatEnum.MaxPM, new EffectEnum[] { EffectEnum.AddPM, EffectEnum.BonusPM }, new EffectEnum[] { EffectEnum.SubPM, EffectEnum.SubPMEsquive }, 3));
            stats.Add(new GenericStat(StatEnum.PO, new EffectEnum[] { EffectEnum.AddPO }, new EffectEnum[] { EffectEnum.SubPO }));
            stats.Add(new GenericStat(StatEnum.InvocationMax, new EffectEnum[] { EffectEnum.AddInvocationMax }, new EffectEnum[] { }, 1));

            stats.Add(new GenericStat(StatEnum.Damage, new EffectEnum[] { EffectEnum.AddDamage }, new EffectEnum[] { EffectEnum.SubDamage, EffectEnum.SubDamageBis }));
            stats.Add(new GenericStat(StatEnum.DamagePhysic, new EffectEnum[] { EffectEnum.AddDamagePhysic }, new EffectEnum[] { EffectEnum.SubDamagePhysic }));
            stats.Add(new GenericStat(StatEnum.DamageMagic, new EffectEnum[] { EffectEnum.AddDamageMagic }, new EffectEnum[] { EffectEnum.SubDamageMagic }));
            stats.Add(new GenericStat(StatEnum.DamagePercent, new EffectEnum[] { EffectEnum.AddDamagePercent }, new EffectEnum[] { EffectEnum.SubDamagePercent }));
            stats.Add(new GenericStat(StatEnum.DamagePiege, new EffectEnum[] { EffectEnum.AddDamagePiege }, new EffectEnum[] { }));
            stats.Add(new GenericStat(StatEnum.DamagePiegePercent, new EffectEnum[] { }, new EffectEnum[] { }));
            stats.Add(new GenericStat(StatEnum.Soins, new EffectEnum[] { EffectEnum.AddSoins }, new EffectEnum[] { EffectEnum.SubSoins }));
            stats.Add(new GenericStat(StatEnum.ReflectDamage, new EffectEnum[] { EffectEnum.AddRenvoiDamage, EffectEnum.AddRenvoiDamageItem }, new EffectEnum[] { }));
            stats.Add(new GenericStat(StatEnum.DamageCritic, new EffectEnum[] { EffectEnum.AddDamageCritic }, new EffectEnum[] { EffectEnum.SubDamageCritic }));
            stats.Add(new GenericStat(StatEnum.EchecCritic, new EffectEnum[] { EffectEnum.AddEchecCritic }, new EffectEnum[] { }));
            stats.Add(new GenericStat(StatEnum.EsquivePA, new EffectEnum[] { EffectEnum.AddEsquivePA }, new EffectEnum[] { EffectEnum.SubEsquivePA }, 0, FormuleEsquive));
            stats.Add(new GenericStat(StatEnum.EsquivePM, new EffectEnum[] { EffectEnum.AddEsquivePM }, new EffectEnum[] { EffectEnum.SubEsquivePM }, 0, FormuleEsquive));

            stats.Add(new GenericStat(StatEnum.ReduceDamagePhysic, new EffectEnum[] { EffectEnum.AddReduceDamagePhysic }, new EffectEnum[] { EffectEnum.SubReduceDamagePhysic }));
            stats.Add(new GenericStat(StatEnum.ReduceDamageMagic, new EffectEnum[] { EffectEnum.AddReduceDamageMagic }, new EffectEnum[] { EffectEnum.SubReduceDamageMagic }));

            stats.Add(new GenericStat(StatEnum.ReduceDamageNeutre, new EffectEnum[] { EffectEnum.AddReduceDamageNeutre }, new EffectEnum[] { EffectEnum.SubReduceDamageNeutre }));
            stats.Add(new GenericStat(StatEnum.ReduceDamagePercentNeutre, new EffectEnum[] { EffectEnum.AddReduceDamagePourcentNeutre }, new EffectEnum[] { EffectEnum.SubReduceDamagePourcentNeutre }));
            stats.Add(new GenericStat(StatEnum.ReduceDamageTerre, new EffectEnum[] { EffectEnum.AddReduceDamageTerre }, new EffectEnum[] { EffectEnum.SubReduceDamageTerre }));
            stats.Add(new GenericStat(StatEnum.ReduceDamagePercentTerre, new EffectEnum[] { EffectEnum.AddReduceDamagePourcentTerre }, new EffectEnum[] { EffectEnum.SubReduceDamagePourcentTerre }));
            stats.Add(new GenericStat(StatEnum.ReduceDamageFeu, new EffectEnum[] { EffectEnum.AddReduceDamageFeu }, new EffectEnum[] { EffectEnum.SubReduceDamageFeu }));
            stats.Add(new GenericStat(StatEnum.ReduceDamagePercentFeu, new EffectEnum[] { EffectEnum.AddReduceDamagePourcentFeu }, new EffectEnum[] { EffectEnum.SubReduceDamagePourcentFeu }));
            stats.Add(new GenericStat(StatEnum.ReduceDamageEau, new EffectEnum[] { EffectEnum.AddReduceDamageEau }, new EffectEnum[] { EffectEnum.SubReduceDamageEau }));
            stats.Add(new GenericStat(StatEnum.ReduceDamagePercentEau, new EffectEnum[] { EffectEnum.AddReduceDamagePourcentEau }, new EffectEnum[] { EffectEnum.SubReduceDamagePourcentEau }));
            stats.Add(new GenericStat(StatEnum.ReduceDamageAir, new EffectEnum[] { EffectEnum.AddReduceDamageAir }, new EffectEnum[] { EffectEnum.SubReduceDamageAir }));
            stats.Add(new GenericStat(StatEnum.ReduceDamagePercentAir, new EffectEnum[] { EffectEnum.AddReduceDamagePourcentAir }, new EffectEnum[] { EffectEnum.SubReduceDamagePourcentAir }));

            stats.Add(new GenericStat(StatEnum.ReduceDamagePvPNeutre, new EffectEnum[] { EffectEnum.AddReduceDamagePvPNeutre }, new EffectEnum[] { }));
            stats.Add(new GenericStat(StatEnum.ReduceDamagePercentPvPNeutre, new EffectEnum[] { EffectEnum.AddReduceDamagePourcentPvPNeutre }, new EffectEnum[] { }));
            stats.Add(new GenericStat(StatEnum.ReduceDamagePvPTerre, new EffectEnum[] { EffectEnum.AddReduceDamagePvPTerre }, new EffectEnum[] { }));
            stats.Add(new GenericStat(StatEnum.ReduceDamagePercentPvPTerre, new EffectEnum[] { EffectEnum.AddReduceDamagePourcentPvPTerre }, new EffectEnum[] { EffectEnum.SubReduceDamagePourcentPvPTerre }));
            stats.Add(new GenericStat(StatEnum.ReduceDamagePvPFeu, new EffectEnum[] { EffectEnum.AddReduceDamagePvPFeu }, new EffectEnum[] { }));
            stats.Add(new GenericStat(StatEnum.ReduceDamagePercentPvPFeu, new EffectEnum[] { EffectEnum.AddReduceDamagePourcentPvPFeu }, new EffectEnum[] { EffectEnum.SubReduceDamagePourcentPvPFeu }));
            stats.Add(new GenericStat(StatEnum.ReduceDamagePvPEau, new EffectEnum[] { EffectEnum.AddReduceDamagePvPEau }, new EffectEnum[] { }));
            stats.Add(new GenericStat(StatEnum.ReduceDamagePercentPvPEau, new EffectEnum[] { EffectEnum.AddReduceDamagePourcentPvPEau }, new EffectEnum[] { EffectEnum.SubReduceDamagePourcentPvPEau }));
            stats.Add(new GenericStat(StatEnum.ReduceDamagePvPAir, new EffectEnum[] { EffectEnum.AddReduceDamagePvPAir }, new EffectEnum[] { }));
            stats.Add(new GenericStat(StatEnum.ReduceDamagePercentPvPAir, new EffectEnum[] { EffectEnum.AddReduceDamagePourcentPvPAir }, new EffectEnum[] { EffectEnum.SubReduceDamagePourcentPvPAir }));

            stats.Add(new GenericStat(StatEnum.Armor, new EffectEnum[] { EffectEnum.AddArmor, EffectEnum.AddArmorBis }, new EffectEnum[] { EffectEnum.SubArmor }));
            stats.Add(new GenericStat(StatEnum.ArmorNeutre, new EffectEnum[] { EffectEnum.AddArmorNeutre }, new EffectEnum[] { EffectEnum.SubArmorNeutre }));
            stats.Add(new GenericStat(StatEnum.ArmorTerre, new EffectEnum[] { EffectEnum.AddArmorTerre }, new EffectEnum[] { EffectEnum.SubArmorTerre }));
            stats.Add(new GenericStat(StatEnum.ArmorFeu, new EffectEnum[] { EffectEnum.AddArmorFeu }, new EffectEnum[] { EffectEnum.SubArmorFeu }));
            stats.Add(new GenericStat(StatEnum.ArmorEau, new EffectEnum[] { EffectEnum.AddArmorEau }, new EffectEnum[] { EffectEnum.SubArmorEau }));
            stats.Add(new GenericStat(StatEnum.ArmorAir, new EffectEnum[] { EffectEnum.AddArmorAir }, new EffectEnum[] { EffectEnum.SubArmorAir }));
        }

        public static int FormuleTotal(int baseValue, int equippedValue, int givenValue, int bonusValue)
        {
            return baseValue + equippedValue + givenValue + bonusValue;
        }

        public int FormuleInitiative(int baseValue, int equippedValue, int givenValue, int bonusValue)
        {
            return (GetStat(StatEnum.Force).Total + GetStat(StatEnum.Intelligence).Total + GetStat(StatEnum.Chance).Total + GetStat(StatEnum.Agilite).Total + (baseValue + equippedValue + givenValue + bonusValue)) * (character.Life / GetStat(StatEnum.MaxLife).Total);
        }

        public int FormuleProspection(int baseValue, int equippedValue, int givenValue, int bonusValue)
        {
            return (baseValue + equippedValue + givenValue + bonusValue) + GetStat(StatEnum.Chance).Total / 10 + (character.Class == 3 ? 20 : 0);
        }

        public int FormuleMaxLife(int baseValue, int equippedValue, int givenValue, int bonusValue)
        {
            return (baseValue + equippedValue + givenValue + bonusValue) + GetStat(StatEnum.Vitalite).Total + character.Level * 5;
        }

        public int FormuleMaxPods(int baseValue, int equippedValue, int givenValue, int bonusValue)
        {
            return (baseValue + equippedValue + givenValue + bonusValue) + GetStat(StatEnum.Force).Total * 5;
        }

        public int FormuleMaxPA(int baseValue, int equippedValue, int givenValue, int bonusValue)
        {
            return (baseValue + equippedValue + givenValue + bonusValue) + (character.Level >= 100 ? 1 : 0);
        }

        public int FormuleEsquive(int baseValue, int equippedValue, int givenValue, int bonusValue)
        {
            return (baseValue + equippedValue + givenValue + bonusValue) + GetStat(StatEnum.Sagesse).Total / 4;
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
