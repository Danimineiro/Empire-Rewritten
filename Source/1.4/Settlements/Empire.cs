using System;
using System.Collections.Generic;
using System.Linq;
using Empire_Rewritten.Controllers;
using Empire_Rewritten.Facilities;
using Empire_Rewritten.Resources;
using Empire_Rewritten.Territories;
using Empire_Rewritten.Utils;
using JetBrains.Annotations;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace Empire_Rewritten.Settlements
{
    /// <summary>
    ///     Manages settlements, and storage.
    /// </summary>
    public class Empire : IExposable, ILoadReferenceable
    {
        public static readonly Dictionary<ThingDef, int> SettlementCost = new Dictionary<ThingDef, int>
        {
            { ThingDefOf.WoodLog, 500 }, { ThingDefOf.Steel, 100 }, { ThingDefOf.ComponentIndustrial, 12 }, { ThingDefOf.Silver, 200 },
        };

        private bool isAIPlayer;
        private bool territoryIsDirty;

        private Dictionary<Settlement, FacilityManager> settlements = new Dictionary<Settlement, FacilityManager>();
        private Faction faction;
        private List<FacilityManager> facilityManagersForLoading = new List<FacilityManager>();

        private List<Settlement> settlementsForLoading = new List<Settlement>();
        private StorageTracker storageTracker = new StorageTracker();
        private Territory cachedTerritory;

        [UsedImplicitly]
        public Empire() { }


        public Empire([NotNull] Faction faction, bool isAIPlayer = true)
        {
            this.faction = faction ?? throw new ArgumentNullException(nameof(faction));
            this.isAIPlayer = isAIPlayer;
        }

        public bool IsAIPlayer => isAIPlayer;
        public Faction Faction => faction;

        public static Empire PlayerEmpire => UpdateController.CurrentWorldInstance.FactionController.ReadOnlyFactionSettlementData.Find(x => !x.SettlementManager.IsAIPlayer).SettlementManager;

        public StorageTracker StorageTracker => storageTracker;
        public Dictionary<Settlement, FacilityManager> Settlements => settlements;
        public List<int> SettlementTiles { get; } = new List<int>();

        private Territory Territory
        {
            get
            {
                if (cachedTerritory == null || territoryIsDirty)
                {
                    territoryIsDirty = false;
                    cachedTerritory = TerritoryManager.GetTerritoryManager.GetTerritory(faction);
                }

                return cachedTerritory;
            }
        }

        public IEnumerable<FacilityManager> AllFacilityManagers => settlements.Values;

        public void ExposeData()
        {
            Scribe_Collections.Look(ref settlements, "settlements", LookMode.Reference, LookMode.Deep, ref settlementsForLoading, ref facilityManagersForLoading);
            Scribe_References.Look(ref faction, "faction");
            Scribe_Deep.Look(ref storageTracker, "storageTracker");
            Scribe_Values.Look(ref isAIPlayer, nameof(isAIPlayer));
        }

        public string GetUniqueLoadID()
        {
            return $"{nameof(Empire)}_{GetHashCode()}";
        }

        public void SetTerritoryDirty()
        {
            territoryIsDirty = true;
        }

        /// <summary>
        ///     Compiles a complete dictionary of all the resources a faction is producing and their modifiers.
        /// </summary>
        /// <returns></returns>
        public Dictionary<ResourceDef, ResourceModifier> ResourceModifiersFromAllFacilities()
        {
            Dictionary<ResourceDef, ResourceModifier> resourceModifiers = new Dictionary<ResourceDef, ResourceModifier>();
            List<FacilityManager> facilities = settlements.Values.ToList();
            foreach (FacilityManager facilityManager in facilities)
            {
                foreach (ResourceModifier resourceModifier in facilityManager.Modifiers)
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

        /// <summary>
        ///     Place a settlement on a <paramref name="tile" />.
        /// </summary>
        /// <param name="tile"></param>
        /// <returns>If the settlement was built.</returns>
        public bool BuildNewSettlementOnTile(Tile tile)
        {
            if (tile != null)
            {
                int tileId = Find.WorldGrid.tiles.IndexOf(tile);
                if (TileFinder.IsValidTileForNewSettlement(tileId))
                {
                    Settlement settlement = (Settlement)WorldObjectMaker.MakeWorldObject(WorldObjectDefOf.Settlement);
                    settlement.Tile = tileId;
                    settlement.SetFaction(faction);

                    List<string> used = new List<string>();
                    List<Settlement> worldSettlements = Find.WorldObjects.Settlements;
                    foreach (Settlement found in worldSettlements)
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
        ///     Checks if it can remove the given <paramref name="settlement"/> of Type <see cref="Settlement"/>, and removes it if able
        /// </summary>
        /// <param name="settlement"></param>
        /// <returns>True, if it could successfully remove the settlement, and false otherwise</returns>
        public bool RemoveSettlement(Settlement settlement)
        {
            if (settlement is null)
            {
                $"Tried to remove a null settlement from an Empire!".ErrorOnce();
                return false;
            }

            if (!settlements.ContainsKey(settlement))
            {
                $"Tried to remove Settlement of name: {settlement.LabelCap} from the Empire belonging to Faction with name: {faction.Name}, but Settlement does not belong to the Empire!".ErrorOnce();
                return false;
            }

            settlements.Remove(settlement);
            return true;
        }

        /// <summary>
        ///     Add a <see cref="Settlement" /> to the <see cref="Empire" />.
        /// </summary>
        /// <param name="settlement">The <see cref="Settlement" /> to add</param>
        public void AddSettlement(Settlement settlement)
        {
            FacilityManager tracker = new FacilityManager(settlement);
            settlements.Add(settlement, tracker);
            Territory.SettlementClaimTiles(settlement);
            SettlementTiles.Add(settlement.Tile);
        }

        /// <summary>
        ///     Add several <see cref="Settlement">Settlements</see> to the <see cref="Empire" />.
        /// </summary>
        /// <param name="settlements">The <see cref="Settlement">Settlements</see> to add</param>
        public void AddSettlements(IEnumerable<Settlement> settlements)
        {
            foreach (Settlement settlement in settlements)
            {
                AddSettlement(settlement);
            }
        }

        public Settlement GetSettlement(FacilityManager manager)
        {
            foreach (Settlement settlement in settlements.Keys)
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

            Logger.Warn($"{settlement.Name} was not in the settlement manager! Returning null.");
            return null;
        }
    }
}
