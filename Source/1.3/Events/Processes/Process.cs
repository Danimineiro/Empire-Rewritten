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
        protected bool suspended = true;
        private int workCompleted;
        private int duration;

        public string Label => label;
        public string ToolTip => toolTip;
        public string LabelCap => label.CapitalizeFirst();

        public Process() { }

        public Process(string label, string toolTip, int duration)
        {
            this.label = label;
            this.toolTip = toolTip;

            workCompleted = 0;
            this.duration = duration;

            Initialize();
        }

        public float Progress => workCompleted / (float)duration;

        protected virtual object[] Parms => new object[] { };

        public bool Suspended { get => suspended; set => suspended = value; }

        private void Initialize()
        {
            if (running) return;

            UpdateController.CurrentWorldInstance.AddUpdateCall(new UpdateControllerAction((_) => Run(), Trigger, ShouldDiscard));

            running = true;
        }

        private bool ShouldDiscard() => Progress > 1f;

        protected virtual void Run() => throw new NotImplementedException();

        private bool Trigger()
        {
            if (suspended) return false;

            workCompleted++;
            if (Progress > 1f) return true;

            return false;
        }

        public virtual void ExposeData()
        {
            Scribe_Values.Look(ref label, nameof(label));
            Scribe_Values.Look(ref toolTip, nameof(toolTip));
            Scribe_Values.Look(ref workCompleted, nameof(workCompleted));
            Scribe_Values.Look(ref duration, nameof(duration));
            Scribe_Values.Look(ref suspended, nameof(suspended));

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                Initialize();
            }
        }
    }
}
