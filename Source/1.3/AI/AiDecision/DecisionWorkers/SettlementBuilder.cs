using Empire_Rewritten.Settlements;
using RimWorld.Planet;
using System.Collections.Generic;
using Verse;

namespace Empire_Rewritten.AI
{
    public class AI_SettlementBuilderWorker : AiDecisionWorker 
    {
        private Tile tileToBuildOn;
        /// <summary>
        ///     Search for tiles to build settlements on based off weights;
        ///     Weights:
        ///     - Resources
        ///     - Territory distance
        ///     Resources AI wants = higher weight
        ///     Resources AI has excess of = lower weight
        /// </summary>
        /// <returns></returns>
        public void SearchForTile(AIPlayer player)
        {
            // TODO: Pretty sure this can be improved...

            IEnumerable<int> tileOptions = player.TileManager.GetTiles;

            // Default largestWeight to -1000 so it's almost always initially overridden.
            float largestWeight = -1000;

            int result = 0;
            foreach (int tileOption in tileOptions)
            {
                float weight = player.TileManager.GetTileWeight(tileOption);
                if (largestWeight < weight && TileFinder.IsValidTileForNewSettlement(tileOption))
                {
                    largestWeight = weight;
                    result = tileOption;
                }
            }

            tileToBuildOn = Current.Game.World.grid[result];
        }

        public override float DecisionWeight(AIPlayer player, BasePlayer other = null)
        {
            int resourceWeight = player.Manager.StorageTracker.GetAverageExcessResources(Empire.SettlementCost);
            return resourceWeight;
        }

        //The AI has confirmed it is going to build a new settlement. 
        public override void MakeDecision(AIPlayer player, BasePlayer other = null)
        {
            SearchForTile(player);
            if(tileToBuildOn!= null)
                player.Manager.BuildNewSettlementOnTile(tileToBuildOn);
        }

        public override bool CanDecide(AIPlayer player, BasePlayer other = null)
        {
            return player.Manager.StorageTracker.CanRemoveThingsFromStorage(Empire.SettlementCost);
        }

        public override float ImpactOnOtherEmpires(AIPlayer player)
        {
            return 1;
        }
    }
}
