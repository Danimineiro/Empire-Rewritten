using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using Empire_Rewritten.Controllers;
using Empire_Rewritten.Utils;

namespace Empire_Rewritten.Events
{
    public class Process : IExposable
    {
        private string label;
        private string description;
        private string workerTypeName;
        private string nameSpace;
        private string assemblyName;
        private bool running = false;
        private int startTick;
        private int endTick;
        private Type worker;

        public string Label => label;
        public string Description => description;
        public string LabelCap => label.CapitalizeFirst();

        public Process(string label, string description, int startTick, int endTick, Type worker)
        {
            this.label = label;
            this.description = description;
            this.startTick = startTick;
            this.endTick = endTick;
            this.worker = worker;
            nameSpace = worker.Namespace;
            assemblyName = nameof(worker.Assembly);
            workerTypeName = nameof(worker);

            if (!worker.IsSubclassOf(typeof(ProcessWorker))) $"Worker {nameof(worker)} does not inherit from {typeof(ProcessWorker).Namespace}.{nameof(ProcessWorker)}!".ErrorOnce();
        }

        public float Progress => (Find.TickManager.TicksGame - startTick) / (endTick - startTick);

        private void Register()
        {
            if (running) return;

            UpdateController.CurrentWorldInstance.AddUpdateCall(new UpdateControllerAction((_) => worker.GetMethod("Run").Invoke(null, new object[] { }), Trigger, 1));

            running = true;
        }

        private bool Trigger()
        {
            if (Progress > 1f) return true;
            return false;
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref label, nameof(label));
            Scribe_Values.Look(ref description, nameof(description));
            Scribe_Values.Look(ref workerTypeName, nameof(workerTypeName));
            Scribe_Values.Look(ref nameSpace, nameof(nameSpace));
            Scribe_Values.Look(ref assemblyName, nameof(assemblyName));
            Scribe_Values.Look(ref startTick, nameof(startTick));
            Scribe_Values.Look(ref endTick, nameof(endTick));

            if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
                worker = Type.GetType($"{nameSpace}.{workerTypeName}, {assemblyName}");
            }
        }
    }

    public abstract class ProcessWorker
    {
        public abstract void Run();
    }
}
