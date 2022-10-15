using System.Collections.Generic;
using System.Linq;
using Empire_Rewritten.Controllers;
using Empire_Rewritten.Facilities;
using Empire_Rewritten.Settlements;
using RimWorld.Planet;
using Verse;

namespace Empire_Rewritten.HarmonyPatches
{
    public static class SettlementGizmoPatch
    {
        /// <summary>
        ///     Adds <see cref="Facility" /> <see cref="Gizmo">Gizmos</see> to gizmos of a <see cref="Settlement" />.
        /// </summary>
        public static void GizmoPatch(Settlement __instance, ref IEnumerable<Gizmo> __result)
        {
            List<FactionSettlementData> data = UpdateController.CurrentWorldInstance.FactionController.ReadOnlyFactionSettlementData;
            if(data.Any(x=>x.SettlementManager.Faction==__instance.Faction && x.SettlementManager.Settlements.ContainsKey(__instance)))
                __result = __result.Concat(__instance.GetExtendedGizmos());
        }
    }
}