using Verse;

namespace Empire_Rewritten
{
    public class WaterBodyValues : IResourceStat
    {
        private const int mountainOffset = 4;
        public float lake;
        public float river;
        public float ocean;

        public float[] values;

        /// <param name="stat"></param>
        /// <returns>the <paramref name="stat"/> as a float</returns>
        public float GetValue(ResourceStat stat) => (values ?? (values = new float[3] { lake, river, ocean }))[(int)stat - mountainOffset];
    }
}
