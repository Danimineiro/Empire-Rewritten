using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Verse;

namespace Empire_Rewritten.Windows
{
    [UsedImplicitly(ImplicitUseKindFlags.Assign, ImplicitUseTargetFlags.WithMembers)]
    public class EmpireMainTabDef : Def
    {
        public int order;

        [Unsaved] private EmpireWindowTab tab;

        public Type tabClass;
        public EmpireWindowTab Tab => tab ?? (tab = (EmpireWindowTab)Activator.CreateInstance(tabClass, this));

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
                yield return $"no tabClass set; It should be a subclass of {nameof(EmpireWindowTab)}";
            }
            else if (!tabClass.IsSubclassOf(typeof(EmpireWindowTab)))
            {
                yield return $"invalid tabClass {tabClass.ToStringSafe()} that is not a subclass of {nameof(EmpireWindowTab)}";
            }
        }
    }
}
