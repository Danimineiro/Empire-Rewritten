using Verse;

namespace Empire_Rewritten.Resources.Stats
{
    public enum ResourceStat
    {
        Flat = 0,
        SmallHills = 1,
        LargeHills = 2,
        Mountainous = 3,
        Lake = 4,
        River = 5,
        Ocean = 6
    }

    public static class ResourceStatExtensions
    {
        public static string Translate(this ResourceStat stat, bool isOffset)
        {
            string translationPartOne = $"Empire_ResourceInfoWindow{stat.ToString().CapitalizeFirst()}".TranslateSimple();
            string translationPartTwo = $"Empire_ResourceInfoWindow{(isOffset ? "Offset" : "Factor")}".TranslateSimple();
            return $"{translationPartOne} {translationPartTwo}";
        }

        /// <param name="stat"></param>
        /// <returns>
        ///     Whether <paramref name="stat" /> is one of <see cref="ResourceStat.Lake" />, <see cref="ResourceStat.River" />, or
        ///     <see cref="ResourceStat.Ocean" />.
        /// </returns>
        public static bool IsWaterBody(this ResourceStat stat)
        {
            return stat > ResourceStat.Mountainous;
        }
    }
}