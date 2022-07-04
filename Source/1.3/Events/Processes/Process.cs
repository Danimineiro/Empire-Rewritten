using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using Empire_Rewritten.Controllers;
using Empire_Rewritten.Utils;

namespace Empire_Rewritten.Events.Processes
{
    public class Process : IExposable
    {
        private string label;
        private string toolTip;
        private bool running = false;
        protected bool suspended = false;
        private int startTick;
        private int endTick;

        public string Label => label;
        public string ToolTip => toolTip;
        public string LabelCap => label.CapitalizeFirst();

        public Process() { }

        public Process(string label, string toolTip, int duration)
        {
            this.label = label;
            this.toolTip = toolTip;

            startTick = Find.TickManager.TicksGame;
            endTick = Find.TickManager.TicksGame + duration;

            Register();
        }

        public float Progress => (Find.TickManager.TicksGame - startTick) / (float)(endTick - startTick);

        protected virtual object[] Parms => new object[] { };

        private void Register()
        {
            if (running) return;

            UpdateController.CurrentWorldInstance.AddUpdateCall(new UpdateControllerAction((_) => Run(), Trigger, ShouldDiscard));

            running = true;
        }

        private bool ShouldDiscard()
        {
            Log.Message($"is true { Progress > 1f}");
            return Progress > 1f;
        }

        protected virtual void Run()
        {
            throw new NotImplementedException();
        }

        private bool Trigger()
        {
            if (Progress > 1f) return true;
            return false;
        }

        public virtual void ExposeData()
        {
            Scribe_Values.Look(ref label, nameof(label));
            Scribe_Values.Look(ref toolTip, nameof(toolTip));
            Scribe_Values.Look(ref startTick, nameof(startTick));
            Scribe_Values.Look(ref endTick, nameof(endTick));
            Scribe_Values.Look(ref suspended, nameof(suspended));

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                Register();
            }
        }
    }
}
