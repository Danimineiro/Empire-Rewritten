using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Empire_Rewritten
{
    public static class SettlementExtensions
    {
        public static Empire GetManager(this Settlement settlement)
        {
            UpdateController updateController = UpdateController.GetUpdateController;
            return updateController.FactionController.GetOwnedSettlementManager(settlement.Faction);
        }
        public static IEnumerable<Gizmo> GetExtendedGizmos(this Settlement settlement)
        {
            if (settlement.Faction == Faction.OfPlayer)
            {
                Empire manager = GetManager(settlement);
               IEnumerable<Gizmo> gizmos = manager.GetFacilityManager(settlement).GetGizmos();
                foreach (Gizmo gizmo in gizmos)
                {
                    if (gizmo != null)
                    {
                        yield return gizmo;
                    }
                }
            }
            yield break;
        }
    }
}
