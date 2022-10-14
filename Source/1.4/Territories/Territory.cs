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
        public Territory() { }

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
        ///     Recursively gets neighboring <see cref="int">Tile IDs</see>.
        /// </summary>
        /// <param name="centerTileId"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        [NotNull]
        public static List<int> GetSurroundingTiles(int centerTileId, int distance)
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
                    foreach (int newTileId in GetSurroundingTiles(tile, distance - currentDistance))
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
