using System.Collections.Generic;
using System.Linq;
using Empire_Rewritten.Windows.MainTabWindowTabs;
using Empire_Rewritten.Utils;
using JetBrains.Annotations;
using RimWorld;
using UnityEngine;
using Verse;

namespace Empire_Rewritten.Windows
{
    [UsedImplicitly]
    public class EmpireMainTabWindow : MainTabWindow
    {
        private readonly Rect rectFull = new Rect(0f, 0f, 350f, 500f);
        private readonly Rect rectMain;
        private readonly Rect rectMenuBar;
        private readonly Rect[] rectMenuList;

        private readonly List<MainTabWindowTabDef> sortedTabs;

        private const float margin = 5f;
        private const float buttonHeight = 30f;

        private BaseMainTabWindowTab selectedTab;

        public EmpireMainTabWindow()
        {
            closeOnClickedOutside = true;
            sortedTabs = DefDatabase<MainTabWindowTabDef>.AllDefs.OrderBy(tabDef => tabDef.order).ToList();
            selectedTab = sortedTabs.FirstOrFallback()?.Tab;

            rectMain = rectFull.ContractedBy(margin);
            rectMenuBar = rectMain.TopPartPixels(buttonHeight);
            rectMenuList = rectMenuBar.DivideHorizontal(sortedTabs.Count, 5f).ToArray();
        }

        protected override float Margin => 0f;

        public override Vector2 RequestedTabSize => rectFull.size;

        public override void DoWindowContents(Rect inRect)
        {
            //Text.Font = GameFont.Medium;
            Text.Anchor = TextAnchor.MiddleCenter;

            for (int i = 0; i < sortedTabs.Count; i++)
            {
                Rect tempRect = rectMenuList[i];
                Widgets.Label(tempRect, sortedTabs[i].Tab.def.LabelCap);
                Widgets.DrawBox(tempRect, 3);
                if (Widgets.ButtonInvisible(tempRect))
                {
                    selectedTab = sortedTabs[i].Tab;
                }
                Widgets.DrawHighlight(tempRect);
                Widgets.DrawHighlightIfMouseover(tempRect);
            }

            Rect tabRect = inRect.ContractedBy(margin);
            tabRect.yMin += buttonHeight;

            WindowHelper.ResetTextAndColor();
            selectedTab?.Draw(tabRect);
        }
    }
}
