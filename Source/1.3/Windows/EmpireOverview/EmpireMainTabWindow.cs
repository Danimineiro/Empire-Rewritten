using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using UnityEngine;
using Verse;

namespace Empire_Rewritten.Windows
{
    [UsedImplicitly]
    public class EmpireMainTabWindow : MainTabWindow
    {
        private readonly List<EmpireMainTabDef> sortedTabs;

        private EmpireWindowTab selectedTab;

        public EmpireMainTabWindow()
        {
            closeOnClickedOutside = true;
            sortedTabs = DefDatabase<EmpireMainTabDef>.AllDefs.OrderBy(tabDef => tabDef.order).ToList();
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

            Text.Anchor = TextAnchor.UpperLeft;
            selectedTab?.Draw(new Rect(inRect.x, inRect.y + buttonHeight, inRect.width, inRect.height - buttonHeight));
        }
    }
}
