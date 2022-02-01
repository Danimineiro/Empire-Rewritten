using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using HarmonyLib;

namespace Empire_Rewritten
{
    public static class DebugActionsMisc
    {
        [DebugAction("Empire", "Resource info window", false, false, allowedGameStates = AllowedGameStates.Entry)]
        public static void PatchNotesDisplayWindow() => Find.WindowStack.Add(new ResourceInfoWindow());
    }

    public class ResourceInfoWindow : Window
    {
        private readonly static Rect rect_DefName = new Rect(90f, 20f, 500f, 64f);
        private readonly static Rect rect_DefIcon = new Rect(10f, 20f, 64f, 64f);
        private readonly static Rect rect_ThingDefContainer = new Rect(610f, 20f, 569f, 40f);
        private readonly static Rect rect_ThingDefIcons = new Rect(0f, 0f, 25f, 25f);
        private readonly static Rect rect_ThingDefs = new Rect(45f, 0f, 515f, 25f);
        private readonly static Rect rect_ThingDefsHighlight = new Rect(0f, 0f, 515f, 25f);
        private readonly static Rect rect_TempCurve = new Rect(10f, 400f, 580f, 190f);
        private readonly static Rect rect_HeightCurve = new Rect(610f, 400f, 580f, 190f);
        private readonly static Rect rect_DefDesc = new Rect(10f, 145f, 580f, 240f);

        private readonly static Rect rect_ScrollRect = new Rect(new Vector2(610f, 20f), new Vector2(580f, 320f));
        private readonly static Rect rect_ScrollViewRect = new Rect(new Vector2(610f, 20f), new Vector2(563f, 320f));

        private readonly static Color gray = new Color(0f, 0f, 0f, 0.5f);

        private List<ResourceDef> resources = null;
        private Vector2 defDescScrollVector = new Vector2();
        private Vector2 scrollRectVector = new Vector2();

        private ResourceDef defSelected;

        public override Vector2 InitialSize => new Vector2(1240, 640);

        public override void DoWindowContents(Rect inRect)
        {
            DrawDefSelectorButton();
            if (defSelected == null) return;

            DrawDefInfo();
            DrawItems();
            DrawCurves();
            DrawBoxes();
        }

        private void DrawCurves()
        {
            SimpleCurveDrawer.DrawCurve(rect_HeightCurve, defSelected.heightCurve);
            DrawLabeledCurve(rect_HeightCurve, defSelected.heightCurve, "Empire_ResourceInfoWindowHeightCurve".Translate());
            DrawLabeledCurve(rect_TempCurve, defSelected.temperatureCurve, "Empire_ResourceInfoWindowTempCurve".Translate());
                Widgets.Label(rect, "Empire_ResourceInfoWindowCurveMissing".Translate());

            SimpleCurveDrawer.DrawCurve(rect_TempCurve, defSelected.temperatureCurve);
        }

        private void DrawItems()
        {
            if (defSelected.ResourcesCreated.AllowedThingDefs.ToList() is List<ThingDef> thingDefs)
            {
                Widgets.BeginScrollView(rect_ScrollRect, ref scrollRectVector, rect_ScrollViewRect);
                for (int i = 0; i < thingDefs.Count; i++)
                {
                    Rect temp = new Rect(rect_ThingDefContainer);
                    temp.y += temp.height * i;
                    ThingDef current = thingDefs[i];

                    GUI.BeginGroup(temp);
                    Widgets.DrawHighlight(rect_ThingDefsHighlight);
                    GUI.DrawTexture(rect_ThingDefIcons, Widgets.GetIconFor(current));
                    Widgets.Label(rect_ThingDefs, current.label);

                    GUI.EndGroup();
                }

                Widgets.EndScrollView();
            }
        }

        private void DrawBoxes()
        {
            Widgets.DrawHighlight(rect_DefDesc);
            Widgets.DrawHighlight(rect_TempCurve);
            Widgets.DrawHighlight(rect_HeightCurve);

            Widgets.DrawBox(rect_DefDesc, 2);
            Widgets.DrawBox(rect_TempCurve, 2);
            Widgets.DrawBox(rect_HeightCurve, 2);
        }

        private void DrawDefInfo()
        {
            GUI.DrawTexture(rect_DefIcon, ContentFinder<Texture2D>.Get(defSelected.iconData.texPath));
            Widgets.TextAreaScrollable(rect_DefDesc, defSelected.description, ref defDescScrollVector, true);
        }

        /// <summary>
        /// Draws a button that allows the user to select a resource to look at
        /// </summary>
        private void DrawDefSelectorButton()
        {
            Widgets.Label(rect_DefSelector, defSelected?.label ?? "Empire_ResourceInfoWindowSelector".Translate());
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
    }
}
