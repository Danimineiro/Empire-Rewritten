using System.Collections.Generic;
using System.Linq;
using Empire_Rewritten.Facilities;
using Empire_Rewritten.Resources;
using RimWorld.Planet;
using Verse;

// This seems too "TODO" to document atm

namespace Empire_Rewritten.AI
{
    public class AISettlementManager : AIModule
    {
        public AISettlementManager(AIPlayer player) : base(player) { }

        public bool CanUpgradeOrBuild { get; private set; }


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
            if (!player.FacilityManager.CanMakeFacilities /* || otherFactor */)
            {
                //Do something
            }

            return false;
        }

        public void BuildOrUpgradeNewSettlement()
        {
            KeyValuePair<Settlement, FacilityManager> settlementAndManager = player.Manager.Settlements.Where(x => true /* !x.Value.IsFullyUpgraded*/).RandomElement();
            Settlement settlement = settlementAndManager.Key;
            FacilityManager facilityManager = settlementAndManager.Value;
            bool UpgradedSettlement = AttemptToUpgradeSettlement(facilityManager);
            bool BuiltSettlement = false;
            if (!UpgradedSettlement) BuiltSettlement = AttemptBuildNewSettlement();

            CanUpgradeOrBuild = UpgradedSettlement || BuiltSettlement;
        }


        /// <summary>
        ///     Search for tiles to build settlements on based off weights;
        ///     Weights:
        ///     - Resources
        ///     - Border distance
        ///     Resources AI wants = higher weight
        ///     Resources AI has excess of = lower weight
        /// </summary>
        /// <returns></returns>
        public Tile SearchForTile()
        {
            Tile t = null;

            //temp test
            //todo when bordermanager is implimented:
            //only pull from owned tiles.
            List<Tile> tiles = Find.WorldGrid.tiles;
            AIResourceManager aIResourceManager = player.ResourceManager;
            List<ResourceDef> lowResources = aIResourceManager.FindLowResources();
            List<ResourceDef> highResources = aIResourceManager.FindExcessResources();

            Dictionary<float, List<Tile>> tileWeights = new Dictionary<float, List<Tile>>();
            foreach (Tile tile in tiles)
            {
                float weight = 0;
                foreach (ResourceDef resourceDef in lowResources)
                {
                    weight += aIResourceManager.GetAmountProduced(resourceDef);
                }

                foreach (ResourceDef resourceDef in highResources)
                {
                    weight -= aIResourceManager.GetAmountProduced(resourceDef);
                }


                /*
                todo: border weight
                */

                if (tileWeights.ContainsKey(weight))
                {
                    tileWeights[weight].Add(tile);
                }
                else
                {
                    tileWeights.Add(weight, new List<Tile> {tile});
                }
            }

            //This should be smarter in the future.
            float largestWeight = tileWeights.Keys.Max();
            t = tileWeights[largestWeight].RandomElement();

            return t;
        }

        public override void DoModuleAction() { }
    }
}