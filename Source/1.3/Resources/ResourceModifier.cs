using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_Rewritten
{
    /// <summary>
    /// this struct holds the offset and multipliers for a resource.
    /// </summary>
    public struct ResourceModifier
    {
        public ResourceDef def;
        public int offset;
        public float multiplier;

        public ResourceModifier(ResourceDef resourceDef, int offsetValue = 0, float multiplierValue = 1)
        {
            def = resourceDef;
            offset = offsetValue;
            multiplier = multiplierValue;
        }
    }
}
