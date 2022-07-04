using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Empire_Rewritten.Facilities;
using Verse;

namespace Empire_Rewritten.Events.Processes
{
    internal class FacilityBuildProcess : Process, ISlotID
    {
        private FacilityManager facilityManager;
        private FacilityDef facilityDef;
        private int slotID;

        public FacilityBuildProcess() { }

        public FacilityBuildProcess(string label, string toolTip, int duration, string iconPath, FacilityManager facilityManager, FacilityDef facilityDef, int slotID) : base(label, toolTip, duration, iconPath) 
        { 
            this.facilityManager = facilityManager; 
            this.facilityDef = facilityDef;
            this.slotID = slotID;
        }

        protected override object[] Parms => new object[] { facilityDef, facilityManager };

        public int SlotID { get => slotID; set => slotID = value; }

        public override void Cancel()
        {
            base.Cancel();
        }

        protected override void Run()
        {
            Run(facilityDef, facilityManager);
            facilityManager.NotifyProcessesChanged();
        }

        public static void Run(FacilityDef facilityDef, FacilityManager facilityManager)
        {
            if (facilityManager[facilityDef] is Facility facility)
            {
                facility.AddFacility();
            }
            else
            {
                facilityManager[facilityDef] = new Facility(facilityDef, facilityManager.Settlement);
            }
        }

        public override void ExposeData()
        {
            Scribe_References.Look(ref facilityManager, nameof(facilityManager));
            Scribe_Defs.Look(ref facilityDef, nameof(facilityDef));
            Scribe_Values.Look(ref slotID, nameof(slotID));
            base.ExposeData();
        }
    }
}
