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
        public virtual IEnumerable<Gizmo> GetGizmos()
        {
            return Enumerable.Empty<Gizmo>();
        }
    }
}