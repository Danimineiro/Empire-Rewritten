using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Empire_Rewritten
{
    public class ResourceDef : Def
    {
        public GraphicData iconData;

        public Graphic Graphic
        {
            get { 
                return iconData.Graphic;
            }
        }

        public ThingFilter resourcesCreated;

        public SimpleCurve temperatureCurve;
        public SimpleCurve heightCurve;

        /// <summary>
        /// Gets the modifier for the resource based on the tile
        /// </summary>
        /// <param name="tile"></param>
        /// <returns></returns>
        public ResourceModifier GetTileModifier(Tile tile)
        {
            float result = 1;
            float tempVal = temperatureCurve.Evaluate(tile.temperature);
            float heightVal = heightCurve.Evaluate(tile.elevation);
            result *= (tempVal * heightVal);

            ResourceModifier modifer = new ResourceModifier(this, 0, result);
            

            return modifer;
        }
    }
}
