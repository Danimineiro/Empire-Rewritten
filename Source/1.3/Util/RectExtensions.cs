using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Empire_Rewritten.Util
{
    public static class RectExtensions
    {
        /// <summary>
        /// Moves a <paramref name="rect"/> using a vector <paramref name="vec"/>
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="vec"></param>
        /// <returns>the moved <paramref name="rect"/></returns>
        public static Rect MoveRect(this Rect rect, Vector2 vec) => new Rect(rect.x + vec.x, rect.y + vec.y, rect.width, rect.height);
        
        /// <summary>
        /// Moves a <paramref name="rect"/> by <paramref name="x"/> and <paramref name="y"/>
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>the moved <paramref name="rect"/></returns>
        public static Rect MoveRect(this Rect rect, int x, int y) => MoveRect(rect, new Vector2(x, y));

        /// <summary>
        /// Adds or subtracts <paramref name="scaleByPixel"/> from a <paramref name="rect"/>s sides
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="scaleByPixel"></param>
        /// <returns></returns>
        public static Rect ScaleXByPixel(this Rect rect, float scaleByPixel) => new Rect(rect.x - scaleByPixel, rect.y, rect.width + scaleByPixel, rect.height);
    }
}
