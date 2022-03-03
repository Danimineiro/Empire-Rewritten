using System;
using Empire_Rewritten.Controllers;
using Empire_Rewritten.Settlements;
using RimWorld;

namespace Empire_Rewritten.AI
{
    public class AIPlayer : BasePlayer
    {
        private SettlementManager cachedManager;
        private bool ManagerIsDirty = true;
        private AISettlementManager settlementManager;

        public AIPlayer(Faction faction) : base(faction)
        {
            ResourceManager = new AIResourceManager(this);
            settlementManager = new AISettlementManager(this);
            FacilityManager = new AIFacilityManager(this);
        }

        public SettlementManager Manager
        {
            get
            {
                if (cachedManager == null || ManagerIsDirty)
                {
                    ManagerIsDirty = true;
                    var updateController = UpdateController.CurrentWorldInstance;
                    FactionController factionController = updateController.FactionController;

                    cachedManager = factionController.GetOwnedSettlementManager(Faction);
                }

                return cachedManager;
            }
        }

        public AIFacilityManager FacilityManager { get; }

        public Faction Faction => faction;

        public AIResourceManager ResourceManager { get; }

        public override void MakeMove()
        {
            throw new NotImplementedException();
        }
    }
}