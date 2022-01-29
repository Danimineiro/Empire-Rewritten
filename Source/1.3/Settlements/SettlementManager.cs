using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Empire_Rewritten
{
    public class SettlementManager : IExposable, ILoadReferenceable
    {
        private static int i = -1;

        public void ExposeData()
        {
            Scribe_Values.Look<int>(ref i, "number");
        }

        public string GetUniqueLoadID()
        {
            i++;
            return $"SettlementManager_{i}";
        }
    }
}
