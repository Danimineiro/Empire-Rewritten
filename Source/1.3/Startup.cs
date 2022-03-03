using Empire_Rewritten.Controllers;
using Empire_Rewritten.HarmonyPatches;
using Empire_Rewritten.Utils.Text;
using JetBrains.Annotations;
using Verse;

namespace Empire_Rewritten
{
    /// <summary>
    ///     The class that should handle all our startup needs
    /// </summary>
    [StaticConstructorOnStartup]
    public static class Startup
    {
        /// <summary>
        ///     Gets called on startup
        /// </summary>
        [UsedImplicitly]
        static Startup()
        {
            Log.Message("[Empire Rewritten] just here to say hello! ^-^ Have a nice day and great fun with Empire!"
                            .Rainbowify(' ', 35));
            AppendActionsToWorldStart();
            HarmonyPatcher.DoPatches();
        }

        /// <summary>
        ///     Sets up necessary <see cref="System.Action{T}">Actions</see> to be run after world start.
        /// </summary>
        private static void AppendActionsToWorldStart()
        {
            Log.Message("<color=orange>[Empire]</color> Attaching actions to world start");
            UpdateController.AddFinalizeInitHook(AddFactionControllerIfMissing);
        }

        /// <summary>
        ///     Adds a <see cref="FactionController" /> to the <see cref="UpdateController" /> class, if not already set
        /// </summary>
        /// <param name="_">
        ///     The necessary <see cref="FactionController" /> parameter for
        ///     <see cref="UpdateController.AddFinalizeInitHook" />;
        ///     Unused here
        /// </param>
        private static void AddFactionControllerIfMissing(FactionController _)
        {
            if (UpdateController.CurrentWorldInstance.HasFactionController) return;

            UpdateController.CurrentWorldInstance.FactionController =
                new FactionController(FactionSettlementData.CreateFactionSettlementData());
        }
    }
}