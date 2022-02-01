using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Empire_Rewritten.Util
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
            if (!(color is Color temp)) temp = GUI.color;

            Rect tempRect = rect.ExpandedBy(width);
            Color prev = GUI.color;
            GUI.color = temp;
            Widgets.DrawBox(tempRect, width);
            GUI.color = prev;
        }
    }
}
