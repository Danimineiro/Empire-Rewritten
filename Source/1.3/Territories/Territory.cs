using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace Empire_Rewritten.Territories
{
    public class Territory : IExposable
    {
        private Faction faction;

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

        public Faction Faction => faction;

        public List<int> Tiles => tiles;

        public void ExposeData()
        {
            Scribe_References.Look(ref faction, "faction");
            Scribe_Collections.Look(ref tiles, "tiles");
        }

        public bool HasTile(int tile)
        {
            return tiles.Contains(tile);
        }

        public void ClaimTile(int id)
        {
            if (!TerritoryManager.GetTerritoryManager.AnyFactionOwnsTile(id))
            {
                tiles.Add(id);
                TerritoryDrawer.dirty = true;
            }
        }

        public void ClaimTiles([NotNull] List<int> ids)
        {
            foreach (int tile in ids)
            {
                ClaimTile(tile);
            }
        }

        public void UnclaimTile(int id)
        {
            if (tiles.Contains(id))
            {
                tiles.Remove(id);
            }
        }

        public void SettlementClaimTiles(Settlement settlement)
        {
            // This could cause a race condition where two Empires claim the same Tile
            Task.Run(() => ClaimTiles(GetSurroundingTiles(settlement.Tile, (int)(faction.def.techLevel + 1))));
        }

        /// <summary>
        ///     Returns the list of <see cref="int">Tile IDs</see> of the tiles reachable within a given distance from some center tile.
        /// Impassable tiles are removed from the output (including the center tile). If the center tile is impassable, this function returns
        /// an empty list.
        /// </summary>
        /// <param name="centerTileId"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        [NotNull]
        public static List<int> GetSurroundingTiles(int centerTileId, int distance)
        {
            if (distance < 0) return new List<int>();

            if (WorldGrid[centerTileId].biome.impassable || WorldGrid[centerTileId].hilliness == Hilliness.Impassable)
                return new List<int>();

            // The perimeter of a hexagon is 6 * its radius (in this case, distance). That's the maximum number of tiles that will be in the queue.
            Queue<int> queue = new Queue<int>(6 * distance + 6);

            //Keep track of which tiles have been processed
            HashSet<int> found = new HashSet<int>();

            //Keep track of which tiles can be returned
            List<int> res = new List<int>();

            queue.Enqueue(centerTileId);
            while (queue.Count != 0 && distance > 0)
            {
                int numTilesAtDepth = queue.Count;
                while (numTilesAtDepth != 0)
                {
                    //Remove a tile and mark it as visited
                    numTilesAtDepth--;
                    int tile = queue.Dequeue();
                    found.Add(tile);

                    //If the tile is impassable, move on to the next tile.
                    if (WorldGrid[tile].biome.impassable || WorldGrid[tile].hilliness == Hilliness.Impassable) continue;

                    //Add this tile to the output
                    res.Add(tile);

                    //Add all of the tiles' neighbors (except for other found tiles, which includes impassable tiles) to the queue
                    List<int> neighbors = new List<int>(7);
                    WorldGrid.GetTileNeighbors(tile, neighbors);
                    foreach (int t in neighbors)
                    {
                        if (!found.Contains(t)) queue.Enqueue(t);
                    }
                }
            }

            return res;
        }

        private static List<int> TileAndNeighborsClaimable(int tile)
        {
            List<int> result = TileAndNeighbors(tile);
            result.RemoveAll(tileID => !WorldPathGrid.PassableFast(tileID));
            return result;
        }

        private static List<int> TileAndNeighbors(int tile)
        {
            List<int> result = new List<int>();
            WorldGrid.GetTileNeighbors(tile, result);
            result.Add(tile);
            return result;
        }
    }
}