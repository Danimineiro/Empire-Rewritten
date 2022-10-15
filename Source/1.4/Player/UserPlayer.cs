using System;
using Empire_Rewritten.Controllers;
using Empire_Rewritten.Settlements;
using RimWorld;

namespace Empire_Rewritten.Player
{
    public class UserPlayer : BasePlayer
    {
        private Empire cachedManager;
        private bool managerIsDirty;

        public UserPlayer(Faction faction) : base(faction) { }

        public Empire Manager
        {
            get
            {
                if (cachedManager == null || managerIsDirty)
                {
                    managerIsDirty = false;
                    UpdateController updateController = UpdateController.CurrentWorldInstance;
                    FactionController factionController = updateController.FactionController;

                    cachedManager = factionController.GetOwnedSettlementManager(faction);
                }

                return cachedManager;
            }
        }

        public override void MakeMove(FactionController factionController)
        {
            throw new NotImplementedException();
        }

        public override void MakeThreadedMove(FactionController factionController)
        {
            throw new NotImplementedException();
        }

        public override bool ShouldExecute()
        {
            return false;
        }

        public override bool ShouldExecuteThreaded()
        {
            return false;
        }
    }
}