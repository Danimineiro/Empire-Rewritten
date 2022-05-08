using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Empire_Rewritten.Resources;
using Empire_Rewritten.Resources.Stats;
using Empire_Rewritten.Utils;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Empire_Rewritten.Windows
{
    public class ResourceInfoWindow : Window
    {
        private const int BorderSize = 2;

        //GUI.Group that can move everything
        private static readonly Rect RectFull = new Rect(22f, 22f, 1202f, 592f);

        private static readonly Rect RectDefIcon = new Rect(2f, 2f, 64f, 64f);
        private static readonly Rect RectDefSelector = new Rect(66f, 0f, 518f, 68f);
        private static readonly Rect RectFullDefDesc = new Rect(2f, 86f, 580f, 240f);
        private static readonly Rect RectDefDescValue = new Rect(0f, 0f, 570f, 24f);

        //In GUI.Group managed by container
        private static readonly Rect RectThingDefContainer = new Rect(602f, 2f, 580f, 29f);

        private static readonly Rect RectThingDefIcons = new Rect(2f, 2f, 25f, 25f);
        private static readonly Rect RectThingDefs = new Rect(47f, 2f, 513f, 25f);
        private static readonly Rect RectThingDefsHighlight = new Rect(0f, 0f, 569f, 29f);

        private static readonly Rect RectCurveContainer = new Rect(0f, 346f, 1184f, 224f);
        private static readonly Rect RectCurve = new Rect(2f, 2f, 291f, 220f);

        private static readonly Rect RectScrollRect = new Rect(602f, 2f, 580f, 324f);

        private static readonly Vector2 CurveOffset = new Vector2(296f, 0f);

        private static readonly Color TransparentGray = new Color(0f, 0f, 0f, 0.5f);

        private readonly List<ResourceDef> resourceDefs;

        private List<FloatMenuOption> cachedOptions;

        private Vector2 defDescScrollVector;

        private ResourceDef defSelected;
        private Rect rectScrollView = new Rect(602f, 2f, 563f, 324f);

        private Vector2 scrollRectVector;

        public ResourceInfoWindow()
        {
            resourceDefs = DefDatabase<ResourceDef>.AllDefsListForReading;
        }

        public ResourceInfoWindow(ResourceDef defSelected) : base()
        {
            this.defSelected = defSelected;
        }

        public override Vector2 InitialSize => new Vector2(1229f, 619f);

        protected override float Margin => 0f;

        /// <summary>
        ///     A <see cref="List{T}" /> of <see cref="FloatMenuOption">FloatMenuOptions</see> that set
        ///     <see cref="ResourceInfoWindow.defSelected" /> to a specific <see cref="ResourceDef">ResourceDefs</see>.
        /// </summary>
        private List<FloatMenuOption> DefOptions => cachedOptions ?? (cachedOptions = CreateFloatMenuOptions());

        public override void DoWindowContents(Rect inRect)
        {
            DrawCloseButton(inRect);
            GUI.BeginGroup(RectFull);
            DrawDefSelectorButton();
            DrawDefContent();
            GUI.EndGroup();
            WindowHelper.ResetTextAndColor();
        }

        /// <summary>
        ///     Draws the contents of <see cref="defSelected" />, if it is not <c>null</c>
        /// </summary>
        private void DrawDefContent()
        {
            if (defSelected is null) return;

            DrawDescriptionAndIcon();
            DrawItems();
            DrawCurves();
        }

        /// <summary>
        ///     Draws <see cref="ResourceInfoWindow.defSelected" />'s <see cref="ResourceDef.temperatureCurve" />,
        ///     <see cref="ResourceDef.heightCurve" />, <see cref="ResourceDef.swampinessCurve" />, and
        ///     <see cref="ResourceDef.rainfallCurve" />
        /// </summary>
        private void DrawCurves()
        {
            GUI.BeginGroup(RectCurveContainer);
            DrawLabeledCurve(RectCurve, defSelected.temperatureCurve, "Empire_ResourceInfoWindowTempCurve".Translate(), "Empire_ResourceInfoWindowTempCurveLabelX".Translate(),
                             new FloatRange(-50f, 50f));
            DrawLabeledCurve(RectCurve.MoveRect(CurveOffset), defSelected.rainfallCurve, "Empire_ResourceInfoWindowRainfallCurve".Translate(),
                             "Empire_ResourceInfoWindowRainfallCurveLabelX".Translate(), new FloatRange(0f, 7500f));
            DrawLabeledCurve(RectCurve.MoveRect(CurveOffset * 2), defSelected.heightCurve, "Empire_ResourceInfoWindowHeightCurve".Translate(), "Empire_ResourceInfoWindowHeightCurveLabelX".Translate(),
                             new FloatRange(0f, 2500f));
            DrawLabeledCurve(RectCurve.MoveRect(CurveOffset * 3), defSelected.swampinessCurve, "Empire_ResourceInfoWindowSwampinessCurve".Translate(),
                             "Empire_ResourceInfoWindowSwampinessCurveLabelX".Translate(), new FloatRange(0f, 1f));
            GUI.EndGroup();
        }

        /// <summary>
        ///     Draws a <see cref="SimpleCurve" /> with a given <see cref="string">label</see> into a <see cref="Rect" />
        /// </summary>
        /// <param name="rect">The <see cref="Rect" /> to draw into</param>
        /// <param name="curve">The <see cref="SimpleCurve" /> to draw</param>
        /// <param name="labelRight">
        ///     A <see cref="string" /> to draw in the bottom-right corner of the <paramref name="rect" />
        /// </param>
        /// <param name="labelX">The <see cref="SimpleCurveDrawerStyle.LabelX" /></param>
        /// <param name="range">The <see cref="SimpleCurveDrawerStyle.FixedSection" /></param>
        private static void DrawLabeledCurve(Rect rect, SimpleCurve curve, string labelRight, string labelX, FloatRange range)
        {
            if (curve == null)
            {
                GUI.color = Color.red + Color.yellow;
                Widgets.Label(rect, "Empire_ResourceInfoWindowCurveMissing".Translate());
                WindowHelper.ResetTextAndColor();

                Widgets.DrawBoxSolid(rect, Color.black);
                rect.DrawBorderAroundRect(BorderSize);
                return;
            }

            Text.Font = GameFont.Tiny;
            Rect tempLabelRect = rect.BottomPartPixels(rect.height / 12f);

            SimpleCurveDrawerStyle style = new SimpleCurveDrawerStyle
            {
                DrawBackground = false,
                DrawBackgroundLines = true,
                DrawCurveMousePoint = true,
                DrawLegend = false,
                DrawMeasures = false,
                DrawPoints = true,
                LabelX = $"<color=orange>{labelX}</color>",
                OnlyPositiveValues = false,
                PointsRemoveOptimization = true,
                UseAntiAliasedLines = true,
                XIntegersOnly = false,
                YIntegersOnly = false,
                FixedSection = range,
                FixedScale = new Vector2(-0.135f, 1.5f),
                UseFixedSection = true,
                UseFixedScale = true
            };

            SimpleCurveDrawer.DrawCurve(rect, curve, style);
            Widgets.DrawBoxSolid(tempLabelRect, TransparentGray);
            rect.DrawBorderAroundRect(BorderSize);

            Text.Anchor = TextAnchor.LowerRight;
            Widgets.Label(tempLabelRect, labelRight + ' ');

            Text.Anchor = TextAnchor.LowerLeft;
            Widgets.Label(tempLabelRect, $" {"Empire_ResourceInfoWindowEfficiency".Translate()} ");
            WindowHelper.ResetTextAndColor();
        }

        /// <summary>
        ///     Draws all the <see cref="ThingDef">ThingDefs</see> allowed in <see cref="ResourceInfoWindow.defSelected" />
        /// </summary>
        private void DrawItems()
        {
            List<ThingDef> thingDefs = defSelected.ResourcesCreated.AllowedThingDefs.ToList();

            Text.Anchor = TextAnchor.MiddleLeft;
            Text.Font = GameFont.Small;
            RectScrollRect.DrawBorderAroundRect(BorderSize);
            rectScrollView.height = thingDefs.Count * RectThingDefContainer.height;
            Widgets.BeginScrollView(RectScrollRect, ref scrollRectVector, rectScrollView);

            for (int i = 0; i < thingDefs.Count; i++)
            {
                Rect temp = new Rect(RectThingDefContainer);
                temp.y += temp.height * i;
                ThingDef current = thingDefs[i];

                GUI.BeginGroup(temp);

                if (i % 2 == 1)
                {
                    Widgets.DrawHighlight(RectThingDefsHighlight);
                }
                else
                {
                    Widgets.DrawLightHighlight(RectThingDefsHighlight);
                }

                MouseoverSounds.DoRegion(RectThingDefsHighlight);
                Widgets.DrawHighlightIfMouseover(RectThingDefsHighlight);
                TooltipHandler.TipRegion(RectThingDefsHighlight, current.description);
                Widgets.ThingIcon(RectThingDefIcons, current);
                Widgets.Label(RectThingDefs, current.LabelCap);
                Widgets.InfoCardButton(RectThingDefs.RightPartPixels(RectThingDefs.height), current);

                GUI.EndGroup();
            }

            Widgets.EndScrollView();
            WindowHelper.ResetTextAndColor();
        }

        /// <summary>
        ///     Draws the current <see cref="ResourceInfoWindow.RectDefIcon" /> as well as
        ///     <see cref="ResourceInfoWindow.defSelected" />'s <see cref="ResourceDef.description" />
        /// </summary>
        private void DrawDescriptionAndIcon()
        {
            Text.Font = GameFont.Small;

            //Def Icon
            GUI.DrawTexture(RectDefIcon, ContentFinder<Texture2D>.Get(defSelected.iconData.texPath));
            Widgets.DrawLightHighlight(RectDefIcon);
            RectDefIcon.DrawBorderAroundRect(BorderSize);

            //Def Description
            Rect tempFullHeight = RectFullDefDesc.ContractedBy(5f);

            float descHeight = Text.CalcHeight(defSelected.description, tempFullHeight.width);

            Rect tempDescRect = tempFullHeight.TopPartPixels(descHeight);
            Rect tempValuesRect = tempFullHeight.BottomPartPixels(tempFullHeight.height - descHeight - 5f);

            Rect tempTex = new Rect(tempValuesRect.x, tempValuesRect.y, tempValuesRect.width - 17f, 15f * RectDefDescValue.height);

            Widgets.Label(tempDescRect, $"{defSelected.description}");

            GUI.color = Color.gray;
            Widgets.DrawLineHorizontal(tempDescRect.x, tempDescRect.y + tempDescRect.height, tempDescRect.width);
            GUI.color = Color.white;
            DrawResourceValues(tempValuesRect, tempTex);
            RectFullDefDesc.DrawBorderAroundRect(BorderSize);

            WindowHelper.ResetTextAndColor();
        }

        /// <summary>
        ///     TODO: Document parameters
        ///     Draws the resource values
        /// </summary>
        /// <param name="tempValuesRect"></param>
        /// <param name="tempTex"></param>
        private void DrawResourceValues(Rect tempValuesRect, Rect tempTex)
        {
            Widgets.BeginScrollView(tempValuesRect, ref defDescScrollVector, tempTex);
            GUI.BeginGroup(tempTex);

            DrawResourceValueBlock(true);
            Widgets.DrawLightHighlight(RectDefDescValue.MoveRect(new Vector2(0f, RectDefDescValue.height * 7)));
            DrawResourceValueBlock(false, 8);

            GUI.EndGroup();
            Widgets.EndScrollView();
        }

        /// <summary>
        ///     Draws a resource value block.
        /// </summary>
        /// <param name="isOffset">
        ///     If <paramref name="isOffset" /> is <c>true</c>, the content is formatted as a percentage,
        ///     otherwise as a normal decimal number.
        /// </param>
        /// <param name="valueOffset">offsets the block</param>
        private void DrawResourceValueBlock(bool isOffset, int valueOffset = 0)
        {
            for (int i = 0; i < 7; i++)
            {
                string addOrMultiply = $"Empire_ResourceInfoWindow{(isOffset ? "Additively" : "Multiplicatively")}".TranslateSimple();

                Rect tempRect = RectDefDescValue.MoveRect(new Vector2(0f, RectDefDescValue.height * (i + valueOffset)));

                if (i % 2 == 0)
                {
                    Widgets.DrawHighlight(tempRect);
                }
                else
                {
                    Widgets.DrawLightHighlight(tempRect);
                }

                Widgets.Label(tempRect.ScaleX(-5f), ((ResourceStat)i).Translate(isOffset).CapitalizeFirst());

                float bonus = defSelected.GetBonus((ResourceStat)i, isOffset);
                // Without InvariantCulture, this would look different on e.g. German systems vs British ones (1,1 vs 1.1)
                Widgets.Label(tempRect.ScaleX(-5f).RightHalf(), isOffset ? bonus.ToString(CultureInfo.InvariantCulture) : bonus.ToStringPercent());

                MouseoverSounds.DoRegion(tempRect);
                Widgets.DrawHighlightIfMouseover(tempRect);
                TooltipHandler.TipRegion(tempRect, $"Empire_ResourceInfoWindowTip{((ResourceStat)i).ToString().CapitalizeFirst()}".Translate(addOrMultiply));
            }
        }

        /// <summary>
        ///     Draws a button that allows the user to select a <see cref="ResourceDef" /> to look at.
        /// </summary>
        private void DrawDefSelectorButton()
        {
            if (WindowHelper.DrawInfoScreenSelectorButton(RectDefSelector, defSelected?.label ?? "Empire_ResourceInfoWindowSelector".Translate()))
            {
                Find.WindowStack.Add(new FloatMenu(DefOptions));
            }
        }

        /// <summary>
        ///     Creates a <see cref="List{T}" /> of <see cref="FloatMenuOption">FloatMenuOptions</see> that set
        ///     <see cref="ResourceInfoWindow.defSelected" /> to a specific <see cref="ResourceDef">ResourceDefs</see>.
        /// </summary>
        /// <returns>
        ///     A <see cref="List{T}" /> of <see cref="FloatMenuOption">FloatMenuOptions</see> that can set
        ///     <see cref="ResourceInfoWindow.defSelected" />
        /// </returns>
        private List<FloatMenuOption> CreateFloatMenuOptions()
        {
            return FloatMenuOptionCreator.CreateFloatMenuOptions(resourceDefs, def => defSelected = def);
        }

        private void DrawCloseButton(Rect inRect)
        {
            if (Widgets.CloseButtonFor(inRect)) Close();
        }
    }
}