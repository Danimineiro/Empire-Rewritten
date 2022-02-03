using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Empire_Rewritten.Util;
using Verse;

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
        private readonly static Rect rect_DefDesc = new Rect(2f, 86f, 580f, 240f);

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
            if (defSelected != null)
            {
                DrawDescriptionAndIcon();
                DrawItems();
                DrawCurves();
            }
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
            Text.Anchor = TextAnchor.MiddleLeft;
            Text.Font = GameFont.Small;

            if (defSelected.ResourcesCreated.AllowedThingDefs.ToList() is List<ThingDef> thingDefs)
            {
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

                    Widgets.ThingIcon(rect_ThingDefIcons, current);
                    Widgets.Label(rect_ThingDefs, current.label);

                    GUI.EndGroup();
                }

                Widgets.EndScrollView();
            }

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
            Widgets.LabelScrollable(rect_DefDesc.ContractedBy(5f), MakeDefDescriptionString(), ref defDescScrollVector, true);
            Widgets.DrawLightHighlight(rect_DefDesc.ExpandedBy(borderSize));
            rect_DefDesc.DrawBorderAroundRect(borderSize);

            ResetTextAndColor();
        }


        /// <returns>The def description string</returns>
        private string MakeDefDescriptionString()
        {
            string desc = defSelected.description;
            string lakeBonus = GetBonusStringWithColor("Empire_ResourceInfoWindowLake", defSelected.lakeBonus);
            string oceanBonus = GetBonusStringWithColor("Empire_ResourceInfoWindowOcean", defSelected.oceanBonus);
            string riverBonus = GetBonusStringWithColor("Empire_ResourceInfoWindowRiver", defSelected.riverBonus);

            return $"{desc}\n\n{lakeBonus}\n{oceanBonus}\n{riverBonus}";
        }

        /// <summary>
        /// Colors a resource info string <paramref name="translationKey"/> based on <paramref name="bonus"/>
        /// </summary>
        /// <param name="translationKey"></param>
        /// <param name="bonus"></param>
        /// <returns></returns>
        private string GetBonusStringWithColor(string translationKey, float bonus)
        {
            string fullString = bonus > 0f ? $"{translationKey}Bonus".Translate(bonus.ToStringPercent()) : $"{translationKey}NoBonus".Translate();
            string color = bonus > 0f ? "green" : "red";
            return $"<color={color}>{fullString}</color>";
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
