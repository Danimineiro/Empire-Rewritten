using RimWorld.Planet;
using System;
using System.Collections.Generic;
using Verse;

namespace Empire_Rewritten
{
    /// <summary>
    /// The "Main" function of the mod, updates other modules and components occasionally
    /// </summary>
    public class UpdateController : WorldComponent, IExposable
    {
        private FactionController factionController;
        private static readonly Dictionary<Action<FactionController>, int> updateFunctionDic = new Dictionary<Action<FactionController>,int>();
        private static readonly List<Action<FactionController>> finalizeInitHooks = new List<Action<FactionController>>();

        /// <summary>
        /// Required Constructor
        /// </summary>
        /// <param name="world"></param>
        public UpdateController(World world) : base(world) 
        {
        }

        internal void RegisterFactionController(FactionController factionController)
        {
            this.factionController = factionController; 
        }

        /// <summary>
        /// Registers an <paramref name="updateCall"/> to be called every <paramref name="intervall"/>
        /// </summary>
        /// <param name="updateCall"></param>
        /// <param name="intervall"></param>
        public static void AddUpdateCall(Action<FactionController> updateCall, int intervall)
        {
            updateFunctionDic.Add(updateCall, intervall);
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
        /// Calls each registered Action when the current game tick is devisible by the int it was saved with
        /// </summary>
        public override void WorldComponentTick()
        {
            foreach (KeyValuePair<Action<FactionController>, int> keyValuePair in updateFunctionDic)
            {
                if (Find.TickManager.TicksGame % keyValuePair.Value == 0)
                {
                    keyValuePair.Key.Invoke(factionController);
                }
            }
        }

        public override void FinalizeInit()
        {
            finalizeInitHooks.ForEach(action => action.Invoke(factionController));
        }
    }
}
