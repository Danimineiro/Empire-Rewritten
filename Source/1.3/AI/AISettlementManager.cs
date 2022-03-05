using Empire_Rewritten.Borders;
using RimWorld;
using Empire_Rewritten.Resources;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Empire_Rewritten.Facilities;

namespace Empire_Rewritten.AI
{
    public class AISettlementManager : AIModule
    {
        public AISettlementManager(AIPlayer player) : base(player)
        {
            worldGrid = Find.WorldGrid;
        }

        private static WorldGrid worldGrid;
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




        public void BuildOrUpgradeNewSettlement()
        {
            bool UpgradedSettlement = false;
            if (player.Manager.Settlements.Count > 0)
            {
                KeyValuePair<Settlement, FacilityManager> settlementAndManager = player.Manager.Settlements.Where(x =>!x.Value.IsFullyUpgraded).RandomElement();
                Settlement settlement = settlementAndManager.Key;
                FacilityManager facilityManager = settlementAndManager.Value;
                UpgradedSettlement = AttemptToUpgradeSettlement(facilityManager);
            }
            bool BuiltSettlement = false;
            if (!UpgradedSettlement)
            {
                SearchForTile();
            }

            canUpgradeOrBuild = UpgradedSettlement || BuiltSettlement;
        }

      

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
        public void SearchForTile()
        {

            IEnumerable<int> tileOptions = player.TileManager.GetTiles;

            //Default largestweight to -1000 so it's almost always initally overridden.
            float largestWeight = -1000;

            int result=0;
           foreach(int tileOption in tileOptions)
            {
                float weight = player.TileManager.GetTileWeight(tileOption);
                if (largestWeight < weight && TileFinder.IsValidTileForNewSettlement(tileOption))
                {
                    largestWeight = weight;
                    result = tileOption;
                }
            }
            tileToBuildOn = worldGrid[result];
        }

        private Tile tileToBuildOn = null;

        public void BuildSettlement()
        {
            if (tileToBuildOn != null)
            {
                player.Manager.BuildNewSettlementOnTile(tileToBuildOn);
                tileToBuildOn = null;
            }
        }

        public override void DoModuleAction()
        {
            SearchForTile();
            BuildSettlement();
        }

        public override void DoThreadableAction()
        {
          // SearchForTile();
        }
    }
}
