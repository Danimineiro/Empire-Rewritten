using Empire_Rewritten.Controllers;
using Empire_Rewritten.Settlements;
using JetBrains.Annotations;
using RimWorld;
using UnityEngine;
using Verse;
#if DEBUG
using System.Linq;
using System.Reflection;
#endif

namespace Empire_Rewritten.Windows.OverviewTabs
{
    [UsedImplicitly]
    public class StorageTab : EmpireWindowTab
    {
        private Vector2 scrollPosition = Vector2.zero;

        public StorageTab([NotNull] EmpireMainTabDef def) : base(def) { }

        public override void Draw(Rect inRect)
        {
            GUI.BeginGroup(inRect);
            Empire playerController = UpdateController.CurrentWorldInstance.FactionController.GetOwnedSettlementManager(Faction.OfPlayer);

#if DEBUG
            if (!playerController.StorageTracker.StoredThings.Any())
            {
                foreach (ThingDef thing in typeof(ThingDefOf).GetFields(BindingFlags.Public | BindingFlags.Static).Where(field => field.FieldType == typeof(ThingDef))
                                                             .Select(field => field.GetValue(null) as ThingDef))
                {
                    playerController.StorageTracker.AddThingsToStorage(thing, Random.Range(1000000, 9999999));
                }
            }
#endif

            float labelHeight = Text.CalcHeight("LiterallyAnyDefName", inRect.width);
            float curY = 0;
            Widgets.BeginScrollView(new Rect(0, curY, inRect.width, inRect.height - 1), ref scrollPosition,
                                    new Rect(0, 0, inRect.width - 20, playerController.StorageTracker.StoredThings.Count * labelHeight));
            foreach ((ThingDef storedThingDef, int count) in playerController.StorageTracker.StoredThings)
            {
                Rect rowRect = new Rect(0, curY, inRect.width - 20, labelHeight);
                Text.Anchor = TextAnchor.UpperLeft;
                Widgets.DefLabelWithIcon(rowRect, storedThingDef);
                Text.Anchor = TextAnchor.UpperRight;
                Widgets.Label(0, ref curY, rowRect.width, count.ToString());
            }

            Text.Anchor = TextAnchor.UpperLeft;

            Widgets.EndScrollView();

            GUI.EndGroup();
        }
    }
}
