using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Verse;

namespace Empire_Rewritten.Windows
{
    [UsedImplicitly(ImplicitUseKindFlags.Assign, ImplicitUseTargetFlags.WithMembers)]
    public class EmpireOverviewTabDef : Def
    {
        public int order;

        [Unsaved] private EmpireOverviewTab tab;

        public Type tabClass;
        public EmpireOverviewTab Tab => tab ?? (tab = (EmpireOverviewTab)Activator.CreateInstance(tabClass));

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
                yield return $"no tabClass set; It should be a subclass of {nameof(EmpireOverviewTab)}";
            }
            else if (!tabClass.IsSubclassOf(typeof(EmpireOverviewTab)))
            {
                yield return $"invalid tabClass {tabClass.ToStringSafe()} that is not a subclass of {nameof(EmpireOverviewTab)}";
            }
        }
    }
}
