using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using Empire_Rewritten.Controllers;
using Empire_Rewritten.Utils;
using UnityEngine;

namespace Empire_Rewritten.Events.Processes
{
    /// <summary>
    ///     This class is used to create an event which triggers at some point in the future, while providing information about its progress and while being save-able
    /// </summary>
    public abstract class Process : IExposable
    {
        private string label;
        private string toolTip;
        private string iconPath;
        private bool running = false;
        private bool canceled = false;
        protected bool suspended = true;
        private int workCompleted;
        private int duration;

        /// <summary>
        ///     The <see cref="Process"/> name
        /// </summary>
        public string Label => label;

        /// <summary>
        ///     Same as <see cref="Label"/>, but the first letter is capitalized
        /// </summary>
        public string LabelCap => label.CapitalizeFirst();

        /// <summary>
        ///     A tooltip that is shown when the user wants an explanation for this proces <see cref="Process"/>
        /// </summary>
        public string ToolTip => toolTip;

        /// <summary>
        ///     An <see cref="Texture2D"/> that is displayed whenever the <see cref="Process"/> is visualized somewhere
        /// </summary>
        public Texture2D Icon => ContentFinder<Texture2D>.Get(iconPath);

        /// <summary>
        ///     A percentage (0f to 1f) that shows how far the <see cref="Process"/> has advanced
        /// </summary>
        public float Progress => workCompleted / (float)duration;

        /// <summary>
        ///     Tells the <see cref="Process"/> to pause if it is set to true, also resumes it when it's false
        /// </summary>
        public bool Suspended { get => suspended; set => suspended = value; }
        
        /// <summary>
        ///     The amount of time in ticks the <see cref="Process"/> must have run before executing the <see cref="Run"/> function
        /// </summary>
        public int Duration => duration;

        /// <summary>
        ///     The amount of time in ticks the <see cref="Process"/> has run so far.
        /// </summary>
        public int WorkCompleted => workCompleted;

        /// <summary>
        ///     To be used during Saving/Loading only!
        /// </summary>
        public Process() { }

        /// <summary>
        ///     Creates a new <see cref="Process"/>
        /// </summary>
        /// <param name="label"></param>
        /// <param name="toolTip"></param>
        /// <param name="duration"></param>
        /// <param name="iconPath"></param>
        public Process(string label, string toolTip, int duration, string iconPath)
        {
            this.label = label;
            this.toolTip = toolTip;
            this.iconPath = iconPath;

            workCompleted = 0;
            this.duration = duration;

            Initialize();
        }

        /// <summary>
        ///     Marks the <see cref="Process"/> for deletion, effectively cancelling it
        /// </summary>
        public virtual void Cancel()
        {
            canceled = true;
        }

        /// <summary>
        ///     Initializes the <see cref="Process"/>, by adding it to the <see cref="UpdateController"/>
        /// </summary>
        private void Initialize()
        {
            if (running) return;

            UpdateController.CurrentWorldInstance.AddUpdateCall(new UpdateControllerAction((_) => Run(), Trigger, ShouldDiscard));

            running = true;
        }

        /// <summary>
        ///     Internal function for the <see cref="UpdateController"/> that informs it that this <see cref="Process"/> can be discarded of
        /// </summary>
        /// <returns>true if the <see cref="Process"/> has finished, or if it was canceled</returns>
        private bool ShouldDiscard() => Progress > 1f || canceled;

        /// <summary>
        ///     Function to be implemented which runs when the <see cref="Process"/> has finished
        /// </summary>
        protected abstract void Run();

        /// <summary>
        ///     Function that does the "work". Triggers the <see cref="Run"/> function when true
        /// </summary>
        /// <returns>true, when the <see cref="Progess"/> reaches completion, false when suspended or otherwise</returns>
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
            Scribe_Values.Look(ref iconPath, nameof(iconPath));
            Scribe_Values.Look(ref workCompleted, nameof(workCompleted));
            Scribe_Values.Look(ref duration, nameof(duration));
            Scribe_Values.Look(ref suspended, nameof(suspended));

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                Initialize();
            }
        }
    }

    /// <summary>
    ///     Interface that adds a SlotID
    /// </summary>
    public interface IProcessSlotID
    {
        int SlotID { get; set; }
    }
}
