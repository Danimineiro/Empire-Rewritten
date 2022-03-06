using System;
using System.Collections.Generic;
using System.Linq;
using Empire_Rewritten.Utils;
using JetBrains.Annotations;
using RimWorld;
using Verse;

namespace Empire_Rewritten.Controllers.CivicEthic
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature | ImplicitUseKindFlags.Assign, ImplicitUseTargetFlags.WithMembers)]
    public class EthicDef : Def
    {
        public readonly List<Type> abilityWorkers;
        public readonly List<string> requiredModIDs = new List<string>();

        public readonly bool requiresIdeology;
        public readonly bool requiresRoyalty;

        public Type facilityWorker;
        public List<EmpireStatModifier> statModifiers;

        /// <summary>
        ///     Whether all required mods and DLCs are loaded and active
        /// </summary>
        public bool RequiredModsLoaded =>
            ModChecker.RequiredModsLoaded(requiredModIDs, requiresRoyalty, requiresIdeology);

        public override IEnumerable<string> ConfigErrors()
        {
            foreach (Type type in abilityWorkers.Where(type => !type.IsSubclassOf(typeof(EthicDef))))
            {
                yield return $"{type} does not inherit from EthicAbilityWorker!";
            }

            foreach (string str in base.ConfigErrors()) yield return str;
        }
    }

    [DefOf]
    public class EthicDefOf
    {
        [UsedImplicitly]
        static EthicDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(EthicDefOf));
        }
    }
}