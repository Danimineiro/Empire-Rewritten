using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_Rewritten.AI
{
    public class AIPlayer
    {
        private AIResourceManager resourceManager;
        private Faction faction;

        public Faction Faction
        {
            get
            {
                return faction;
            }
        }

        public AIPlayer(Faction faction)
        {
            this.faction = faction;
            this.resourceManager = new AIResourceManager(this);
        }

        public AIResourceManager ResourceManager
        {
            get
            {
                return resourceManager;
            }
        }
    }
}
