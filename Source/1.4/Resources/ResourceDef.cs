using System;
using System.Collections.Generic;
using Empire_Rewritten.Resources.Stats;
using JetBrains.Annotations;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace Empire_Rewritten.Resources
{
    /// <summary>
    ///     TODO: Document
    /// </summary>
    public class BiomeModifier : ResourceMod
    {
        public BiomeDef biome;
    }

    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature | ImplicitUseKindFlags.Assign, ImplicitUseTargetFlags.WithMembers)]
    public class ResourceDef : Def
    {
        private readonly Dictionary<BiomeDef, ResourceModifier> cachedBiomeModifiers = new Dictionary<BiomeDef, ResourceModifier>();

        public List<ThingDef> allowedThingDefs;

        public List<BiomeModifier> biomeModifiers;

        /// <summary>
        ///     The AI will start trying to get rid of facilities and this resource if it produces more than this number.
        /// </summary>
        public int desiredAIMaximum = 50;

        /// <summary>
        ///     The AI will focus more on this resource if its income is below this amount.
        /// </summary>
        public int desiredAIMinimum = 30;

        private bool hasCachedThingDefs;
        public SimpleCurve heightCurve;

        public HillinessValues hillinessFactors;
        public HillinessValues hillinessOffsets;

        public GraphicData iconData;
        public List<ThingDef> postRemoveThingDefs;
        public SimpleCurve rainfallCurve;
        public List<StuffCategoryDef> removeStuffCategoryDefs;
        public List<ThingCategoryDef> removeThingCategoryDefs;

        public ThingFilter resourcesCreated = new ThingFilter();
        public Type resourceWorker;
        public List<StuffCategoryDef> stuffCategoryDefs;
        public SimpleCurve swampinessCurve;

        public SimpleCurve temperatureCurve;
        public List<ThingCategoryDef> thingCategoryDefs;

        public WaterBodyValues waterBodyFactors;
        public WaterBodyValues waterBodyOffsets;

        private ResourceWorker worker;

        public Graphic Graphic => iconData.Graphic;

        /// <summary>
        ///     Maintains a cached <see cref="ThingFilter" /> of the <see cref="ThingDef">resources</see> created from this
        ///     <see cref="ResourceDef" />
        /// </summary>
        public ThingFilter ResourcesCreated
        {
            get
            {
                if (hasCachedThingDefs) return resourcesCreated;

                resourcesCreated.SetDisallowAll();

                stuffCategoryDefs?.ForEach(category => resourcesCreated.SetAllow(category, true));
                removeStuffCategoryDefs?.ForEach(category => resourcesCreated.SetAllow(category, false));

                thingCategoryDefs?.ForEach(category => resourcesCreated.SetAllow(category, true));
                removeThingCategoryDefs?.ForEach(category => resourcesCreated.SetAllow(category, false));

                allowedThingDefs?.ForEach(thingDef => resourcesCreated.SetAllow(thingDef, true));
                postRemoveThingDefs?.ForEach(thingDef => resourcesCreated.SetAllow(thingDef, false));

                ResourceWorker?.PostModifyThingFilter();

                hasCachedThingDefs = true;

                return resourcesCreated;
            }
        }

        /// <summary>
        ///     Maintains a cached <see cref="ResourceWorker" /> of the <see cref="ResourceDef.resourceWorker">specified Type</see>
        /// </summary>
        public ResourceWorker ResourceWorker
        {
            get
            {
                if (resourceWorker == null) return null;
                return worker ?? (worker = (ResourceWorker)Activator.CreateInstance(resourceWorker, resourcesCreated));
            }
        }

        /// <summary>
        ///     Gets the <see cref="ResourceModifier" /> of a given <see cref="Tile" />
        /// </summary>
        /// <param name="tile">The <see cref="Tile" /> to get the <see cref="ResourceModifier" /> of</param>
        /// <returns>The <see cref="ResourceModifier" /> for <paramref name="tile" /></returns>
        public ResourceModifier GetTileModifier(Tile tile)
        {
            ResourceModifier biomeModifier = GetBiomeModifier(tile);

            if (tile.WaterCovered || tile.hilliness == Hilliness.Impassable) return new ResourceModifier(this, biomeModifier.offset, 0f);

            float result = 1;

            float tempVal = temperatureCurve.Evaluate(tile.temperature);
            float heightVal = heightCurve.Evaluate((float)tile.hilliness);
            float swampinessVal = swampinessCurve.Evaluate(tile.swampiness);
            float rainfallVal = rainfallCurve.Evaluate(tile.rainfall);

            float hillFacVal = hillinessFactors.GetValue((ResourceStat)(tile.hilliness - 1));
            float hillOffVal = hillinessOffsets.GetValue((ResourceStat)(tile.hilliness - 1));

            float waterFacVal = waterBodyFactors.GetValueMult(tile);
            float waterOffVal = waterBodyOffsets.GetValueAdd(tile);

            result = result * tempVal * heightVal * biomeModifier.multiplier * swampinessVal * rainfallVal * hillFacVal * waterFacVal + hillOffVal + waterOffVal;

            ResourceModifier modifer = new ResourceModifier(this, biomeModifier.offset, result);

            return modifer;
        }

        /// <summary>
        ///     Checks for the <see cref="float">resource production bonus</see> of a given <see cref="ResourceStat" />.
        ///     Will return offset value if <paramref name="isOffset" /> is true, otherwise the factor value.
        /// </summary>
        /// <param name="stat">The <see cref="ResourceStat" /> to get the bonus of</param>
        /// <param name="isOffset">Whether to get the offset value instead of the factor</param>
        /// <returns>The value of the given <paramref name="stat" /> and <paramref name="isOffset" /> combination</returns>
        public float GetBonus(ResourceStat stat, bool isOffset)
        {
            if (stat.IsWaterBody())
            {
                return isOffset ? waterBodyOffsets.GetValue(stat) : waterBodyFactors.GetValue(stat);
            }

            return isOffset ? hillinessOffsets.GetValue(stat) : hillinessFactors.GetValue(stat);
        }

        /// <summary>
        ///     Gets the <see cref="BiomeDef" />-based <see cref="ResourceModifier" /> of a given <see cref="Tile" />
        /// </summary>
        /// <param name="tile">The <see cref="Tile" /> to get the <see cref="ResourceModifier" /> of</param>
        /// <returns>
        ///     The <see cref="ResourceModifier" /> of the given <see cref="Tile" /> based on its <see cref="Tile.biome" />
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     if <paramref name="tile" />'s <see cref="Tile.biome" /> is <c>null</c>
        /// </exception>
        public ResourceModifier GetBiomeModifier(Tile tile)
        {
            BiomeDef biome = tile.biome;
            if (biome is null)
            {
                throw new ArgumentNullException(nameof(tile.biome));
            }

            if (cachedBiomeModifiers.ContainsKey(biome)) return cachedBiomeModifiers[biome];

            if (!biomeModifiers.NullOrEmpty() && biomeModifiers.Any(x => x.biome == biome))
            {
                BiomeModifier biomeModifier = biomeModifiers.Find(x => x.biome == biome);
                ResourceModifier modifier = new ResourceModifier(this, biomeModifier.offset, biomeModifier.multiplier);
                cachedBiomeModifiers.Add(biome, modifier);
                return modifier;
            }
            else
            {
                ResourceModifier modifier = new ResourceModifier(this);
                cachedBiomeModifiers.Add(biome, modifier);
                return modifier;
            }
        }

        public override void ClearCachedData()
        {
            cachedBiomeModifiers.Clear();
            hasCachedThingDefs = false;
            base.ClearCachedData();
        }

        public override IEnumerable<string> ConfigErrors()
        {
            if (resourceWorker != null && !resourceWorker.IsSubclassOf(typeof(ResourceWorker)))
            {
                yield return $"{resourceWorker} does not inherit from ResourceWorker!";
            }

            foreach (string str in base.ConfigErrors())
            {
                yield return str;
            }
        }
    }
}
