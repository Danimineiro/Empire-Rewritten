using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_Rewritten
{
    public class EthicAbilityWorker
    {
        public virtual string GetActionNameTranslationKey()
        {
            return "Empire_UnimplementedEthicActionName";
        }

        public virtual string GetActionDescriptionTranslationKey()
        {
            return "Empire_UnimplementedEthicActionDescription";
        }

        public virtual Action<SettlementManager> ExecutedAction()
        {
            return null;
        }
    }
}
