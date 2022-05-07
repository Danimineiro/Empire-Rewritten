using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Empire_Rewritten.Events
{
    public class EventDef : Def
    {
        public Type eventWorker;

        private EventWorker worker;

      
        public EventWorker EventWorker
        {
            get
            {

                return worker;
            }
        }


        public bool canAffectAI = true;

        public Type aiEventWorker;

        private EventWorker aiWorker;

        public EventWorker AIWorker
        {
            get
            {
                return aiWorker!=null ? aiWorker : EventWorker;
            }
        }

        public override IEnumerable<string> ConfigErrors()
        {
            IEnumerable<string> errors = base.ConfigErrors();
            if(eventWorker.GetType() != typeof(EventWorker))
            {
                errors.Append($"{eventWorker.Name} is not a {nameof(EventWorker)}");
            }
            if (aiEventWorker!=null && aiEventWorker.GetType() != typeof(EventWorker))
            {
                errors.Append($"{aiEventWorker.Name} is not a {nameof(EventWorker)}");
            }

            return errors;
        }

        public override void ResolveReferences()
        {
            if (eventWorker.GetType() == typeof(EventWorker))
            {
                worker = (EventWorker)Activator.CreateInstance(eventWorker);
                worker.def = this;
            }
            if (aiEventWorker!=null && aiEventWorker.GetType() == typeof(EventWorker))
            {
                aiWorker = (EventWorker)Activator.CreateInstance(aiEventWorker);
                aiWorker.def = this;
            }
            base.ResolveReferences();
        }
    }
}
