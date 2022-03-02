using System;
using Empire_Rewritten.Settlements;

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