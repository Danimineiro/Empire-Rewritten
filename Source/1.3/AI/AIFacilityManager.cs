using Empire_Rewritten.Resources;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Empire_Rewritten.AI
{
    public class AIFacilityManager : AIModule
    {

        public AIFacilityManager(AIPlayer player) : base(player)
        {
          
        }

        private List<FacilityManager> cachedFacilities;
        HashSet<FacilityDef> cachedFacilitiesDef;
        private bool updateCache = true;
        private bool updateDefCache = true;

        private bool canMakeFacilities = false;

        public bool CanMakeFacilities
        {
            get
            {
                return canMakeFacilities;
            }
        }

        public List<FacilityManager> Facilities
        {
            get
            {
                if (cachedFacilities.NullOrEmpty() ||updateCache)
                {
                    updateCache= false;
                    cachedFacilities= player.Manager.AllFacilityManagers.ToList();
                    updateDefCache = true;
                }
                return cachedFacilities;
            }
        }

        public HashSet<FacilityDef> FacilityDefsInstalled
        {
            get
            {
                if(cachedFacilitiesDef.EnumerableNullOrEmpty() || updateDefCache)
                {
                    updateDefCache= false;
                    foreach(FacilityManager manager in Facilities)
                    {
                        cachedFacilitiesDef.AddRange(manager.FacilityDefsInsalled);
                    }
                }
                return cachedFacilitiesDef;
            }
        }

        /// <summary>
        /// Find a low resource and attempt to build a facility for it.
        /// </summary>
        /// <returns></returns>
        public bool BuildResourceFacility()
        {
            List<ResourceDef> resourceDefs = player.ResourceManager.FindLowResources();
            ResourceDef def = resourceDefs.RandomElement();

            if (FacilityDefsInstalled.Any(x=>x.ProducedResources.Contains(def)))
            {
                FacilityDef facilityDef = FacilityDefsInstalled.Where(x=>x.ProducedResources.Contains(def)).RandomElement();
                return BuildNewFacility(facilityDef);
            }
            return false;
        }


        /// <summary>
        /// Attempt to build a facility
        /// </summary>
        /// <param name="facility"></param>
        /// <returns></returns>
        public bool BuildNewFacility(FacilityDef facilityDef)
        {
            bool hasRemovedAll = true;
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
                        storageTracker.RemoveThingsFromStorage(thingDefCountClass.thingDef, thingDefCountClass.count);
                    }
                    manager.AddFacility(facilityDef);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// If we have an excess of resources, the AI will uninstall potential facilities to allocate space for new ones.
        /// </summary>
        /// <returns></returns>
        public bool UninstallResourceProducingFacility()
        {
            List<ResourceDef> resourceDefs = player.ResourceManager.FindExcessResources();

            if (!resourceDefs.NullOrEmpty())
            {
                ResourceDef resourceDef = resourceDefs.RandomElement();

                if (FacilityDefsInstalled.Any(x => x.ProducedResources.Contains(resourceDef)))
                {
                    FacilityDef facilityDef = FacilityDefsInstalled.Where(x => x.ProducedResources.Contains(resourceDef)).RandomElement();
                    return RemoveFacility(facilityDef);
                }
            }
            return false;
        }


        public bool RemoveFacility(FacilityDef facilityDef)
        {
            KeyValuePair<Settlement, FacilityManager> settlementAndManager = player.Manager.Settlements.First(x => x.Value.HasFacility(facilityDef));
            Settlement settlement = settlementAndManager.Key;
            FacilityManager facilityManager = settlementAndManager.Value;

            if(settlement!=null&& facilityManager != null)
            {
                facilityManager.RemoveFacility(facilityDef);
            }
            return false;
        }

        public override void DoModuleAction()
        {
            //Some basic facility action
            bool builtSomething = BuildResourceFacility();
            bool uninstalledFacility =  UninstallResourceProducingFacility();
            canMakeFacilities = !builtSomething && player.ResourceManager.HasCriticalResource && !uninstalledFacility; 
        }
    }
}
