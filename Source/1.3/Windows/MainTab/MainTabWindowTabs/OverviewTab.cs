using Empire_Rewritten.Controllers;
using Empire_Rewritten.Settlements;
using JetBrains.Annotations;
using RimWorld;
using UnityEngine;
using Verse;

namespace Empire_Rewritten.Windows.MainTabWindowTabs
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
    public class OverviewTab : BaseMainTabWindowTab
    {
        public OverviewTab([NotNull] MainTabWindowTabDef def) : base(def) { }

        public override void Draw(Rect inRect)
        {
            Empire playerController = UpdateController.CurrentWorldInstance.FactionController.GetOwnedSettlementManager(Faction.OfPlayer);
            GUI.BeginGroup(inRect);
            float curY = 0;

            // TODO: Remove this text, or replace with localized version
            DrawRow(ref curY, inRect.width, "Faction:", Faction.OfPlayer.NameColored);
            DrawRow(ref curY, inRect.width, "Owned Settlements:", playerController.Settlements.Count.ToString());
            DrawRow(ref curY, inRect.width, "Occupied Territory:", $"{playerController.Territory.Tiles.Count} tiles");

            GUI.EndGroup();
        }

        private static void DrawRow(ref float y, float width, TaggedString label, TaggedString content)
        {
            const float labelPercentage = 0.45f;
            float labelWidth = width * labelPercentage;
            float contentWidth = width - labelWidth;
            float rowHeight = Mathf.Max(Text.CalcHeight(label, labelWidth), Text.CalcHeight(content, contentWidth));

            Widgets.DrawHighlightIfMouseover(new Rect(0, y, width, rowHeight));
            Widgets.Label(new Rect(0, y, labelWidth, rowHeight), label);
            Widgets.Label(new Rect(labelWidth, y, contentWidth, rowHeight), content);

            y += rowHeight;
        }
    }
}
