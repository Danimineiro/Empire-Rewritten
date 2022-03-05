using System.Collections.Generic;
using System.Linq;
using Empire_Rewritten.Facilities;
using Empire_Rewritten.Resources;
using RimWorld.Planet;
using Verse;

namespace Empire_Rewritten.Settlements
{
    /// <summary>
    ///     Manages <see cref="Settlement">Settlements</see> and their <see cref="FacilityManager" />
    /// </summary>
    public class SettlementManager : IExposable, ILoadReferenceable
    {
        private Dictionary<Settlement, FacilityManager> settlements = new Dictionary<Settlement, FacilityManager>();
        private StorageTracker storageTracker = new StorageTracker();

        public StorageTracker StorageTracker => storageTracker;

        public Dictionary<Settlement, FacilityManager> Settlements => settlements;

        public IEnumerable<FacilityManager> AllFacilityManagers => settlements.Values;

        public void ExposeData()
        {
            Scribe_Collections.Look(ref settlements, "settlements", LookMode.Reference, LookMode.Deep);
            Scribe_Deep.Look(ref storageTracker, "storageTracker");
        }

        public string GetUniqueLoadID()
        {
            return $"SettlementTracker_{GetHashCode()}";
        }

        // NOTE: which faction? Does a SettlementManager belong to a faction? Where is that tracked?
        /// <summary>
        ///     Compiles a complete <see cref="Dictionary{TKey,TValue}" /> of all the <see cref="ResourceDef">ResourceDefs</see> a
        ///     faction is producing, as well as their <see cref="ResourceModifier">ResourceModifiers</see>
        /// </summary>
        /// <returns>
        ///     A <see cref="Dictionary{TKey,TValue}" /> mapping any produced <see cref="ResourceDef" /> to its corresponding
        ///     <see cref="ResourceModifier" />
        /// </returns>
        public Dictionary<ResourceDef, ResourceModifier> ResourceModifiersFromAllFacilities()
        {
            Dictionary<ResourceDef, ResourceModifier> resourceModifiers = new Dictionary<ResourceDef, ResourceModifier>();

            foreach (ResourceModifier resourceModifier in settlements.Values.SelectMany(facilityManager => facilityManager.Modifiers))
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

            return resourceModifiers;
        }

        /// <summary>
        ///     Adds a <see cref="Settlement" /> to the tracker.
        /// </summary>
        /// <param name="settlement">The <see cref="Settlement" /> to add</param>
        public void AddSettlement(Settlement settlement)
        {
            FacilityManager tracker = new FacilityManager(settlement);
            settlements.Add(settlement, tracker);
        }

        /// <summary>
        ///     Fetches the <see cref="FacilityManager" /> of a given <see cref="Settlement" />
        /// </summary>
        /// <param name="settlement">The <see cref="Settlement" /> to get the <see cref="FacilityManager" /> for</param>
        /// <returns>
        ///     The <see cref="FacilityManager" /> of <paramref name="settlement" />; <c>null</c> if it is not being tracked
        ///     by this <see cref="SettlementManager" />
        /// </returns>
        public FacilityManager GetFacilityManager(Settlement settlement)
        {
            if (settlements.ContainsKey(settlement))
            {
                return settlements[settlement];
            }

            Log.Warning($"[Empire]: {settlement.Name} was not in the settlement manager! Returning null.");
            return null;
        }
    }
}