using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Empire_Rewritten.Borders
{
    public class Border : IExposable
    {
        private Faction faction;
        private static WorldGrid worldGrid;
        private static WorldGrid WorldGrid
        {
            get
            {
                if (worldGrid == null)
                    worldGrid = Find.WorldGrid;
                return worldGrid;
            }
        }
        public Faction Faction
        {
            get
            {
                return faction;
            }
        }

        private List<int> tiles = new List<int>();

        public List<int> Tiles
        {
            get
            {
                return tiles;
            }
        }

        public Border(Faction faction)
        {
            this.faction = faction;
            tiles = new List<int>();
        }

        public void ClaimTile(int id)
        {
            
            if(!BorderManager.GetBorderManager.AnyFactionOwnsTile(id)){
                tiles.Add(id);
            }
            
        }

        public void ClaimTiles(List<int> ids)
        {
            
            foreach(int tile in ids)
            {
                ClaimTile(tile);
            }
            Log.Message($"Claiming {ids.Count} tiles for {faction}");
            Log.Message($"Tiles usuable: {tiles.Count}");
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
            int tileID = settlement.Tile;
            void TestTask()
            {
                List<int> tiles = GetTilesRecursively(tileID, (int)(faction.def.techLevel+1));
                ClaimTiles(tiles);
            }
            Task task = new Task(TestTask);
            task.Start();

        }

        /// <summary>
        /// Get neighbors recursively.
        /// 
        /// 
        /// </summary>
        /// <param name="neighbor"></param>
        /// <param name="times"></param>
        /// <returns></returns>
        public List<int> GetTilesRecursively(int tileID, int times)
        {
            List<int> result = TileAndNeighbors(tileID);

            if(times <= 0)
                return result;

            int count = 1;
            List<int> resultCopy = new List<int>();
            resultCopy.AddRange(result);
           
            foreach (int tile in resultCopy)
            {
                List<int> tiles = GetTilesRecursively(tile, times-count);
                foreach (int nTile in tiles)
                {
                    if (!result.Contains(nTile))
                        result.Add(nTile);
                }
                count++;
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

        public void ExposeData()
        {
            Scribe_References.Look(ref faction, "faction");
            Scribe_Collections.Look(ref tiles, "tiles");
        }
    }

    public class BorderManager : IExposable
    {
        private List<Border> borders = new List<Border>();

        private Dictionary<Faction, int> borderIDs = new Dictionary<Faction, int>();

        private static BorderManager borderManager;
        
        public static BorderManager GetBorderManager
        {
            get
            {
                return borderManager;
            }
        }
        
        public BorderManager()
        {
            borderManager = this;
        }

        public bool AnyFactionOwnsTile(int tile)
        {
            foreach(Border border in borders)
            {
                if (border.Tiles.Contains(tile))
                {
                    return true;
                }
            }
            return false;
        }

        public bool FactionOwnsTile(Faction faction, int tile)
        {
            Border border = GetBorder(faction);
            return border != null && border.Tiles.Contains(tile);
        }

        public bool HasFactionRegistered(Faction faction)
        {
            return borderIDs.ContainsKey(faction);
        }

        public Border GetBorder(Faction faction)
        {
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

        public void ExposeData()
        {
            Scribe_Collections.Look(ref borders, "borders");
        }
    }
}
