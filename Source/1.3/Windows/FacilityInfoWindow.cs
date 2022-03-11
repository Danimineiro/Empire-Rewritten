using System.Collections.Generic;
using Empire_Rewritten.Facilities;
using Empire_Rewritten.Utils;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Empire_Rewritten.Windows
{
    public class FacilityInfoWindow : Window
    {
        private const float ItemHeight = 29f;

        private readonly List<FacilityDef> facilityDefs;
        private readonly Rect rectContentMain;
        private readonly Rect rectContentMainLeft;
        private readonly Rect rectContentMainRight;
        private readonly Rect rectFacilityDescriptionArea;
        private readonly Rect rectSelectionAreaFacilityIcon;
        private readonly Rect rectSelectionAreaFacilityName;
        private readonly Rect rectWindow = new Rect(0f, 0f, 1200f, 600f);

        private List<FloatMenuOption> cachedOptions;

        private Vector2 defDescScrollVector;

        private FacilityDef defSelected;

        private Rect rectFacilityDescription;

        public FacilityInfoWindow()
        {
            facilityDefs = DefDatabase<FacilityDef>.AllDefsListForReading;

            rectContentMain = rectWindow.ContractedBy(25f);

            rectContentMainLeft = rectContentMain.LeftHalf();
            rectContentMainLeft.width -= 3f;

            rectContentMainRight = rectContentMain.RightHalf();
            rectContentMainRight.width -= 2f;
            rectContentMainRight.x += 2f;

            Rect rectSelectionArea = rectContentMainLeft.TopPartPixels(66f);
            rectSelectionAreaFacilityIcon = rectSelectionArea.LeftPartPixels(66f);
            rectSelectionAreaFacilityName = rectSelectionArea.RightPartPixels(rectSelectionArea.width - rectSelectionAreaFacilityIcon.width + 2f);

            rectFacilityDescriptionArea = rectContentMainLeft.BottomPartPixels(rectContentMainLeft.height - rectSelectionAreaFacilityIcon.height - 10f);
            rectFacilityDescriptionArea.height = 240f;

            rectFacilityDescription = rectFacilityDescriptionArea.ContractedBy(5f);
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

            Color color = Color.white;
            color.a = 0.5f;
            GUI.color = color;
            Widgets.DrawBox(rectContentMain);
            Widgets.DrawBox(rectContentMainLeft);
            Widgets.DrawBox(rectContentMainRight);
            GUI.color = Color.white;

            if (defSelected == null) return;
            DrawDescriptionBox();
            DrawCostList();

            DrawFacilityIcon();
        }

        private void DrawDescriptionBox()
        {
            float descHeight = Text.CalcHeight(defSelected.description, rectFacilityDescription.width);

            rectFacilityDescription.height = descHeight;

            Widgets.DrawBox(rectFacilityDescriptionArea, 2);
            Widgets.Label(rectFacilityDescription, defSelected.description);
        }

        private void DrawCostList()
        {
            GUI.color = Color.gray;
            Widgets.DrawLineHorizontal(rectFacilityDescription.x, rectFacilityDescription.y + rectFacilityDescription.height, rectFacilityDescription.width);
            GUI.color = Color.white;

            Rect rectScrollOuter = rectFacilityDescriptionArea.BottomPartPixels(rectFacilityDescriptionArea.height - rectFacilityDescription.height - 5f).ContractedBy(5f);
            Rect rectScrollInner = new Rect(rectScrollOuter.x, rectScrollOuter.y, rectScrollOuter.width, ItemHeight * defSelected.costList.Count);

            if (rectScrollInner.height > rectScrollOuter.height) rectScrollInner.width -= 17f;

            Rect rectItemBase = new Rect(0f, 0f, rectScrollInner.width, ItemHeight);
            Widgets.BeginScrollView(rectScrollOuter, ref defDescScrollVector, rectScrollInner);

            GUI.BeginGroup(rectScrollInner);
            for (int i = 0; i < defSelected.costList.Count; i++)
            {
                Rect rectItemTemp = rectItemBase.MoveRect(new Vector2(0f, rectItemBase.height * i));
                ThingDefCountClass countClass = defSelected.costList[i];
                ThingDef thing = countClass.thingDef;
                int count = countClass.count;

                if (i % 2 == 1)
                {
                    Widgets.DrawHighlight(rectItemTemp);
                }
                else
                {
                    Widgets.DrawLightHighlight(rectItemTemp);
                }

                MouseoverSounds.DoRegion(rectItemTemp);
                Widgets.DrawHighlightIfMouseover(rectItemTemp);
                TooltipHandler.TipRegion(rectItemTemp, thing.description);
                Widgets.ThingIcon(rectItemTemp.LeftPartPixels(rectItemTemp.height).MoveRect(new Vector2(5f, 0f)), thing);

                Text.Anchor = TextAnchor.MiddleLeft;
                Widgets.Label(rectItemTemp.RightPartPixels(rectItemTemp.width - rectItemTemp.height - 10f), $"{thing.LabelCap}: {count}");
                WindowHelper.ResetTextAndColor();

                Widgets.InfoCardButton(rectItemTemp.RightPartPixels(rectItemTemp.height).ContractedBy(2).MoveRect(new Vector2(-3f, 0f)), thing);
            }

            GUI.EndGroup();
            Widgets.EndScrollView();

            GUI.color = Color.gray;
            Widgets.DrawBox(rectScrollOuter);
            GUI.color = Color.white;
        }

        private void DrawFacilityIcon()
        {
            GUI.DrawTexture(rectSelectionAreaFacilityIcon.ContractedBy(2f), ContentFinder<Texture2D>.Get(defSelected.iconData.texPath));
            Widgets.DrawBox(rectSelectionAreaFacilityIcon, 2);
        }

        /// <summary>
        ///     Draws the Facility Selector Button
        /// </summary>
        private void DrawFacilitySelectorButton()
        {
            if (WindowHelper.DrawInfoScreenSelectorButton(rectSelectionAreaFacilityName, defSelected?.label ?? "Empire_FacilityInfoWindowSelector".Translate()))
            {
                Find.WindowStack.Add(new FloatMenu(DefOptions));
            }
        }

        /// <summary>
        ///     Generates the <see cref="FloatMenuOption">FloatMenuOptions</see> for the
        ///     <see cref="FacilityInfoWindow.facilityDefs" />
        /// </summary>
        /// <returns>
        ///     A <see cref="List{T}" /> of <see cref="FloatMenuOption">FloatMenuOptions</see> that set
        ///     <see cref="FacilityInfoWindow.defSelected" /> to one of <see cref="FacilityInfoWindow.facilityDefs" />
        /// </returns>
        private List<FloatMenuOption> CreateFloatMenuOptions()
        {
            return FloatMenuOptionCreator.CreateFloatMenuOptions(facilityDefs, def => defSelected = def);
        }

        private void DrawCloseButton(Rect inRect)
        {
            if (Widgets.CloseButtonFor(inRect)) Close();
        }
    }
}