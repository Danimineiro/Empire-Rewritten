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
        /// <summary>
        /// Needed for loading
        /// </summary>
        public FactionController() { }

        /// <summary>
        /// Creates a new FactionController using a List of <c>FactionSettlementData</c> structs
        /// </summary>
        /// <param name="factionSettlementDataList"></param>
        public FactionController(List<FactionSettlementData> factionSettlementDataList)
        {
            this.factionSettlementDataList = factionSettlementDataList;
        }


        private List<FactionSettlementData> factionSettlementDataList = new List<FactionSettlementData>();

        /// <param name="faction"></param>
        /// <returns>The <c>SettlementManager</c>s owned by a given <paramref name="faction"/></returns>
        public IEnumerable<SettlementManager> GetOwnedSettlementManagers(Faction faction)
        {
            foreach (FactionSettlementData factionSettlementData in factionSettlementDataList)
            {
                if (factionSettlementData.owner == faction) yield return factionSettlementData.SettlementManager;
            }

            yield break;
        }

        public void ExposeData()
        {
            Scribe_Collections.Look(ref factionSettlementDataList, "FactionSettlementDataList");
        }
    }
}
