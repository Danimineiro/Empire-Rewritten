﻿using System.Linq;
using Verse;

namespace Empire_Rewritten
{
    public class ResourceWorker_Rancher : ResourceWorker
    {
        public ResourceWorker_Rancher(ThingFilter filter) : base(filter) { }

        /// <summary>
        /// Allows all animals marked as Roamers to be produced through ranching
        /// </summary>
        /// <returns></returns>
        public override ThingFilter PostModifyThingFilter()
        {
            foreach(PawnKindDef def in DefDatabase<PawnKindDef>.AllDefsListForReading.Where(def => def.RaceProps.Roamer))
            {
                filter.SetAllow(def.race, true);
            }

            return filter;
        }
    }
}
