using RimWorld.Planet;
using System;
using System.Collections.Generic;
using Verse;

namespace Empire_Rewritten
{
    /// <summary>
    /// The "Main" function of the mod, updates other modules and components occasionally
    /// </summary>
    public class UpdateController : WorldComponent
    {
        private FactionController factionController;
        private static readonly List<UpdateControllerAction> actions = new List<UpdateControllerAction>();
        private static readonly List<Action<FactionController>> finalizeInitHooks = new List<Action<FactionController>>();
        private static UpdateController updateControllerCached;

        /// <summary>
        /// Required Constructor
        /// </summary>
        /// <param name="world"></param>
        public UpdateController(World world) : base(world) 
        {
            updateControllerCached = this;
        }

        /// <summary>
        /// Returns true if there is a FactionController, false otherwise
        /// </summary>
        internal bool HasFactionController => factionController != null;

        /// <summary>
        /// Sets the FactionController, if it doesn't exist already
        /// </summary>
        internal FactionController FactionController 
        {
            set
            {
                if (HasFactionController)
                {
                    Log.Error("factionController is already set, skipping assignment");
                    return;
                }

                factionController = value;
            }
            get
            {
                return factionController;
            }
        }

        public static UpdateController GetUpdateController => updateControllerCached;

        /// <summary>
        /// Registers an <paramref name="updateCall"/> to be called whenever <paramref name="shouldExecute"/> is true
        /// </summary>
        /// <param name="updateCall"></param>
        /// <param name="shouldExecute"></param>
        public static void AddUpdateCall(Action<FactionController> updateCall, Func<bool> shouldExecute)
        {
            actions.Add(new UpdateControllerAction(updateCall, shouldExecute));
        }

        /// <summary>
        /// Registers an <paramref name="action"/> to be called as determined by its internal functions
        /// </summary>
        /// <param name="action"></param>
        public static void AddUpdateCall(UpdateControllerAction action)
        {
            actions.Add(action);
        }

        /// <summary>
        /// Registers an <paramref name="action"/> to be called after world generation
        /// </summary>
        /// <param name="action"></param>
        public static void AddFinalizeInitHook(Action<FactionController> action)
        {
            finalizeInitHooks.Add(action);
        }

        /// <summary>
        /// Registers <paramref name="actions"/> to be called after world generation
        /// </summary>
        /// <param name="actions"></param>
        public static void AddFinalizeInitHooks(IEnumerable<Action<FactionController>> actions)
        {
            foreach (var action in actions) AddFinalizeInitHook(action);
        }

        /// <summary>
        /// Calls each registered Action when the current game tick is devisible by the int it was saved with
        /// </summary>
        public override void WorldComponentTick()
        {
            for (int i = 0; i < actions.Count; i++)
            {
                UpdateControllerAction action = actions[i];
                if (action.ShouldExecute.Invoke())
                {
                    action.Action.Invoke(factionController);
                }
            }

            actions.RemoveAll(action => action.ShouldDiscard);
        }

        public override void FinalizeInit()
        {
            finalizeInitHooks.ForEach(action => action.Invoke(factionController));
        }

        public override void ExposeData()
        {
            Scribe_Deep.Look(ref factionController, "factionController");
        }
    }
}
