using System;
using System.Collections.Generic;
using Empire_Rewritten.Resources;
using Empire_Rewritten.Utils.Misc;
using JetBrains.Annotations;
using Verse;

namespace Empire_Rewritten.Facilities
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature | ImplicitUseKindFlags.Assign,
                    ImplicitUseTargetFlags.WithMembers)]
    public class FacilityDef : Def
    {
        private readonly List<ResourceDef> producedResources = new List<ResourceDef>();
        public readonly List<string> requiredModIDs = new List<string>();

        public readonly bool requiresIdeology;
        public readonly bool requiresRoyalty;

        public List<ThingDefCountClass> costList;

        public Type facilityWorker;
        public Dictionary<ResourceDef, float> resourceMultipliers = new Dictionary<ResourceDef, float>();
        public Dictionary<ResourceDef, int> resourceOffsets = new Dictionary<ResourceDef, int>();

        /// <summary>
        ///     Maintains a cache of the <see cref="ResourceDef">ResourceDefs</see> that are produced by this
        ///     <see cref="FacilityDef" />
        /// </summary>
        public List<ResourceDef> ProducedResources
        {
            get
            {
                if (producedResources.Any()) return producedResources;

                producedResources.AddRange(resourceMultipliers.Keys);
                producedResources.AddRange(resourceOffsets.Keys);

                return producedResources;
            }
        }

        /// <summary>
        ///     Whether or not all required mods and DLCs are installed and active.
        /// </summary>
        public bool RequiredModsLoaded =>
            ModChecker.RequiredModsLoaded(requiredModIDs, requiresRoyalty, requiresIdeology);

        public override IEnumerable<string> ConfigErrors()
        {
            if (facilityWorker != null && !facilityWorker.IsSubclassOf(typeof(FacilityWorker)))
            {
                yield return $"{facilityWorker} does not inherit from FacilityWorker!";
            }

            foreach (var str in base.ConfigErrors())
            {
                yield return str;
            }
        }
    }
}