using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SunDofus.World.Game.Characters.Stats;
using SunDofus.World.Game.Maps.Fights.Effects;

namespace SunDofus.Game.Maps.Fights
{
    public enum BuffActiveType
    {
        ACTIVE_STATS,
        ACTIVE_BEGINTURN,
        ACTIVE_ENDTURN,
        ACTIVE_ENDMOVE,
        ACTIVE_ATTACK_POST_JET,
        ACTIVE_ATTACK_AFTER_JET,
        ACTIVE_ATTACKED_POST_JET,
        ACTIVE_ATTACKED_AFTER_JET,
    }

    public enum BuffDecrementType
    {
        TYPE_BEGINTURN,
        TYPE_ENDTURN,
        TYPE_ENDMOVE,
    }

    class FighterBuff
    {
        private Dictionary<BuffActiveType, List<EffectCast>> buffActives = new Dictionary<BuffActiveType, List<EffectCast>>()
        {
            { BuffActiveType.ACTIVE_ATTACKED_AFTER_JET, new List<EffectCast>() },
            { BuffActiveType.ACTIVE_ATTACKED_POST_JET, new List<EffectCast>() },
            { BuffActiveType.ACTIVE_ATTACK_AFTER_JET, new List<EffectCast>() },
            { BuffActiveType.ACTIVE_ATTACK_POST_JET, new List<EffectCast>() },
            { BuffActiveType.ACTIVE_BEGINTURN, new List<EffectCast>() },
            { BuffActiveType.ACTIVE_ENDTURN, new List<EffectCast>() },
            { BuffActiveType.ACTIVE_ENDMOVE, new List<EffectCast>() },
            { BuffActiveType.ACTIVE_STATS, new List<EffectCast>() }
        };

        private Dictionary<BuffDecrementType, List<EffectCast>> buffDecrements = new Dictionary<BuffDecrementType, List<EffectCast>>()
        {
            { BuffDecrementType.TYPE_BEGINTURN, new List<EffectCast>() },
            { BuffDecrementType.TYPE_ENDTURN, new List<EffectCast>() },
            { BuffDecrementType.TYPE_ENDMOVE, new List<EffectCast>() }
        };

        public void AddBuff(EffectCast buff, BuffActiveType active, BuffDecrementType decrement)
        {
            buff = buff.CopyToBuff();

            if (buff.Target == buff.Target.Fight.CurrentFighter)
                buff.Duration += 1;

            buffActives[active].Add(buff);
            buffDecrements[decrement].Add(buff);

            switch (buff.Type)
            {
                case EffectEnum.ChanceEcaflip:
                    buff.Target.Fight.Send(string.Format("GIE{0};{1};{2};{3};{4};{5};{6};{7}", (int)buff.Type, buff.Target.ID, buff.Value1, buff.Value2, buff.Value3, buff.Chance, buff.Duration, buff.SpellID));
                    break;

                default:
                    buff.Target.Fight.Send(string.Format("GIE{0};{1};{2};{3};;{4};{5};{6}", (int)buff.Type, buff.Target.ID, buff.Value1, (buff.Value2 == -1 ? "" : buff.Value2.ToString()), buff.Chance, buff.Duration, buff.SpellID));
                    break;
            }
        }

        public void OnTurnBegin()
        {
            foreach (var buff in buffActives[BuffActiveType.ACTIVE_BEGINTURN])
                BuffProcessor.ApplyBuff(buff);

            foreach (var buff in buffDecrements[BuffDecrementType.TYPE_BEGINTURN])
            {
                buff.Duration -= 1;

                if (buff.Duration <= 0)
                {
                    BuffProcessor.RemoveBuff(buff);

                    foreach (var active in buffActives.Keys)
                    {
                        if (buffActives[active].Contains(buff))
                        {
                            buffActives[active].Remove(buff);
                            break;
                        }
                    }
                }
            }

            buffDecrements[BuffDecrementType.TYPE_BEGINTURN].RemoveAll(x => x.Duration <= 0);
        }

        public void OnTurnEnd()
        {
            foreach (var buff in buffActives[BuffActiveType.ACTIVE_ENDTURN])
                BuffProcessor.ApplyBuff(buff);

            foreach (var buff in buffDecrements[BuffDecrementType.TYPE_ENDTURN])
            {
                buff.Duration -= 1;

                if (buff.Duration <= 0)
                {
                    BuffProcessor.RemoveBuff(buff);

                    foreach (var active in buffActives.Keys)
                    {
                        if (buffActives[active].Contains(buff))
                        {
                            buffActives[active].Remove(buff);
                            break;
                        }
                    }
                }
            }

            buffDecrements[BuffDecrementType.TYPE_ENDTURN].RemoveAll(x => x.Duration <= 0);
        }

        public void OnMoveEnd()
        {
            foreach (var buff in buffActives[BuffActiveType.ACTIVE_ENDMOVE])
                BuffProcessor.ApplyBuff(buff);

            foreach (var buff in buffDecrements[BuffDecrementType.TYPE_ENDMOVE])
            {
                buff.Duration -= 1;

                if (buff.Duration <= 0)
                {
                    BuffProcessor.RemoveBuff(buff);

                    foreach (var active in buffActives.Keys)
                    {
                        if (buffActives[active].Contains(buff))
                        {
                            buffActives[active].Remove(buff);
                            return;
                        }
                    }
                }
            }

            buffDecrements[BuffDecrementType.TYPE_ENDMOVE].RemoveAll(x => x.Duration <= 0);
        }

        public void OnAttackPostJet(EffectCast cast)
        {
            foreach (var buff in buffActives[BuffActiveType.ACTIVE_ATTACK_POST_JET])
                BuffProcessor.ApplyBuff(buff, cast);
        }

        public void OnAttackAfterJet(EffectCast cast)
        {
            foreach (var buff in buffActives[BuffActiveType.ACTIVE_ATTACK_AFTER_JET])
                BuffProcessor.ApplyBuff(buff, cast);
        }

        public void OnAttackedPostJet(EffectCast cast)
        {
            foreach (var buff in buffActives[BuffActiveType.ACTIVE_ATTACKED_POST_JET])
                BuffProcessor.ApplyBuff(buff, cast);
        }

        public void OnAttackedAfterJet(EffectCast cast)
        {
            foreach (EffectCast buff in buffActives[BuffActiveType.ACTIVE_ATTACKED_AFTER_JET])
                BuffProcessor.ApplyBuff(buff, cast);
        }

        public void Debuff()
        {
            foreach (var decrement in buffDecrements.Keys)
            {
                foreach (var buff in buffDecrements[decrement])
                    if (IsDebuffable(buff))
                        BuffProcessor.RemoveBuff(buff);

                buffDecrements[decrement].RemoveAll(x => IsDebuffable(x));
            }

            foreach (var active in buffActives.Keys)
                buffActives[active].RemoveAll(x => IsDebuffable(x));
        }

        private bool IsDebuffable(EffectCast cast)
        {
            switch (cast.Type)
            {
                case EffectEnum.AddPA:
                case EffectEnum.AddPABis:
                case EffectEnum.AddPM:
                case EffectEnum.SubPA:
                case EffectEnum.SubPAEsquive:
                    return false;
            }

            return true;
        }
    }
}
