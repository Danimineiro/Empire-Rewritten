using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_Rewritten
{
    /// <summary>
    /// This is for use in defs, because structs cannot be loaded from XML. Holds a resource multiplier and offset value.
    /// </summary>
    public class ResourceMod
    {
        public float multiplier = 1;
        public int offset = 0;
    }


    /// <summary>
    /// This is for use in code. Holds a resource multiplier and offset value.
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
