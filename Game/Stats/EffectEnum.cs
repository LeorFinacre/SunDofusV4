﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.Game.Characters.Stats
{
    public enum EffectEnum
    {
        None = -1,

        Teleport = 4,
        PushBack = 5,
        PushFront = 6,
        Transpose = 8,
        MissBack = 9,

        TurnPass = 140,
        Doom = 141,

        MPSteal = 77,
        BonusMP = 78,
        EcaflipsLuck = 79,
        LifeSteal = 82,
        APSteal = 84,
        LuckSteal = 266,
        VitaSteal = 267,
        AgilitySteal = 268,
        IntelligenceSteal = 269,
        WisdomSteal = 270,
        StrengthSteal = 271,

        DamageLifeWater = 85,
        DamageLifeStrength = 86,
        DamageLifeAir = 87,
        DamageLifeFire = 88,
        DamageLifeNeutral = 89,
        DamageDropLife = 90,
        WaterSteal = 91,
        EarthSteal = 92,
        AirSteal = 93,
        FireSteal = 94,
        NeutralSteal = 95,
        DamageWater = 96,
        DamageStrength = 97,
        DamageAir = 98,
        DamageFire = 99,
        DamageNeutral = 100,
        AddArmor = 105,
        AddArmorBis = 265,

        AddRenvoiDamageItem = 220,
        ReflectSpell = 106,
        AddRenvoiDamage = 107,
        Heal = 108,
        DamageLanceur = 109,
        AddLife = 110,
        AddAP = 111,
        AddDamage = 112,
        MultiplyDamage = 114,

        AddAPBis = 120,
        AddAgility = 119,
        AddLuck = 123,
        AddDamagePercent = 138,
        SubDamagePercent = 186,
        AddDamageCritic = 115,
        AddDamagePiege = 225,
        AddDamagePhysic = 142,
        AddDamageMagic = 143,
        AddEchecCritic = 122,
        AddStrengthAP = 160,
        AddStrengthMP = 161,
        AddStrength = 118,
        AddInitiative = 174,
        AddIntelligence = 126,
        AddMaxMonstersNb = 182,
        AddMP = 128,
        AddRP = 117,
        AddPods = 158,
        AddProspection = 176,
        AddWisdom = 124,
        AddHeal = 178,
        AddVitality = 125,
        SubAgility = 154,

        DamagePerAP = 131,
        IncreaseSpellDamage = 293,
        ControlWeapon = 165,
        RPSteal = 320,
        Punition = 672,
        Sacrifice = 765,
        Con = 130,
        ChastisementDamage = 776,

        SubLuck = 152,
        SubDamage = 164,
        SubDamageBis = 145,
        SubDamageCritic = 171,
        SubDamageMagic = 172,
        SubDamagePhysic = 173,
        SubStrengthAP = 162,
        SubStrengthMP = 163,
        SubStrength = 157,
        SubInitiative = 175,
        SubIntelligence = 155,
        SubAPStrength = 101,
        SubMPStrength = 127,
        SubAP = 168,
        SubMP = 169,
        SubRP = 116,
        SubPods = 159,
        SubProspection = 177,
        SubWisdom = 156,
        SubHeal = 179,
        SubVitality = 153,

        InvocDouble = 180,
        Invocation = 181,

        AddReduceDamagePhysic = 183,
        AddReduceDamageMagic = 184,

        AddReduceDamagePourcentWater = 211,
        AddReduceDamagePourcentStrength = 210,
        AddReduceDamagePourcentAir = 212,
        AddReduceDamagePourcentFire = 213,
        AddReduceDamagePourcentNeutral = 214,
        AddReduceDamagePourcentPvPWater = 251,
        AddReduceDamagePourcentPvPStrength = 250,
        AddReduceDamagePourcentPvPAir = 252,
        AddReduceDamagePourcentPvPFire = 253,
        AddReduceDamagePourcentPvPNeutral = 254,

        AddReduceDamageWater = 241,
        AddReduceDamageStrength = 240,
        AddReduceDamageAir = 242,
        AddReduceDamageFire = 243,
        AddReduceDamageNeutral = 244,
        AddReduceDamagePvPWater = 261,
        AddReduceDamagePvPStrength = 260,
        AddReduceDamagePvPAir = 262,
        AddReduceDamagePvPFire = 263,
        AddReduceDamagePvPNeutral = 264,

        SubReduceDamagePourcentWater = 216,
        SubReduceDamagePourcentStrength = 215,
        SubReduceDamagePourcentAir = 217,
        SubReduceDamagePourcentFire = 218,
        SubReduceDamagePourcentNeutral = 219,
        SubReduceDamagePourcentPvPWater = 255,
        SubReduceDamagePourcentPvPStrength = 256,
        SubReduceDamagePourcentPvPAir = 257,
        SubReduceDamagePourcentPvPFire = 258,
        SubReduceDamagePourcentPvpNeutral = 259,
        SubReduceDamageWater = 246,
        SubReduceDamageStrength = 245,
        SubReduceDamageAir = 247,
        SubReduceDamageFire = 248,
        SubReduceDamageNeutral = 249,

        Porter = 50,
        Lancer = 51,
        Perception = 202,
        ChangeSkin = 149,
        SpellBoost = 293,
        UseTrap = 400,
        UseGlyph = 401,
        DoNothing = 666,
        PushFear = 783,
        AddChastisement = 788,
        AddState = 950,
        LostState = 951,
        Invisible = 150,
        DeleteAllBonus = 132,

        AddSpell = 604,
        AddCharactStrength = 607,
        AddCharactWisdom = 678,
        AddCharactLuck = 608,
        AddCharactAgility = 609,
        AddCharactVitality = 610,
        AddCharactIntelligence = 611,
        AddCharactPoint = 612,
        AddSpellPoint = 613,

        InvocInfo = 628,

        SoulStoneStats = 705,
        MountCaptureProba = 706,

        SoulCaptureBonus = 750,
        MountExpBonus = 751,

        LastEat = 808,

        AlignementId = 960,
        AlignementGrade = 961,
        TargetLevel = 962,
        CreateTime = 963,
        TargetName = 964,

        MountOwner = 995,

        LivingGfxId = 970,
        LivingMood = 971,
        LivingSkin = 972,
        LivingType = 973,
        LivingXp = 974,

        CanBeExchange = 983,

        AddArmorNeutral,
        AddArmorStrength,
        AddArmorFire,
        AddArmorWater,
        AddArmorAir,

        SubArmorNeutral,
        SubArmorStrength,
        SubArmorFire,
        SubArmorWater,
        SubArmorAir,
        SubArmor,

        SubReduceDamagePhysic,
        SubReduceDamageMagic,
    }
}