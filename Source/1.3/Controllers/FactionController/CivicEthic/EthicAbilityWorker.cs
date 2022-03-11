using System;
using Empire_Rewritten.Settlements;

// TODO: idk what this is supposed to do so I can't even pretend to document it -Toby

namespace Empire_Rewritten.Controllers.CivicEthic
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

        public virtual Action<Empire> ExecutedAction()
        {
            return null;
        }
    }
}