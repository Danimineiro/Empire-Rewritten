using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Empire_Rewritten.Deliveries
{
    public class DeliveryWorker
    {
        private Map PlayerMap => Find.AnyPlayerHomeMap;

        public void DeliverItemsTo(Map map, List<Thing> things)
        {
            map = map ?? PlayerMap;
            IntVec3 position = DeliveryUtil.TryGetTaxSpotLocation(map) ?? new IntVec3();

            foreach(Thing thing in things)
            {
                GenPlace.TryPlaceThing(thing, position, map, ThingPlaceMode.Near);
            }
        }
    }
}
