using UnityEngine;

namespace Empire_Rewritten.Utils.GUI
{
    public static class RectExtensions
    {
        /// <summary>
        ///     Creates a copy of this <see cref="Rect" /> moved by a <see cref="Vector2" />
        /// </summary>
        /// <param name="rect">the <see cref="Rect" /> to move</param>
        /// <param name="vec">the distance to move <paramref name="rect" /></param>
        /// <returns>A copy of <paramref name="rect" />, moved by the distance specified in <paramref name="vec" /></returns>
        public static Rect MoveRect(this Rect rect, Vector2 vec)
        {
            var newRect = new Rect(rect);
            newRect.position += vec;
            return newRect;
        }

        /// <summary>
        ///     Creates a copy of a <see cref="Rect" /> with its left edge moved by <paramref name="scaleBy" />, while
        ///     maintaining its width.
        /// </summary>
        /// <param name="rect">The <see cref="Rect" /> to modify</param>
        /// <param name="scaleBy">The amount of units to scale <paramref name="rect" /> by</param>
        /// <returns>A copy of <paramref name="rect" /> with its left edge moved to the left by <paramref name="scaleBy" /> units</returns>
        public static Rect ScaleX(this Rect rect, float scaleBy)
        {
            var newRect = new Rect(rect);
            newRect.xMin -= scaleBy;
            return newRect;
        }
    }
}
