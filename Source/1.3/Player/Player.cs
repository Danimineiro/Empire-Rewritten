using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_Rewritten.Player
{
    public class UserPlayer : BasePlayer
    {
        public UserPlayer(Faction faction) : base(faction)
        {

        }
        private Empire cachedManager;
        private bool ManagerIsDirty;

        public Empire Manager
        {
            get
            {
                if (cachedManager == null || ManagerIsDirty)
                {

                    ManagerIsDirty = false;
                    UpdateController updateController = UpdateController.GetUpdateController;
                    FactionController factionController = updateController.FactionController;

                    cachedManager = factionController.GetOwnedSettlementManager(faction);
                }
                return cachedManager;
            }
        }

        public override void MakeMove(FactionController factionController)
        {
         //   throw new NotImplementedException();
        }

        public override void MakeThreadedMove(FactionController factionController)
        {
         //   throw new NotImplementedException();
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
