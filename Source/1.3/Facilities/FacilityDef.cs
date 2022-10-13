using System;
using System.Collections.Generic;
using Empire_Rewritten.Resources;
using Empire_Rewritten.Utils;
using JetBrains.Annotations;
using Verse;

namespace Empire_Rewritten.Facilities
{
    public class ResourceChange
    {
        public float amount;
        public ResourceDef def;
    }

    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature | ImplicitUseKindFlags.Assign, ImplicitUseTargetFlags.WithMembers)]
    public class FacilityDef : Def
    {
        private readonly List<ResourceDef> producedResources = new List<ResourceDef>();
        public readonly List<string> requiredModIDs = new List<string>();

        public readonly bool requiresIdeology;
        public readonly bool requiresRoyalty;

        public int buildDuration;

        public List<ThingDefCountClass> costList;

        public Type facilityWorker;

        public GraphicData iconData;
        public List<ResourceChange> resourceMultipliers = new List<ResourceChange>();

        public List<ResourceChange> resourceOffsets = new List<ResourceChange>();

        private FacilityWorker worker;

        public FacilityWorker FacilityWorker
        {
            get
            {
                if (facilityWorker == null) return null;
                return worker ?? (worker = (FacilityWorker)Activator.CreateInstance(facilityWorker, this));
            }
        }

        /// <summary>
        ///     Maintains a cache of the <see cref="ResourceDef">ResourceDefs</see> that are produced by this
        ///     <see cref="FacilityDef" />
        /// </summary>
        public List<ResourceDef> ProducedResources
        {
            get
            {
                if (producedResources.NullOrEmpty())
                {
                    foreach (ResourceChange change in resourceOffsets)
                    {
                        producedResources.Add(change.def);
                    }

                    foreach (ResourceChange change in resourceMultipliers)
                    {
                        producedResources.Add(change.def);
                    }
                }

                return producedResources;
            }
        }

        /// <summary>
        ///     Whether or not all required mods and DLCs are installed and active.
        /// </summary>
        public bool RequiredModsLoaded => ModChecker.RequiredModsLoaded(requiredModIDs, requiresRoyalty, requiresIdeology);

        public override IEnumerable<string> ConfigErrors()
        {
            if (facilityWorker != null && !facilityWorker.IsSubclassOf(typeof(FacilityWorker)))
            {
                yield return $"{facilityWorker} does not inherit from FacilityWorker!";
            }

            foreach (string str in base.ConfigErrors())
            {
                yield return str;
            }
        }
    }
}