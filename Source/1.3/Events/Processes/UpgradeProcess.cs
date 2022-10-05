using Empire_Rewritten.Facilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Empire_Rewritten.Events.Processes
{
    /// <summary>
    ///     A <see cref="Process"/> that is used to upgrade a settlement to a higher stage
    /// </summary>
    internal class UpgradeProcess : Process
    {
        private FacilityManager facilityManager;

        /// <summary>
        ///     Used for saving/loading
        /// </summary>
        public UpgradeProcess() : base() { }

        /// <summary>
        ///     Default constructor, defining what <see cref="FacilityManager"/> <paramref name="facilityManager"/> to upgrade, and how long it should take to do so in ticks
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="facilityManager"></param>
        public UpgradeProcess(int duration, FacilityManager facilityManager) : base("Empire_UP_Label".Translate(), "Empire_UP_Desc".Translate(facilityManager.MaxFacilities + 1), duration, "UI/Overlays/Arrow") 
        {
            this.facilityManager = facilityManager;
        }

        protected override void Run()
        {
            facilityManager.ModifyStageBy(1);
            facilityManager.NotifyProcessesChanged();
        }

        public override void ExposeData()
        {
            Scribe_References.Look(ref facilityManager, nameof(facilityManager));
            base.ExposeData();
        }
    }
}
