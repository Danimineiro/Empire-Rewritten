using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Empire_Rewritten.Borders;
using RimWorld.Planet;

namespace Empire_Rewritten
{
    public class CivicWorker
    {
        public virtual float CalculateDistanceWeight(float distanceWeight)
        {
            return distanceWeight / 5;
        }

    }
}
