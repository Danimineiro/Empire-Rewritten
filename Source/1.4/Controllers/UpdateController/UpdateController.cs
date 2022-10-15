using System;
using System.Collections.Generic;
using Empire_Rewritten.Events;
using Empire_Rewritten.Utils;
using JetBrains.Annotations;
using RimWorld.Planet;
using Verse;

namespace Empire_Rewritten.Controllers
{
    /// <summary>
    ///     The "Main" function of the mod, updates other modules and components occasionally
    /// </summary>
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
    public class UpdateController : WorldComponent
    {
        private static readonly List<Action<FactionController>> FinalizeInitHooks = new List<Action<FactionController>>();
        private readonly List<UpdateControllerAction> actions = new List<UpdateControllerAction>();

        private FactionController factionController;

       

        public UpdateController(World world) : base(world)
        {
            // TODO: Cry if someone installs SoS2
            CurrentWorldInstance = this;
        }

        /// <summary>
        ///     Whether <see cref="UpdateController.factionController" /> is set
        /// </summary>
        internal bool HasFactionController => factionController != null;

        /// <summary>
        ///     Sets the <see cref="UpdateController.factionController" />, if it doesn't exist already
        /// </summary>
        internal FactionController FactionController
        {
            set
            {
                if (HasFactionController)
                {
                    Logger.Warn("factionController is already set, skipping assignment");
                    return;
                }

                factionController = value;
            }
            get => factionController;
        }

        /// <summary>
        ///     Static instance of <see cref="UpdateController" />
        /// </summary>
        public static UpdateController CurrentWorldInstance { get; private set; }

        /// <summary>
        ///     Registers a new <see cref="UpdateControllerAction" />
        /// </summary>
        /// <param name="updateCall">
        ///     An <see cref="Action{T}" /> that gets called when the <see cref="UpdateControllerAction" /> should be executed.
        ///     Takes a single <see cref="FactionController" /> as parameter.
        /// </param>
        /// <param name="shouldExecute">
        ///     A <see cref="Func{T}" /> that returns whether the <see cref="UpdateControllerAction" />
        ///     should be executed.
        /// </param>
        public void AddUpdateCall([NotNull] Action<FactionController> updateCall, [NotNull] Func<bool> shouldExecute)
        {
            actions.Add(new UpdateControllerAction(updateCall, shouldExecute));
        }

        /// <summary>
        ///     Registers an <see cref="UpdateControllerAction" /> to be called as determined by its
        ///     <see cref="UpdateControllerAction.ShouldExecute" /> method
        /// </summary>
        /// <param name="action">The <see cref="UpdateControllerAction" /> to be added</param>
        public void AddUpdateCall([NotNull] UpdateControllerAction action)
        {
            actions.Add(action);
        }

        /// <summary>
        ///     Registers an <see cref="Action{T}" /> to be called after world generation
        /// </summary>
        /// <param name="action">
        ///     The <see cref="Action{T}" /> to call after world gen; takes a single
        ///     <see cref="FactionController" /> as parameter
        /// </param>
        public static void AddFinalizeInitHook([NotNull] Action<FactionController> action)
        {
            if (CurrentWorldInstance != null)
            {
                Logger.Warn("Tried to add a FinalizeInitHook after WorldComp was already created! Skipping...");
                return;
            }

            FinalizeInitHooks.Add(action);
        }

        /// <summary>
        ///     Registers multiple <see cref="Action{T}" /> to be called after world generation
        /// </summary>
        /// <param name="actions">
        ///     An <see cref="IEnumerable{T}" /> of <see cref="Action{T}" /> to call after world gen;
        ///     The actions take a single <see cref="FactionController" /> as parameter
        /// </param>
        public static void AddFinalizeInitHooks([NotNull] IEnumerable<Action<FactionController>> actions)
        {
            FinalizeInitHooks.AddRange(actions);
        }

        /// <summary>
        ///     Calls each registered <see cref="UpdateControllerAction" />, removing the ones that
        ///     <see cref="UpdateControllerAction.ShouldDiscard">should be discarded</see> afterwards
        /// </summary>
        public override void WorldComponentTick()
        {
            actions.RemoveAll(action =>
            {
                action.TryExecute(factionController, out bool shouldDiscard);
                return shouldDiscard;
            });
        }

        public override void FinalizeInit()
        {
            FinalizeInitHooks.ForEach(action => action(factionController));
        }

        public override void ExposeData()
        {
            Scribe_Deep.Look(ref factionController, "factionController");
        }
    }
}