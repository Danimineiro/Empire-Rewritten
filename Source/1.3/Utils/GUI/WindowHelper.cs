using UnityEngine;
using Verse;

namespace Empire_Rewritten.Utils
{
    /// <summary>
    ///     This class provides some useful and often used basic window operations
    /// </summary>
    public static class WindowHelper
    {
        /// <summary>
        ///     This draws a border around a <see cref="Rect" />, using a given <see cref="int">border width</see> and
        ///     <see cref="Color" />
        /// </summary>
        /// <param name="rect">The <see cref="Rect" /> to draw a border around</param>
        /// <param name="width">The width of the border</param>
        /// <param name="maybeColor">
        ///     The <see cref="Color">Color?</see> of the border to draw. If null, Defaults to
        ///     <see cref="UnityEngine.GUI.color">UnityEngine's GUI color</see>
        /// </param>
        public static void DrawBorderAroundRect(this Rect rect, int width = 1, Color? maybeColor = null)
        {
            Rect borderRect = rect.ExpandedBy(width);
            GUI.color = maybeColor ?? Color.white;
            Widgets.DrawBox(borderRect, width);
            GUI.color = Color.white;
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
        ///     Creates a <see cref="Widgets.ButtonInvisible(Rect, bool)" /> using a given <see cref="string" /> as label, inside a
        ///     given <see cref="Rect" />
        /// </summary>
        /// <param name="inRect">The <see cref="Rect" /> to draw in</param>
        /// <param name="label">The <see cref="string" /> that should be used as label</param>
        /// <returns><c>true</c> if the button is pressed, <c>false</c> otherwise</returns>
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