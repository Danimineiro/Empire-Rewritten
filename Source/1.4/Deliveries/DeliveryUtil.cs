using Empire_Rewritten.DefOf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Empire_Rewritten.Deliveries
{
    public static class DeliveryUtil
    {
        public static IntVec3? TryGetTaxSpotLocation(Map map) => map.listerThings.ThingsOfDef(EmpireThingDefOf.EmpireTaxSpot).FirstOrFallback()?.Position;
    }
}
