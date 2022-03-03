using UnityEngine;
using Verse;

namespace Empire_Rewritten.Utils.GUI
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
            if (maybeColor is Color color)
            {
                // Use passed color to draw border, then reset GUI color to what it was before
                Color previousColor = UnityEngine.GUI.color;
                UnityEngine.GUI.color = color;
                Widgets.DrawBox(borderRect, width);
                UnityEngine.GUI.color = previousColor;
            }
            else
            {
                // Just use UnityEngine's color, no need to assign variables.
                Widgets.DrawBox(borderRect, width);
            }
        }
    }
}