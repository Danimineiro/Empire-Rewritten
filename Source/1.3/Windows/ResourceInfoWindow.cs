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
        private readonly static Rect Rect_DefName = new Rect(90f, 20f, 500f, 64f);
        private readonly static Rect Rect_DefIcon = new Rect(10f, 20f, 64f, 64f);
        private readonly static Rect Rect_ThingDefContainer = new Rect(610f, 20f, 569f, 40f);
        private readonly static Rect Rect_ThingDefIcons = new Rect(0f, 0f, 25f, 25f);
        private readonly static Rect Rect_ThingDefs = new Rect(45f, 0f, 515f, 25f);
        private readonly static Rect Rect_ThingDefsHighlight = new Rect(0f, 0f, 515f, 25f);
        private readonly static Rect Rect_TempCurve = new Rect(10f, 400f, 580f, 190f);
        private readonly static Rect Rect_HeightCurve = new Rect(610f, 400f, 580f, 190f);
        private readonly static Rect Rect_DefDesc = new Rect(10f, 145f, 580f, 240f);

        private readonly static Rect Rect_ScrollRect = new Rect(new Vector2(610f, 20f), new Vector2(580f, 320f));
        private readonly static Rect Rect_ScrollViewRect = new Rect(new Vector2(610f, 20f), new Vector2(563f, 320f));

        private List<ResourceDef> resources = null;
        private Vector2 DefDescScrollVector = new Vector2();
        private Vector2 ScrollRectVector = new Vector2();

        private ResourceDef def;

        public override Vector2 InitialSize => new Vector2(1240, 640);

        public override void DoWindowContents(Rect inRect)
        {
            DrawSelectorButton();
            DrawDefInfo();
            DrawItems();

            if (def != null)
            {
                SimpleCurveDrawer.DrawCurve(Rect_HeightCurve, def.heightCurve);
                SimpleCurveDrawer.DrawCurve(Rect_TempCurve, def.temperatureCurve);
            }

            DrawBoxes();
        }

        private void DrawItems()
        {
            if (def != null && def.ResourcesCreated.AllowedThingDefs.ToList() is List<ThingDef> thingDefs)
            {
                Widgets.BeginScrollView(Rect_ScrollRect, ref ScrollRectVector, Rect_ScrollViewRect);
                for (int i = 0; i < thingDefs.Count; i++)
                {
                    Rect temp = new Rect(Rect_ThingDefContainer);
                    temp.y += temp.height * i;
                    ThingDef current = thingDefs[i];

                    GUI.BeginGroup(temp);
                    Widgets.DrawHighlight(Rect_ThingDefsHighlight);
                    GUI.DrawTexture(Rect_ThingDefIcons, Widgets.GetIconFor(current));
                    Widgets.Label(Rect_ThingDefs, current.label);

                    GUI.EndGroup();
                }

                Widgets.EndScrollView();
            }
        }

        private void DrawBoxes()
        {
            if (def == null) return;
            Widgets.DrawHighlight(Rect_DefDesc);
            Widgets.DrawHighlight(Rect_TempCurve);
            Widgets.DrawHighlight(Rect_HeightCurve);

            Widgets.DrawBox(Rect_DefDesc, 2);
            Widgets.DrawBox(Rect_TempCurve, 2);
            Widgets.DrawBox(Rect_HeightCurve, 2);
        }

        private void DrawDefInfo()
        {
            if (def == null) return;

            GUI.DrawTexture(Rect_DefIcon, ContentFinder<Texture2D>.Get(def.iconData.texPath));
            Widgets.TextAreaScrollable(Rect_DefDesc, def.description, ref DefDescScrollVector, true);
        }

        /// <summary>
        /// Draws a button that allows the user to select a resource to look at
        /// </summary>
        private void DrawSelectorButton()
        {
            if (Widgets.ButtonText(Rect_DefName, def?.label ?? "Select a resource def to look at"))
            {
                Find.WindowStack.Add(new FloatMenu(DefOptions()));
            }
        }

        /// <summary>
        /// Makes a list out of all ResourceDefs to select from
        /// </summary>
        /// <returns>the list</returns>
        private List<FloatMenuOption> DefOptions()
        {
            resources = resources ?? (resources = DefDatabase<ResourceDef>.AllDefsListForReading);
            List<FloatMenuOption> floatMenuOptions = new List<FloatMenuOption>();

            foreach (ResourceDef def in resources)
            {
                floatMenuOptions.Add(new FloatMenuOption(def.label, delegate
                {
                    this.def = def;
                }));
            }

            return floatMenuOptions;
        }
    }
}
