using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

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

        /// <summary>
        /// Merge two ResourceModifiers.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public ResourceModifier MergeWithModifier(ResourceModifier other)
        {
            if (other.def == this.def)
            {
                return new ResourceModifier(this.def, other.offset + this.offset,other.multiplier*this.multiplier);
            }
            else
            {
                Log.Error("[Empire]: Trying to merge two modifiers of seperate resource def types!");
                return this;
            }
        }

        /// <summary>
        /// NEEDS math changes.
        /// I know this is incorrect, but for some reason I cannot think of a correct way.
        /// </summary>
        /// <returns></returns>
        public float TotalProduced()
        {
            return (float)(this.multiplier+ this.offset);
        }
    }
}
