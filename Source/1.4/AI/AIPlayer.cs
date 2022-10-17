using System.Threading.Tasks;
using Empire_Rewritten.Controllers;
using RimWorld;

namespace Empire_Rewritten.AI
{
    public class AIPlayer : BasePlayer
    {
        private int threadTick;

        private int tick;
        private AITileManager tileManager;

        public AIPlayer(Faction faction) : base(faction)
        {
            ResourceManager = new AIResourceManager(this);
            SettlementManager = new AISettlementManager(this);
            FacilityManager = new AIFacilityManager(this);
        }

        public AITileManager TileManager => tileManager ?? (tileManager = new AITileManager(this));

        public AISettlementManager SettlementManager { get; }
        public AIFacilityManager FacilityManager { get; }
        public AIResourceManager ResourceManager { get; }

        public override void MakeMove(FactionController factionController)
        {
            ResourceManager.DoModuleAction();
            FacilityManager.DoModuleAction();
            SettlementManager.DoModuleAction();
            TileManager.DoModuleAction();
        }

        public override bool ShouldExecute()
        {
            if (tick == 120)
            {
                tick = 0;
                return true;
            }

            tick++;
            return false;
        }

        public override void MakeThreadedMove(FactionController factionController)
        {
            Task.Run(SettlementManager.DoThreadableAction);
        }

        public override bool ShouldExecuteThreaded()
        {
            if (threadTick == 2)
            {
                threadTick = 0;
                return true;
            }

            threadTick++;
            return false;
        }
    }
}