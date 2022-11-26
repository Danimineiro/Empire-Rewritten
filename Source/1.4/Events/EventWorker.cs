using Empire_Rewritten.Settlements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_Rewritten.Events
{
    public class EventWorker
    {
        public EventDef def;
        public virtual float Chance
        {
            get
            {
                return 0f;
            }
        }
        /// <summary>
        /// The part that actually starts an Event. Returns true if the event fired successfully, else returns false.
        /// </summary>
        /// <param name="empire"></param>
        /// <returns></returns>
        public virtual bool Event(Empire empire)
        {
            if (TryToExecute())
                //...
                return true;
            else
                return false;
        }
        /// <summary>
        /// This fires if the event is cancelled. Also accepts a <see cref="CancelReason"/> for further logic.
        /// </summary>
        /// <param name="reason"></param>
        public virtual void CancelConsequence (CancelReason reason)
        {

        }

        /// <summary>
        /// This is a preliminary check to see whether all requirements that the event needs to run are fulfilled.
        /// </summary>
        /// <returns></returns>
        public virtual bool TryToExecute ()
        {
            return true;
        }

    }
}
