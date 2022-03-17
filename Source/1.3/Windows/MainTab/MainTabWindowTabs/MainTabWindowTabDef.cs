using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Verse;

namespace Empire_Rewritten.Windows.MainTabWindowTabs
{
    [UsedImplicitly(ImplicitUseKindFlags.Assign, ImplicitUseTargetFlags.WithMembers)]
    public class MainTabWindowTabDef : Def
    {
        public int order;

        [Unsaved] private BaseMainTabWindowTab tab;

        public Type tabClass;
        public BaseMainTabWindowTab Tab => tab ?? (tab = (BaseMainTabWindowTab)Activator.CreateInstance(tabClass, this));

        public override IEnumerable<string> ConfigErrors()
        {
            foreach (string error in base.ConfigErrors())
            {
                yield return error;
            }

            if (label is null)
            {
                yield return "no label set; It is the name of the tab as presented in the MainTabWindow";
            }

            if (tabClass is null)
            {
                yield return $"no tabClass set; It should be a subclass of {nameof(BaseMainTabWindowTab)}";
            }
            else if (!tabClass.IsSubclassOf(typeof(BaseMainTabWindowTab)))
            {
                yield return $"invalid tabClass {tabClass.ToStringSafe()} that is not a subclass of {nameof(BaseMainTabWindowTab)}";
            }
        }
    }
}
