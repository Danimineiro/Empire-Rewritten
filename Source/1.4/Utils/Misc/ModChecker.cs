using System.Collections.Generic;
using Verse;

namespace Empire_Rewritten.Utils
{
    public static class ModChecker
    {
        /// <summary>
        ///     Checks whether all mod IDs in a given <see cref="List{T}">List&lt;string&gt;</see> are active
        /// </summary>
        /// <param name="modIDs">A <see cref="List{T}">List&lt;string&gt;</see> of mod IDs to check</param>
        /// <returns>Whether all specified <paramref name="modIDs" /> are currently loaded and active</returns>
        public static bool AreModsLoaded(List<string> modIDs)
        {
            return modIDs.TrueForAll(ModsConfig.IsActive);
        }

        /// <summary>
        ///     Checks whether all mod IDs in a given <see cref="List{T}">List&lt;string&gt;</see>, as well as the specified DLCs
        ///     are active
        /// </summary>
        /// <param name="requiredModIDs">A <see cref="List{T}">List&lt;string&gt;</see> of mod IDs to check</param>
        /// <param name="requiresRoyalty">Whether the Royalty DLC is required</param>
        /// <param name="requiresIdeology">Whether the Ideology DLC is required</param>
        /// <returns>
        ///     Whether all specified <paramref name="requiredModIDs" />, as well as the specified DLCs are currently loaded
        ///     and active
        /// </returns>
        public static bool RequiredModsLoaded(List<string> requiredModIDs, bool requiresRoyalty = false, bool requiresIdeology = false)
        {
            return (ModsConfig.RoyaltyActive || !requiresRoyalty) && (ModsConfig.IdeologyActive || !requiresIdeology) && AreModsLoaded(requiredModIDs);
        }
    }
}