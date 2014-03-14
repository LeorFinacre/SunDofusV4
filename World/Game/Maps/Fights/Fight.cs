using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SunDofus.World.Game.Characters;
using System.Threading;
using System.Runtime.CompilerServices;
using SunDofus.World.Game.Characters.Spells;
using SunDofus.Utilities;
using SunDofus.World.Game.Effects;
using SunDofus.World.Game.Maps.Fights.Effects;
using SunDofus.World.Game.Characters.Stats;
using SunDofus.World.Game.Characters.Items;

namespace SunDofus.World.Game.Maps.Fights
{
    #region Enums

    public enum FightType
    {
        CHALLENGE = 0,
        AGRESSION = 1,
        PVMA = 2,
        MXVM = 3,
        PVM = 4,
        PVT = 5,
        PVMU = 6,
        PRISME = 7,
        COLLECTOR = 8
    }

    public enum FightState
    {
        STARTING,
        PLAYING,
        WAITING,
        FINISHED
    }

    public enum FighterType
    {
        CHARACTER,
        MONSTER,
        TAX_COLLECTOR
    }

    #endregion

    abstract class Fight
    {
        public int ID { get; set; }

        public Map Map { get; set; }
        public FightType Type { get; set; }
        public FightState State { get; set; }

        public FightTeam Team1 { get; set; }
        public FightTeam Team2 { get; set; }
        public Fighter CurrentFighter { get; set; }

        private Timer timer;
        private long startTimeOut;
        private long turnTimeOut;
        private long waitingTimeOut;

        private List<Fighter> turnFighters = new List<Fighter>();
        private List<Character> spectators = new List<Character>();

        public long EndTime
        {
            get
            {
                return (Environment.TickCount - startTimeOut);
            }
        }

        public Fight(FightType type, Map map)
        {
            ID = map.NextFightID();
            Type = type;
            State = FightState.STARTING;

            Map = map;
            timer = new Timer(new TimerCallback(TimerLoop), null, 1000, 1000);

            Dictionary<int, int[]> places = GeneratePlaces();

            Team1 = new FightTeam(0, places[0]);
            Team2 = new FightTeam(1, places[1]);
        }

        public abstract int StartTime();
        public abstract int TurnTime();

        public abstract void PlayerLeave(Fighter fighter);
        public abstract void FightEnd(FightTeam winners, FightTeam loosers);

        public FightTeam GetTeam(int leaderID)
        {
            return (Team1.Leader.ID == leaderID ? Team1 : Team2);
        }

        public FightTeam GetEnnemyTeam(FightTeam team)
        {
            return (team == Team1 ? Team2 : Team1);
        }

        public FightTeam GetWinners()
        {
            if (!Team1.HasAliveFighters())
                return this.Team2;
            else if (!Team2.HasAliveFighters())
                return this.Team1;

            return null;
        }

        public Fighter GetFighter(int ID)
        {
            return Team1.GetFighters().Concat(Team2.GetFighters()).First(x => x.ID == ID);
        }

        public Fighter GetAliveFighter(int cell)
        {
            return GetAliveFighters().FirstOrDefault(x => x.Cell == cell);
        }

        public Fighter[] GetFighters()
        {
            return Team1.GetFighters().Concat(Team2.GetFighters()).ToArray();
        }

        public Fighter[] GetAliveFighters()
        {
            return Team1.GetAliveFighters().Concat(Team2.GetAliveFighters()).ToArray();
        }

        public bool IsFreeCell(int cell)
        {
            return !GetFighters().Any(x => x.Cell == cell);
        }

        #region Format

        public string FormatFighterShow(Fighter[] fighters)
        {
            var builder = new StringBuilder("GM");

            foreach (Fighter fighter in fighters)
                builder.Append("|+").Append(fighter.GetPattern());

            return builder.ToString();

        }

        public string FormatFighterShow(Fighter fighter)
        {
            return new StringBuilder("GM|+").Append(fighter.GetPattern()).ToString();
        }

        public string FormatFighterDestroy(Fighter fighter)
        {
            return new StringBuilder("GM|-").Append(fighter.ID).ToString();
        }

        public string FormatFlagShow()
        {
            var builder = new StringBuilder("Gc+");

            builder.Append(ID).Append(';').Append((int)Type).Append('|');
            builder.Append(Team1.Leader.ID).Append(';').Append(Team1.Cell).Append(';');
            builder.Append('0').Append(';').Append((Type == FightType.AGRESSION ? Team1.Faction.ToString() : "-1")).Append('|');
            builder.Append(Team2.Leader.ID).Append(';').Append(Team2.Cell).Append(';');
            builder.Append('0').Append(';').Append((Type == FightType.AGRESSION ? Team2.Faction.ToString() : "-1"));

            return builder.ToString();
        }

        public string FormatFlagDestroy()
        {
            return new StringBuilder("Gc-").Append(ID).ToString();
        }

        public string FormatFlagFighter(Fighter[] fighters)
        {
            var builder = new StringBuilder("Gt");
            builder.Append(fighters[0].Team.Leader.ID);

            foreach (var fighter in fighters)
                builder.Append("|+").Append(fighter.ID).Append(";").Append(fighter.Name).Append(";").Append(fighter.Level);

            return builder.ToString();
        }

        public string FormatFlagFighterShow(Fighter fighter)
        {
            return new StringBuilder("Gt").Append(fighter.Team.Leader.ID).Append("|+").Append(fighter.ID).Append(";").Append(fighter.Name).Append(";").Append(fighter.Level).ToString();
        }

        public string FormatFlagFighterDestroy(Fighter fighter)
        {
            return new StringBuilder("Gt").Append(fighter.Team.Leader.ID).Append("|-").Append(fighter.ID).Append(";").Append(fighter.Name).Append(";").Append(fighter.Level).ToString();
        }

        public string FormatFlagOption(ToggleType type, FightTeam team)
        {
            return new StringBuilder("Go").Append(team.IsToggle(type) ? '+' : '-').Append(type == ToggleType.LOCK ? 'A' : (char)type).Append(team.ID).ToString();
        }

        public string FormatJoinInformation()
        {
            return new StringBuilder("GJK2|").Append(Type == FightType.CHALLENGE ? 1 : 0).Append("|1|0|").Append(StartTime()).ToString();
        }

        public string FormatJoinPlaces(int team)
        {
            return new StringBuilder("GP").Append(PlacesToString(Team1.Places)).Append("|").Append(PlacesToString(Team2.Places)).Append("|").Append(team).ToString();
        }

        public string FormatPlace(Fighter fighter)
        {
            return new StringBuilder("GIC|").Append(fighter.ID).Append(";").Append(fighter.Cell).Append(";1").ToString();
        }

        public string FormatFightReady(Fighter fighter)
        {
            return new StringBuilder("GR").Append(fighter.FightReady ? "1" : "0").Append(fighter.ID).ToString();
        }

        public string FormatFightStart()
        {
            return "GS";
        }

        public string FormatTurnList()
        {
            var builder = new StringBuilder("GTL");

            foreach (var fighter in turnFighters)
                builder.Append("|").Append(fighter.ID);

            return builder.ToString();
        }

        public string FormatTurnStart()
        {
            return new StringBuilder("GTS").Append(CurrentFighter.ID).Append("|").Append(TurnTime()).ToString();
        }

        public string FormatTurnEnd()
        {
            return new StringBuilder("GTF").Append(CurrentFighter.ID).ToString();
        }

        public string FormatTurnWait()
        {
            return new StringBuilder("GTR").Append(CurrentFighter.ID).ToString();
        }

        public string FormatTurnStats()
        {
            var builder = new StringBuilder("GTM");

            foreach (var fighter in turnFighters)
            {
                builder.Append('|');
                builder.Append(fighter.ID).Append(';');
                builder.Append(fighter.Dead ? '1' : '0').Append(';');
                builder.Append(fighter.Life).Append(';');
                builder.Append(fighter.Stats.GetStat(StatEnum.MaxPA).Total).Append(';');
                builder.Append(fighter.Stats.GetStat(StatEnum.MaxPM).Total).Append(';');
                builder.Append(fighter.Cell).Append(";;");
                builder.Append(fighter.Stats.GetStat(StatEnum.MaxLife).Total);
            }

            return builder.ToString();
        }

        public string FormatLeave()
        {
            return "GV";
        }

        #endregion

        public void Send(string packet, Fighter except = null)
        {
            foreach (var fighter in GetFighters())
            {
                if (fighter != except & fighter.Type == FighterType.CHARACTER && fighter.Character.State.InFight) 
                    fighter.Character.NClient.Send(packet);
            }

            foreach (var spectator in spectators)
                spectator.NClient.Send(packet);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void TimerLoop(Object obj = null)
        {
            switch (State)
            {
                case FightState.STARTING:
                    if (StartTime() > 0 & startTimeOut < Environment.TickCount) 
                        FightStart(); 
                    break;

                case FightState.PLAYING:
                    if (turnTimeOut < Environment.TickCount) 
                        TurnEnd(); 
                    break;

                case FightState.WAITING:
                    if (waitingTimeOut < Environment.TickCount) 
                        WaitFighters(); 
                    break;
            }
        }

        #region Starting

        #region Join

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void PlayerJoin(Character player, int team)
        {
            FighterJoin(new CharacterFighter(player, this), team);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool CanJoin(Character player, FightTeam team)
        {
            var result = State == FightState.STARTING && team.CanJoin(player, this.Type == FightType.AGRESSION);

            if (result)
            {
                if (this.Type == FightType.AGRESSION && player.Faction.ID != 0)
                    player.Faction.IsEnabled = true;
            }

            return result;
        }

        public void FighterJoin(Fighter fighter, int team, bool isLeader = false)
        {
            switch (fighter.Type)
            {
                case FighterType.CHARACTER:

                    fighter.Character.GetMap().DelPlayer(fighter.Character);
                    fighter.Character.State.InFight = true;

                    switch (team)
                    {
                        case 0: 
                            Team1.FighterJoin(fighter, isLeader);
                            break;

                        case 1:
                            Team2.FighterJoin(fighter, isLeader);
                            break;
                    }

                    Send(FormatFighterShow(fighter), fighter);

                    if (!isLeader)
                        Map.Send(FormatFlagFighterShow(fighter));

                    fighter.Character.NClient.Send(FormatJoinInformation());
                    fighter.Character.NClient.Send(FormatJoinPlaces(team));
                    fighter.Character.NClient.Send(FormatFighterShow(GetFighters()));

                    foreach (var forFighter in GetFighters())
                    {
                        if (forFighter.FightReady) 
                            fighter.Character.NClient.Send(FormatFightReady(forFighter));
                    }

                    break;
            }
        }

        #endregion

        #region Toggle

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Toggle(Fighter fighter, ToggleType toggle)
        {
            if (!fighter.IsLeader)
                return;

            var isToggle = !fighter.Team.IsToggle(toggle);

            fighter.Team.Toggle(toggle, isToggle);

            if (State == FightState.STARTING)
                Map.Send(FormatFlagOption(toggle, fighter.Team));

            switch (toggle)
            {
                case ToggleType.LOCK:
                    if (isToggle)
                        fighter.Team.Send("Im095");
                    else
                        fighter.Team.Send("Im096");
                    break;

                case ToggleType.HELP:
                    if (isToggle)
                        fighter.Team.Send("Im0103");
                    else
                        fighter.Team.Send("Im0104");
                    break;

                case ToggleType.PARTY:
                    if (isToggle)
                    {
                        KickNoParty(fighter.Team);
                        fighter.Team.Send("Im093");
                    }
                    else
                        fighter.Team.Send("Im094");
                    break;

                case ToggleType.SPECTATOR:
                    if (isToggle)
                    {
                        KickSpectator();
                        fighter.Team.Send("Im040");
                    }
                    else
                        fighter.Team.Send("Im039");
                    break;
            }
        }

        private void KickNoParty(FightTeam team)
        {
            if (State != FightState.STARTING)
                return;

            foreach (var fighter in team.GetFighters())
            {
                if (!team.CanJoin(fighter.Character))
                    PlayerLeave(fighter);
            }
        }

        public void KickSpectator(bool force = false)
        {
            if (CanJoinSpectator() & !force)
                return;

            foreach (var spectator in spectators)
                SpectatorLeave(spectator);
        }

        #endregion

        #region Places

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void PlayerPlace(Fighter fighter, int cell)
        {
            if (State != FightState.STARTING)
                return;

            if (fighter.FightReady)
                return;

            if (!fighter.Team.IsAvailablePlace(cell))
                return;

            fighter.Cell = cell;

            Send(FormatPlace(fighter));
        }

        public Dictionary<int, int[]> GeneratePlaces()
        {
            var rand = new Random();
            var cells = Map.RushablesCells.OrderBy(x => rand.Next()).ToList();

            return new Dictionary<int, int[]> { { 0, cells.GetRange(0, 8).ToArray() }, { 1, cells.GetRange(7, 8).ToArray() } };
        }

        public string PlacesToString(int[] places)
        {
            StringBuilder builder = new StringBuilder();

            foreach (int cell in places)
                builder.Append(Pathfinding.GetCellChars(cell));

            return builder.ToString();
        }

        #endregion

        #region Ready

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void PlayerFightReady(Fighter fighter, bool ready)
        {
            if (State != FightState.STARTING)
                return;

            fighter.FightReady = ready;

            Send(FormatFightReady(fighter));

            if (IsAllFightReady())
                FightStart();
        }

        private bool IsAllFightReady()
        {
            return GetFighters().All(fighter => fighter.FightReady);
        }

        #endregion

        public void FightInit(Fighter attacker, Fighter defender)
        {
            FighterJoin(attacker, 0, true);
            FighterJoin(defender, 1, true);

            Map.Send(FormatFlagShow());
            Map.Send(Map.FormatFightCount());
            Map.Send(FormatFlagFighterShow(Team1.Leader));
            Map.Send(FormatFlagFighterShow(Team2.Leader));

            startTimeOut = Environment.TickCount + StartTime();
        }

        private void FightStart()
        {
            var team1 = Team1.GetFighters().OrderByDescending(x => x.Stats.GetStat(StatEnum.Initiative).Total).ToList();
            var team2 = Team2.GetFighters().OrderByDescending(x => x.Stats.GetStat(StatEnum.Initiative).Total).ToList();
            var maxCount = (team1.Count >= team2.Count ? team1.Count : team2.Count);

            for (var i = 0; i <= maxCount - 1; i++)
            {
                var fighterInitiative = -1;
                var oppositeFighterInitiative = -1;

                if (team1.Count - 1 >= i)
                    fighterInitiative = team1[i].Stats.GetStat(StatEnum.Initiative).Total;

                if (team2.Count - 1 >= i)
                    oppositeFighterInitiative = team2[i].Stats.GetStat(StatEnum.Initiative).Total;

                if (fighterInitiative == -1 & oppositeFighterInitiative == -1)
                    break;

                if (fighterInitiative > oppositeFighterInitiative)
                {
                    turnFighters.Add(team1[i]);

                    if (oppositeFighterInitiative != -1)
                        turnFighters.Add(team2[i]);
                }
                else
                {
                    turnFighters.Add(team2[i]);

                    if (fighterInitiative != -1)
                        turnFighters.Add(team1[i]);
                }
            }

            Map.Send(FormatFlagDestroy());
            Map.Send(Map.FormatFightCount());

            Send(FormatFightStart());
            Send(FormatTurnList());

            TurnStart();
        }

        #endregion

        #region Playing

        #region Spectator

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void PlayerJoinSpectator(Character player)
        {
            if (State != FightState.PLAYING)
                return;

            player.GetMap().DelPlayer(player);

            player.State.IsSpectator = true;
            player.Fight = this;

            spectators.Add(player);

            player.NClient.Send(FormatJoinInformation());
            player.NClient.Send(FormatFighterShow(GetFighters()));
            player.NClient.Send(FormatFightStart());
            player.NClient.Send(FormatTurnList());
            player.NClient.Send(FormatTurnStart());

            Send(string.Concat("Im036;", player.Name));
        }

        public bool CanJoinSpectator()
        {
            return State == Fights.FightState.PLAYING && !Team1.IsToggle(ToggleType.SPECTATOR) && !Team2.IsToggle(ToggleType.SPECTATOR);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SpectatorLeave(Character player)
        {
            spectators.Remove(player);

            player.State.IsSpectator = false;
            player.Fight = null;

            player.NClient.Send(FormatLeave());
        }

        #endregion

        #region Turn

        private void TurnStart()
        {
            NextFighter();

            State = FightState.PLAYING;
            turnTimeOut = Environment.TickCount + TurnTime();

            CurrentFighter.Buffs.OnTurnBegin();

            Send(FormatTurnStart());
        }

        private void NextFighter()
        {
            do
            {
                if (CurrentFighter == null || CurrentFighter == turnFighters.LastOrDefault())
                    CurrentFighter = turnFighters[0];
                else
                    CurrentFighter = turnFighters[turnFighters.IndexOf(CurrentFighter) + 1];

            }
            while (CurrentFighter.Dead);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void PlayerTurnPass(Fighter fighter)
        {
            if (State != FightState.PLAYING)
                return;

            if (CurrentFighter != fighter)
                return;

            TurnEnd();
        }

        public void TurnEnd()
        {
            SetAllTurnUnready();

            State = FightState.WAITING;
            waitingTimeOut = Environment.TickCount + 5000;

            CurrentFighter.SpellController.OnTurnEnd();
            CurrentFighter.Buffs.OnTurnEnd();

            CurrentFighter.AP = CurrentFighter.Stats.GetStat(StatEnum.MaxPA).Total;
            CurrentFighter.MP = CurrentFighter.Stats.GetStat(StatEnum.MaxPM).Total;

            Send(FormatTurnStats());
            Send(FormatTurnEnd());
            Send(FormatTurnWait());
        }

        private void SetAllTurnUnready()
        {
            foreach (var fighter in turnFighters)
                fighter.TurnReady = false;
        }

        #endregion

        #region Waiting

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void PlayerTurnReady(Fighter fighter)
        {
            if (State != FightState.WAITING)
                return;

            fighter.TurnReady = true;

            fighter.Character.NClient.Send("BN");

            if (IsAllTurnReady())
                TurnStart();
        }

        private bool IsAllTurnReady()
        {
            return turnFighters.Where(x => !x.Left).All(x => x.TurnReady);
        }

        public void WaitFighters()
        {
            var noReadyFighters = GetFighters().Where(x => !x.TurnReady).ToArray();

            if (noReadyFighters.Length == 1)
                Send(string.Concat("Im128;", noReadyFighters[0].Name));
            else if (noReadyFighters.Length > 1)
            {
                var names = new StringBuilder();

                foreach (var fighter in noReadyFighters)
                    names.Append(fighter.Name + ", ");

                Send(string.Concat("Im129;", names.ToString().Substring(0, names.ToString().Length - 2)));
            }

            TurnStart();
        }

        #endregion

        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool TryMove(Fighter fighter, Pathfinding path)
        {
            if (State != FightState.PLAYING)
                return false;

            if (CurrentFighter != fighter)
                return false;

            int length = path.GetLength();

            if (length > fighter.MP)
                return false;

            fighter.MP -= length;
            Send(string.Format("GA;129;{0};{1},-{2}", fighter.ID, fighter.ID, length));

            fighter.Buffs.OnMoveEnd();

            return true;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void LaunchSpell(Fighter fighter, CharacterSpell spell, int cellID)
        {
            if (State != FightState.PLAYING)
                return;

            Fighter firstTarget = GetAliveFighters().FirstOrDefault(x => x.Cell == cellID);
            int targetID = firstTarget == null ? -1 : firstTarget.ID;

            if (!CanLaunchSpell(fighter, spell, cellID, targetID))
                return;

            fighter.AP -= spell.LevelModel.Cost;

            bool isEchec = false;

            if (spell.LevelModel.EC != 0)
            {
                int echecRate = spell.LevelModel.EC - fighter.Stats.GetStat(StatEnum.EchecCritic).Total;

                if (echecRate < 2)
                    echecRate = 2;

                if (Basic.Rand(1, echecRate) == 1)
                    isEchec = true;
            }

            if (isEchec)
            {
                Send(string.Format("GA;302;{0};{1}", fighter.ID, spell.ID));
            }
            else
            {
                fighter.SpellController.OnLaunchSpell(spell, targetID);

                Send(string.Format("GA;300;{0};{1},{2},{3},{4},{5}", fighter.ID, spell.ID, cellID, spell.Model.Sprite, spell.Level, spell.Model.SpriteInfos));

                var isCritic = false;

                if (spell.LevelModel.CC != 0 && spell.LevelModel.CriticalEffects.Count > 0)
                {
                    var criticRate = spell.LevelModel.CC - fighter.Stats.GetStat(StatEnum.DamageCritic).Total;

                    if (criticRate < 2)
                        criticRate = 2;

                    if (Basic.Rand(1, criticRate) == 1)
                        isCritic = true;
                }

                if (isCritic)
                    Send(string.Format("GA;301;{0};{1}", fighter.ID, spell.ID));

                var effects = isCritic ? spell.LevelModel.CriticalEffects : spell.LevelModel.Effects;
                var targets = new List<Fighter>();

                foreach (var cell in CellZone.GetCells(Map, cellID, fighter.Cell, spell.LevelModel.Type))
                {
                    var target = GetAliveFighters().FirstOrDefault(x => x.Cell == cell);

                    if (target != null)
                        targets.Add(target);
                }

                var actualChance = 0;

                foreach (var effect in effects)
                {
                    if (effect.Chance > 0)
                    {
                        if (Basic.Rand(1, 100) > effect.Chance + actualChance)
                        {
                            actualChance += effect.Chance;
                            continue;
                        }

                        actualChance -= 100;
                    }

                    targets.RemoveAll(x => x.Dead);

                    EffectProcessor.ApplyEffect(new EffectCast
                        ((EffectEnum)effect.ID, spell.ID, cellID, effect.Value, effect.Value2, effect.Value3, effect.Chance, effect.Round, (spell.LevelModel.MinRP == 1 & spell.LevelModel.MaxRP == 1), fighter, effect.Target.RemixTargets(fighter, targets)));
                }
            }

            Send(string.Format("GA;102;{0};{1},-{2}", fighter.ID, fighter.ID, spell.LevelModel.Cost));

            if (GetWinners() != null)
                FightEnd(GetWinners(), GetEnnemyTeam(GetWinners()));
            else if (isEchec & spell.LevelModel.isECEndTurn)
                TurnEnd();
        }

        public bool CanLaunchSpell(Fighter fighter, CharacterSpell spell, int spellCell, long target)
        {
            if (fighter != CurrentFighter)
                return false;

            //if (!myMap.RushablesCells.Contains(spellCell))
            //    return false;

            if (fighter.AP < spell.LevelModel.Cost)
                return false;

            var distance = Pathfinding.GetDistanceBetween(Map, fighter.Cell, spellCell);
            var maxPO = spell.LevelModel.MaxRP + (spell.LevelModel.isAlterablePO ? fighter.Stats.GetStat(StatEnum.PO).Total : 0);

            if (maxPO < spell.LevelModel.MinRP)
                maxPO = spell.LevelModel.MinRP;

            if (distance > maxPO || distance < spell.LevelModel.MinRP)
                return false;

            return fighter.SpellController.CanLaunchSpell(spell, target);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UseWeapon(Fighter fighter, CharacterItem weapon, int cellID)
        {
            if (State != FightState.PLAYING)
                return;

            var firstTarget = GetAliveFighters().FirstOrDefault(x => x.Cell == cellID);
            var targetID = firstTarget == null ? -1 : firstTarget.ID;

            if (!CanUseWeapon(fighter, weapon, cellID, targetID))
                return;

            fighter.AP -= weapon.Model.CostAP;

            var isEchec = false;

            if (weapon.Model.TauxEC != 0)
            {
                var echecRate = weapon.Model.TauxEC - fighter.Stats.GetStat(StatEnum.EchecCritic).Total;

                if (echecRate < 2)
                    echecRate = 2;

                if (Basic.Rand(1, echecRate) == 1)
                    isEchec = true;
            }

            if (isEchec)
            {
                Send(string.Format("GA;305;{0};", fighter.ID));
            }
            else
            {
                Send(string.Format("GA;303;{0};{1}", fighter.ID, cellID));

                var isCritic = false;

                if (weapon.Model.TauxCC != 0)
                {
                    var criticRate = weapon.Model.TauxCC - fighter.Stats.GetStat(StatEnum.DamageCritic).Total;

                    if (criticRate < 2)
                        criticRate = 2;

                    if (Basic.Rand(1, criticRate) == 1)
                        isCritic = true;
                }

                var effects = new List<EffectSpell>();

                foreach (var effect in weapon.Model.EffectsList)
                {
                    if (effect.IsWeaponEffect())
                        effects.Add(effect.CopyToSpellEffect());
                }

                if (isCritic)
                {
                    Send(string.Format("GA;301;{0};0", fighter.ID));

                    foreach (EffectSpell effect in effects)
                    {
                        if (effect.Value2 == -1)
                        {
                            effect.Value += weapon.Model.BonusCC;
                            effect.Effect = string.Concat("0d0+", effect.Value);
                        }
                        else
                        {
                            effect.Value += weapon.Model.BonusCC;
                            effect.Value2 += weapon.Model.BonusCC;
                            effect.Effect = string.Format("1d{0}+{1}", (effect.Value2 - effect.Value + 1), (effect.Value - 1));
                        }
                    }
                }

                var targets = new List<Fighter>();

                foreach (var cell in CellZone.GetCells(Map, cellID, fighter.Cell, weapon.Model.GetCellRange()))
                {
                    var target = GetAliveFighters().FirstOrDefault(x => x.Cell == cell);

                    if (target != null)
                        targets.Add(target);
                }

                foreach (var effect in effects)
                {
                    targets.RemoveAll(x => x.Dead);

                    EffectProcessor.ApplyEffect(new EffectCast
                        ((EffectEnum)effect.ID, 0, cellID, effect.Value, effect.Value2, effect.Value3, effect.Chance, effect.Round, true, fighter, effect.Target.RemixTargets(fighter, targets)));
                }
            }

            Send(string.Format("GA;102;{0};{1},-{2}", fighter.ID, fighter.ID, weapon.Model.CostAP));

            if (GetWinners() != null)
                FightEnd(GetWinners(), GetEnnemyTeam(GetWinners()));
            else if (isEchec)
                TurnEnd();
        }

        public bool CanUseWeapon(Fighter fighter, CharacterItem weapon, int spellCell, long target)
        {
            if (fighter != CurrentFighter)
                return false;

            //if (!myMap.RushablesCells.Contains(spellCell))
            //    return false;

            if (fighter.AP < weapon.Model.CostAP)
                return false;

            var distance = Pathfinding.GetDistanceBetween(Map, fighter.Cell, spellCell);

            if (distance > weapon.Model.MaxPO || distance < weapon.Model.MinPO)
                return false;

            return true;
        }

        #endregion
    }
}
