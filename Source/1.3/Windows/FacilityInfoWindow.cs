using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Empire_Rewritten.Utils;
using Empire_Rewritten.Utils.Misc;
using UnityEngine;
using Verse;

namespace Empire_Rewritten.Windows
{
    public class FacilityInfoWindow : Window
    {
        private readonly Rect RectWindow = new Rect(0f, 0f, 1200f, 600f);
        private readonly Rect RectContentMain;
        private readonly Rect RectContentMainLeft;
        private readonly Rect RectContentMainRight;
        private readonly Rect RectSelectionArea;
        private readonly Rect RectSelectionAreaFacilityIcon;
        private readonly Rect RectSelectionAreaFacilityName;

        private List<FloatMenuOption> cachedOptions = null;
        private List<FacilityDef> facilityDefs;

        private FacilityDef defSelected = null;

        public FacilityInfoWindow()
        {
            facilityDefs = DefDatabase<FacilityDef>.AllDefsListForReading;

            RectContentMain = RectWindow.ContractedBy(25f);

            RectContentMainLeft = RectContentMain.LeftHalf();
            RectContentMainLeft.width -= 3f;

            RectContentMainRight = RectContentMain.RightHalf();
            RectContentMainRight.width -= 2f;
            RectContentMainRight.x += 2f;

            RectSelectionArea = RectContentMainLeft.TopPartPixels(66f);
            RectSelectionAreaFacilityIcon = RectSelectionArea.LeftPartPixels(66f);
            RectSelectionAreaFacilityName = RectSelectionArea.RightPartPixels(RectSelectionArea.width - RectSelectionAreaFacilityIcon.width);
        }

        public override Vector2 InitialSize => RectWindow.size;

        protected override float Margin => 0f;

        public override void DoWindowContents(Rect inRect)
        {
            DrawCloseButton(inRect);
            DrawFacilitySelectorButton();

            //Widgets.DrawBox(RectContentMain);
            //Widgets.DrawBox(RectContentMainLeft);
            //Widgets.DrawBox(RectContentMainRight);

            if (defSelected == null) return;

            Widgets.DrawBox(RectSelectionArea);
            Widgets.DrawBox(RectSelectionAreaFacilityIcon, 2);
        }

        private void DrawFacilitySelectorButton()
        {
            if (WindowHelper.DrawInfoScreenSelectorButton(RectSelectionAreaFacilityName, defSelected?.label ?? "Empire_FacilityInfoWindowSelector".Translate()))
            {
                Find.WindowStack.Add(new FloatMenu(DefOptions));
            }
        }

        /// <summary>
        /// A list of defs to choose from
        /// </summary>
        private List<FloatMenuOption> DefOptions => cachedOptions ?? (cachedOptions = CreateFloatMenuOptions());

        /// <summary>
        /// Makes a list out of all FacilityDefs to select from
        /// </summary>
        /// <returns>the list</returns>
        private List<FloatMenuOption> CreateFloatMenuOptions()
        {
            return FloatMenuOptionCreator.CreateFloatMenuOptions(facilityDefs, (def) => defSelected = def);
        }

        private void DrawCloseButton(Rect inRect)
        {
            if (Widgets.CloseButtonFor(inRect)) Close();
        }
    }
}
