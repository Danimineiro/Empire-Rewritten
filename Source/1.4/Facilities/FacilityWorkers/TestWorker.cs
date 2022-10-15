using System.Collections.Generic;
using Verse;

namespace Empire_Rewritten.Facilities.FacilityWorkers
{
    public class TestWorker : FacilityWorker
    {
        public TestWorker(FacilityDef facilityDef) : base(facilityDef) { }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (Gizmo gizmo in base.GetGizmos())
            {
                yield return gizmo;
            }
        }
    }
}