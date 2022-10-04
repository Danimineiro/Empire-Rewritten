using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Empire_Rewritten.Controllers;
using Empire_Rewritten.Settlements;
using RimWorld;
using Verse;

namespace Empire_Rewritten.AI
{
    public class AIPlayer : BasePlayer
    {
        private Empire cachedManager;
        private bool managerIsDirty = true;

        private int threadTick;

        private int tick;
        private AITileManager tileManager;

        private static Dictionary<AiDecisionWorker, bool> aiDecisions;

        public AIPlayer(Faction faction) : base(faction)
        {
            ResourceManager = new AIResourceManager(this);
          
            if (aiDecisions == null)
            {
                aiDecisions = new Dictionary<AiDecisionWorker, bool>();
                foreach(AiDecisionDef def in DefDatabase<AiDecisionDef>.AllDefsListForReading)
                {
                    aiDecisions.Add(def.AiDecisionWorker, def.actsOnOtherEmpires);
                }
            }
        }

        public AITileManager TileManager => tileManager ?? (tileManager = new AITileManager(this));

        public Empire Manager
        {
            get
            {
                if (cachedManager == null || managerIsDirty)
                {
                    managerIsDirty = false;
                    UpdateController updateController = UpdateController.CurrentWorldInstance;
                    FactionController factionController = updateController.FactionController;

                    cachedManager = factionController.GetOwnedSettlementManager(Faction);
                }

                return cachedManager;
            }
        }

        public AIResourceManager ResourceManager { get; }
        public Faction Faction => faction;

        public override void MakeMove(FactionController factionController)
        {
            ResourceManager.DoResourceCalculations();
            TileManager.CalculateAllUnknownTiles();

            AiDecisionWorker(out BasePlayer player).MakeDecision(this, player);
            
        }

        public AiDecisionWorker AiDecisionWorker(out BasePlayer player)
        {
            player = null;
            Random weightRand = new Random();
            List<FactionSettlementData> factionSettlementDatas = UpdateController.CurrentWorldInstance.FactionController.ReadOnlyFactionSettlementData;
            AiDecisionWorker resultWorker = null;
            while (resultWorker == null)
            {
                AiDecisionWorker worker = aiDecisions.Keys.RandomElement();
                
                float impact = worker.ImpactOnOtherEmpires(this);
               
                if (worker != null)
                {

                    if (aiDecisions[worker])
                    {
                        if (impact >= 0 && impact < 20)
                            player = factionSettlementDatas.Where(x => x.SettlementManager.Faction.RelationKindWith(faction) == FactionRelationKind.Neutral || x.SettlementManager.Faction.RelationKindWith(faction) == FactionRelationKind.Ally).RandomElement().SettlementManager.Faction.GetPlayer();
                        if (impact >= 20)
                            player = factionSettlementDatas.Where(x => x.SettlementManager.Faction.RelationKindWith(faction) == FactionRelationKind.Ally).RandomElement().SettlementManager.Faction.GetPlayer();
                        if (impact <= 0)
                            player = factionSettlementDatas.Where(x => x.SettlementManager.Faction.RelationKindWith(faction) == FactionRelationKind.Hostile).RandomElement().SettlementManager.Faction.GetPlayer();
                    }
                    if (worker.CanDecide(this, player) && worker.DecisionWeight(this, player) >= weightRand.Next(0, 100))
                    {
                        resultWorker = worker;


                    }
                }
            }
            return resultWorker;
        }

        public override bool ShouldExecute()
        {
            if (tick == 120)
            {
                tick = 0;
                return true;
            }

            tick++;
            return false;
        }

        public override void MakeThreadedMove(FactionController factionController)
        {
            
        }

        public override bool ShouldExecuteThreaded()
        {
            if (threadTick == 2)
            {
                threadTick = 0;
                return true;
            }

            threadTick++;
            return false;
        }
    }
}