using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Empire_Rewritten.Utils
{
    /// <summary>
    /// This class provides some useful and often used basic window operations
    /// </summary>
    public static class WindowHelper
    {
        /// <summary>
        /// This draws a border around any <paramref name="rect"/> using the given <paramref name="color"/> and <paramref name="width"/>
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="width"></param>
        /// <param name="color"></param>
        public static void DrawBorderAroundRect(this Rect rect, int width = 1, Color? color = null)
        {
            Color temp = color ?? GUI.color;

            Rect tempRect = rect.ExpandedBy(width);
            Color prev = GUI.color;
            GUI.color = temp;
            Widgets.DrawBox(tempRect, width);
            GUI.color = prev;
        }

        /// <summary>
        ///     Resets the Text.Font, Text.Anchor and GUI.color setting
        /// </summary>
        public static void ResetTextAndColor()
        {
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
            GUI.color = Color.white;
        }

        /// <summary>
        ///     Creates a <see cref="Widgets.ButtonInvisible(Rect, bool)"/> using a <paramref name="label"/>
        ///     inside the given <paramref name="inRect"/>
        /// </summary>
        /// <param name="inRect"></param>
        /// <param name="label"></param>
        /// <returns><c>true</c> if the button is pressed, false otherwise</returns>
        public static bool DrawInfoScreenSelectorButton(Rect inRect, string label)
        {
            Text.Anchor = TextAnchor.MiddleCenter;
            Text.Font = GameFont.Medium;

            Widgets.DrawLightHighlight(inRect);
            Widgets.DrawHighlightIfMouseover(inRect);
            Widgets.Label(inRect, label);
            Widgets.DrawBox(inRect, 2);

            ResetTextAndColor();

            return Widgets.ButtonInvisible(inRect);
        }
    }
}
