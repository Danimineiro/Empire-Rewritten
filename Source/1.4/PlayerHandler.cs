using System.Collections.Generic;
using System.Threading.Tasks;
using Empire_Rewritten.Controllers;

namespace Empire_Rewritten
{
    public static class PlayerHandler
    {
        private static List<BasePlayer> _players = new List<BasePlayer>();

        private static int _tick;

        private static bool _hasRegisteredUser;

        public static void Initialize(FactionController _)
        {
            _hasRegisteredUser = false;
            _players = new List<BasePlayer>();

            UpdateController controller = UpdateController.CurrentWorldInstance;

            controller.AddUpdateCall(MakeMoves, DoPlayerTick);
            controller.AddUpdateCall(MakeThreadedMoves, DoPlayerTick);
            controller.AddUpdateCall(RegisterPlayerFactionAsPlayer, ShouldRegisterPlayerFaction);
        }

        public static void RegisterPlayer(BasePlayer basePlayer)
        {
            _players.Add(basePlayer);
        }

        private static bool DoPlayerTick()
        {
            if (_tick == 2)
            {
                _tick = 0;
                return true;
            }

            _tick++;
            return false;
        }

        public static void MakeMoves(FactionController controller)
        {
            for (int i = 0; i < _players.Count; i++)
            {
                BasePlayer player = _players[i];
                if (player.ShouldExecute())
                {
                    player.MakeMove(controller);
                }
            }
        }

        public static void MakeThreadedMoves(FactionController controller)
        {
            for (int i = 0; i < _players.Count; i++)
            {
                BasePlayer player = _players[i];

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

        public static bool ShouldRegisterPlayerFaction()
        {
            bool result = !_hasRegisteredUser;
            _hasRegisteredUser = true;
            return result;
        }
    }
}
