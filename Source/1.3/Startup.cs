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
        /// Appends a function to <code>UpdateController.finalizeInitHooks</code> that creates FactionSettlementDatas for every faction, then gives that list to the FactionController
        /// </summary>
        private static void AppendCreateFactionDatasFunction()
        {
            UpdateController.AddFinalizeInitHook((controller) =>
            {
                List<FactionSettlementData> factionSettlementDatas = CreateFactionSettlementDatas();

                FactionController factionController = new FactionController(factionSettlementDatas);

            });
        }

        private static List<FactionSettlementData> CreateFactionSettlementDatas()
        {
            List<FactionSettlementData> factionSettlementDatas = new List<FactionSettlementData>();
            foreach (Faction faction in Find.FactionManager.AllFactionsListForReading)
            {
                factionSettlementDatas.Add(new FactionSettlementData(faction, new SettlementManager()));
            }

            return factionSettlementDatas;
        }
    }
}
