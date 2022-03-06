using System;
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

        private List<int> tiles = new List<int>();

        public Border(Faction faction)
        {
            this.faction = faction;
            tiles = new List<int>();
        }

        private static WorldGrid WorldGrid
        {
            get
            {
                if (_worldGrid == null)
                {
                    _worldGrid = Find.WorldGrid;
                }

                return _worldGrid;
            }
        }

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

        public void ClaimTiles(List<int> ids)
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
            int tileId = settlement.Tile;

            void GetTiles()
            {
                ClaimTiles(GetTilesRecursively(tileId, (int)(faction.def.techLevel + 1)));
            }

            Task task = new Task(GetTiles);
            task.Start();
        }

        /// <summary>
        ///     Get neighbors recursively.
        /// </summary>
        /// <param name="neighboringTileId"></param>
        /// <param name="times"></param>
        /// <returns></returns>
        public List<int> GetTilesRecursively(int neighboringTileId, int times)
        {
            List<int> result = TileAndNeighbors(neighboringTileId);

            if (times <= 0)
            {
                return result;
            }

            int count = 1;
            List<int> resultCopy = new List<int>();
            resultCopy.AddRange(result);

            foreach (int tile in resultCopy)
            {
                Tile worldTile = _worldGrid[tile];
                if (!worldTile.WaterCovered && worldTile.hilliness != Hilliness.Impassable)
                {
                    foreach (int nTile in GetTilesRecursively(tile, times - count))
                    {
                        Tile newTile = _worldGrid[nTile];
                        if (!result.Contains(nTile) && !newTile.WaterCovered && newTile.hilliness != Hilliness.Impassable)
                        {
                            result.Add(nTile);
                        }
                    }

                    count++;
                }
            }

            return result;
        }

        private List<int> TileAndNeighbors(int tile)
        {
            List<int> addition = new List<int>();
            WorldGrid.GetTileNeighbors(tile, addition);
            addition.Add(tile);
            return addition;
        }
    }

    public class BorderManager : IExposable
    {
        private readonly Dictionary<Faction, int> borderIDs = new Dictionary<Faction, int>();
        private List<Border> borders = new List<Border>();

        public BorderManager()
        {
            GetBorderManager = this;
        }

        public List<Border> Borders => borders;

        public static BorderManager GetBorderManager { get; private set; }

        public void ExposeData()
        {
            Scribe_Collections.Look(ref borders, "borders");
        }

        public bool AnyFactionOwnsTile(int tile)
        {
            foreach (Border border in borders)
            {
                if (border.Tiles.Contains(tile))
                {
                    return true;
                }
            }

            return false;
        }

        public bool FactionOwnsTile([NotNull] Faction faction, int tile)
        {
            Border border = GetBorder(faction);
            return border != null && border.Tiles.Contains(tile);
        }

        public bool HasFactionRegistered([NotNull] Faction faction)
        {
            if (faction is null)
            {
                throw new ArgumentNullException(nameof(faction));
            }

            return borderIDs.ContainsKey(faction);
        }

        public Border GetBorder([NotNull] Faction faction)
        {
            if (faction is null)
            {
                throw new ArgumentNullException(nameof(faction));
            }

            if (HasFactionRegistered(faction))
            {
                return borders[borderIDs[faction]];
            }

            //Setup new faction border.
            Border newBorder = new Border(faction);
            borderIDs.Add(faction, borders.Count);
            borders.Add(newBorder);

            return newBorder;
        }
    }
}