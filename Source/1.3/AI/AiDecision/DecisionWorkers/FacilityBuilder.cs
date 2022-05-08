using Empire_Rewritten.Facilities;
using Empire_Rewritten.Resources;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Empire_Rewritten.AI
{
    public class FacilityBuilder :AiDecisionWorker
    {
        /// <summary>
        ///     Select a facility to build, based on what the AI needs and what is produced on the tile.
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        public FacilityDef SelectFacilityToBuild(FacilityManager manager, AIPlayer player)
        {
            Dictionary<float, List<FacilityDef>> facilityWeights = new Dictionary<float, List<FacilityDef>>();
            List<FacilityDef> defs = DefDatabase<FacilityDef>.AllDefsListForReading;
            List<Tile> tiles = Find.WorldGrid.tiles;
            foreach (FacilityDef facilityDef in defs)
            {
                float weight = 0;
                weight += manager.FacilityDefsInstalled.Contains(facilityDef) ? 0.5f : -0.5f;
                weight += player.ResourceManager.GetTileResourceWeight(tiles[player.Manager.GetSettlement(manager).Tile]);

                if (facilityWeights.ContainsKey(weight))
                {
                    facilityWeights[weight].Add(facilityDef);
                }
                else
                {
                    facilityWeights.Add(weight, new List<FacilityDef> { facilityDef });
                }
            }

            float key = facilityWeights.Keys.Max();
            return facilityWeights[key].RandomElement();
        }
        /// <summary>
        ///     Find a manager the AI can build on.
        /// </summary>
        /// <returns></returns>
        public FacilityManager FindManagerToBuildOn(AIPlayer player)
        {
            List<ResourceDef> resourceDefs = player.ResourceManager.LowResources;
            IEnumerable<FacilityManager> managers = player.Manager.AllFacilityManagers.Where(x => x.CanBuildNewFacilities);
            List<FacilityManager> potentialResults = new List<FacilityManager>();
            foreach (FacilityManager facilityManager in managers)
            {
                IEnumerable<FacilityDef> facilityDefs = facilityManager.FacilityDefsInstalled.Where(x => x.ProducedResources.Any(y => resourceDefs.Contains(y)));
                if (facilityDefs.Any())
                {
                    potentialResults.Add(facilityManager);
                }
            }

            if (potentialResults.Any())
            {
                return potentialResults.RandomElement();
            }
            return null;
        }
        public override bool CanDecide(AIPlayer player, BasePlayer other = null)
        {
            return player.Manager.AllFacilityManagers.Any(x => x.CanBuildNewFacilities);
        }

        public override float DecisionWeight(AIPlayer player, BasePlayer other = null)
        {
            int baseWeight = player.ResourceManager.FindLowResources().Any(x => player.ResourceManager.GetAmountProduced(x) < x.desiredAIMinimum) ? 5 : 0;
            baseWeight += player.ResourceManager.HasCriticalResource ? 20 : 0;
            return baseWeight;
        }
        public override void MakeDecision(AIPlayer player, BasePlayer other = null)
        {
            FacilityManager manager = FindManagerToBuildOn(player);
            if(manager!= null)
            {
                FacilityDef def = SelectFacilityToBuild(manager, player);
                if(def!=null)
                    manager.AddFacility(def);
            }
            base.MakeDecision(player, other);
        }
    }
}
