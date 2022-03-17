using System.Collections.Generic;
using System.Linq;
using Empire_Rewritten.Windows.MainTabWindowTabs;
using JetBrains.Annotations;
using RimWorld;
using UnityEngine;
using Verse;

namespace Empire_Rewritten.Windows
{
    [UsedImplicitly]
    public class EmpireMainTabWindow : MainTabWindow
    {
        private readonly List<MainTabWindowTabDef> sortedTabs;

        private BaseMainTabWindowTab selectedTab;

        public EmpireMainTabWindow()
        {
            closeOnClickedOutside = true;
            sortedTabs = DefDatabase<MainTabWindowTabDef>.AllDefs.OrderBy(tabDef => tabDef.order).ToList();
            selectedTab = sortedTabs.FirstOrFallback()?.Tab;
        }

        public override Vector2 RequestedTabSize => new Vector2(350f, 400f);

        public override void DoWindowContents(Rect inRect)
        {
            const float buttonHeight = 30f;
            Text.Font = GameFont.Medium;
            if (Widgets.ButtonTextSubtle(inRect.TopPartPixels(buttonHeight), (selectedTab?.def.label ?? "Empire_SelectTab").TranslateSimple()))
            {
                FloatMenuUtility.MakeMenu(sortedTabs, tabDef => tabDef.label.TranslateSimple(), tabDef => delegate { selectedTab = tabDef.Tab; });
            }

            const float margin = 5f;
            Text.Anchor = TextAnchor.UpperLeft;
            Rect tabRect = inRect.ContractedBy(margin);
            tabRect.yMin += buttonHeight;
            selectedTab?.Draw(tabRect);
        }
    }
}
