using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
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
        }
    }
}
