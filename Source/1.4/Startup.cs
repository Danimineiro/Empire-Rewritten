using Empire_Rewritten.Controllers;
using Empire_Rewritten.HarmonyPatches;
using Empire_Rewritten.Utils;
using JetBrains.Annotations;
using RimWorld;
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
            Logger.Message("[Empire] just here to say hello! ^-^ Have a nice day and great fun with Empire!".Rainbowify(' ', 35));
            AppendActionsToWorldStart();
            HarmonyPatcher.DoPatches();
        }

        /// <summary>
        ///     Sets up necessary <see cref="System.Action{T}">Actions</see> to be run after world start.
        /// </summary>
        private static void AppendActionsToWorldStart()
        {
            Logger.Log("Attaching actions to world start");
            UpdateController.AddFinalizeInitHook(AddFactionControllerIfMissing);
            UpdateController.AddFinalizeInitHook(InitPlayerHandler);
            UpdateController.AddFinalizeInitHook(AddAIToNonPlayers);
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

            UpdateController.CurrentWorldInstance.FactionController = new FactionController(FactionSettlementData.CreateFactionSettlementData());
        }

        /// <summary>
        ///     Add AI to non-player factions
        /// </summary>
        private static void AddAIToNonPlayers(FactionController factionController)
        {
            foreach (Faction faction in Find.World.factionManager.AllFactions)
            {
                if (!faction.IsPlayer && !faction.Hidden)
                {
                    Logger.Log("Creating AI for faction: '" + faction.NameColored.Formatted());
                    factionController.CreateNewAIPlayer(faction);
                }
            }
        }

        private static void InitPlayerHandler(FactionController factionController)
        {
            PlayerHandler.Initialize(factionController);
        }
    }
}