using Empire_Rewritten.Utils;
using Empire_Rewritten.Utils.Misc;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Empire_Rewritten
{
    public class EthicDef : Def
    {
        public List<EmpireStatModifier> statModifiers;
        public readonly List<Type> abilityWorkers;

        public readonly bool requiresIdeology = false;
        public readonly bool requiresRoyality = false;
        public readonly List<string> requiredModIDs = new List<string>();

        /// <summary>
        /// Returns if all required Mods are loaded
        /// </summary>
        public bool RequiredModsLoaded => ModChecker.RequiredModsLoaded(requiredModIDs, requiresRoyality, requiresIdeology);

        public Type facilityWorker;

        public override IEnumerable<string> ConfigErrors()
        {
            foreach (Type type in abilityWorkers)
            {
                if (!type.IsSubclassOf(typeof(EthicDef)))
                {
                    yield return $"{type} does not inherit from EthicAbilityWorker!";
                }
            }

            foreach (string str in base.ConfigErrors())
            {
                yield return str;
            }
        }
    }

    [DefOf]
    public class EthicDefOf
    {
        static EthicDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(EthicDefOf));
    }
}
