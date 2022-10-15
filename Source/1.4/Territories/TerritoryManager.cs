using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using RimWorld;
using Verse;

namespace Empire_Rewritten.Territories
{
    /// <summary>
    ///     Manages faction <see cref="Territory">territories</see>. There is one per <see cref="RimWorld.Planet.World" />.
    /// </summary>
    public class TerritoryManager : IExposable
    {
        /// <summary>
        /// A list of all territories that this object manages; one per faction.
        /// </summary>
        private List<Territory> territories = new List<Territory>();

        /// <summary>
        /// A map from factions to their IDs in <see cref="territories"/>.
        /// </summary>
        private Dictionary<Faction, int> territoryIDs = new Dictionary<Faction, int>();

        private List<Faction> territoryIDsKeysListForSaving = new List<Faction>();
        private List<int> territoryIDsValuesListForSaving = new List<int>();

        public TerritoryManager()
        {
            GetTerritoryManager = this;
        }

        /// <summary>
        /// A list of all territories that this object manages; one per faction.
        /// </summary>
        public List<Territory> Territories => territories;

        /// <summary>
        /// Gets the instance of <see cref="TerritoryManager"/>.
        /// </summary>
        public static TerritoryManager GetTerritoryManager { get; private set; }

        public void ExposeData()
        {
            Scribe_Collections.Look(ref territories, "territories");
            Scribe_Collections.Look(ref territoryIDs, "territoryIDs", LookMode.Reference, LookMode.Value, ref territoryIDsKeysListForSaving, ref territoryIDsValuesListForSaving);
        }

        /// <summary>
        ///     Gets the <see cref="Faction" /> that owns a given world tile.
        /// </summary>
        /// <param name="tileId">The <see cref="int">ID</see> of the world tile to check</param>
        /// <returns>The <see cref="Faction" /> that owns <paramref name="tileId" />; <c>null</c> if the tile not owned</returns>
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

        /// <summary>
        /// Checks if a tile has an owner.
        /// </summary>
        /// <param name="tile">The <see cref="int">ID</see> of the world tile to check</param>
        /// <returns>True if <paramref name="tile"/> has an owner; false otherwise</returns>
        public bool AnyFactionOwnsTile(int tile)
        {
            return GetTileOwner(tile) != null;
        }

        /// <summary>
        /// Checks if a tile is owned by a specified faction.
        /// </summary>
        /// <param name="faction">The specified <see cref="Faction"/> to check for ownership</param>
        /// <param name="tile">The <see cref="int">ID</see> of the world tile to check</param>
        /// <returns>True if <paramref name="tile"/> is owned by <paramref name="faction"/>; false otherwise</returns>
        public bool FactionOwnsTile([NotNull] Faction faction, int tile)
        {
            Territory territory = GetTerritory(faction);
            return territory != null && territory.Tiles.Contains(tile);
        }

        /// <summary>
        /// Checks if a faction has territory that is controlled by this object.
        /// </summary>
        /// <param name="faction">The <see cref="Faction"/> to look for</param>
        /// <returns>True if <paramref name="faction"/>'s territory is managed by this object; false otherwise</returns>
        public bool HasFactionRegistered([NotNull] Faction faction)
        {
            if (faction is null)
            {
                throw new ArgumentNullException(nameof(faction));
            }

            return territoryIDs.ContainsKey(faction);
        }

        /// <summary>
        /// Gets the territory object of a given faction. If none are registered, this creates a new object.
        /// </summary>
        /// <param name="faction">The <see cref="Faction"/> to look for</param>
        /// <returns>The territory of <paramref name="faction"/></returns>
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