using Empire_Rewritten.Controllers;
using Empire_Rewritten.Settlements;
using RimWorld;

namespace Empire_Rewritten
{
    /// <summary>
    ///     The base player template
    /// </summary>
    public abstract class BasePlayer
    {
        private Empire cachedEmpire;
        public Faction Faction { get; }
        private bool cacheDirty = true;

        public BasePlayer(Faction faction)
        {
            Faction = faction;
            PlayerHandler.RegisterPlayer(this);
        }
        
        public Empire Empire
        {
            get
            {
                if (cachedEmpire != null && !cacheDirty) return cachedEmpire;
                
                cachedEmpire = UpdateController.CurrentWorldInstance.FactionController.GetOwnedSettlementManager(Faction);
                cacheDirty = false;

                return cachedEmpire;
            }
        }

        /// <summary>
        ///     Pass a move to be executed.
        /// </summary>
        public abstract void MakeMove(FactionController factionController);

        public abstract bool ShouldExecute();

        /// <summary>
        ///     Pass a move to be executed in a thread.
        /// </summary>
        /// <param name="factionController"></param>
        public abstract void MakeThreadedMove(FactionController factionController);

        public abstract bool ShouldExecuteThreaded();
    }
}