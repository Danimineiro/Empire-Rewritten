using System;
using System.Collections.Generic;
using System.Linq;
using Empire_Rewritten.Borders;
using Empire_Rewritten.Facilities;
using Empire_Rewritten.Resources;
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
        private bool borderIsDirty;
        private Border cachedBorder;
        private List<FacilityManager> facilityManagersForLoading = new List<FacilityManager>();
        private Faction faction;

        private Dictionary<Settlement, FacilityManager> settlements = new Dictionary<Settlement, FacilityManager>();

        private List<Settlement> settlementsForLoading = new List<Settlement>();
        private StorageTracker storageTracker = new StorageTracker();

        [UsedImplicitly]
        public Empire() { }

        public Empire([NotNull] Faction faction)
        {
            this.faction = faction ?? throw new ArgumentNullException(nameof(faction));
        }

        public StorageTracker StorageTracker => storageTracker;
        public Dictionary<Settlement, FacilityManager> Settlements => settlements;
        public List<int> SettlementTiles { get; } = new List<int>();

        private Border Border
        {
            get
            {
                if (cachedBorder == null || borderIsDirty)
                {
                    borderIsDirty = false;
                    cachedBorder = BorderManager.GetBorderManager.GetBorder(faction);
                }

                return cachedBorder;
            }
        }

        public IEnumerable<FacilityManager> AllFacilityManagers => settlements.Values;

        public void ExposeData()
        {
            Scribe_Collections.Look(ref settlements, "settlements", LookMode.Reference, LookMode.Deep, ref settlementsForLoading, ref facilityManagersForLoading);
            Scribe_References.Look(ref faction, "faction");
            Scribe_Deep.Look(ref storageTracker, "storageTracker");
        }

        public string GetUniqueLoadID()
        {
            return $"{nameof(Empire)}_{GetHashCode()}";
        }

        public void SetBorderDirty()
        {
            borderIsDirty = true;
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
        ///     Add a settlement to the tracker.
        /// </summary>
        /// <param name="settlement"></param>
        public void AddSettlement(Settlement settlement)
        {
            FacilityManager tracker = new FacilityManager(settlement);
            settlements.Add(settlement, tracker);
            Border.SettlementClaimTiles(settlement);
            SettlementTiles.Add(settlement.Tile);
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