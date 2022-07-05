using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Empire_Rewritten.AI
{
    public class AiDecisionDef : Def 
    {
        public Type aiDecisionWorker;
        private AiDecisionWorker worker;
        public AiDecisionWorker AiDecisionWorker => worker;

        public bool actsOnOtherEmpires;

        public override IEnumerable<string> ConfigErrors()
        {
            if (!typeof(AiDecisionWorker).IsAssignableFrom(aiDecisionWorker))
                yield return $"{aiDecisionWorker} is not assignable from AiDecisionWorker";

            foreach(string error in base.ConfigErrors())
                yield return error;
        }

        public override void ResolveReferences()
        {
            if (!typeof(AiDecisionWorker).IsAssignableFrom(aiDecisionWorker))
                worker = (AiDecisionWorker)Activator.CreateInstance(aiDecisionWorker);
            base.ResolveReferences();
        }
    }
}
