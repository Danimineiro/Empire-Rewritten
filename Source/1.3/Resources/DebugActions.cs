using JetBrains.Annotations;
using Verse;

namespace Empire_Rewritten.Resources
{
    public static class DebugActions
    {
        private static void ClearData()
        {
            DefDatabase<ResourceDef>.AllDefsListForReading.ForEach(def => def.ClearCachedData());
        }

        /// <summary>
        ///     Force-clears the caches of all <see cref="ResourceDef">ResourceDefs</see>
        /// </summary>
        [PublicAPI]
        [DebugAction("Empire", "Clear Cached Resources", allowedGameStates = AllowedGameStates.Entry)]
        public static void ClearDataEntry()
        {
            ClearData();
        }

        /// <summary>
        ///     Force-clears the caches of all <see cref="ResourceDef">ResourceDefs</see>
        /// </summary>
        [PublicAPI]
        [DebugAction("Empire", "Clear Cached Resources", allowedGameStates = AllowedGameStates.Playing)]
        public static void ClearDataPlaying()
        {
            ClearData();
        }
    }
}