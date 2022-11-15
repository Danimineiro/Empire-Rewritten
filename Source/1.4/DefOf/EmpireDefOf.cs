using RimWorld;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Empire_Rewritten.DefOf
{
    public static class EmpireThingDefOf
    {
        static EmpireThingDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(ThingDefOf));

        public static ThingDef EmpireTaxSpot;
    }
}
