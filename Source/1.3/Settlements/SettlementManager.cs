using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Empire_Rewritten.Resources;
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
        private StorageTracker storageTracker = new StorageTracker();

        /// <summary>
        /// Compiles a complete dictionary of all the resources a faction is producing and their modifiers.
        /// </summary>
        /// <returns></returns>
        public Dictionary<ResourceDef,ResourceModifier> ResourceModifiersFromAllFacilities()
        {
            Dictionary<ResourceDef,ResourceModifier> resourceModifiers = new Dictionary<ResourceDef, ResourceModifier>();
            List<FacilityManager> facilities = settlements.Values.ToList();
            foreach(FacilityManager facilityManager in facilities)
            {
                List<ResourceModifier> facilityMods = facilityManager.modifiers;
                foreach(ResourceModifier resourceModifier in facilityMods)
                {
                    if (resourceModifiers.ContainsKey(resourceModifier.def))
                    {
                        ResourceModifier newModifier = resourceModifiers[resourceModifier.def].MergeWithModifier(resourceModifier);
                        resourceModifiers[resourceModifier.def] = newModifier;
                    }
                    else
                    {
                        resourceModifiers.Add(resourceModifier.def, resourceModifier);
                    }
                }
            }
            return resourceModifiers;
        }

        public StorageTracker StorageTracker
        {
            get
            {
                return storageTracker;
            }
        }
        public SettlementManager()
        {

        }

        public Dictionary<Settlement,FacilityManager> Settlements
        {
            get { return settlements; }
        }

        /// <summary>
        /// Add a settlement to the tracker.
        /// </summary>
        /// <param name="settlement"></param>
        public void AddSettlement(Settlement settlement)
        {
            FacilityManager tracker = new FacilityManager(settlement);
            settlements.Add(settlement, tracker);
        }

        public void ExposeData()
        {
            Scribe_Collections.Look(ref settlements, "settlements", LookMode.Reference, LookMode.Deep);
            Scribe_Deep.Look(ref storageTracker, "storageTracker");

        }

        public FacilityManager GetFacilityManager(Settlement settlement)
        {
            if (settlements.ContainsKey(settlement))
            {
                return settlements[settlement];
            }
            Log.Warning($"[Empire]: {settlement.Name} was not in the settlement manager! Returning null.");
            return null;
        }

        public IEnumerable<FacilityManager> GetAllFacilityManagers()
        {
            return settlements.Values;
        }
        public string GetUniqueLoadID()
        {
            return $"SettlementTracker_{GetHashCode()}";
        }
    }
}
