using Empire_Rewritten.Deliveries;
using Empire_Rewritten.Events.Processes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Empire_Rewritten.Events.Processes
{
    public class DeliveryProcess : Process, IExposable
    {
        private DeliveryDef def;
        private Map map;
        private List<Thing> things;

        public DeliveryProcess(DeliveryDef def, int duration, Map map, List<Thing> things) : base(def.LabelCap, def.description, duration, def.IconPath)
        {
            this.def = def;
            this.map = map;
            this.things = things;
        }

        public DeliveryDef Def => def;

        public DeliveryWorker Worker => def.Worker;

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Defs.Look(ref def, nameof(def));
            Scribe_References.Look(ref map, nameof(map));
            Scribe_Collections.Look(ref things, nameof(things), LookMode.Deep);
        }

        protected override void Run()
        {
            if (def.Worker is DeliveryWorker worker)
            {
                worker.DeliverItemsTo(map, things);
            }
        }
    }
}
