using HarmonyLib;
using RimWorld.Planet;
using Verse;

namespace Empire_Rewritten.HarmonyPatches
{
    public static class HarmonyPatcher
    {
        /// <summary>
        ///     Runs our harmony patches.
        /// </summary>
        public static void DoPatches()
        {
            var harmony = new Harmony("EmpireRewritten.HarmonyPatches");

            harmony.Patch(typeof(Settlement).GetMethod(nameof(Settlement.GetGizmos)), null, new HarmonyMethod(typeof(SettlementGizmoPatch), nameof(SettlementGizmoPatch.GizmoPatch)));

            Log.Message("<color=orange>[Empire]</color> Patches completed!");
        }
    }
}