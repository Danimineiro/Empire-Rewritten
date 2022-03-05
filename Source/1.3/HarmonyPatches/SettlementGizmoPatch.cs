using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Empire_Rewritten.HarmonyPatching
{
    public static class SettlementGizmoPatch
    {
        /// <summary>
        ///     is postfix
        /// </summary>
        public static void GizmoPatch(Settlement __instance, ref IEnumerable<Gizmo> __result)
        {
            __result.Concat(__instance.GetExtendedGizmos());
        }
    }
}
