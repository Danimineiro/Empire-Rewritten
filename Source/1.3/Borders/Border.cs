using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace Empire_Rewritten.Borders
{
    public class Border : IExposable
    {
        private static WorldGrid _worldGrid;
        private Faction faction;

        private List<int> tiles;

        public Border(Faction faction)
        {
            this.faction = faction;
            tiles = new List<int>();
        }

        private static WorldGrid WorldGrid => _worldGrid ?? (_worldGrid = Find.WorldGrid);

        public Faction Faction => faction;

        public List<int> Tiles => tiles;

        public void ExposeData()
        {
            Scribe_References.Look(ref faction, "faction");
            Scribe_Collections.Look(ref tiles, "tiles");
        }

        public void ClaimTile(int id)
        {
            if (!BorderManager.GetBorderManager.AnyFactionOwnsTile(id))
            {
                tiles.Add(id);
                BorderDrawer.dirty = true;
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
                return new List<int> {centerTileId};
            }

            if (distance == 1)
            {
                return TileAndNeighbors(centerTileId);
            }

            List<int> result = TileAndNeighbors(centerTileId);

            int currentDistance = 1;
            List<int> resultCopy = new List<int>(result);

            foreach (int tile in resultCopy)
            {
                Tile worldTile = WorldGrid[tile];
                if (!worldTile.biome.impassable && worldTile.hilliness != Hilliness.Impassable)
                {
                    foreach (int newTileId in GetSurroundingTiles(tile, distance - currentDistance))
                    {
                        Tile newTile = WorldGrid[newTileId];
                        if (!result.Contains(newTileId) && !newTile.WaterCovered && newTile.hilliness != Hilliness.Impassable)
                        {
                            result.Add(newTileId);
                        }
                    }

                    currentDistance++;
                }
            }

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
