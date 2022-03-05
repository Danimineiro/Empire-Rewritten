using RimWorld;
using Verse;
namespace Empire_Rewritten.HarmonyPatching
{
    public static class PlaySettingsControlsPatch
    {
        public static void Postfix(WidgetRow row, bool worldView)
        {
            if(worldView)
                row.ToggleableIcon(ref showBorders, TexButton.ShowExpandingIcons, "Toggle border view", SoundDefOf.Mouseover_ButtonToggle, null);
        }

        private static bool showBorders = true;

        public static bool ShowBorders
        {
            get
            {
                return showBorders;
            }
        }
    }

    
}
