using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_Rewritten { 
    /// <summary>
    /// The base player template
    /// </summary>
    public abstract class BasePlayer
    {
        public BasePlayer(Faction faction)
        {
            this.faction = faction;
            PlayerHandler.RegisterPlayer(this);
        }
        public Faction faction;
        /// <summary>
        /// Pass a move to be executed.
        /// </summary>
        public abstract void MakeMove(FactionController factionController);

        public abstract bool ShouldExecute();

        /// <summary>
        /// Pass a move to be executed in a thread.
        /// </summary>
        /// <param name="factionController"></param>
        public abstract void MakeThreadedMove(FactionController factionController);

        public abstract bool ShouldExecuteThreaded();
    }

    public static class PlayerHandler
    {
        private static List<BasePlayer> players = new List<BasePlayer>();
        private static bool hasStartedOnce = false;
        public static void Initalize(FactionController factionController)
        {
            hasRegisteredUser = false;
            players = new List<BasePlayer>();
            if (!hasStartedOnce)
            {
                UpdateController controller = UpdateController.GetUpdateController;

                controller.AddUpdateCall(MakeMoves, DoPlayerTick);
                controller.AddUpdateCall(MakeThreadedMoves, DoPlayerTick);
                controller.AddUpdateCall(RegisterPlayerFactionAsPlayer, ShouldRegisterPlayerFaction);
                hasStartedOnce = true;
            }
        }
        public static void RegisterPlayer(BasePlayer basePlayer)
        {
            players.Add(basePlayer);
        }

        private static int tick = 0;
        private static bool DoPlayerTick()
        {
            if (tick == 60)
            {
                tick = 0;
                return true;
            }
            tick++;
            return false;
        }

        public static void MakeMoves(FactionController controller)
        {
            for(int i = 0; i < players.Count; i++)
            {
                BasePlayer player = players[i];
                if (player.ShouldExecute())
                {
                    player.MakeMove(controller);
                }
            }
        }

        public static void MakeThreadedMoves(FactionController controller)
        {
            for (int i = 0; i < players.Count; i++)
            {
                BasePlayer player = players[i];
                void RunThreadedMove()
                {
                    if (player.ShouldExecuteThreaded())
                    {
                        player.MakeThreadedMove(controller);
                    }
                }
                Task.Run(RunThreadedMove);
            }
        }

        public static void RegisterPlayerFactionAsPlayer(FactionController factionController)
        {
            factionController.CreatePlayer();
        }

        private static bool hasRegisteredUser = false;
        public static bool ShouldRegisterPlayerFaction()
        {
            bool result = !hasRegisteredUser;
            hasRegisteredUser = true;
            return result;
        }
    }
}
