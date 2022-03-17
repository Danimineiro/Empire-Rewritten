using JetBrains.Annotations;
using UnityEngine;
using Verse;

namespace Empire_Rewritten.Windows.OverviewTabs
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
    public class MainTab : EmpireWindowTab
    {
        public MainTab([NotNull] EmpireMainTabDef def) : base(def) { }

        public override void Draw(Rect inRect)
        {
            GUI.BeginGroup(inRect);
            float curY = 0;
            // TODO: Remove this text, or replace with localized version
            Widgets.Label(0, ref curY, inRect.width, Find.FactionManager.OfPlayer?.NameColored, new TipSignal("The name of your faction"));
            GUI.EndGroup();
        }
    }
}
