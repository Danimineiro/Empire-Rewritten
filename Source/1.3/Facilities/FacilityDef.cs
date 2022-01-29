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

        public float GetResourceModifier(ResourceDef resourceDef)
        {
            if (resourceMultipliers.ContainsKey(resourceDef))
            {
                return resourceMultipliers[resourceDef];
            }
            //No effect on the resource
            return 1f;
        }



    }
}
