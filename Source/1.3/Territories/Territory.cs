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
            Logger.Log("Claiming tiles...");
            int extra = 2;
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var n = GetSurroundingTilesN(settlement.Tile, (int)faction.def.techLevel + extra);
            watch.Stop();
            long ntime = watch.ElapsedMilliseconds;

            watch = System.Diagnostics.Stopwatch.StartNew();
            var o = GetSurroundingTilesO(settlement.Tile, (int)faction.def.techLevel + extra);
            watch.Stop();

            Logger.Log(string.Format("Settlement {0} tile claims (radius {1}) took {2}/{3} ms. Found {4}/{5} tiles.",
                settlement.HasName ? settlement.Name : "",
                (int)faction.def.techLevel + extra,
                ntime, watch.ElapsedMilliseconds.ToString(), n.Count, o.Count));

            ClaimTiles(n);
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
        public static List<int> GetSurroundingTilesN(int centerTileId, int distance)
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

        public static List<int> GetSurroundingTilesO(int centerTileId, int distance)
        {
            if (distance <= 0)
            {
                return new List<int> { centerTileId };
            }

            if (distance == 1)
            {
                return TileAndNeighborsClaimable(centerTileId);
            }

            List<int> result = TileAndNeighborsClaimable(centerTileId);

            int currentDistance = 1;
            List<int> resultCopy = new List<int>(result);

            foreach (int tile in resultCopy)
            {
                Tile worldTile = WorldGrid[tile];
                if (!worldTile.biome.impassable && worldTile.hilliness != Hilliness.Impassable)
                {
                    foreach (int newTileId in GetSurroundingTilesO(tile, distance - currentDistance))
                    {
                        if (!result.Contains(newTileId) && WorldPathGrid.PassableFast(newTileId))
                        {
                            result.Add(newTileId);
                        }
                    }

                    currentDistance++;
                }
            }

            return result;
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