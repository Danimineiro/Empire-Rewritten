using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_Rewritten.AI
{
    public class AIPlayer : BasePlayer
    {
        private AIFacilityManager facilityManager;
        private AISettlementManager settlementManager;
        private AIResourceManager resourceManager;

        private SettlementManager cachedManager;
        private bool ManagerIsDirty = true;

        public SettlementManager Manager
        {
            get
            {
                if (cachedManager== null || ManagerIsDirty)
                {
                  
                    ManagerIsDirty = true;
                    UpdateController updateController = UpdateController.GetUpdateController;
                    FactionController factionController = updateController.FactionController;

                    cachedManager = factionController.GetOwnedSettlementManager(Faction);
                }
                return cachedManager;
            }
        }

        public AISettlementManager AISettlementManager
        {
            get
            {
                return settlementManager;
            }
        }

        public AIFacilityManager FacilityManager
        {
            get
            {
                return facilityManager;
            }
        }

        public Faction Faction
        {
            get
            {
                return faction;
            }
        }

        public AIPlayer(Faction faction) : base(faction)
        {
            this.resourceManager = new AIResourceManager(this);
            this.settlementManager = new AISettlementManager(this);
            this.facilityManager = new AIFacilityManager(this);

            UpdateController.AddUpdateCall(MakeMove,ShouldExecute);
            UpdateController.AddUpdateCall(MakeThreadedMove, ShouldExecuteThreaded);
        }

        public AIResourceManager ResourceManager
        {
            get
            {
                return resourceManager;
            }
        }




        public override void MakeMove(FactionController factionController)
        {
            ResourceManager.DoModuleAction();
            FacilityManager.DoModuleAction();
            AISettlementManager.DoModuleAction();

        }

        int tick = 0;
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
           Task.Run(AISettlementManager.DoThreadableAction);
        }

        int threadTick = 0;
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
