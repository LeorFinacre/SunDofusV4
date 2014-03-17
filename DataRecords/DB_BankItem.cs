using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyCore;
using SunDofus.World.Game.Effects;

namespace SunDofus.DataRecords
{
    [OTable("banks_items")]
    class DB_BankItem : ORecord
    {
        [OProperty("ID", OProperty.TinyPropertyType.INT, true)]
        public int ID { get; set; }

        [OProperty("bankID", OProperty.TinyPropertyType.INT)]
        public int BankID { get; set; }

        [OProperty("itemID", OProperty.TinyPropertyType.INT)]
        public int ItemID { get; set; }

        [OProperty("quantity", OProperty.TinyPropertyType.INT)]
        public int Quantity { get; set; }

        [OProperty("position", OProperty.TinyPropertyType.INT)]
        public int Position { get; set; }

        [OProperty("effects", OProperty.TinyPropertyType.STRING)]
        public string Effects { get; set; }

        public void ParseEffects()
        {
            var effectsList = Effects.Split(',');

            foreach (var effect in effectsList)
            {
                var NewEffect = new EffectItem();
                string[] EffectInfos = effect.Split('#');

                NewEffect.ID = Convert.ToInt32(EffectInfos[0], 16);

                if (EffectInfos[1] != "")
                    NewEffect.Value = Convert.ToInt32(EffectInfos[1], 16);

                if (EffectInfos[2] != "")
                    NewEffect.Value2 = Convert.ToInt32(EffectInfos[2], 16);

                if (EffectInfos[3] != "")
                    NewEffect.Value3 = Convert.ToInt32(EffectInfos[3], 16);

                NewEffect.Effect = EffectInfos[4];

                lock (item.EffectsList)
                    item.EffectsList.Add(NewEffect);
            }
        }
    }
}
