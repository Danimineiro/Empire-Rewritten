using Empire_Rewritten.Controllers;
using RimWorld;

namespace Empire_Rewritten
{
    /// <summary>
    ///     The base player template
    /// </summary>
    public abstract class BasePlayer
    {
        public Faction faction;

        public BasePlayer(Faction faction)
        {
            this.faction = faction;
            PlayerHandler.RegisterPlayer(this);
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