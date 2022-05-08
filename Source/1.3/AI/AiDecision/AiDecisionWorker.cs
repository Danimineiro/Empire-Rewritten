
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_Rewritten.AI
{
    public class AiDecisionWorker
    {

        public virtual bool CanDecide(AIPlayer player, BasePlayer other = null)
        {
            return false;
        }

        public virtual float DecisionWeight(AIPlayer player, BasePlayer other = null)
        {
            return 0.0f;
        }

        public virtual void MakeDecision(AIPlayer player, BasePlayer other = null)
        {

        }
    }
}
