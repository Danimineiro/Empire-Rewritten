using System;
using System.Text;

namespace Empire_Rewritten.Utils
{
    internal static class RainbowTex
    {
        private static int _red = 255;
        private static int _green;
        private static int _blue = 255;

        private static string CurrentHex => $"#{_red.AsHex()}{_green.AsHex()}{_blue.AsHex()}";

        /// <summary>
        ///     Converts a <see cref="T:string[]" /> into a rainbowified <see cref="string" />.
        /// </summary>
        /// <param name="words">The <see cref="T:string[]" /> to rainbowify</param>
        /// <param name="change"></param>
        /// <param name="joiner">
        ///     The <see cref="string" /> to put between each of the <paramref name="words">words</paramref>; If null, uses the
        ///     <see cref="string.Empty">empty string</see>
        /// </param>
        /// <param name="maxLength">The maximum length of the returned <see cref="string" /></param>
        /// <returns>
        ///     The elements of <paramref name="words" /> joined and rainbowified.
        ///     Will not be longer than <paramref name="maxLength" />, but may be missing words at the end.
        /// </returns>
        public static string Rainbowify(this string[] words, int change = 17, string joiner = " ", int maxLength = 1000)
        {
            if (joiner is null) joiner = string.Empty;
            StringBuilder final = new StringBuilder();

            for (int i = 0; i < words.Length; i++)
            {
                string word = words[i];
                // If word would overflow maxLength, stop
                if (final.Length + joiner.Length + word.Length + "<color=#000000></color>".Length > maxLength)
                {
                    break;
                }

                final.Append($"<color={CurrentHex}>{word}</color>");

                ChangeHexColors(_red, ref _green, ref _blue, change);
                ChangeHexColors(_green, ref _blue, ref _red, change);
                ChangeHexColors(_blue, ref _red, ref _green, change);

                if (i < words.Length - 1)
                {
                    final.Append(joiner);
                }
            }

            return final.ToString();
        }

        /// <summary>
        ///     Rainbowifies a <see cref="string" />
        /// </summary>
        /// <param name="str">The <see cref="string" /> to rainbowify</param>
        /// <param name="splitAt">
        ///     The <see cref="char">char?</see> to split <paramref name="str" /> at; If <c>null</c>, splits after every
        ///     character
        /// </param>
        /// <param name="change"></param>
        /// <param name="maxLength">The maximum length of the returned <see cref="string" /></param>
        /// <returns>
        ///     A rainbowified version of <paramref name="str" />. Will not be longer than <paramref name="maxLength" />, but
        ///     may be cut off at the end.
        /// </returns>
        public static string Rainbowify(this string str, char? splitAt = ' ', int change = 17, int maxLength = 1000)
        {
            return SplitString(str, splitAt).Rainbowify(change, splitAt?.ToString(), maxLength);
        }

        /// <summary>
        ///     Splits a <see cref="string" /> on a given <see cref="char">char?</see>.
        /// </summary>
        /// <param name="str">The <see cref="string" /> to split</param>
        /// <param name="splitAt">The <see cref="char">char?</see> to split on; If <c>null</c>, splits into 1-character strings</param>
        /// <returns>A <see cref="T:string[]" /> of the split strings</returns>
        private static string[] SplitString(string str, char? splitAt)
        {
            return splitAt is char splitAtChar ? str.Split(splitAtChar) : SplitString(str);
        }

        /// <summary>
        ///     Splits a <see cref="string" /> into an <see cref="T:string[]" /> of equal-length strings.
        /// </summary>
        /// <param name="str">The <see cref="string" /> to split</param>
        /// <param name="splitSize">The length each string should have</param>
        /// <returns>
        ///     A <see cref="T:string[]" /> with members of length <paramref name="splitSize" />. The last entry may be
        ///     shorter if <paramref name="str" />'s length is not evenly divisible by <paramref name="splitSize" />
        /// </returns>
        private static string[] SplitString(string str, int splitSize = 1)
        {
            string[] splitArray = new string[str.Length / splitSize + 1];
            splitSize = Math.Max(splitSize, 1);

            for (int i = 0; i < splitArray.Length; i++)
            {
                if ((i + 1) * splitSize > str.Length)
                {
                    splitArray[i] = str.Substring(i * splitSize);
                }
                else
                {
                    splitArray[i] = str.Substring(i * splitSize, splitSize);
                }
            }

            return splitArray;
        }

        /// <summary>
        ///     TODO: Document
        /// </summary>
        /// <param name="color0"></param>
        /// <param name="color1"></param>
        /// <param name="color2"></param>
        /// <param name="change"></param>
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
        ///     Converts an <see cref="int" /> to its hex representation (as a <see cref="string" />), with a length of at least 2,
        ///     padding '0' if necessary.
        /// </summary>
        /// <param name="num">The <see cref="int" /> to convert</param>
        /// <returns><paramref name="num" /> as a hex <see cref="string" /></returns>
        private static string AsHex(this int num)
        {
            return Convert.ToString(num, 16).PadLeft(2, '0');
        }
    }
}