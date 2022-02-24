namespace Empire_Rewritten.Resources
{
    public class HillinessValues : IResourceStat
    {
        public float flat;
        public float smallHills;
        public float largeHills;
        public float mountainous;

        private float[] values;

        /// <param name="stat"></param>
        /// <returns>the float value for the given <paramref name="stat"/></returns>
        public float GetValue(ResourceStat stat) => (values ?? (values = new float[4] { flat, smallHills, largeHills, mountainous }))[(int)stat];
    }
}