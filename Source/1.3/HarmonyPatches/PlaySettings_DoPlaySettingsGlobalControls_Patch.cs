using RimWorld;
using Verse;

namespace Empire_Rewritten.HarmonyPatches
{
    public static class PlaySettingsControlsPatch
    {
        private static bool _showBorders = true;

        public static bool ShowBorders => _showBorders;

        public static void Postfix(WidgetRow row, bool worldView)
        {
            if (worldView)
            {
                row.ToggleableIcon(ref _showBorders, TexButton.ShowExpandingIcons, "Toggle border view", SoundDefOf.Mouseover_ButtonToggle);
            }
        }
    }
}