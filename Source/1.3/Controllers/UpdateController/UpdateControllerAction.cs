using System;

namespace Empire_Rewritten
{
    public struct UpdateControllerAction
    {
        /// <summary>
        /// The function that determines if a func should be executed
        /// </summary>
        private readonly Func<bool> shouldExecute;

        /// <summary>
        /// The action to be executed
        /// </summary>
        private readonly Action<FactionController> action;

        private readonly int useCounter;
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
        public Action<FactionController> Action => action;


        /// <summary>
        /// Constructs a new <c>UpdateControllerAction</c> that controls when a given <paramref name="action"/> should execute using the func <paramref name="shouldExecute"/>.
        /// Will only execute a limited amount of times if <paramref name="maxExecutions"/> is set higher than 0
        /// </summary>
        /// <param name="shouldExecute"></param>
        /// <param name="action"></param>
        /// <param name="maxExecutions"></param>
        public UpdateControllerAction(Func<bool> shouldExecute, Action<FactionController> action, int maxExecutions = 0)
        {
            this.shouldExecute = shouldExecute;
            this.action = action;
            useCounter = 0;
            this.maxExecutions = maxExecutions;
        }
    }
}
