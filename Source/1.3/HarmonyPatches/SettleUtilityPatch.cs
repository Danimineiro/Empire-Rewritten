using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Empire_Rewritten.Borders;
namespace Empire_Rewritten.HarmonyPatching
{
    public static class SettleUtilityPatch
    {
        public static void Postfix(int tile, Faction faction, Settlement __result)
        {
            if (faction != Faction.OfPlayer)
                return;
            BorderManager.GetBorderManager.GetBorder(faction).SettlementClaimTiles(__result);
        }
    }
}
