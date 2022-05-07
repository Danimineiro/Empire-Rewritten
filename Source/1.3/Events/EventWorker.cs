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
        public virtual void Event(Empire empire)
        {

        }
    }
}
