using RimWorld;
using Verse;
namespace Empire_Rewritten
{
    public static class PlaySettingsControlsPatch
    {
        public static void Postfix(WidgetRow row, bool worldView)
        {
            if(worldView)
                row.ToggleableIcon(ref Borders.BorderUtils.enabled, TexButton.ShowExpandingIcons, "Toggle border view", SoundDefOf.Mouseover_ButtonToggle, null);
        }
    }

    
}
