using Empire_Rewritten.Utils;
using JetBrains.Annotations;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using Verse;

namespace Empire_Rewritten.Resources.Stats
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature | ImplicitUseKindFlags.Assign, ImplicitUseTargetFlags.WithMembers)]
    public class WaterBodyValues : IResourceValues
    {
        public float lake;
        public float ocean;
        public float river;

        /// <summary>
        ///     Gets the <see cref="float">value</see> of a given <see cref="ResourceStat" />
        /// </summary>
        /// <param name="stat">The <see cref="ResourceStat" /> to get the Value of</param>
        /// <returns>The corresponding <see cref="float">value</see> of <paramref name="stat" /></returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     If <paramref name="stat" /> is not one of
        ///     <see cref="ResourceStat.Lake" />, <see cref="ResourceStat.River" />, <see cref="ResourceStat.Ocean" />
        /// </exception>
        public float GetValue(ResourceStat stat)
        {
            switch (stat)
            {
                case ResourceStat.Lake:
                    return lake;
                case ResourceStat.River:
                    return river;
                case ResourceStat.Ocean:
                    return ocean;
                case ResourceStat.Flat:
                case ResourceStat.SmallHills:
                case ResourceStat.LargeHills:
                case ResourceStat.Mountainous:
                    throw new ArgumentOutOfRangeException(nameof(stat), stat, "Invalid ResourceStat, has to be a Water Body");
                default:
                    throw new ArgumentOutOfRangeException(nameof(stat), stat, "Invalid value");
            }
        }

        public float GetValueMult(Tile tile)
        {
            float value = 1;
            List<int> neighbourTiles = new List<int>();
            WorldGrid worldGrid = Find.WorldGrid;

            worldGrid.GetTileNeighbors(worldGrid.tiles.IndexOf(tile), neighbourTiles);

            if (!tile.Rivers.NullOrEmpty())
            {
                value *= river;
            }

            if (neighbourTiles.Any(neighbour => worldGrid.tiles[neighbour].biome == BiomeDefOf.Lake))
            {
                value *= lake;
            }

            if (neighbourTiles.Any(neighbour => worldGrid.tiles[neighbour].biome == BiomeDefOf.Ocean))
            {
                value *= ocean;
            }

            return value;
        }

        public float GetValueAdd(Tile tile)
        {
            float value = 0;
            List<int> neighbourTiles = new List<int>();
            WorldGrid worldGrid = Find.WorldGrid;

            worldGrid.GetTileNeighbors(worldGrid.tiles.IndexOf(tile), neighbourTiles);

            if (!tile.Rivers.NullOrEmpty())
            {
                value += river;
            }

            if (neighbourTiles.Any(neighbour => worldGrid.tiles[neighbour].biome == BiomeDefOf.Lake))
            {
                value += lake;
            }

            if (neighbourTiles.Any(neighbour => worldGrid.tiles[neighbour].biome == BiomeDefOf.Ocean))
            {
                value += ocean;
            }

            return value;
        }
    }
}