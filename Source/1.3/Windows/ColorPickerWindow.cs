using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Empire_Rewritten.Utils;
using UnityEngine;
using Verse;

namespace Empire_Rewritten.Windows
{
    public class ColorPickerWindow : Window
    {
        private readonly Rect rectFull = new Rect(0f, 0f, 600f, 600f);
        private readonly Rect rectMain;
        private readonly Rect rectColorSquare;
        private readonly Rect rectColorBar;
        private readonly Rect rectColorInput;
        private readonly List<Rect> rectColorInputBoxes;
        private readonly List<Rect> rectRGBInputBoxes;
        private readonly List<int> rectRGBValues = new List<int>(3) { 0, 0, 0 };
        private readonly List<string> colorBuffers = new List<string>(3) { "255", "255", "255" };

        private readonly Regex hexRx = new Regex(@"[^a-f0-9]", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private bool hexChanged = true;
        private bool rgbChanged = false;
        private const int CreatedBoxes = 4;
        private string hexCode = "#FFFFFF";

        public override Vector2 InitialSize => rectFull.size;

        protected override float Margin => 0f;

        public ColorPickerWindow()
        {
            rectMain = new Rect(rectFull).ContractedBy(25f);
            rectColorSquare = new Rect(rectMain.position, new Vector2(200f, 200f));
            rectColorBar = rectColorSquare.MoveRect(new Vector2(rectColorSquare.width + 10f, 0f)).LeftPartPixels(20f);
            rectColorInput = rectColorBar.MoveRect(new Vector2(rectColorBar.width + 10f, 0f));
            rectColorInput.size = new Vector2(rectMain.width - rectColorInput.position.x + 25f, rectColorSquare.height);
            rectColorInputBoxes = rectColorInput.DivideVertical(CreatedBoxes).ToList();
            rectRGBInputBoxes = rectColorInputBoxes[3].DivideHorizontal(3).ToList();
        }

        public override void DoWindowContents(Rect inRect)
        {
            DrawCloseButton(inRect);
            WindowHelper.DrawBoxes(new Rect[] { rectMain, rectColorSquare, rectColorBar, rectColorInput });
            
            DrawInputFieldLabels();
            DrawHexCodeInputField();
            DrawRGBInputValues();
        }

        /// <summary>
        /// Draws the input field labels
        /// </summary>
        private void DrawInputFieldLabels()
        {
            Text.Anchor = TextAnchor.MiddleCenter;
            Text.Font = GameFont.Medium;
            Widgets.Label(rectColorInputBoxes[0], "Hex");
            Widgets.Label(rectColorInputBoxes[2], "RGB");
            WindowHelper.ResetTextAndColor();
        }

        /// <summary>
        ///     Creates the <see cref="hexCode"/> value input field
        ///     Changes the <see cref="rectRGBValues"/> when a new value is inputted
        /// </summary>
        private void DrawHexCodeInputField()
        {
            if (hexRx.IsMatch(hexCode.Substring(1)) || hexCode.Length != 7) //Mark the field red if there is an error
            {
                GUI.color = Color.red;
            }
            else if (hexChanged) //Only changes if the hexcode is legal
            {
                for (int i = 0; i < 3; i++)
                {
                    rectRGBValues[i] = int.Parse(hexCode.Substring(1 + 2 * i, 2), System.Globalization.NumberStyles.HexNumber);
                }

                hexChanged = false;
            }

            string hexBefore = hexCode;
            hexCode = Widgets.TextField(rectColorInputBoxes[1].ContractedBy(5f), hexCode);
            hexChanged = !hexBefore.Equals(hexCode) || hexChanged;
            GUI.color = Color.white;

            //Checks if a hex code starts with the # char and sets it if it's missing
            if (!hexCode.StartsWith("#"))
            {
                hexCode = $"#{hexCode}";

                //Fixes the # char being moved to the third position if someone writes before it
                if (hexCode.Length >= 3 && hexCode[2].Equals('#'))
                {
                    hexCode = $"{hexCode.Substring(0, 2)}{(hexCode.Length >= 4 ? hexCode.Substring(4) : string.Empty)}";
                }
            }
        }

        /// <summary>
        ///     Creates the RGB value input fields and stores the inputs inside <see cref="rectRGBValues"/>
        ///     Changes the <see cref="hexCode"/> when new values are inputted
        /// </summary>
        private void DrawRGBInputValues()
        {
            //Creates the RGB value inputs and handles them
            for (int i = 0; i < 3; i++)
            {
                int value = rectRGBValues[i];
                string colorBuffer = colorBuffers[i];

                int colorBefore = value;
                Widgets.TextFieldNumeric(rectRGBInputBoxes[i].ContractedBy(5f), ref value, ref colorBuffer, 0f, 255f);

                rectRGBValues[i] = value;
                colorBuffers[i] = colorBuffer;

                if (!colorBefore.Equals(rectRGBValues[i]))
                {
                    rgbChanged = true;
                }
            }

            //Adjusts the hexCode if the rgb values were changed
            if (rgbChanged)
            {
                hexCode = "#";

                for (int i = 0; i < 3; i++)
                {
                    hexCode += rectRGBValues[i].ToString("X2");
                }

                rgbChanged = false;
            }
        }

        private void DrawCloseButton(Rect inRect)
        {
            if (Widgets.CloseButtonFor(inRect)) Close();
        }
    }
}
