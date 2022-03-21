using JetBrains.Annotations;
using UnityEngine;
using Verse;

namespace Empire_Rewritten.Windows
{
    public static class DebugActions
    {
        [PublicAPI]
        [DebugAction("Empire", "Resource info window", allowedGameStates = AllowedGameStates.Entry)]
        public static void DisplayResourceInfoWindow()
        {
            Find.WindowStack.Add(new ResourceInfoWindow());
        }

        [PublicAPI]
        [DebugAction("Empire", "Facility info window", allowedGameStates = AllowedGameStates.Entry)]
        public static void FacilityInfoWindow()
        {
            Find.WindowStack.Add(new FacilityInfoWindow());
        }

        [PublicAPI]
        [DebugAction("Empire", "Color picker window", allowedGameStates = AllowedGameStates.Entry)]
        public static void ColorPickerWindow()
        {
            Color color = Color.green;
            Color[] colors = new Color[10];
            Find.WindowStack.Add(new ColorPickerWindow(color, colors, (_) => { }, (_) => { }));
        }
    }
}