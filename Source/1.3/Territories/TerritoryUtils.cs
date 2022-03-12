using Empire_Rewritten.HarmonyPatches;

namespace Empire_Rewritten.Territories
{
    public static class TerritoryUtils
    {
        public static float TerritoryAlpha => PlaySettingsControlsPatch.ShowTerritories ? 0.8f : 0;
    }
}
