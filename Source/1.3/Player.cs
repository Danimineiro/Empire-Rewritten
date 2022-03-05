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
        }

        /// <summary>
        ///     Pass a move to be executed.
        /// </summary>
        public abstract void MakeMove();
    }
}