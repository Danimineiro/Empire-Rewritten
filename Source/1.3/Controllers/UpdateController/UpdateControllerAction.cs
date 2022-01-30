using System;
using Verse;

namespace Empire_Rewritten
{
    public class UpdateControllerAction
    {
        /// <summary>
        /// The function that determines if a func should be executed
        /// </summary>
        private readonly Func<bool> shouldExecute;

        /// <summary>
        /// The action to be executed
        /// </summary>
        private readonly Action<FactionController> action;

        public int useCounter;
        private readonly int maxExecutions;

        /// <summary>
        /// Decides if the Action can be discarded
        /// </summary>
        public bool ShouldDiscard => maxExecutions > 0 && useCounter >= maxExecutions;

        /// <summary>
        /// Returns if the action should be executed
        /// </summary>
        public Func<bool> ShouldExecute => shouldExecute;

        /// <summary>
        /// Returns the action to be executed
        /// </summary>
        public Action<FactionController> Action
        {
            get
            {
                useCounter++;
                return action;
            }
        }

        /// <summary>
        /// Constructs a new <c>UpdateControllerAction</c> that controls when a given <paramref name="action"/> should execute using the func <paramref name="shouldExecute"/>.
        /// Will only execute a limited amount of times if <paramref name="maxExecutions"/> is set higher than 0
        /// </summary>
        /// <param name="shouldExecute"></param>
        /// <param name="action"></param>
        /// <param name="maxExecutions"></param>
        public UpdateControllerAction(Action<FactionController> action, Func<bool> shouldExecute, int maxExecutions = 0)
        {
            this.shouldExecute = shouldExecute;
            this.action = action;
            useCounter = 0;
            this.maxExecutions = maxExecutions;
        }

        /// <summary>
        /// Constructs a new <c>UpdateControllerAction</c> that controls when a given <paramref name="action"/> should execute
        /// Executes whenever <code>Find.TickManager.TicksGame % <paramref name="tickInterval"/> == 0</code>
        /// Will only execute a limited amount of times if <paramref name="maxExecutions"/> is set higher than 0
        /// </summary>
        /// <param name="tickInterval"></param>
        /// <param name="action"></param>
        /// <param name="maxExecutions"></param>
        public UpdateControllerAction(Action<FactionController> action, int tickInterval, int maxExecutions = 0)
        {
            this.shouldExecute = () => Find.TickManager.TicksGame % tickInterval == 0;
            this.action = action;
            useCounter = 0;
            this.maxExecutions = maxExecutions;
        }

        public override string ToString() => $"[{this.GetType().Name}] useCounter: {useCounter}, maxExecutions: {maxExecutions}, has shouldExecute: {shouldExecute != null}, has action {action != null}";
    }
}
