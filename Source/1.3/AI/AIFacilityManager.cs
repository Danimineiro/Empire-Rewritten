using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Empire_Rewritten.AI
{
    public class AIFacilityManager : AIModule
    {
        public AIFacilityManager(AIPlayer player) : base(player)
        {
          
        }

        public bool BuildNewFacility(FacilityDef facility)
        {
            bool hasRemovedAll = true;
            KeyValuePair<Settlement, FacilityManager> settlementAndManager = player.Manager.Settlements.First(x => x.Value.CanBuildAt(facility));
            FacilityManager manager = settlementAndManager.Value;
            Settlement settlement = settlementAndManager.Key;


            StorageTracker storageTracker = player.Manager.StorageTracker;
            foreach (ThingDefCountClass thingDefCount in facility.costList)
            {
                hasRemovedAll = hasRemovedAll && storageTracker.CanRemoveThingsFromStorage(thingDefCount.thingDef, thingDefCount.count);
            }
            if (hasRemovedAll)
            {
                foreach(ThingDefCountClass thingDefCountClass in facility.costList)
                {
                    storageTracker.RemoveThingsFromStorage(thingDefCountClass.thingDef, thingDefCountClass.count);
                }
                manager.AddFacility(facility);
            }

            
            return false;
        }
    }
}
