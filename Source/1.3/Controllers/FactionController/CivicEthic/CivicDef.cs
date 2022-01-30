using RimWorld;
using System.Collections.Generic;
using Verse;
using Empire_Rewritten.Utils;

namespace Empire_Rewritten
{
    public class CivicDef : Def
    {
        public List<EmpireStatModifier> statModifiers;

        public readonly bool requiresIdeology = false;
        public readonly bool requiresRoyality = false;
        public readonly List<string> requiredModIDs = new List<string>();

        [NoTranslate]
        public readonly List<string> requiredEthicDefNames = new List<string>();

        /// <summary>
        /// Returns if all required Ethics are present
        /// </summary>
        public bool HasRequiredEthics(FactionCivicAndEthicData data) => requiredEthicDefNames.TrueForAll((name) => data.Ethics.Exists((ethic) => ethic.defName == name));
        
        /// <summary>
        /// Returns if all required Mods are loaded
        /// </summary>
        public bool RequiredModsLoaded => ModChecker.RequiredModsLoaded(requiredModIDs, requiresRoyality, requiresIdeology);
    }

    [DefOf]
    public class CivicDefOf
    {
        static CivicDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(CivicDefOf));
    }
}
