using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Empire_Rewritten
{
    public class FactionController : IExposable
    {
        public FactionController(List<FactionSettlementData> factionSettlementDataList)
        {
            this.factionSettlementDataList = factionSettlementDataList;
        }

        public FactionController()
        {

        }

        private List<FactionSettlementData> factionSettlementDataList = new List<FactionSettlementData>();
        
        /// <summary>
        /// Saves Data
        /// </summary>
        public void ExposeData()
        {
            Scribe_Collections.Look(ref factionSettlementDataList, "FactionSettlementDataList");
        }
    }
}
