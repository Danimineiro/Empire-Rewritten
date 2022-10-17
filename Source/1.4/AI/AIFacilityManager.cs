using System;
using System.Collections.Generic;
using System.Linq;
using Empire_Rewritten.Facilities;
using Empire_Rewritten.Resources;
using RimWorld.Planet;
using Verse;

namespace Empire_Rewritten.AI
{
    public class AIFacilityManager : AIModule
    {
        private readonly HashSet<FacilityDef> cachedFacilitiesDef = new HashSet<FacilityDef>();
        private List<FacilityManager> cachedFacilities;

        private bool updateCache = true;
        private bool updateDefCache = true;

        public AIFacilityManager(AIPlayer player) : base(player) { }

        public bool CanMakeFacilities { get; private set; }

        public List<FacilityManager> Facilities
        {
            get
            {
                if (cachedFacilities.NullOrEmpty() || updateCache)
                {
                    updateCache = false;
                    cachedFacilities = player.Empire.AllFacilityManagers.ToList();
                    updateDefCache = true;
                }

                return cachedFacilities;
            }
        }

        public HashSet<FacilityDef> FacilityDefsInstalled
        {
            get
            {
                if (cachedFacilitiesDef.EnumerableNullOrEmpty() || updateDefCache)
                {
                    updateDefCache = false;
                    foreach (FacilityManager manager in Facilities)
                    {
                        cachedFacilitiesDef.AddRange(manager.FacilityDefsInstalled);
                    }
                }

                return cachedFacilitiesDef;
            }
        }

        /// <summary>
        ///     Select a facility to build, based on what the AI needs and what is produced on the tile.
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        public FacilityDef SelectFacilityToBuild(FacilityManager manager)
        {
            Dictionary<float, List<FacilityDef>> facilityWeights = new Dictionary<float, List<FacilityDef>>();
            List<FacilityDef> defs = DefDatabase<FacilityDef>.AllDefsListForReading;
            List<Tile> tiles = Find.WorldGrid.tiles;
            foreach (FacilityDef facilityDef in defs)
            {
                float weight = 0;
                weight += manager.FacilityDefsInstalled.Contains(facilityDef) ? 0.5f : -0.5f;
                weight += player.ResourceManager.GetTileResourceWeight(tiles[player.Empire.GetSettlement(manager).Tile]);

                if (facilityWeights.ContainsKey(weight))
                {
                    facilityWeights[weight].Add(facilityDef);
                }
                else
                {
                    facilityWeights.Add(weight, new List<FacilityDef> {facilityDef});
                }
            }

            float key = facilityWeights.Keys.Max();
            return facilityWeights[key].RandomElement();
        }

        /// <summary>
        ///     Checks that the AI has the resources to build the <paramref name="facilityDef" />.
        /// </summary>
        /// <param name="facilityDef"></param>
        /// <returns></returns>
        public bool CanBuildFacility(FacilityDef facilityDef)
        {
            bool allResourcesPullable = true;
            foreach (ThingDefCountClass thingDefCountClass in facilityDef.costList)
            {
                allResourcesPullable = allResourcesPullable && player.Empire.StorageTracker.CanRemoveThingsFromStorage(thingDefCountClass.thingDef, thingDefCountClass.count);
            }

            return allResourcesPullable;
        }

        /// <summary>
        ///     Builds a facility in a settlement.
        /// </summary>
        /// <returns></returns>
        public bool BuildFacility(FacilityManager manager)
        {
            if (manager != null)
            {
                FacilityDef def = SelectFacilityToBuild(manager);
                if (def.FacilityWorker is FacilityWorker worker && worker.CanBuildAt(manager) && CanBuildFacility(def))
                {
                    foreach (ThingDefCountClass thingDefCountClass in def.costList)
                    {
                        player.Empire.StorageTracker.TryRemoveThingsFromStorage(thingDefCountClass.thingDef, thingDefCountClass.count);
                    }

                    manager.AddFacility(def);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     Find a manager the AI can build on.
        /// </summary>
        /// <returns></returns>
        public FacilityManager FindManagerToBuildOn()
        {
            List<ResourceDef> resourceDefs = player.ResourceManager.LowResources;
            IEnumerable<FacilityManager> managers = player.Empire.AllFacilityManagers.Where(x => x.CanBuildNewFacilities);
            List<FacilityManager> potentialResults = new List<FacilityManager>();
            foreach (FacilityManager facilityManager in managers)
            {
                IEnumerable<FacilityDef> facilityDefs = facilityManager.FacilityDefsInstalled.Where(x => x.ProducedResources.Any(y => resourceDefs.Contains(y)));
                if (facilityDefs.Any())
                {
                    potentialResults.Add(facilityManager);
                }
            }

            if (potentialResults.Any())
            {
                return potentialResults.RandomElement();
            }

            CanMakeFacilities = false;
            return null;
        }

        /// <summary>
        ///     If we have an excess of resources, the AI will uninstall potential facilities to allocate space for new ones.
        /// </summary>
        /// <returns></returns>
        public bool UninstallResourceProducingFacility()
        {
            List<ResourceDef> resourceDefs = player.ResourceManager.ExcessResources;

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
            KeyValuePair<Settlement, FacilityManager> settlementAndManager = player.Empire.Settlements.First(x => x.Value.HasFacility(facilityDef));
            Settlement settlement = settlementAndManager.Key;
            FacilityManager facilityManager = settlementAndManager.Value;

            if (settlement != null && facilityManager != null)
            {
                facilityManager.RemoveFacility(facilityDef);
            }

            return false;
        }

        public override void DoModuleAction()
        {
            //Some basic facility action
            FacilityManager manager = FindManagerToBuildOn();
            if (manager != null)
            {
                bool builtSomething = BuildFacility(manager);
                bool uninstalledFacility = !builtSomething && UninstallResourceProducingFacility();
                CanMakeFacilities = !builtSomething && player.ResourceManager.HasCriticalResource && !uninstalledFacility;
            }
        }

        public override void DoThreadableAction()
        {
            throw new NotImplementedException();
        }
    }
}