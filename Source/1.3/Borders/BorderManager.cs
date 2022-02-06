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

            List<int> tiles = new List<int>();

            void GetTiles()
            {
                tiles = GetTilesRecursively(tileID, 5);
                ClaimTiles(tiles);
            }
            Task task = new Task(GetTiles);
            task.Start();
        }

        /// <summary>
        /// Get neighbors recursively.
        /// </summary>
        /// <param name="neighbor"></param>
        /// <param name="times"></param>
        /// <returns></returns>
        public List<int> GetTilesRecursively(int tileID, int times)
        {
            List<int> result = new List<int>();

            Find.WorldGrid.GetTileNeighbors(tileID, result);
            int max = result.Count;
            result.Add(tileID);
            //Already did it once so times-1
            for (int index = 0; index<times-1; index++)
            {
                int count = result.Count;
               
                for (int tile = 0; tile <count; tile++)
                {
                    List<int> addition = new List<int>();
                    Find.WorldGrid.GetTileNeighbors(result[tile], addition);

                    foreach (int newTile in addition)
                    {
                        if (!result.Contains(newTile))
                        {
                            result.Add(newTile);
                        }
                    }
                }
            }
            return result;
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

        public HashSet<int> GetTiles(Faction faction)
        {
            Border border = GetBorder(faction);
            return border != null ? border.Tiles: new HashSet<int>();
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
