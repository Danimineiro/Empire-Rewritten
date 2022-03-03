using System.Collections.Generic;
using System.Linq;
using Empire_Rewritten.Facilities;
using Empire_Rewritten.Resources;
using JetBrains.Annotations;
using RimWorld.Planet;
using Verse;

// TODO: Document

namespace Empire_Rewritten.AI
{
    public class AIFacilityManager : AIModule
    {
        private List<FacilityManager> cachedFacilities;
        private HashSet<FacilityDef> cachedFacilityDefs;

        private bool updateCache = true;
        private bool updateDefCache = true;

        public AIFacilityManager(AIPlayer player) : base(player)
        {
        }

        public bool CanMakeFacilities { get; private set; }

        public List<FacilityManager> Facilities
        {
            get
            {
                if (cachedFacilities.NullOrEmpty() || updateCache)
                {
                    updateCache = false;
                    cachedFacilities = player.Manager.AllFacilityManagers.ToList();
                    updateDefCache = true;
                }

                return cachedFacilities;
            }
        }

        public HashSet<FacilityDef> FacilityDefsInstalled
        {
            get
            {
                if (cachedFacilityDefs.EnumerableNullOrEmpty() || updateDefCache)
                {
                    updateDefCache = false;
                    foreach (FacilityManager manager in Facilities)
                    {
                        cachedFacilityDefs.AddRange(manager.FacilityDefsInstalled);
                    }
                }

                return cachedFacilityDefs;
            }
        }

        /// <summary>
        ///     Find a low resource and attempt to build a facility for it.
        /// </summary>
        /// <returns>Whether a <see cref="Facility" /> was successfully built</returns>
        public bool BuildResourceFacility()
        {
            List<ResourceDef> resourceDefs = player.ResourceManager.FindLowResources();
            ResourceDef def = resourceDefs.RandomElement();

            FacilityDef facilityDef = FacilityDefsInstalled.Where(facility => facility.ProducedResources.Contains(def)).RandomElementWithFallback();

            return facilityDef != null && BuildNewFacility(facilityDef);
        }


        /// <summary>
        ///     Attempt to build a facility
        /// </summary>
        /// <param name="facilityDef"></param>
        /// <returns></returns>
        public bool BuildNewFacility(FacilityDef facilityDef)
        {
            var hasRemovedAll = true;
            KeyValuePair<Settlement, FacilityManager> settlementAndManager = player.Manager.Settlements.First(x => x.Value.CanBuildAt(facilityDef));

            FacilityManager manager = settlementAndManager.Value;
            Settlement settlement = settlementAndManager.Key;

            if (settlement != null && manager != null)
            {
                StorageTracker storageTracker = player.Manager.StorageTracker;
                foreach (ThingDefCountClass thingDefCount in facilityDef.costList)
                {
                    hasRemovedAll = hasRemovedAll && storageTracker.CanRemoveThingsFromStorage(thingDefCount.thingDef, thingDefCount.count);
                }

                if (hasRemovedAll)
                {
                    foreach (ThingDefCountClass thingDefCountClass in facilityDef.costList)
                    {
                        storageTracker.TryRemoveThingsFromStorage(thingDefCountClass.thingDef, thingDefCountClass.count);
                    }

                    manager.AddFacility(facilityDef);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     If we have an excess of resources, the AI will uninstall potential facilities to allocate space for new ones.
        /// </summary>
        /// <returns></returns>
        public bool UninstallResourceProducingFacility()
        {
            ResourceDef resourceDef = player.ResourceManager.FindExcessResources().RandomElementWithFallback();

            if (resourceDef == null) return false;

            FacilityDef facilityDef = FacilityDefsInstalled.Where(x => x.ProducedResources.Contains(resourceDef)).RandomElementWithFallback();

            return facilityDef != null && RemoveFacility(facilityDef);
        }


        public bool RemoveFacility([NotNull] FacilityDef facilityDef)
        {
            (Settlement settlement, FacilityManager facilityManager) = player.Manager.Settlements.First(x => x.Value.HasFacility(facilityDef));

            if (settlement == null || facilityManager == null) return false;

            facilityManager.RemoveFacility(facilityDef);
            return true;
        }

        public override void DoModuleAction()
        {
            //Some basic facility action
            bool builtSomething = BuildResourceFacility();
            bool uninstalledFacility = UninstallResourceProducingFacility();
            CanMakeFacilities = !builtSomething && player.ResourceManager.HasCriticalResource && !uninstalledFacility;
        }
    }
}