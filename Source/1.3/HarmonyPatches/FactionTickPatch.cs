using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Empire_Rewritten.Borders;

namespace Empire_Rewritten.HarmonyPatches
{
    public static class FactionTickPatch
    {
        public static void Postfix(Faction __instance)
        {
            if (!(Find.TickManager.HasSettledNewColony || Find.TickManager.TicksGame > 10) && !__instance.IsPlayer)
                return;

            Settlement settlement = Find.WorldObjects.SettlementBases.First(x=>x.Faction== __instance);
            BorderManager.GetBorderManager.GetBorder(__instance).SettlementClaimTiles(settlement);
            
        }
    }
}
