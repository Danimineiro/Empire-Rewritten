using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Empire_Rewritten
{
    public class FacilityDef : Def
    {
        public Dictionary<ResourceDef, float> resourceMultipliers = new Dictionary<ResourceDef, float>();
        public Dictionary<ResourceDef,int> resourceOffsets = new Dictionary<ResourceDef, int>();

        private List<ResourceDef> producedResources = new List<ResourceDef>();
        public List<ResourceDef> ProducedResources
        {
            get
            {
                if (producedResources.NullOrEmpty())
                {
                    producedResources.AddRange(this.resourceMultipliers.Keys);
                    producedResources.AddRange(this.resourceOffsets.Keys);
                }
                return producedResources;
            }
        }

        public Type facilityWorker;

        public List<ThingDefCountClass> costList;

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
