using Verse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;

namespace Empire_Rewritten
{
    /// <summary>
    /// The class that should handle all out startup needs
    /// </summary>
    [StaticConstructorOnStartup]
    public static class Startup
    {
        /// <summary>
        /// Gets called on startup
        /// </summary>
        static Startup()
        {
            AppendCreateFactionDatasFunction();
        }

        /// <summary>
        /// If the <c>UpdateController</c> doesn't have a FactionController already,
        /// appends a function to <c>UpdateController.finalizeInitHooks</c> that creates FactionSettlementDatas for every faction, then gives that list to the FactionController
        /// </summary>
        private static void AppendCreateFactionDatasFunction()
        {
            UpdateController.AddFinalizeInitHook((controller) =>
            {
                if (UpdateController.GetUpdateController.HasFactionController) return;
                UpdateController.GetUpdateController.FactionController = new FactionController(FactionSettlementData.CreateFactionSettlementDatas());
            });
        }
    }
}
