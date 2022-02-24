using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Empire_Rewritten.Resources
{
    public class BiomeModifier : ResourceMod
    {
        public BiomeDef biome;
    }

    public class ResourceDef : Def
    {
        private Dictionary<BiomeDef, ResourceModifier> cachedBiomeModifiers = new Dictionary<BiomeDef, ResourceModifier>();

        public GraphicData iconData;

        public ThingFilter resourcesCreated = new ThingFilter();

        private ResourceWorker worker = null;
        public Type resourceWorker;

        public SimpleCurve temperatureCurve;
        public SimpleCurve rainfallCurve;
        public SimpleCurve heightCurve;
        public SimpleCurve swampinessCurve;

        public List<BiomeModifier> biomeModifiers;
        public List<StuffCategoryDef> stuffCategoryDefs;
        public List<StuffCategoryDef> removeStuffCategoryDefs;
        public List<ThingCategoryDef> thingCategoryDefs;
        public List<ThingCategoryDef> removeThingCategoryDefs;
        public List<ThingDef> allowedThingDefs;
        public List<ThingDef> postRemoveThingDefs;

        public HillinessValues hillinessFactors;
        public HillinessValues hillinessOffsets;

        public WaterBodyValues waterBodyFactors;
        public WaterBodyValues waterBodyOffsets;

        private bool hasCachedThingDefs = false;

        public Graphic Graphic => iconData.Graphic;

        /// <summary>
        /// Caches the resources created from this Resource Def inside a ThingFilter and returns it
        /// </summary>
        public ThingFilter ResourcesCreated
        {
            get
            {
                if (!hasCachedThingDefs)
                {
                    stuffCategoryDefs?.ForEach(category => resourcesCreated.SetAllow(category, true));
                    removeStuffCategoryDefs?.ForEach(category => resourcesCreated.SetAllow(category, false));

                    thingCategoryDefs?.ForEach(category => resourcesCreated.SetAllow(category, true));
                    removeThingCategoryDefs?.ForEach(category => resourcesCreated.SetAllow(category, false));

                    allowedThingDefs?.ForEach(thingDef => resourcesCreated.SetAllow(thingDef, true));
                    postRemoveThingDefs?.ForEach(thingDef => resourcesCreated.SetAllow(thingDef, false));

                    ResourceWorker?.PostModifyThingFilter();

                    hasCachedThingDefs = true;
                }

                return resourcesCreated;
            }
        }

        /// <summary>
        /// returns the defs ResourceWorker, if it has one
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
        /// Gets the modifier for the resource based on the tile
        /// </summary>
        /// <param name="tile"></param>
        /// <returns></returns>
        public ResourceModifier GetTileModifier(Tile tile)
        {
            float result = 1;

            float tempVal = temperatureCurve.Evaluate(tile.temperature);
            float heightVal = heightCurve.Evaluate((float)tile.hilliness);
            float swampinessVal = swampinessCurve.Evaluate(tile.swampiness);
            float rainfallVal = rainfallCurve.Evaluate(tile.rainfall);
            ResourceModifier biomeModifier = GetBiomeModifier(tile);
            result *= (tempVal * heightVal*biomeModifier.multiplier * swampinessVal * rainfallVal);

            ResourceModifier modifer = new ResourceModifier(this, biomeModifier.offset, result);


            return modifer;
        }

        /// <param name="stat"></param>
        /// <param name="isOffset"></param>
        /// <returns>the value of any given <paramref name="stat"/> and <paramref name="isOffset"/> combination</returns>
        public float GetBonus(ResourceStat stat, bool isOffset)
        {
            if (stat.IsWaterBody())
            {
                if (isOffset)
                {
                    return waterBodyOffsets.GetValue(stat);
                }

                return waterBodyFactors.GetValue(stat);
            }

            if (isOffset)
            {
                return hillinessOffsets.GetValue(stat);
            }

            return hillinessFactors.GetValue(stat);
        }

        public ResourceModifier GetBiomeModifier(Tile tile)
        {
            BiomeDef biome = tile.biome;
            if (biome != null && !cachedBiomeModifiers.ContainsKey(biome))
            {
                if (!biomeModifiers.NullOrEmpty() && biomeModifiers.Any(x => x.biome == biome))
                {
                    BiomeModifier biomeModifier = biomeModifiers.Find(x => x.biome == biome);
                    ResourceModifier modifier = new ResourceModifier(this, biomeModifier.offset, biomeModifier.multiplier); ;
                    cachedBiomeModifiers.Add(biome, modifier);
                }
                else
                {
                    cachedBiomeModifiers.Add(biome, new ResourceModifier(this, 0, 1));
                }
            }
            return cachedBiomeModifiers[biome];
        }

        public override IEnumerable<string> ConfigErrors()
        {
            if (resourceWorker != null && !resourceWorker.IsSubclassOf(typeof(ResourceWorker)))
            {
                yield return $"{resourceWorker} does not inherit from FacilityWorker!";
            }
            foreach (string str in base.ConfigErrors())
            {
                yield return str;
            }
        }
    }
}
