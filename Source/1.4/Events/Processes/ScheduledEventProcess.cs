using Empire_Rewritten.AI;
using Empire_Rewritten.Events;
using Empire_Rewritten.Events.Processes;
using Empire_Rewritten.Facilities;
using Empire_Rewritten.Settlements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Empire_Rewritten
{
    public class ScheduledEventProcess : Process
    {
        private EventDef def;
        public bool visible;

        /// <summary>
        /// Used for saving/loading.
        /// </summary>
        public ScheduledEventProcess() : base() { }

        /// <summary>
        ///     Default constructor, defining what <see cref="EventDef"/> <paramref name="def"/> to schedule, and how much time needs to pass before it fires.
        /// </summary>
        /// <param name="def"></param>
        /// <param name="duration"></param>
        public ScheduledEventProcess(EventDef def, int duration) 
            : base("Empire_SchedEvent_Label".Translate() + def.label, "Empire_SchedEvent_Description".Translate(def.label), duration, "UI/Overlays/Arrow")
        {
            this.def = def;
        }

        protected override void Run()
        {
            bool success = this.def.EventWorker.Event(Empire.PlayerEmpire);  
            if (!success)
            {

            }
        }

        public override void Cancel(CancelReason reason, string log = null, string alert = null)
        {
            if (!def.scheduled)
                base.Cancel(CancelReason.ErrorOccured, def.defName + " somehow snuck into the schedule, even though it's not scheduleable.");
            if (!def.playerCancelable && reason == CancelReason.CancelledByPlayer)
            {
                //Notify player that they're fucking up
            }
            def.EventWorker.CancelConsequence(reason);
            base.Cancel(reason, log, alert);
        }

        public override void ExposeData()
        {
            Scribe_Defs.Look(ref def, def.defName);
            base.ExposeData();
        }
    }
}
