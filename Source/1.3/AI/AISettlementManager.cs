using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Empire_Rewritten.AI
{
    public class AISettlementManager : AIModule
    {
        public AISettlementManager(AIPlayer player) : base(player)
        {
        }

        private bool canUpgradeOrBuild;

        public bool CanUpgradeOrBuild
        {
            get
            {
                return canUpgradeOrBuild;
            }
        }


        public bool AttemptToUpgradeSettlement(Settlement settlement)
        {
            FacilityManager facilityManager = player.Manager.GetFacilityManager(settlement);

            return AttemptToUpgradeSettlement(facilityManager);
        }

        public bool AttemptToUpgradeSettlement(FacilityManager manager)
        {
            if (!player.FacilityManager.CanMakeFacilities /* || otherFactor */)
            {
                //Do something
            }

            return false;
        }


        public bool AttemptBuildNewSettlement()
        {
            return !player.FacilityManager.CanMakeFacilities && player.Manager.BuildNewSettlementOnTile(SearchForTile());
        }

        public void BuildOrUpgradeNewSettlement()
        {
            bool UpgradedSettlement = false;
            if (player.Manager.Settlements.Count > 0)
            {
                KeyValuePair<Settlement, FacilityManager> settlementAndManager = player.Manager.Settlements.Where(x => true /* !x.Value.IsFullyUpgraded*/).RandomElement();
                Settlement settlement = settlementAndManager.Key;
                FacilityManager facilityManager = settlementAndManager.Value;
                UpgradedSettlement = AttemptToUpgradeSettlement(facilityManager);
            }
            bool BuiltSettlement = false;
            if (!UpgradedSettlement)
            {
                Log.Message($"{player.faction.Name}: Couldn't upgrade a settlement, attempting to build a new one...");
                BuiltSettlement = AttemptBuildNewSettlement();
                if (BuiltSettlement)
                {
                    Log.Message($"{player.faction.Name}: Built a new one!");
                }
            }

            canUpgradeOrBuild = UpgradedSettlement || BuiltSettlement;
        }

        int tempOffset = 0;
        /// <summary>
        /// Search for tiles to build settlements on based off weights;
        /// Weights:
        /// - Resources
        /// - Border distance
        /// 
        /// Resources AI wants = higher weight
        /// Resources AI has excess of = lower weight
        /// </summary>
        /// <returns></returns>
        public Tile SearchForTile()
        {
            IEnumerable<Settlement> settlements = player.Manager.Settlements.Keys;
            AIResourceManager aIResourceManager = player.ResourceManager;
            List<int> tiles = new List<int>();
            int tileID = UnityEngine.Random.Range(0, Find.WorldGrid.TilesCount - 1);
            if (!settlements.EnumerableNullOrEmpty())
            {
                tileID = player.Manager.Settlements.RandomElement().Key.Tile;
            }
            Find.WorldGrid.GetTileNeighbors(tileID, tiles);
            float largestWeight = -1000;

            Tile result = null;
            foreach (int t in tiles)
            {
                if (Find.WorldGrid[t].hilliness != Hilliness.Impassable)
                {
                    List<int> newTiles = new List<int>();
                    Find.WorldGrid.GetTileNeighbors(t, newTiles);
                    foreach (int i in newTiles)
                    {
                        if (TileFinder.IsValidTileForNewSettlement(i))
                        {
                            Tile tile = Find.WorldGrid[i];
                            float weight = aIResourceManager.GetTileResourceWeight(tile);
                            if (largestWeight < weight)
                            {
                                largestWeight = weight;
                                result = tile;
                            }
                        }
                    }
                }
            }
            return result;
            /*
            Tile t = null;

            //temp test
            //todo when bordermanager is implimented:
            //only pull from owned tiles.

            List<Tile> tiles = Find.WorldGrid.tiles.Where(x => TileFinder.IsValidTileForNewSettlement(Find.WorldGrid.tiles.IndexOf(x))).ToList();
            AIResourceManager aIResourceManager = player.ResourceManager;

            float largestWeight = -1000;

            int offsetter = 0;
            //Temporary limiter
            for (int a = tempOffset; a < 20 + tempOffset; a++)
            {
                Tile tile = tiles[a];
                float weight = aIResourceManager.GetTileResourceWeight(tile);
                if (largestWeight < weight)
                {
                    largestWeight = weight;
                    t = tile;
                }
            }
            /*
            todo: border weight
   

            return t;
            */
        }
       

        public override void DoModuleAction()
        {
            BuildOrUpgradeNewSettlement();
        }
    }
}
