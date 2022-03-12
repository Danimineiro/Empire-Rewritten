using Empire_Rewritten.Territories;
using RimWorld;
using RimWorld.Planet;

namespace Empire_Rewritten.HarmonyPatches
{
    public static class SettleUtilityPatch
    {
        public static void Postfix(int tile, Faction faction, Settlement __result)
        {
            if (faction != Faction.OfPlayer)
            {
                return;
            }

            TerritoryManager.GetTerritoryManager.GetTerritory(faction).SettlementClaimTiles(__result);
        }
    }
}
