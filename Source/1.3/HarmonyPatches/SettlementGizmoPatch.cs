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
        public static void GizmoPatch(Settlement __instance, ref IEnumerable<Gizmo> __result)
        {
            List<Gizmo> gizmos = (List<Gizmo>)__result;   
            gizmos.AddRange(__instance.GetExtendedGizmos());
            __result = gizmos;
        }
    }
}
