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
    public class AITileManager : AIModule
    {
        public AITileManager(AIPlayer player) : base(player)
        {
            worldGrid = Find.WorldGrid;
        }

        public IEnumerable<int> GetTiles
        {
            get
            {
                return tileWeights.Keys;
            }
        }
       
        private static WorldGrid worldGrid;
        private TechLevel cachedTechLevel = TechLevel.Undefined;

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

        private readonly Dictionary<int, float> tileWeights = new Dictionary<int, float>();


        public float GetTileWeight(int id)
        {
            if(!tileWeights.ContainsKey(id))
                tileWeights.Add(id,CalculateTileWeight(id));
            return tileWeights[id];
        }

        private float CalculateTileWeight(int id)
        {
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
            int smallestDist = 0;
            int range = TechLevel.IsNeolithicOrWorse() ? 3 : 7;

            List<int> tiles = BorderManager.GetBorderManager.GetBorder(player.faction).GetTilesRecursively(id, range);
            foreach (int other in tiles)
            {
                Settlement settlement = Find.WorldObjects.SettlementAt(other);
                if (settlement != null)
                {
                    int dist = worldGrid.TraversalDistanceBetween(id, other);

                    if (dist < smallestDist || !foundASettlement)
                    {
                        smallestDist = dist;
                        foundASettlement = true;
                    }
                }
            }

            float distanceWeight = (foundASettlement ? smallestDist : -1000) * -1;

            /*
            FactionController factionController = UpdateController.GetUpdateController.FactionController;
            FactionCivicAndEthicData factionCivicAndEthicData = factionController.GetOwnedCivicAndEthicData(player.Faction);
            List<CivicDef> civicDefs = factionCivicAndEthicData.Civics;
            foreach(CivicDef civicDef in civicDefs)
            {
                CivicWorker worker = civicDef.Worker;
                if(worker != null)
                {
                    distanceWeight += worker.CalculateDistanceWeight(distanceWeight);
                }
            }
            

            */

            weight += techWeight + hillinessOffsetWeight + randomOffsetWeight + distanceWeight;
            return weight;
        }

        public void CalculateAllUnknownTiles()
        {
            IEnumerable<int> tiles = BorderManager.GetBorderManager.GetBorder(player.Faction).Tiles.ListFullCopy();
            int counter = 0;
            foreach (int tile in tiles)
            {
                if (!tileWeights.ContainsKey(tile))
                {
                    tileWeights.Add(tile, CalculateTileWeight(tile));
                    counter++;
                }
                if (counter == 10)
                    break;
            }
        }

        public override void DoModuleAction()
        {
            CalculateAllUnknownTiles();
        }

        public override void DoThreadableAction()
        {
            
        }
    }
}
