using RimWorld;
using Verse;

namespace Empire_Rewritten.HarmonyPatches
{
    public static class PlaySettingsControlsPatch
    {
        private static bool _showTerritories = true;

        public static bool ShowTerritories => _showTerritories;

        public static void Postfix(WidgetRow row, bool worldView)
        {
            if (worldView)
            {
                row.ToggleableIcon(ref _showTerritories, TexButton.ShowExpandingIcons, "Empire_ToggleTerritoriesView".TranslateSimple(), SoundDefOf.Mouseover_ButtonToggle);
            }
        }
    }
}
