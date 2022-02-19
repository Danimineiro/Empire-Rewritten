using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Empire_Rewritten.Utils;
using Verse;
using Verse.Sound;

namespace Empire_Rewritten
{
    public static class DebugActionsMisc
    {
        [DebugAction("Empire", "Resource info window", false, false, allowedGameStates = AllowedGameStates.Entry)]
        public static void PatchNotesDisplayWindow() => Find.WindowStack.Add(new ResourceInfoWindow());
    }

    public class ResourceInfoWindow : Window
    {
        //GUI.Group that can move everything
        private readonly static Rect rect_fullRect = new Rect(22f, 22f, 1202f, 592f);

        private readonly static Rect rect_DefIcon = new Rect(2f, 2f, 64f, 64f);
        private readonly static Rect rect_DefSelector = new Rect(68f, 2f, 514f, 64f);
        private readonly static Rect rect_FullDefDesc = new Rect(2f, 86f, 580f, 240f);
        private readonly static Rect rect_DefDescValue = new Rect(0f, 0f, 570f, 24f);

        //In GUI.Group managed by container
        private readonly static Rect rect_ThingDefContainer = new Rect(602f, 2f, 580f, 29f);
        private readonly static Rect rect_ThingDefIcons = new Rect(2f, 2f, 25f, 25f);
        private readonly static Rect rect_ThingDefs = new Rect(47f, 2f, 513f, 25f);
        private readonly static Rect rect_ThingDefsHighlight = new Rect(0f, 0f, 569f, 29f);

        private readonly static Rect rect_CurveContainer = new Rect(0f, 346f, 1184f, 224f);
        private readonly static Rect rect_Curve = new Rect(2f, 2f, 291f, 220f);

        private readonly static Rect rect_ScrollRect = new Rect(602f, 2f, 580f, 324f);

        private readonly static Vector2 curveOffset = new Vector2(296f, 0f);

        private readonly static Color transGray = new Color(0f, 0f, 0f, 0.5f);

        private readonly static int borderSize = 2;

        private static readonly GameFont prevFont = Text.Font;
        private static readonly TextAnchor prevAnchor = Text.Anchor;
        private static readonly Color prevColor = GUI.color;

        private List<ResourceDef> resources = null;
        private List<FloatMenuOption> cachedOptions = null;
        private Vector2 defDescScrollVector = new Vector2();
        private Vector2 scrollRectVector = new Vector2();
        private Rect rect_ScrollViewRect = new Rect(602f, 2f, 563f, 324f);
        
        private ResourceDef defSelected;

        public override Vector2 InitialSize => new Vector2(1229f, 619f);
        
        protected override float Margin => 0f;

        public override void DoWindowContents(Rect inRect)
        {
            if (Widgets.CloseButtonFor(inRect)) Close();

            GUI.BeginGroup(rect_fullRect);
            DrawDefSelectorButton();
            DrawDefContent();
            GUI.EndGroup();
        }

        /// <summary>
        /// Draws the contents of the selected def if available
        /// </summary>
        private void DrawDefContent()
        {
            if (defSelected == null) return;

            DrawDescriptionAndIcon();
            DrawItems();
            DrawCurves();
        }

        /// <summary>
        /// Draws the defs curves
        /// </summary>
        private void DrawCurves()
        {
            GUI.BeginGroup(rect_CurveContainer);
            DrawLabeledCurve(rect_Curve, defSelected.temperatureCurve, "Empire_ResourceInfoWindowTempCurve".Translate(), "Empire_ResourceInfoWindowTempCurveLabelX".Translate(), new FloatRange(-50f, 50f));
            DrawLabeledCurve(rect_Curve.MoveRect(curveOffset), defSelected.rainfallCurve, "Empire_ResourceInfoWindowRainfallCurve".Translate(), "Empire_ResourceInfoWindowRainfallCurveLabelX".Translate(), new FloatRange(0f, 7500f));
            DrawLabeledCurve(rect_Curve.MoveRect(curveOffset * 2), defSelected.heightCurve, "Empire_ResourceInfoWindowHeightCurve".Translate(), "Empire_ResourceInfoWindowHeightCurveLabelX".Translate(), new FloatRange(0f, 2500f));
            DrawLabeledCurve(rect_Curve.MoveRect(curveOffset * 3), defSelected.swampinessCurve, "Empire_ResourceInfoWindowSwampinessCurve".Translate(), "Empire_ResourceInfoWindowSwampinessCurveLabelX".Translate(), new FloatRange(0f, 1f));
            GUI.EndGroup();
        }

        /// <summary>
        /// Draws a <paramref name="curve"/> with a <paramref name="labelRight"/> into a <paramref name="rect"/>
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="curve"></param>
        /// <param name="labelRight"></param>
        /// <param name="labelX"></param>
        /// <param name="range">
        private void DrawLabeledCurve(Rect rect, SimpleCurve curve, string labelRight, string labelX, FloatRange range)
        {
            if (curve == null)
            {
                GUI.color = Color.red + Color.yellow;
                Widgets.Label(rect, "Empire_ResourceInfoWindowCurveMissing".Translate());
                ResetTextAndColor();

                Widgets.DrawBoxSolid(rect, Color.black);
                rect.DrawBorderAroundRect(borderSize);
                return;
            }


            Rect tempLabelRect = rect.BottomPartPixels(rect.height / 12f);

            SimpleCurveDrawerStyle style = new SimpleCurveDrawerStyle()
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
            Widgets.DrawBoxSolid(tempLabelRect, transGray);
            rect.DrawBorderAroundRect(borderSize);

            Text.Anchor = TextAnchor.LowerRight;
            Widgets.Label(tempLabelRect, $"{labelRight} ");

            Text.Anchor = TextAnchor.LowerLeft;
            Widgets.Label(tempLabelRect, $" {"Empire_ResourceInfoWindowEfficiency".Translate()} ");
            ResetTextAndColor();
        }

        /// <summary>
        /// Draws all the items allowed in the ResourceDef
        /// </summary>
        private void DrawItems()
        {
            if (!(defSelected.ResourcesCreated.AllowedThingDefs.ToList() is List<ThingDef> thingDefs)) return;

            Text.Anchor = TextAnchor.MiddleLeft;
            Text.Font = GameFont.Small;
            rect_ScrollRect.DrawBorderAroundRect(borderSize);
            rect_ScrollViewRect.height = thingDefs.Count * rect_ThingDefContainer.height;
            Widgets.BeginScrollView(rect_ScrollRect, ref scrollRectVector, rect_ScrollViewRect);

            for (int i = 0; i < thingDefs.Count; i++)
            {
                Rect temp = new Rect(rect_ThingDefContainer);
                temp.y += temp.height * i;
                ThingDef current = thingDefs[i];

                GUI.BeginGroup(temp);

                if (i % 2 != 0)
                    Widgets.DrawHighlight(rect_ThingDefsHighlight);
                else
                    Widgets.DrawLightHighlight(rect_ThingDefsHighlight);

                MouseoverSounds.DoRegion(rect_ThingDefsHighlight);
                Widgets.DrawHighlightIfMouseover(rect_ThingDefsHighlight);
                Widgets.ThingIcon(rect_ThingDefIcons, current);
                Widgets.Label(rect_ThingDefs, current.LabelCap);
                Widgets.InfoCardButton(rect_ThingDefs.RightPartPixels(rect_ThingDefs.height), current);

                GUI.EndGroup();
            }

            Widgets.EndScrollView();
            ResetTextAndColor();
        }

        /// <summary>
        /// Draws the ResourceIcon and it's description
        /// </summary>
        private void DrawDescriptionAndIcon()
        {
            Text.Font = GameFont.Small;

            //Def Icon
            GUI.DrawTexture(rect_DefIcon, ContentFinder<Texture2D>.Get(defSelected.iconData.texPath));
            Widgets.DrawLightHighlight(rect_DefIcon);
            rect_DefIcon.DrawBorderAroundRect(borderSize);

            //Def Description
            Rect tempFullHeight = rect_FullDefDesc.ContractedBy(5f);

            float descHeight = Text.CalcHeight(defSelected.description, tempFullHeight.width);

            Rect tempDescRect = tempFullHeight.TopPartPixels(descHeight);
            Rect tempValuesRect = tempFullHeight.BottomPartPixels(tempFullHeight.height - descHeight - 5f);

            Rect tempTex = new Rect(tempValuesRect.x, tempValuesRect.y, tempValuesRect.width - 17f, 15f * rect_DefDescValue.height);

            Widgets.Label(tempDescRect, $"{defSelected.description}");

            GUI.color = Color.gray;
            Widgets.DrawLineHorizontal(tempDescRect.x, tempDescRect.y + tempDescRect.height, tempDescRect.width);
            GUI.color = Color.white;
            DrawResourceValues(tempValuesRect, tempTex);
            rect_FullDefDesc.DrawBorderAroundRect(borderSize);

            ResetTextAndColor();
        }

        /// <summary>
        /// Draws the resource values
        /// </summary>
        /// <param name="tempValuesRect"></param>
        /// <param name="tempTex"></param>
        private void DrawResourceValues(Rect tempValuesRect, Rect tempTex)
        {
            Widgets.BeginScrollView(tempValuesRect, ref defDescScrollVector, tempTex);
            GUI.BeginGroup(tempTex);

            DrawResourceValueBlock(true);
            Widgets.DrawLightHighlight(rect_DefDescValue.MoveRect(new Vector2(0f, rect_DefDescValue.height * 7)));
            DrawResourceValueBlock(false, 8);

            GUI.EndGroup();
            Widgets.EndScrollView();
        }

        /// <summary>
        /// Draws a resource value block <paramref name="isOffset"/> says if it is in percent or not, <paramref name="valueOffset"/> offsets the block
        /// </summary>
        /// <param name="isOffset"></param>
        /// <param name="valueOffset"></param>
        private void DrawResourceValueBlock(bool isOffset, int valueOffset = 0)
        {
            for (int i = 0; i < 7; i++)
            {
                string addOrMult = $"Empire_ResourceInfoWindow{(isOffset ? "Additively" : "Multiplicatively")}".Translate();

                Rect tempRect = rect_DefDescValue.MoveRect(new Vector2(0f, rect_DefDescValue.height * (i + valueOffset)));

                if (i % 2 == 0)
                    Widgets.DrawHighlight(tempRect);
                else
                    Widgets.DrawLightHighlight(tempRect);

                Widgets.Label(tempRect.ScaleXByPixel(-5f), ((ResourceStat)i).Translate(isOffset).CapitalizeFirst());

                if (isOffset)
                    Widgets.Label(tempRect.ScaleXByPixel(-5f).RightHalf(), defSelected.GetBonus((ResourceStat)i, isOffset).ToString());
                else
                    Widgets.Label(tempRect.ScaleXByPixel(-5f).RightHalf(), defSelected.GetBonus((ResourceStat)i, isOffset).ToStringPercent());

                MouseoverSounds.DoRegion(tempRect);
                Widgets.DrawHighlightIfMouseover(tempRect);
                TooltipHandler.TipRegion(tempRect, $"Empire_ResourceInfoWindowTip{((ResourceStat)i).ToString().CapitalizeFirst()}".Translate(addOrMult));
            }
        }

        /// <summary>
        /// Draws a button that allows the user to select a resource to look at
        /// </summary>
        private void DrawDefSelectorButton()
        {
            Text.Anchor = TextAnchor.MiddleCenter;
            Text.Font = GameFont.Medium;

            Widgets.DrawLightHighlight(rect_DefSelector);
            Widgets.DrawHighlightIfMouseover(rect_DefSelector);
            Widgets.Label(rect_DefSelector, defSelected?.label ?? "Empire_ResourceInfoWindowSelector".Translate());
            rect_DefSelector.DrawBorderAroundRect(borderSize);

            ResetTextAndColor();

            if (Widgets.ButtonInvisible(rect_DefSelector))
            {
                Find.WindowStack.Add(new FloatMenu(DefOptions));
            }
        }

        /// <summary>
        /// A list of defs to choose from
        /// </summary>
        private List<FloatMenuOption> DefOptions => cachedOptions ?? (cachedOptions = CreateFloatMenuOptions());

        /// <summary>
        /// Makes a list out of all ResourceDefs to select from
        /// </summary>
        /// <returns>the list</returns>
        private List<FloatMenuOption> CreateFloatMenuOptions()
        {
            resources = resources ?? (resources = DefDatabase<ResourceDef>.AllDefsListForReading);

            List<FloatMenuOption> floatMenuOptions = new List<FloatMenuOption>();

            foreach (ResourceDef def in resources)
            {
                floatMenuOptions.Add(new FloatMenuOption(def.label, delegate
                {
                    defSelected = def;
                }));
            }

            return floatMenuOptions;
        }

        /// <summary>
        /// Resets the Text.Font, Text.Anchor and GUI.color setting
        /// </summary>
        private static void ResetTextAndColor()
        {
            Text.Font = prevFont;
            Text.Anchor = prevAnchor;
            GUI.color = prevColor;
        }
    }
}
