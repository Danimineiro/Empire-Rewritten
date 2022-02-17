using Empire_Rewritten.Borders;
using RimWorld;
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
            worldGrid = Find.WorldGrid;
        }

        private static WorldGrid worldGrid;
        private bool canUpgradeOrBuild;
        private TechLevel cachedTechLevel = TechLevel.Undefined;

        private Dictionary<int, float> tileWeights = new Dictionary<int, float>();

        public TechLevel TechLevel
        {
            get
            {
                if (cachedTechLevel == TechLevel.Undefined)
                {
                    cachedTechLevel = player.Faction.def.techLevel;
                }
                return cachedTechLevel;
            }
        }
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
                KeyValuePair<Settlement, FacilityManager> settlementAndManager = player.Manager.Settlements.Where(x => true /* !x.Value.IsFullyUpgraded*/).RandomElement();
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

        public float GetTileWeight(int id)
        {
            if (tileWeights.ContainsKey(id))
                return tileWeights[id];

            AIResourceManager aIResourceManager = player.ResourceManager;
            Tile tile = Find.WorldGrid[id];
            //The weight from a tile's resources.
            float weight = aIResourceManager.GetTileResourceWeight(tile);

            //Neolithic AI should not have pin point actions, while spacer AI should be better at understanding resources.
            float techWeight = (float)(TechLevel - 4) * UnityEngine.Random.Range(1, 10);

            //Make the AI more "Organic"
            float randomOffsetWeight = UnityEngine.Random.Range(-5, 10);

            //Hills are hard to build on.
            float hillinessOffsetWeight = (float)tile.hilliness * UnityEngine.Random.Range(-10, -2);


            bool foundASettlement = false;
            int smallestDist = 10000;
            int range = 7;

            List<int> tiles = BorderManager.GetBorderManager.GetBorder(player.faction).GetTilesRecursively(id, range);
            foreach (int other in tiles)
            {
                Settlement settlement = Find.WorldObjects.SettlementAt(other);
                if (settlement != null)
                {
                    int dist = worldGrid.TraversalDistanceBetween(id, other);

                    if (dist < smallestDist)
                    {
                        smallestDist = dist;
                        foundASettlement = true;
                    }
                }
            }
            float distanceWeight = ((foundASettlement?smallestDist:-1000)*-5);
           


            weight += techWeight + hillinessOffsetWeight + randomOffsetWeight + distanceWeight;

            tileWeights.Add(id, weight);
            return weight;
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

            List<int> tileOptions = BorderManager.GetBorderManager.GetBorder(player.Faction).Tiles;

            //Default largestweight to -1000 so it's almost always initally overridden.
            float largestWeight = -1000;

            int result=0;
            for (int a = 0; a < tileOptions.Count; a++)
            {
                int t = tileOptions[a];

                float weight = GetTileWeight(t);
                if (largestWeight < weight && TileFinder.IsValidTileForNewSettlement(t))
                {
                    largestWeight = weight;
                    result = t;
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
