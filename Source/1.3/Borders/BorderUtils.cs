using Empire_Rewritten.HarmonyPatches;

namespace Empire_Rewritten.Borders
{
    public static class BorderUtils
    {
        public static float BorderAlpha => PlaySettingsControlsPatch.ShowBorders ? 0.8f : 0;
    }
}