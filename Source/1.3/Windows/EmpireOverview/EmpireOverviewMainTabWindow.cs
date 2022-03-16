using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using UnityEngine;
using Verse;

namespace Empire_Rewritten.Windows
{
    [UsedImplicitly]
    public class EmpireOverviewMainTabWindow : MainTabWindow
    {
        private static List<EmpireOverviewTabDef> _sortedTabs;
        private EmpireOverviewTab selectedTab;

        public EmpireOverviewMainTabWindow()
        {
            closeOnClickedOutside = true;
        }

        public override Vector2 RequestedTabSize => new Vector2(350f, 400f);
        private static List<EmpireOverviewTabDef> SortedTabs => _sortedTabs ?? (_sortedTabs = DefDatabase<EmpireOverviewTabDef>.AllDefsListForReading.OrderByDescending(def => def.Weight()).ToList());

        public override void DoWindowContents(Rect inRect)
        {
            DrawTabbar(inRect.TopPartPixels(24f));
            if (selectedTab is null) return;

            selectedTab.Draw(new Rect(inRect.x, inRect.y + 24f, inRect.width, inRect.height));
        }

        private void DrawTabbar(Rect inRect)
        {
            WidgetRow row = new WidgetRow(inRect.x, inRect.y, UIDirection.RightThenDown, inRect.width, 0f);
            foreach (EmpireOverviewTabDef tab in SortedTabs)
            {
                if (row.ButtonText(tab.label, tab.description))
                {
                    selectedTab = (EmpireOverviewTab)Activator.CreateInstance(tab.tabClass);
                }
            }
        }
    }
}
