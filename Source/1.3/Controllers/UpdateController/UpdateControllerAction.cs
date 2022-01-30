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

        /// <summary>
        /// Returns if the action should be executed
        /// </summary>
        public Func<bool> ShouldExecute => shouldExecute;

        /// <summary>
        /// Returns the action to be executed
        /// </summary>
        public Action<FactionController> Action => action;

        /// <summary>
        /// Constructs a new <c>UpdateControllerAction</c> that controls when a given <paramref name="action"/> should execute using the func <paramref name="shouldExecute"/>
        /// </summary>
        /// <param name="shouldExecute"></param>
        /// <param name="action"></param>
        public UpdateControllerAction(Func<bool> shouldExecute, Action<FactionController> action)
        {
            this.shouldExecute = shouldExecute;
            this.action = action;
        }
    }
}
