using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Empire_Rewritten.Settlements;

namespace Empire_Rewritten
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
