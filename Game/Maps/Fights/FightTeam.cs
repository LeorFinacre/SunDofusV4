using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.Game.Maps.Fights
{
    public enum ToggleType
    {
        LOCK = 'N',
        HELP = 'H',
        PARTY = 'P',
        SPECTATOR = 'S'
    }

    class FightTeam
    {
        public int ID { get; set; }
        public int Cell { get; set; }
        public int[] Places { get; set; }

        public Fighter Leader;

        private List<Fighter> fighters = new List<Fighter>(8);

        private Dictionary<ToggleType, bool> toggles = new Dictionary<ToggleType, bool>()
        {
            { ToggleType.LOCK, false },
            { ToggleType.HELP, false },
            { ToggleType.PARTY, false },
            { ToggleType.SPECTATOR, false },
        };

        public FightTeam(int nID, int[] places)
        {
            ID = nID;
            Places = places;
        }

        public int Faction
        {
            get
            {
                return Leader.Character.Faction.ID;
            }
        }

        public Fighter[] GetFighters()
        {
            return fighters.ToArray();
        }

        public Fighter[] GetAliveFighters()
        {
            return fighters.Where(x => !x.Dead).ToArray();
        }

        public bool HasAliveFighters()
        {
            return fighters.Any(x => !x.Dead);
        }

        public int GetAvailablePlace()
        {
            return Places.First(x => !fighters.Any(f => f.Cell == x));
        }

        public bool IsAvailablePlace(int cell)
        {
            return !fighters.Any(x => x.Cell == cell);
        }

        public void Send(string packet)
        {
            foreach (var fighter in GetFighters())
            {
                if (fighter.Type == FighterType.CHARACTER) 
                    fighter.Character.NClient.Send(packet);
            }
        }

        public void Toggle(ToggleType toggle, bool value)
        {
            lock (toggles)
                toggles[toggle] = value;
        }

        public bool IsToggle(ToggleType toggle)
        {
            lock (toggles)
                return toggles[toggle];
        }

        public bool CanJoin(Character character, bool isPvpChallenge = false)
        {
            if (isPvpChallenge && Leader != null)
            {
                if (character.Faction.ID != Faction)
                    return false;
            }

            if (fighters.Count >= 8)
                return false;

            if (IsToggle(ToggleType.LOCK))
                return false;

            if (IsToggle(ToggleType.PARTY))
            { 
                if ((character.State.Party != null) && character.State.Party.Members.Keys.Any(x => x.ID == Leader.ID))
                    return true;
                else
                    return false;
            }

            return true;
        }

        public void FighterJoin(Fighter Fighter, bool isLeader = false)
        {
            Fighter.Team = this;
            Fighter.Cell = GetAvailablePlace();

            if (isLeader)
            {
                Leader = Fighter;
                Cell = Fighter.Character.MapCell;
            }

            fighters.Add(Fighter);
        }

        public void FighterLeave(Fighter Fighter)
        {
            fighters.Remove(Fighter);
        }
    }
}
