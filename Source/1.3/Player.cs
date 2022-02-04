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
        }
        public Faction faction;
        /// <summary>
        /// Pass a move to be executed.
        /// </summary>
        public abstract void MakeMove(FactionController factionController);

        public abstract bool ShouldExecute();
    }
}
