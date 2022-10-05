using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Empire_Rewritten.Facilities;
using Verse;

namespace Empire_Rewritten.Events.Processes
{
    /// <summary>
    ///     A type of <see cref="Process"/> used to construct a new <see cref="Facility"/> inside a <see cref="FacilityManager"/>
    /// </summary>
    internal class FacilityBuildProcess : Process, IProcessSlotID
    {
        private FacilityManager facilityManager;
        private FacilityDef facilityDef;
        private int slotID;

        /// <summary>
        ///     Used for saving/loading
        /// </summary>
        public FacilityBuildProcess() : base() { }

        /// <summary>
        ///     Creates a new <see cref="FacilityBuildProcess"/>
        /// </summary>
        /// <param name="label"></param>
        /// <param name="toolTip"></param>
        /// <param name="duration"></param>
        /// <param name="iconPath"></param>
        /// <param name="facilityManager"></param>
        /// <param name="facilityDef"></param>
        /// <param name="slotID"></param>
        public FacilityBuildProcess(string label, string toolTip, int duration, string iconPath, FacilityManager facilityManager, FacilityDef facilityDef, int slotID) : base(label, toolTip, duration, iconPath) 
        { 
            this.facilityManager = facilityManager; 
            this.facilityDef = facilityDef;
            this.slotID = slotID;
        }

        /// <summary>
        ///     A number marking in which slot this <see cref="FacilityBuildProcess"/> works in
        /// </summary>
        public int SlotID { get => slotID; set => slotID = value; }

        /// <summary>
        ///     Overrides <see cref="Process.Run"/>; Builds a new <see cref="Facility"/>, or adds to it using <see cref="Facility.AddFacility"/> if it already exists, inside the given <see cref="FacilityManager"/> <paramref name="facilityManager"/> using the <see cref="FacilityDef"/> <paramref name="facilityDef"/>; Notifies the <see cref="facilityManager"/>
        /// </summary>
        protected override void Run()
        {
            if (facilityManager[facilityDef] is Facility facility)
            {
                facility.AddFacility();
            }
            else
            {
                facilityManager[facilityDef] = new Facility(facilityDef, facilityManager.Settlement);
            }
            facilityManager.NotifyProcessesChanged();
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
