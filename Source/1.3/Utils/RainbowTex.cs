using System;

namespace Empire_Rewritten.Utils
{
    internal static class RainbowTex
    {
        private static int red = 255;
        private static int green = 0;
        private static int blue = 255;

        private const int change = 17;

        /// <summary>
        /// Rainbowifies any string
        /// </summary>
        /// <param name="str"></param>
        /// <param name="maxlength"></param>
        /// <returns>the string, but longer because it now has a lot of color attributes</returns>
        public static string Rainbowify(this string str, char splitAtChar = ' ', int maxlength = 990)
        {
            string[] words = str.Split(splitAtChar);
            string returnString = "";
            int processed = 0;

            foreach (string word in words)
            {
                processed += word.Length + 1;
                returnString += $"<color=#{Convert.ToString(red, 16).AddLeading()}{Convert.ToString(green, 16).AddLeading()}{Convert.ToString(blue, 16).AddLeading()}>{word}</color> ";

                ChangeHexColors(red, ref green, ref blue);
                ChangeHexColors(green, ref blue, ref red);
                ChangeHexColors(blue, ref red, ref green);

                if ((returnString.Length + str.Length - processed) > maxlength) break;
            }

            return returnString;
        }

        /// <summary>
        /// Rainbow
        /// </summary>
        /// <param name="color0"></param>
        /// <param name="color1"></param>
        /// <param name="color2"></param>
        private static void ChangeHexColors(int color0, ref int color1, ref int color2)
        {
            if (color0 == 255 && color1 < 255)
            {
                color1 += change;
                color2 -= change;
            }
        }

        /// <summary>
        /// Adds a leading 0 to the given string if the string has a length of less than 1
        /// </summary>
        /// <param name="num"></param>
        /// <returns>another string</returns>
        private static string AddLeading(this string num)
        {
            if (num.Length == 1) return $"0{num}";

            return num;
        }
    }
}
