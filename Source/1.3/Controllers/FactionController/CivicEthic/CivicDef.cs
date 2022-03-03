using System.Collections.Generic;
using Empire_Rewritten.Utils.Misc;
using JetBrains.Annotations;
using RimWorld;
using Verse;

namespace Empire_Rewritten.Controllers.CivicEthic
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature | ImplicitUseKindFlags.Assign,
                    ImplicitUseTargetFlags.WithMembers)]
    public class CivicDef : Def
    {
        [NoTranslate] public readonly List<string> requiredEthicDefNames = new List<string>();

        public readonly List<string> requiredModIDs = new List<string>();

        public readonly bool requiresIdeology;
        public readonly bool requiresRoyalty;
        public List<EmpireStatModifier> statModifiers;

        /// <summary>
        ///     Whether all required Mods and DLCs are loaded and active
        /// </summary>
        public bool RequiredModsLoaded =>
            ModChecker.RequiredModsLoaded(requiredModIDs, requiresRoyalty, requiresIdeology);

        /// <summary>
        ///     Checks if a given <see cref="FactionCivicAndEthicData" /> has the prerequisite <see cref="EthicDef">EthicDefs</see>
        ///     for this <see cref="CivicDef" />
        /// </summary>
        /// <param name="data">The <see cref="FactionCivicAndEthicData" /> to check</param>
        /// <returns>Whether all required <see cref="EthicDef">EthicDefs</see> are present in <paramref name="data" /></returns>
        public bool HasRequiredEthics(FactionCivicAndEthicData data)
        {
            return requiredEthicDefNames.TrueForAll(name => data.Ethics.Exists(ethic => ethic.defName == name));
        }
    }

    [DefOf]
    public class CivicDefOf
    {
        [UsedImplicitly]
        static CivicDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(CivicDefOf));
        }
    }
}