using System;
using Verse;

namespace Empire_Rewritten.Controllers
{
    public class UpdateControllerAction
    {
        private readonly Action<FactionController> action;
        private readonly Func<bool> shouldDiscardOverride;

        private readonly int maxExecutions;

        /// <summary>
        ///     How often this <see cref="UpdateControllerAction" /> has been executed so far.
        /// </summary>
        public int useCounter;

        /// <summary>
        ///     Constructs a new <see cref="UpdateControllerAction" /> that executes a given <see cref="Action{T}" /> if conditions
        ///     are met.
        ///     Will be discarded if <paramref name="shouldDiscardOverride" /> returns true
        /// </summary>
        /// <param name="shouldExecute">
        ///     The function that determines if the <see cref="UpdateControllerAction.action" /> should be
        ///     executed.
        /// </param>
        /// <param name="action">
        ///     The <see cref="Action{T}" /> to execute. Takes a single <see cref="FactionController" /> as
        ///     parameter.
        /// </param>
        /// <param name="shouldDiscardOverride">
        ///     Discards this <see cref="UpdateControllerAction" /> if it returns true. Runs every tick!
        /// </param>
        public UpdateControllerAction(Action<FactionController> action, Func<bool> shouldExecute, Func<bool> shouldDiscardOverride)
        {
            ShouldExecute = shouldExecute;
            this.action = action;
            useCounter = 0;
            this.shouldDiscardOverride = shouldDiscardOverride;
        }

        /// <summary>
        ///     Constructs a new <see cref="UpdateControllerAction" /> that controls when a given <paramref name="action" /> should
        ///     execute
        ///     Executes whenever <code>Find.TickManager.TicksGame % <paramref name="tickInterval" /> == 0</code>
        ///     Will be discarded if <paramref name="shouldDiscardOverride" /> returns true
        /// </summary>
        /// <param name="tickInterval"></param>
        /// <param name="action"></param>
        /// <param name="shouldDiscardOverride">
        ///     Discards this <see cref="UpdateControllerAction" /> if it returns true. Runs every tick!
        /// </param>
        public UpdateControllerAction(Action<FactionController> action, int tickInterval, Func<bool> shouldDiscardOverride)
        {
            ShouldExecute = () => Find.TickManager.TicksGame % tickInterval == 0;
            this.action = action;
            useCounter = 0;
            this.shouldDiscardOverride = shouldDiscardOverride;
        }

        /// <summary>
        ///     Constructs a new <see cref="UpdateControllerAction" /> that executes a given <see cref="Action{T}" /> if conditions
        ///     are met.
        /// </summary>
        /// <param name="shouldExecute">
        ///     The function that determines if the <see cref="UpdateControllerAction.action" /> should be
        ///     executed.
        /// </param>
        /// <param name="action">
        ///     The <see cref="Action{T}" /> to execute. Takes a single <see cref="FactionController" /> as
        ///     parameter.
        /// </param>
        /// <param name="maxExecutions">
        ///     The amount of executions before this <see cref="UpdateControllerAction" /> should be discarded.
        ///     If <c>&lt;= 0</c>, don't discard
        /// </param>
        public UpdateControllerAction(Action<FactionController> action, Func<bool> shouldExecute, int maxExecutions = 0)
        {
            ShouldExecute = shouldExecute;
            this.action = action;
            useCounter = 0;
            this.maxExecutions = maxExecutions;
        }

        /// <summary>
        ///     Constructs a new <see cref="UpdateControllerAction" /> that controls when a given <paramref name="action" /> should
        ///     execute
        ///     Executes whenever <code>Find.TickManager.TicksGame % <paramref name="tickInterval" /> == 0</code>
        ///     Will only execute a limited amount of times if <paramref name="maxExecutions" /> is set higher than 0
        /// </summary>
        /// <param name="tickInterval"></param>
        /// <param name="action"></param>
        /// <param name="maxExecutions"></param>
        public UpdateControllerAction(Action<FactionController> action, int tickInterval, int maxExecutions = 0)
        {
            ShouldExecute = () => Find.TickManager.TicksGame % tickInterval == 0;
            this.action = action;
            useCounter = 0;
            this.maxExecutions = maxExecutions;
        }

        /// <summary>
        ///     Decides if the <seealso cref="UpdateControllerAction.action" /> can be discarded
        ///     Uses <see cref="UpdateControllerAction.shouldDiscardOverride"/> if available
        /// </summary>
        private bool ShouldDiscard
        {
            get
            {
                if (shouldDiscardOverride != null)
                {
                    return shouldDiscardOverride();
                }

                return maxExecutions > 0 && useCounter >= maxExecutions;
            }
        }

        /// <summary>
        ///     The <see cref="Func{TResult}" /> that determines if the <see cref="UpdateControllerAction.action" /> should be
        ///     executed
        /// </summary>
        private Func<bool> ShouldExecute { get; }

        /// <summary>
        ///     Tries to run the <see cref="UpdateControllerAction.action" /> if conditions are met.
        /// </summary>
        /// <param name="factionController">
        ///     The <see cref="FactionController" /> to pass to the
        ///     <see cref="UpdateControllerAction.action" />
        /// </param>
        /// <param name="shouldDiscard">Whether this <see cref="UpdateControllerAction" /> should be discarded after this call</param>
        /// <returns>If the <see cref="UpdateControllerAction" /> was executed</returns>
        public bool TryExecute(FactionController factionController, out bool shouldDiscard)
        {
            bool shouldExecute = ShouldExecute();
            if (shouldExecute)
            {
                action(factionController);
                useCounter++;
            }

            shouldDiscard = ShouldDiscard;

            return shouldExecute;
        }

        public override string ToString()
        {
            string discardInfo = (shouldDiscardOverride == null) ? (maxExecutions == 0) ? "never discards" : $"maxExecutions: {maxExecutions}" : " uses discard override";

            return $"[{GetType().Name}] useCounter: {useCounter}, {discardInfo}, has shouldExecute: {ShouldExecute != null}, has action {action != null}";
        }
    }
}
