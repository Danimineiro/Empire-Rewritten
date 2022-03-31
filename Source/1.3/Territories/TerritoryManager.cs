using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using RimWorld;
using Verse;

namespace Empire_Rewritten.Territories
{
    public class TerritoryManager : IExposable
    {
        private List<Territory> territories = new List<Territory>();
        private Dictionary<Faction, int> territoryIDs = new Dictionary<Faction, int>();

        private List<Faction> territoryIDsKeysListForSaving = new List<Faction>();
        private List<int> territoryIDsValuesListForSaving = new List<int>();

        public TerritoryManager()
        {
            GetTerritoryManager = this;
        }

        public List<Territory> Territories => territories;

        public static TerritoryManager GetTerritoryManager { get; private set; }

        public void ExposeData()
        {
            Scribe_Collections.Look(ref territories, "territories");
            Scribe_Collections.Look(ref territoryIDs, "territoryIDs", LookMode.Reference, LookMode.Value, ref territoryIDsKeysListForSaving, ref territoryIDsValuesListForSaving);
        }

        /// <summary>
        ///     Gets the <see cref="Faction" /> that owns a given world tile
        /// </summary>
        /// <param name="tileId">The <see cref="int">ID</see> of the world tile to check</param>
        /// <returns>The <see cref="Faction" /> that owns <paramref name="tileId" />, if the tile not owned, <c>null</c></returns>
        [CanBeNull]
        public Faction GetTileOwner(int tileId)
        {
            foreach (Territory territory in territories)
            {
                if (territory.Tiles.Contains(tileId))
                {
                    return territory.Faction;
                }
            }

            return null;
        }

        public bool AnyFactionOwnsTile(int tile)
        {
            return GetTileOwner(tile) != null;
        }

        public bool FactionOwnsTile([NotNull] Faction faction, int tile)
        {
            Territory territory = GetTerritory(faction);
            return territory != null && territory.Tiles.Contains(tile);
        }

        public bool HasFactionRegistered([NotNull] Faction faction)
        {
            if (faction is null)
            {
                throw new ArgumentNullException(nameof(faction));
            }

            return territoryIDs.ContainsKey(faction);
        }

        public Territory GetTerritory([NotNull] Faction faction)
        {
            if (faction is null)
            {
                throw new ArgumentNullException(nameof(faction));
            }

            if (HasFactionRegistered(faction))
            {
                return territories[territoryIDs[faction]];
            }

            // Set up new faction territory.
            Territory newTerritory = new Territory(faction);
            territoryIDs.Add(faction, territories.Count);
            territories.Add(newTerritory);

            return newTerritory;
        }
    }
}
