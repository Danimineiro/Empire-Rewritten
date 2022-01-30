using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Empire_Rewritten.AI
{
    public class AISettlementManager : AIModule
    {
        public AISettlementManager(AIPlayer player) : base(player)
        {
        }

        private bool canUpgradeOrBuild;

        public bool CanUpgradeOrBuild
        {
            get
            {
                return canUpgradeOrBuild;
            }
        }

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
            KeyValuePair<Settlement,FacilityManager> settlementAndManager = player.Manager.Settlements.Where(x => true /* !x.Value.IsFullyUpgraded*/).RandomElement();
            Settlement settlement = settlementAndManager.Key;
            FacilityManager facilityManager = settlementAndManager.Value;
            bool UpgradedSettlement = AttemptToUpgradeSettlement(facilityManager);
            bool BuiltSettlement = false;
            if (!UpgradedSettlement)
            {
                BuiltSettlement = AttemptBuildNewSettlement();
            }

            canUpgradeOrBuild = UpgradedSettlement || BuiltSettlement;
        }


        public override void DoModuleAction()
        {
            
        }
    }
}
