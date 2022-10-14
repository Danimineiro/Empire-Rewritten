using System;
using JetBrains.Annotations;

namespace Empire_Rewritten.Resources
{
    /// <summary>
    ///     This is for use in defs, because structs cannot be loaded from XML. Holds a resource multiplier and offset value.
    /// </summary>
    [UsedImplicitly(ImplicitUseKindFlags.Assign, ImplicitUseTargetFlags.Members)]
    public class ResourceMod
    {
        public float multiplier = 1;
        public int offset;
    }

    /// <summary>
    ///     This is for use in code. Holds a resource multiplier and offset value.
    /// </summary>
    public struct ResourceModifier
    {
        public readonly ResourceDef def;
        public float offset;
        public float multiplier;

        public ResourceModifier(ResourceDef resourceDef, float offsetValue = 0, float multiplierValue = 1)
        {
            def = resourceDef;
            offset = offsetValue;
            multiplier = multiplierValue;
        }

        /// <summary>
        ///     Merges another <see cref="ResourceModifier" /> into this one.
        /// </summary>
        /// <param name="other">The <see cref="ResourceModifier" /> to merge into this one</param>
        /// <returns>A new <see cref="ResourceModifier" /> that is this one and <paramref name="other" /> combined</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     If <paramref name="other" />'s <see cref="ResourceModifier.def" /> is not
        ///     the same one as this one's
        /// </exception>
        public ResourceModifier MergeWithModifier(ResourceModifier other)
        {
            if (other.def == def)
            {
                return new ResourceModifier(def, other.offset + offset, other.multiplier * multiplier);
            }

            throw new ArgumentOutOfRangeException(nameof(other), "Trying to merge two modifiers of separate ResourceDef types");
            // Logger.Error("[Empire]: Trying to merge two modifiers of separate resource def types!");
            // return this;
        }

        // TODO: NEEDS math changes. I know this is incorrect, but for some reason I (Nesi) cannot think of a correct way.
        // Either <c>(1 + offset) * multiplier</c> or <c>(1 * multiplier) + offset</c> (<c>multiplier + offset</c>), based solely on the names? -Toby
        /// <summary>
        ///     Shortcut for (1 + offset) * multiplier
        /// </summary>
        /// <returns>(1 + offset) * multiplier</returns>
        public float TotalProduced()
        {
            return (1 + offset) * multiplier;
        }
    }
}