using Empire_Rewritten.Utils;
using JetBrains.Annotations;
using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using Verse;

namespace Empire_Rewritten.Territories
{
    public class Territory : IExposable
    {
        /// <summary>
        /// The <see cref="RimWorld.Faction"/> that owns this territory.
        /// </summary>
        private Faction faction;

        /// <summary>
        /// A list of the <see cref="int">IDs</see> of the tiles that belong to this territory.
        /// </summary>
        private List<int> tiles = new List<int>();

        [UsedImplicitly]
        public Territory()
        { }

        public Territory(Faction faction)
        {
            this.faction = faction;
        }

        private static WorldGrid WorldGrid => Find.WorldGrid;

        private static WorldPathGrid WorldPathGrid => Find.WorldPathGrid;

        /// <summary>
        /// The <see cref="RimWorld.Faction"/> that owns this territory.
        /// </summary>
        public Faction Faction => faction;

        /// <summary>
        /// A list of the <see cref="int">IDs</see> of the tiles that belong to this territory.
        /// </summary>
        public List<int> Tiles => tiles;

        public void ExposeData()
        {
            Scribe_References.Look(ref faction, "faction");
            Scribe_Collections.Look(ref tiles, "tiles");
        }

        /// <summary>
        /// Checks if this territory contains <paramref name="tile"/>.
        /// </summary>
        /// <param name="tile">The <see cref="int">ID</see> of the world tile to check</param>
        /// <returns>True if <paramref name="tile"/> belongs to this territory</returns>
        public bool HasTile(int tile)
        {
            return tiles.Contains(tile);
        }

        /// <summary>
        /// Claims a given tile, adding it to the territory. Does nothing if the tile is already claimed.
        /// </summary>
        /// <param name="id">The <see cref="int">ID</see> of the world tile to claim</param>
        public void ClaimTile(int id)
        {
            if (!TerritoryManager.GetTerritoryManager.AnyFactionOwnsTile(id))
            {
                tiles.Add(id);
                TerritoryDrawer.dirty = true;
            }
        }

        /// <summary>
        /// Claims a given list of tiles, adding them to the territory. Tiles that are already claimed are ignored.
        /// </summary>
        /// <param name="ids">The <see cref="int">IDs</see> of the world tiles to claim</param>
        public void ClaimTiles([NotNull] List<int> ids)
        {
            foreach (int tile in ids)
            {
                ClaimTile(tile);
            }
        }

        /// <summary>
        /// Unclaims a given tile, removing it from the territory and setting it to ownerless. If the tile is not owned by this territory,
        /// this function does nothing
        /// </summary>
        /// <param name="id">The <see cref="int">ID</see> of the world tile to unclaim</param>
        public void UnclaimTile(int id)
        {
            if (tiles.Contains(id))
            {
                tiles.Remove(id);
            }
        }

        /// <summary>
        /// Claims all the tiles surrounding a <see cref="Settlement"/>. The radius is computed from <see cref="Faction"/>'s tech level.
        /// This function does not check if the <see cref="RimWorld.Faction">settlement's owner</see> is the same as this
        /// <see cref="RimWorld.Faction">territory's owner</see>.
        /// </summary>
        /// <param name="settlement">The settlement whose neighboring tiles should be claimed</param>
        public void SettlementClaimTiles(Settlement settlement)
        {
            ClaimTiles(GetSurroundingTiles(settlement.Tile, (int)faction.def.techLevel + 1));
        }

        /// <summary>
        /// Returns the list of tile <see cref="int">IDs</see> of all passable tiles reachable within a given distance from some center tile.
        /// If the center tile is impassable, this function returns an empty list.
        /// </summary>
        /// <param name="centerTileId">The tile <see cref="int">ID</see> of the center tile</param>
        /// <param name="distance">The maximum allowed distance from the center tile</param> TODO Figure out if this is inclusive or exclusive
        /// <returns>The list of tile <see cref="int">IDs</see> of the passable tiles within <paramref name="distance"/> of <paramref name="centerTileId"/></returns>
        [NotNull]
        public static List<int> GetSurroundingTiles(int centerTileId, int distance)
        {
            if (distance < 0) return new List<int>();

            if (WorldGrid[centerTileId].biome.impassable || WorldGrid[centerTileId].hilliness == Hilliness.Impassable)
                return new List<int>();

            // The perimeter of a hexagon is 6 * its radius (in this case, distance). That's the maximum number of tiles that will be in the queue.
            Queue<int> queue = new Queue<int>(6 * distance + 6);

            //Keep track of which tiles have been found
            HashSet<int> found = new HashSet<int>();

            queue.Enqueue(centerTileId);
            found.Add(centerTileId);
            while (queue.Count != 0 && distance > 0)
            {
                int numTilesAtDepth = queue.Count;
                while (numTilesAtDepth != 0)
                {
                    //Remove a tile and mark it as visited
                    numTilesAtDepth--;
                    int tile = queue.Dequeue();

                    //Add all of the tiles' neighbors (except for other found tiles, which includes impassable tiles) to the queue
                    List<int> neighbors = new List<int>(7);
                    WorldGrid.GetTileNeighbors(tile, neighbors);
                    foreach (int t in neighbors)
                    {
                        if (!(found.Contains(t) || WorldGrid[t].biome.impassable ||
                              WorldGrid[t].hilliness == Hilliness.Impassable))
                        {
                            queue.Enqueue(t);
                            found.Add(t);
                        }
                    }
                }

                distance--;
            }

            return new List<int>(found);
        }
    }
}
