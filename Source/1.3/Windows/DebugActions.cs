using JetBrains.Annotations;
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
    }
}
