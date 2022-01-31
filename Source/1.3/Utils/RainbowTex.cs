using System;
using System.Linq;
using Verse;
using HarmonyLib;

namespace Empire_Rewritten.Utils
{
    internal static class RainbowTex
    {
        private static int red = 255;
        private static int green = 0;
        private static int blue = 255;

        /// <summary>
        /// Rainbowifys an array <paramref name="words"/> of words
        /// </summary>
        /// <param name="words"></param>
        /// <param name="maxLength"></param>
        /// <returns>the string, but longer because it now has a lot of color attributes</returns>
        public static string Rainbowify(this string[] words, int change = 17, string putInBetweenWords = " ", int maxLength = 1000)
        {
            int length = 0;

            foreach (string word in words.Where(str => str != null))
            {
                length += word.Length;
            }

            string returnString = "";
            int processed = 0;

            for (int i = 0; i < words.Length; i++)
            {
                if (words[i] == null)
                {
                    Log.Message("Here");
                    break;
                }

                string word = words[i];
                processed += word.Length + 1;
                returnString += $"<color=#{Convert.ToString(red, 16).AddLeading()}{Convert.ToString(green, 16).AddLeading()}{Convert.ToString(blue, 16).AddLeading()}>{word}</color>{putInBetweenWords}";

                ChangeHexColors(red, ref green, ref blue, change);
                ChangeHexColors(green, ref blue, ref red, change);
                ChangeHexColors(blue, ref red, ref green, change);

                if (words.Length < i + 1 && (returnString.Length + (words[i+1]?.Length ?? 0) - processed + "<color=#FFFFFF></color>".Length) > maxLength - 30) break;
            }

            return returnString;
        }

        /// <summary>
        /// Rainbowifies any string
        /// </summary>
        /// <param name="str"></param>
        /// <param name="maxlength"></param>
        /// <returns>the string, but longer because it now has a lot of color attributes</returns>
        public static string Rainbowify(this string str, string splitAtChar = " ", int change = 17, string putInBetweenWords = " ", int maxLength = 1000) =>
            Rainbowify(SplitString(str, splitAtChar), change, putInBetweenWords, maxLength);

        /// <summary>
        /// Rainbowifys any string <paramref name="str"/>, by splitting it into parts of size <paramref name="splitSize"/>
        /// </summary>
        /// <param name="str"></param>
        /// <param name="splitSize"></param>
        /// <param name="maxLength"></param>
        /// <returns>the string, but longer because it now has a lot of color attributes</returns>
        public static string Rainbowify(this string str, int splitSize = 0, int change = 17, string putInBetweenWords = "", int maxLength = 1000) =>
            Rainbowify(SplitString(str, splitSize), change, putInBetweenWords, maxLength);


        /// <summary>
        /// Splits a string <paramref name="str"/> at the given <paramref name="splitAtChar"/> or every letter
        /// </summary>
        /// <param name="str"></param>
        /// <param name="splitAtChar"></param>
        /// <returns>the generated array</returns>
        private static string[] SplitString(string str, string splitAtChar)
        {
            if (splitAtChar.NullOrEmpty()) return SplitString(str, 1);

            return str.Split(splitAtChar[0]);
        }

        /// <summary>
        /// Splits a string <paramref name="str"/> into an array where each entry has a length of <paramref name="splitSize"/>
        /// </summary>
        /// <param name="str"></param>
        /// <param name="splitSize"></param>
        /// <returns>the generated array</returns>
        private static string[] SplitString(string str, int splitSize = 1)
        {
            string[] array = new string[str.Length];
            splitSize = Math.Max(splitSize, 1);

            for (int i = 0; i < array.Length ; i += splitSize)
            {
                string temp = "";

                Log.Message($"arrayLength: {array.Length}, i: {i}");

                for (int j = i; j < splitSize + i && j < array.Length; j++)
                {
                    temp += str[j];
                    Log.Message($"arrayLength: {temp}, j: {j}");
                }

                array[i] = temp;
            }

            return array;
        }

        /// <summary>
        /// Rainbow
        /// </summary>
        /// <param name="color0"></param>
        /// <param name="color1"></param>
        /// <param name="color2"></param>
        private static void ChangeHexColors(int color0, ref int color1, ref int color2, int change)
        {
            if (color0 == 255 && color1 < 255)
            {
                color1 += change;
                color2 -= change;
            }

            color1 = Math.Min(255, color1);
            color2 = Math.Max(0, color2);
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
