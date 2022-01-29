using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Empire_Rewritten
{
    public class CivicDef : Def
    {
        public float StatMultiplier;
        public float StatOffset;
    }

    [DefOf]
    public class CivicDefOf
    {
        static CivicDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(CivicDefOf));
    }

}
