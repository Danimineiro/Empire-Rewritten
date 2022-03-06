using System.Collections.Generic;
using Empire_Rewritten.Facilities;
using Empire_Rewritten.Utils.GUI;
using Empire_Rewritten.Utils.Misc;
using HarmonyLib;
using UnityEngine;
using Verse;

namespace Empire_Rewritten.Windows
{
    public class FacilityInfoWindow : Window
    {
        private readonly List<FacilityDef> facilityDefs;
        private readonly Rect rectContentMain;
        private readonly Rect rectContentMainLeft;
        private readonly Rect rectContentMainRight;
        private readonly Rect rectSelectionArea;
        private readonly Rect rectSelectionAreaFacilityIcon;
        private readonly Rect rectSelectionAreaFacilityName;
        private readonly Rect rectWindow = new Rect(0f, 0f, 1200f, 600f);

        private List<FloatMenuOption> cachedOptions;

        private FacilityDef defSelected;

        public FacilityInfoWindow()
        {
            facilityDefs = DefDatabase<FacilityDef>.AllDefsListForReading;
            Log.Message("<color=orange>[Empire]</color> FacilityInfoWindow.facilityDefs: " + facilityDefs.Join(def => def.defName));

            rectContentMain = rectWindow.ContractedBy(25f);

            rectContentMainLeft = rectContentMain.LeftHalf();
            rectContentMainLeft.width -= 3f;

            rectContentMainRight = rectContentMain.RightHalf();
            rectContentMainRight.width -= 2f;
            rectContentMainRight.x += 2f;

            rectSelectionArea = rectContentMainLeft.TopPartPixels(66f);
            rectSelectionAreaFacilityIcon = rectSelectionArea.LeftPartPixels(66f);
            rectSelectionAreaFacilityName = rectSelectionArea.RightPartPixels(rectSelectionArea.width - rectSelectionAreaFacilityIcon.width);
        }

        public override Vector2 InitialSize => rectWindow.size;

        protected override float Margin => 0f;

        /// <summary>
        ///     A list of defs to choose from
        /// </summary>
        private List<FloatMenuOption> DefOptions => cachedOptions ?? (cachedOptions = CreateFloatMenuOptions());

        public override void DoWindowContents(Rect inRect)
        {
            DrawCloseButton(inRect);
            DrawFacilitySelectorButton();

            Widgets.DrawBox(rectContentMain);
            Widgets.DrawBox(rectContentMainLeft);
            Widgets.DrawBox(rectContentMainRight);

            if (defSelected == null) return;

            Widgets.DrawBox(rectSelectionArea);
            Widgets.DrawBox(rectSelectionAreaFacilityIcon, 2);
        }

        private void DrawFacilitySelectorButton()
        {
            if (WindowHelper.DrawInfoScreenSelectorButton(rectSelectionAreaFacilityName, defSelected?.label ?? "Empire_FacilityInfoWindowSelector".Translate()))
            {
                Find.WindowStack.Add(new FloatMenu(DefOptions));
            }
        }

        /// <summary>
        ///     Makes a list out of all FacilityDefs to select from
        /// </summary>
        /// <returns>the list</returns>
        private List<FloatMenuOption> CreateFloatMenuOptions()
        {
            if (facilityDefs.NullOrEmpty())
            {
                Log.Warning("<color=orange>[Empire]</color> There are no FacilityDefs loaded");
                return new List<FloatMenuOption>();
            }

            return FloatMenuOptionCreator.CreateFloatMenuOptions(facilityDefs, def => defSelected = def);
        }

        private void DrawCloseButton(Rect inRect)
        {
            if (Widgets.CloseButtonFor(inRect)) Close();
        }
    }
}