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
        private const int TabBarHeight = 42;
        private const float TabBarItemGap = 4f;
        private readonly List<EmpireOverviewTabDef> sortedTabs;

        private readonly float tabBarWidth;
        private EmpireOverviewTab selectedTab;

        private Vector2 tabBarScrollPosition = Vector2.zero;

        public EmpireOverviewMainTabWindow()
        {
            closeOnClickedOutside = true;
            sortedTabs = DefDatabase<EmpireOverviewTabDef>.AllDefs.OrderBy(tabDef => tabDef.order).ToList();
            tabBarWidth = sortedTabs.Aggregate(-TabBarItemGap, (curWidth, tabDef) => curWidth + ButtonSize(tabDef.LabelCap).x);
            selectedTab = sortedTabs.FirstOrFallback()?.Tab;
        }

        public override Vector2 RequestedTabSize => new Vector2(350f, 400f);

        /// <summary>
        ///     Modified version of <see cref="WidgetRow.ButtonRect" />
        ///     Measures the size of a theoretical <see cref="WidgetRow.ButtonText" /> with a given <see cref="string" /> label
        /// </summary>
        /// <param name="label">The <see cref="string" /> content of the theoretical button</param>
        /// <returns>The <see cref="Vector2">size</see> of the button</returns>
        private static Vector2 ButtonSize(string label)
        {
            Vector2 vector = Text.CalcSize(label);
            vector.x += 16f + TabBarItemGap;
            vector.y += 2f;
            return vector;
        }

        public override void DoWindowContents(Rect inRect)
        {
            DrawTabBar(inRect.TopPartPixels(TabBarHeight));

            selectedTab?.Draw(new Rect(inRect.x, inRect.y + TabBarHeight, inRect.width, inRect.height - TabBarHeight));
        }

        private void DrawTabBar(Rect inRect)
        {
            GUI.BeginGroup(inRect);

            Rect viewRect = new Rect(0, 0, tabBarWidth, Text.CalcHeight("A", 10));
            Rect screenRect = new Rect(0, 0, inRect.width, inRect.height);

            Widgets.ScrollHorizontal(screenRect, ref tabBarScrollPosition, viewRect);
            Widgets.BeginScrollView(screenRect, ref tabBarScrollPosition, viewRect);

            WidgetRow row = new WidgetRow(0, 0, gap: TabBarItemGap);
            foreach (EmpireOverviewTabDef tab in sortedTabs)
            {
                if (row.ButtonText(tab.LabelCap, tab.description))
                {
                    selectedTab = tab.Tab;
                }
            }

            Widgets.EndScrollView();

            GUI.EndGroup();
        }
    }
}
