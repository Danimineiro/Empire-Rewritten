using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld.Planet;
using Verse;

namespace Empire_Rewritten
{
    /// <summary>
    /// Manages settlements and their FacilitiyManager
    /// </summary>
    public class SettlementManager : IExposable, ILoadReferenceable
    {

        private Dictionary<Settlement, FacilityManager> settlements = new Dictionary<Settlement, FacilityManager>();


        public Dictionary<Settlement,FacilityManager> Settlements
        {
            get { return settlements; }
        }

        public void AddSettlement(Settlement settlement)
        {
            FacilityManager tracker = new FacilityManager(settlement);
            settlements.Add(settlement, tracker);
        }

        public void ExposeData()
        {
            Scribe_Collections.Look(ref settlements, "settlements", LookMode.Undefined, LookMode.Deep);

        }

        public FacilityManager GetFacilityTracker(Settlement settlement)
        {
            if (settlements.ContainsKey(settlement))
            {
                return settlements[settlement];
            }
            Log.Warning($"[Empire]: {settlement.Name} was not in the settlement manager! Returning null.");
            return null;
        }

        public string GetUniqueLoadID()
        {
            return $"SettlementTracker_{GetHashCode()}";
        }
    }
}
