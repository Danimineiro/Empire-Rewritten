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
                    cachedFacilities= (List<FacilityManager>)player.Manager.GetAllFacilityManagers();
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
        /// Select a facility to build, based on what the AI needs and what is produced on the tile.
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        public FacilityDef SelectFacilityToBuild(FacilityManager manager)
        {
            Dictionary<float, List<FacilityDef>> facilityWeights = new Dictionary<float, List<FacilityDef>>();
            List<FacilityDef> defs = DefDatabase<FacilityDef>.AllDefsListForReading;
            List<Tile> tiles = Find.WorldGrid.tiles;
            foreach(FacilityDef facilityDef in defs)
            {
                float weight = 0;
                weight += manager.FacilityDefsInsalled.Contains(facilityDef) ? 0.5f : -0.5f;
                weight += player.ResourceManager.GetTileResourceWeight(tiles[player.Manager.GetSettlement(manager).Tile]);

                if (facilityWeights.ContainsKey(weight))
                {
                    facilityWeights[weight].Add(facilityDef);
                }
                else
                {
                    facilityWeights.Add(weight, new List<FacilityDef>() { facilityDef});
                }
            }

            float key = facilityWeights.Keys.Max();
            return facilityWeights[key].RandomElement();
        }

        /// <summary>
        /// Checks that the AI has the resources to build the facility.
        /// </summary>
        /// <param name="facilityDef"></param>
        /// <returns></returns>
        public bool CanBuildFacility(FacilityDef facilityDef)
        {
            bool allResourcesPullable = true;
            foreach(ThingDefCountClass thingDefCountClass in facilityDef.costList)
            {
                allResourcesPullable = allResourcesPullable && player.Manager.StorageTracker.CanRemoveThingsFromStorage(thingDefCountClass.thingDef, thingDefCountClass.count);
            }
            return allResourcesPullable;
        }
     
        /// <summary>
        /// Builds a facility in a settlement.
        /// </summary>
        /// <returns></returns>
        public bool BuildFacility(FacilityManager manager)
        {
            if (manager != null)
            {
                FacilityDef def = SelectFacilityToBuild(manager);
                if (manager.CanBuildAt(def) && CanBuildFacility(def))
                {
                    foreach (ThingDefCountClass thingDefCountClass in def.costList)
                    {
                        player.Manager.StorageTracker.RemoveThingsFromStorage(thingDefCountClass.thingDef, thingDefCountClass.count);
                    }
                    manager.AddFacility(def);
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// Find a manager the AI can build on.
        /// </summary>
        /// <returns></returns>
        public FacilityManager FindManagerToBuildOn()
        {
            List<ResourceDef> resourceDefs = player.ResourceManager.FindLowResources();
            List<FacilityManager> managers = (List<FacilityManager>)player.Manager.GetAllFacilityManagers();
            List<FacilityManager> potentialResults = new List<FacilityManager>();
            foreach (ResourceDef resourceDef in resourceDefs)
            {
                foreach (FacilityManager facilityManager in managers)
                {
                    IEnumerable<FacilityDef> facilityDefs = facilityManager.FacilityDefsInsalled.Where(x => x.ProducedResources.Any(y => resourceDefs.Contains(y)));
                    if (facilityDefs.Count() > 0)
                    {
                        potentialResults.Add(facilityManager);
                    }
                }
            }
            return potentialResults.RandomElement();
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
            bool builtSomething = BuildFacility(FindManagerToBuildOn());
            bool uninstalledFacility =  builtSomething ? false : UninstallResourceProducingFacility();
            canMakeFacilities = !builtSomething && player.ResourceManager.HasCriticalResource && !uninstalledFacility; 
        }
    }
}
