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
        private readonly FactionController factionController = new FactionController();
        private static readonly Dictionary<Action<FactionController>, int> updateFunctionDic = new Dictionary<Action<FactionController>,int>();

        /// <summary>
        /// Required Constructor
        /// </summary>
        /// <param name="world"></param>
        public UpdateController(World world) : base(world) 
        {
        }

        /// <summary>
        /// Registeres an <paramref name="updateCall"/> to be called every <paramref name="intervall"/>
        /// </summary>
        /// <param name="updateCall"></param>
        /// <param name="intervall"></param>
        public static void AddUpdateCall(Action<FactionController> updateCall, int intervall)
        {
            updateFunctionDic.Add(updateCall, intervall);
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
    }
}
