using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Empire_Rewritten.Utils
{
    public static class ModChecker
    {
        public static bool AreModsLoaded(List<string> ModIDs) => ModIDs.TrueForAll(mod => ModsConfig.IsActive(mod));

        public static bool RequiredModsLoaded(List<string> requiredModIDs, bool requiresRoyality = false, bool requiresIdeology = false)
        {
            return (ModsConfig.RoyaltyActive || !requiresRoyality) && (ModsConfig.IdeologyActive || !requiresIdeology) && AreModsLoaded(requiredModIDs);
        }
    }
}
