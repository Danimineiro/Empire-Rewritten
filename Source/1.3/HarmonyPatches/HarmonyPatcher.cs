using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace Empire_Rewritten
{
    public static class HarmonyPatcher
    {
        /// <summary>
        /// Run our harmony patches.
        /// </summary>
        public static void DoPatches()
        {
            Harmony harmony = new Harmony("EmpireRewritten.HarmonyPatches");
            harmony.Patch(AccessTools.Method(typeof(Settlement), "GetGizmos"),postfix: new HarmonyMethod(typeof(SettlementGizmoPatch), "GizmoPatch"));
            harmony.Patch(AccessTools.Method(typeof(PlaySettings), "DoPlaySettingsGlobalControls"), postfix: new HarmonyMethod(typeof(PlaySettingsControlsPatch), "Postfix"));
            Log.Message("[Empire]: Patches completed!");
        }
    }
}
