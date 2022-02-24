using Empire_Rewritten.Resources;
using Empire_Rewritten.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Empire_Rewritten
{
    public class ResourceChange
    {
        public ResourceDef def;
        public float amount;
    }
    public class FacilityDef : Def
    {
        public List<ResourceChange> resourceMultipliers = new List<ResourceChange>();
        public List<ResourceChange> resourceOffsets = new List<ResourceChange>();

        private FacilityWorker worker;

        public FacilityWorker FacilityWorker
        {
            get
            {
                if(worker == null)
                {
                    worker = (FacilityWorker)Activator.CreateInstance(this.facilityWorker);
                    worker.facilityDef = this;
                }
                return worker;
            }
        }

        private List<ResourceDef> producedResources = new List<ResourceDef>();
        public List<ResourceDef> ProducedResources
        {
            get
            {
                if (producedResources.NullOrEmpty())
                {
                    List<ResourceDef> resourceDefs = new List<ResourceDef>();
                    foreach(ResourceChange change in this.resourceOffsets)
                    {
                        resourceDefs.Add(change.def);
                    }
                    foreach (ResourceChange change in this.resourceMultipliers)
                    {
                        resourceDefs.Add(change.def);
                    }
                }
                return producedResources;
            }
        }

        public Type facilityWorker;

        public List<ThingDefCountClass> costList;
        public readonly bool requiresIdeology = false;
        public readonly bool requiresRoyality = false;
        public readonly List<string> requiredModIDs = new List<string>();


        /// Returns if all required Mods are loaded
        public bool RequiredModsLoaded => ModChecker.RequiredModsLoaded(requiredModIDs, requiresRoyality, requiresIdeology);

        public override IEnumerable<string> ConfigErrors()
        {
            if (facilityWorker!=null && !facilityWorker.IsSubclassOf(typeof(FacilityWorker)))
            {
                yield return $"{facilityWorker} does not inherit from FacilityWorker!";
            }
            foreach(string str in base.ConfigErrors())
            {
                yield return str;
            }
        }
    }
}
