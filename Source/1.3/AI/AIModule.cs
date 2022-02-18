using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_Rewritten.AI
{
    public abstract class AIModule
    {
        public  AIPlayer player;
        public AIModule(AIPlayer player)
        {
            this.player = player;
        }

        public abstract void DoModuleAction();
    }
}
