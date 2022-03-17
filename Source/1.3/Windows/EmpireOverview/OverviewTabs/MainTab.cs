using JetBrains.Annotations;
using UnityEngine;
using Verse;

namespace Empire_Rewritten.Windows.OverviewTabs
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
    public class MainTab : EmpireOverviewTab
    {
        public override void Draw(Rect inRect)
        {
            GUI.BeginGroup(inRect);
            Widgets.DrawLineHorizontal(0, 0, inRect.width);
            float curY = 1;
            Widgets.Label(0, ref curY, inRect.width, Find.FactionManager.OfPlayer?.NameColored, new TipSignal("The name of your faction"));
            GUI.EndGroup();
        }
    }
}
