using Empire_Rewritten.Resources;
using Verse;

namespace Empire_Rewritten.Resources
{
    public enum ResourceStat
    {
        flat,
        smallHills,
        largeHills,
        mountainous,
        lake,
        river,
        ocean
    }

    public static class ResourceStatExtensions
    {
        public static string Translate(this ResourceStat stat, bool isOffset)
        {
            return $"{$"Empire_ResourceInfoWindow{stat.ToString().CapitalizeFirst()}".Translate()} {$"Empire_ResourceInfoWindow{(isOffset ? "Offset" : "Factor")}".Translate()}";
        }

        /// <param name="stat"></param>
        /// <returns>true if <paramref name="stat"/> is either <c>lake</c>, <c>ocean</c> or <c>river</c>.</returns>
        public static bool IsWaterBody(this ResourceStat stat) => stat > ResourceStat.mountainous;
    }
}
