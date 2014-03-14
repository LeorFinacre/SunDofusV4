using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Entities.Models.Items
{
    class ItemModel
    {
        public int ID { get; set; }
        public int Type { get; set; }
        public int Level { get; set; }
        public int Pods { get; set; }
        public int Price { get; set; }
        public int Set { get; set; }

        public bool isUsable { get; set; }
        public bool isTwoHands { get; set; }

        public int CostAP { get; set; }
        public int MinPO { get; set; }
        public int MaxPO { get; set; }
        public int BonusCC { get; set; }
        public int TauxCC { get; set; }
        public int TauxEC { get; set; }

        public string Jet { get; set; }
        public string Condistr { get; set; }

        public List<Game.Effects.EffectItem> EffectsList { get; set; }

        public ItemModel()
        {
            Price = 0;
            Set = -1;
            Jet = "";
            isTwoHands = false;
            isUsable = false;

            EffectsList = new List<Game.Effects.EffectItem>();
        }

        public void ParseWeaponInfos(string datas)
        { 
            if (!datas.Contains(','))
                return;

            string[] splitter = datas.Split(',');

            BonusCC = Convert.ToInt32(splitter[0]);
            CostAP = Convert.ToInt32(splitter[1]);
            MinPO = Convert.ToInt32(splitter[2]);
            MaxPO = Convert.ToInt32(splitter[3]);
            TauxCC = Convert.ToInt32(splitter[4]);
            TauxEC = Convert.ToInt32(splitter[5]);
        }

        public void ParseRandomJet()
        {
            if (EffectsList.Count != 0)
                return;

            var jet = Jet;

            foreach (var _jet in jet.Split(','))
            {
                if (_jet == "") continue;
                var infos = _jet.Split('#');

                var myEffect = new Game.Effects.EffectItem();
                myEffect.ID = Utilities.Basic.HexToDeci(infos[0]);

                if (infos.Length > 1) myEffect.Value = Utilities.Basic.HexToDeci(infos[1]);
                if (infos.Length > 2) myEffect.Value2 = Utilities.Basic.HexToDeci(infos[2]);
                if (infos.Length > 3) myEffect.Value3 = Utilities.Basic.HexToDeci(infos[3]);
                if (infos.Length > 4) myEffect.Effect = infos[4];

                lock(EffectsList)
                    EffectsList.Add(myEffect);
            }
        }

        public string EffectInfos()
        {
            return string.Join(",", EffectsList);
        }
        
        public string GetCellRange()
        {
            switch (Type)
            {
                case 4:
                    return "Tb";
                case 7:
                    return "Xb";
                case 102:
                    return "Lc";
            }

            return "Pa";
        }
    }
}
