using System.Collections.Generic;
using Empire_Rewritten.Territories;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace Empire_Rewritten.AI
{
    public class AITileManager : AIModule
    {
        private static WorldGrid _worldGrid;

        private readonly Dictionary<int, float> tileWeights = new Dictionary<int, float>();
        private TechLevel cachedTechLevel = TechLevel.Undefined;

        public AITileManager(AIPlayer player) : base(player)
        {
            _worldGrid = Find.WorldGrid;
        }

        public IEnumerable<int> GetTiles => tileWeights.Keys;

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

        public float GetTileWeight(int id)
        {
            if (!tileWeights.ContainsKey(id))
            {
                tileWeights.Add(id, CalculateTileWeight(id));
            }

            return tileWeights[id];
        }

        private float CalculateTileWeight(int id)
        {
            AIResourceManager aIResourceManager = player.ResourceManager;
            Tile tile = Find.WorldGrid[id];
            //The weight from a tile's resources.
            float weight = aIResourceManager.GetTileResourceWeight(tile);

            //Neolithic AI should not have pin point actions, while spacer AI should be better at understanding resources.
            float techWeight = (float)(TechLevel - 4) * Random.Range(1, 10);

            //Make the AI more "Organic"
            float randomOffsetWeight = Random.Range(-2, 2);

            //Hills are hard to build on.
            float hillinessOffsetWeight = (float)tile.hilliness * Random.Range(-3, -2);

            bool foundASettlement = false;
            int smallestDist = 0;
            int range = TechLevel.IsNeolithicOrWorse() ? 3 : 7;

            List<int> tiles = Territory.GetSurroundingTiles(id, range);
            foreach (int other in tiles)
            {
                Settlement settlement = Find.WorldObjects.SettlementAt(other);
                if (settlement != null)
                {
                    int dist = _worldGrid.TraversalDistanceBetween(id, other);

                    if (dist < smallestDist || !foundASettlement)
                    {
                        smallestDist = dist;
                        foundASettlement = true;
                    }
                }
            }

            float distanceWeight = (foundASettlement ? smallestDist : -1000) * -1;

            /*
            FactionController factionController = UpdateController.CurrentWorldInstance.FactionController;
            FactionCivicAndEthicData factionCivicAndEthicData = factionController.GetOwnedCivicAndEthicData(player.Faction);
            List<CivicDef> civicDefs = factionCivicAndEthicData.Civics;
            if (!civicDefs.NullOrEmpty())
            {
                foreach (CivicDef civicDef in civicDefs)
                {
                    CivicWorker worker = civicDef.Worker;
                    if (worker != null)
                    {
                        distanceWeight += worker.CalculateDistanceWeight(distanceWeight);
                    }
                }
            }
            */

            weight += techWeight + hillinessOffsetWeight + randomOffsetWeight + distanceWeight;
            return weight;
        }

        public void CalculateAllUnknownTiles()
        {
            IEnumerable<int> tiles = TerritoryManager.GetTerritoryManager.GetTerritory(player.Faction).Tiles.ListFullCopy();
            int counter = 0;
            foreach (int tile in tiles)
            {
                if (!tileWeights.ContainsKey(tile))
                {
                    tileWeights.Add(tile, CalculateTileWeight(tile));
                    counter++;
                }

                if (counter == 10)
                {
                    break;
                }
            }
        }

        public override void DoModuleAction()
        {
            CalculateAllUnknownTiles();
        }

        public override void DoThreadableAction()
        { }
    }
}