using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
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
        private Faction faction;

        /// <summary>
        /// Compiles a complete dictionary of all the resources a faction is producing and their modifiers.
        /// </summary>
        /// <returns></returns>
        public Dictionary<ResourceDef, ResourceModifier> ResourceModifiersFromAllFacilities()
        {
            Dictionary<ResourceDef, ResourceModifier> resourceModifiers = new Dictionary<ResourceDef, ResourceModifier>();
            List<FacilityManager> facilities = settlements.Values.ToList();
            foreach (FacilityManager facilityManager in facilities)
            {
                List<ResourceModifier> facilityMods = facilityManager.modifiers;
                foreach (ResourceModifier resourceModifier in facilityMods)
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

        public SettlementManager(Faction faction)
        {
            this.faction = faction;
        }

        public Dictionary<Settlement, FacilityManager> Settlements
        {
            get { return settlements; }
        }

        /// <summary>
        /// Place a settlement on a <paramref name="tile"/>.
        /// </summary>
        /// <param name="tile"></param>
        /// <returns>If the settlement was built.</returns>
        public bool BuildNewSettlementOnTile(Tile tile)
        {
            if (tile != null)
            {
                int tileID = Find.WorldGrid.tiles.IndexOf(tile);
                if (TileFinder.IsValidTileForNewSettlement(tileID))
                {
                    Log.Message($"Tile is null: {tile == null}");
                    Log.Message($"Faction is null: {faction == null}");
                    Settlement settlement = (Settlement)WorldObjectMaker.MakeWorldObject(WorldObjectDefOf.Settlement);
                    settlement.Tile = tileID;
                    settlement.SetFaction(this.faction);

                    List<string> used = new List<string>();
                    List<Settlement> settlements = Find.WorldObjects.Settlements;
                    foreach (Settlement found in settlements)
                    {
                        used.Add(found.Name);
                    }

                    settlement.Name = NameGenerator.GenerateName(faction.def.factionNameMaker, used, true);
                    Find.WorldObjects.Add(settlement);
                    AddSettlement(settlement);
                    return true;
                }
            }
            return false;
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

        private List<Settlement> settlementsForLoading = new List<Settlement>();
        private List<FacilityManager> facilityManagersForLoading = new List<FacilityManager>();
        public void ExposeData()
        {
            Scribe_Collections.Look<Settlement,FacilityManager>(ref settlements, "settlements", LookMode.Reference, LookMode.Deep,keysWorkingList:ref settlementsForLoading,valuesWorkingList:ref facilityManagersForLoading);
            Scribe_References.Look(ref faction, "faction");
            Scribe_Deep.Look(ref storageTracker, "storageTracker");

        }

        public Settlement GetSettlement(FacilityManager manager)
        {
            foreach(Settlement settlement in settlements.Keys)
            {
                if (settlements[settlement] == manager)
                {
                    return settlement;
                }
            }
            return null;
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
