using Empire_Rewritten.Territories;
using RimWorld;
using Verse;

namespace Empire_Rewritten.HarmonyPatches
{
    public static class TileInspectStringFaction
    {
        public static void AppendFactionToTileInspectString(ref string __result)
        {
            Faction tileOwner = TerritoryManager.GetTerritoryManager.GetTileOwner(Find.WorldSelector.selectedTile);
            if (tileOwner != null)
            {
                __result += "\n" + "Faction".TranslateSimple() + ": " + tileOwner.Name;
            }
        }
    }
}
