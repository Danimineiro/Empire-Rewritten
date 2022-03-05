using System;
using JetBrains.Annotations;

namespace Empire_Rewritten.Resources.Stats
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature | ImplicitUseKindFlags.Assign, ImplicitUseTargetFlags.WithMembers)]
    public class HillinessValues : IResourceValues
    {
        public float flat;
        public float largeHills;
        public float mountainous;
        public float smallHills;

        /// <summary>
        ///     Gets the <see cref="float">value</see> of a given <see cref="ResourceStat" />
        /// </summary>
        /// <param name="stat">The <see cref="ResourceStat" /> to get the Value of</param>
        /// <returns>The corresponding <see cref="float">value</see> of <paramref name="stat" /></returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     If <paramref name="stat" /> is not one of <see cref="ResourceStat.Flat" />, <see cref="ResourceStat.LargeHills" />,
        ///     <see cref="ResourceStat.Mountainous" />, <see cref="ResourceStat.SmallHills" />
        /// </exception>
        public float GetValue(ResourceStat stat)
        {
            switch (stat)
            {
                case ResourceStat.Flat:
                    return flat;
                case ResourceStat.SmallHills:
                    return smallHills;
                case ResourceStat.LargeHills:
                    return largeHills;
                case ResourceStat.Mountainous:
                    return mountainous;
                case ResourceStat.Lake:
                case ResourceStat.River:
                case ResourceStat.Ocean:
                    throw new ArgumentOutOfRangeException(nameof(stat), stat, "Invalid ResourceStat, has to be a terrain type");
                default:
                    throw new ArgumentOutOfRangeException(nameof(stat), stat, "Invalid value");
            }
        }
    }
}