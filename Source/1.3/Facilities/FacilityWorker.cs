using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Empire_Rewritten
{
    /// <summary>
    /// Used to define actions for facility types.
    /// </summary>
    public class FacilityWorker
    {
        public FacilityDef facilityDef;
        public virtual IEnumerable<Gizmo> GetGizmos()
        {
            yield return null;
        }

        public virtual bool CanBuildAt(FacilityManager manager)
        {
            return false;
        }
    }
}
