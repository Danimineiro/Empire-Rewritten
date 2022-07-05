
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_Rewritten.AI
{
    public abstract class AiDecisionWorker
    {
        private Settlement settlement;
        private bool SettlementLocked = false;

        public Settlement Settlement
        {
            get
            {
                return settlement;
            }
            set
            {
                if (!SettlementLocked)
                {
                    settlement = value;
                    SettlementLocked = true;
                }
            }
        }


        /// <summary>
        /// Does this Decision target a settlement?
        /// </summary>
        public virtual bool TargetsSetSettlement
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Get a settlement to modify.
        /// This is good for things such as upgrading facilities, specific settlements, etc.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public virtual Settlement GetSettlementToTarget(AIPlayer player, BasePlayer other = null)
        {
            return null;
        }

        /// <summary>
        /// Can the AI make this choice?
        /// </summary>
        /// <param name="player"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public abstract bool CanDecide(AIPlayer player, BasePlayer other = null);

        /// <summary>
        /// How tempting is it to do this?
        /// </summary>
        /// <param name="player"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public abstract float DecisionWeight(AIPlayer player, BasePlayer other = null);

        /// <summary>
        /// Code to run when choice is made.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="other"></param>
        public abstract void MakeDecision(AIPlayer player, BasePlayer other = null);

        /// <summary>
        /// The impact scale is as follows:
        /// 20+: Only for allies
        /// 0-20: Allies, neutrals
        /// 0-: Only hostile.
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public abstract float ImpactOnOtherEmpires(AIPlayer player);
    }

}
