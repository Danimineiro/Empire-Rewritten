using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Empire_Rewritten.Facilities
{
    /// <summary>
    ///     Used to define actions for facility types.
    /// </summary>
    public abstract class FacilityWorker
    {
        public FacilityDef facilityDef;

        public virtual IEnumerable<Gizmo> GetGizmos()
        {
            return Enumerable.Empty<Gizmo>();
        }

        public virtual bool CanBuildAt(FacilityManager manager)
        {
            return false;
        }
    }
}