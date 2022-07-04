using System.Collections.Generic;
using System.Linq;
using Empire_Rewritten.Controllers;
using Empire_Rewritten.Facilities;
using Empire_Rewritten.Utils;
using Empire_Rewritten.Windows;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace Empire_Rewritten.Settlements
{
    public static class SettlementExtensions
    {
        /// <summary>
        ///     Gets the <see cref="Empire" /> of a given <see cref="Settlement" />.
        /// </summary>
        /// <param name="settlement">The <see cref="Settlement" /> to get the <see cref="Empire" /> of</param>
        /// <returns>The <see cref="Empire" /> of <paramref name="settlement" /></returns>
        public static Empire GetManager(this Settlement settlement)
        {
            return UpdateController.CurrentWorldInstance.FactionController.GetOwnedSettlementManager(settlement.Faction);
        }

        public static bool IsPlayerOperatedEmpireSettlement(this Settlement settlement)
        {
            List<FactionSettlementData> data = UpdateController.CurrentWorldInstance.FactionController.ReadOnlyFactionSettlementData;
            return data.Any(element => element.SettlementManager.Faction == settlement.Faction && element.SettlementManager.Settlements.ContainsKey(settlement));
        }

        /// <summary>
        ///     Shortcut for <see cref="GetManager(Settlement)"/> + <see cref="Empire.GetFacilityManager(Settlement)"/>
        /// </summary>
        /// <param name="settlement"></param>
        /// <returns></returns>
        public static FacilityManager GetFacilityManager (this Settlement settlement)
        {
            return settlement.GetManager().GetFacilityManager(settlement);
        }

        /// <summary>
        ///     Gets all <see cref="Gizmo">Gizmos</see> provided by <see cref="Facility">Facilities</see> of a given
        ///     <see cref="Settlement" />.
        /// </summary>
        /// <param name="settlement">The <see cref="Settlement" /> to get the <see cref="Gizmo">Gizmos</see> of</param>
        /// <returns>The <see cref="Gizmo">Gizmos</see> of <paramref name="settlement" /></returns>
        public static IEnumerable<Gizmo> GetExtendedGizmos(this Settlement settlement)
        {
            if (settlement.Faction == Faction.OfPlayer)
            {
                yield return SettlementInfoWindow.OpenOverviewGizmo(settlement);

                foreach (Gizmo gizmo in settlement.GetFacilityManager().GetGizmos())
                {
                    yield return gizmo;
                }
            }

            yield break;
        }
    }
}