using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Empire_Rewritten
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

        public SimpleCurve temperatureCurve;
        public SimpleCurve elevationAboveSeaLevelCurve;
        public SimpleCurve rainfallCurve;
        public SimpleCurve heightCurve;
        public SimpleCurve swampinessCurve;

        public List<BiomeModifier> biomeModifiers;
        public List<StuffCategoryDef> stuffCategories;
        public List<ThingCategoryDef> thingCategoryDefs;
        public List<ThingDef> allowedThingDefs;
        public List<ThingDef> postRemoveThingDefs;

        public bool getsOceanBonus;
        public bool getsRiverBonus;
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
                    stuffCategories?.ForEach(category => resourcesCreated.SetAllow(category, true));
                    thingCategoryDefs?.ForEach(category => resourcesCreated.SetAllow(category, true));
                    allowedThingDefs?.ForEach(thingDef => resourcesCreated.SetAllow(thingDef, true));
                    postRemoveThingDefs?.ForEach(thingDef => resourcesCreated.SetAllow(thingDef, false));
                    hasCachedThingDefs = true;
                }

                return resourcesCreated;
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
            float elevationVal = elevationAboveSeaLevelCurve.Evaluate(tile.elevation);
            float swampinessVal = swampinessCurve.Evaluate(tile.swampiness);
            float rainfallVal = rainfallCurve.Evaluate(tile.rainfall);
            ResourceModifier biomeModifier = GetBiomeModifier(tile);
            result *= (tempVal * heightVal*biomeModifier.multiplier * swampinessVal * rainfallVal * elevationVal);

            ResourceModifier modifer = new ResourceModifier(this, biomeModifier.offset, result);
            

            return modifer;
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
    }
}
