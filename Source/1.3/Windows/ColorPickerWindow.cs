﻿using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Empire_Rewritten.Utils;
using UnityEngine;
using Verse;

namespace Empire_Rewritten.Windows
{
    public class ColorPickerWindow : Window
    {
        private const int CreatedBoxes = 6;
        private const int ColorComponentHeight = 200;
        private const int HueBarWidth = 20;

        private const int HistoryColumns = 5;
        private const int HistoryRows = 2;

        private readonly string[] colorBuffers = {"255", "255", "255"};
        private readonly List<Color>[] colorHistory;

        private readonly Regex hexRx = new Regex(@"#[a-f0-9]{6}", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private readonly Texture2D hueBarTexture = new Texture2D(1, ColorComponentHeight);

        private readonly Rect rectColorInput;

        private readonly List<Rect> rectColorInputBoxes;
        private readonly Rect rectFull = new Rect(0f, 0f, 600f, 300f);
        private readonly Rect[,] rectHistoryArray;
        private readonly Rect rectHistoryMain;
        private readonly Rect rectHueBar;
        private readonly Rect rectMain;
        private readonly List<Rect> rectRGBInputBoxes;

        private readonly int[] rectRGBValues = {0, 0, 0};
        private readonly Rect rectSaturationValueSquare;
        private bool hexChanged = true;
        private string hexCode = "#FFFFFF";
        private float hue;
        private bool keepTrackingMouseHue;

        private bool keepTrackingMouseSaturation;
        private float oldHue = 1f;

        private Color selectedColor = Color.red;
        private Texture2D texture;

        public ColorPickerWindow()
        {
            closeOnClickedOutside = true;
            forcePause = true;
            preventCameraMotion = true;
            onlyOneOfTypeAllowed = true;

            rectMain = new Rect(rectFull).ContractedBy(25f);
            rectSaturationValueSquare = new Rect(rectMain.position, new Vector2(ColorComponentHeight, ColorComponentHeight));
            rectHueBar = rectSaturationValueSquare.MoveRect(new Vector2(rectSaturationValueSquare.width + 10f, 0f)).LeftPartPixels(HueBarWidth);
            rectColorInput = rectHueBar.MoveRect(new Vector2(rectHueBar.width + 10f, 0f));
            rectColorInput.size = new Vector2(rectMain.width - rectColorInput.position.x + 25f, rectSaturationValueSquare.height);
            rectColorInputBoxes = rectColorInput.DivideVertical(CreatedBoxes).ToList();
            rectRGBInputBoxes = rectColorInputBoxes[3].DivideHorizontal(3).ToList();
            rectHistoryMain = new Rect(rectMain.position.x, ColorComponentHeight + 25f, rectMain.width, rectMain.height - ColorComponentHeight);
            rectHistoryArray = new Rect[HistoryRows, HistoryColumns];
            colorHistory = new List<Color>[HistoryRows];

            Rect[] historyRowRects = rectHistoryMain.DivideVertical(HistoryRows).ToArray();
            for (int i = 0; i < HistoryRows; i++)
            {
                Rect[] historyColumnRects = historyRowRects[i].DivideHorizontal(HistoryColumns).ToArray();
                colorHistory[i] = new List<Color>(HistoryColumns);
                for (int j = 0; j < HistoryColumns; j++)
                {
                    rectHistoryArray[i, j] = historyColumnRects[j];
                }
            }

            for (int y = 0; y < ColorComponentHeight; y++)
            {
                hueBarTexture.SetPixel(0, y, Color.HSVToRGB((float)y / ColorComponentHeight, 1, 1));
            }

            hueBarTexture.Apply();

            RefreshSaturationTexture();
        }

        private Color SelectedColor
        {
            get => selectedColor;
            set
            {
                selectedColor = value;
                UpdateColor();
            }
        }

        public static Color LastSelectedColor { get; private set; }

        public override Vector2 InitialSize => rectFull.size;

        protected override float Margin => 0f;

        private void UpdateColor()
        {
            hexCode = "#";

            for (int i = 0; i < 3; i++)
            {
                rectRGBValues[i] = (int)(SelectedColor[i] * 255);
                colorBuffers[i] = ((int)(SelectedColor[i] * 255)).ToString();
                hexCode += ((int)(SelectedColor[i] * 255)).ToString("X2");
            }

            //If gray, don't update hue => gray tones don't have a hue and would lose the user input
            if (SelectedColor.r != SelectedColor.g || SelectedColor.g != selectedColor.b)
            {
                Color.RGBToHSV(SelectedColor, out hue, out _, out _);
                RefreshSaturationTexture();
            }
        }

        private void SaveCurrentColor()
        {
            if (HistoryContains(SelectedColor, out int index0, out int index1))
            {
                Color temp = colorHistory[index0][index1];
                colorHistory[index0].RemoveAt(index1);
                InsertColorInHistory(temp);
            }
            else
            {
                InsertColorInHistory(SelectedColor);
            }
        }

        private void InsertColorInHistory(Color color)
        {
            colorHistory[0].Insert(0, color);

            if (colorHistory[0].Count > HistoryColumns)
            {
                colorHistory[1].Insert(0, colorHistory[0].Pop());

                if (colorHistory[1].Count > HistoryColumns)
                {
                    colorHistory[1].Pop();
                }
            }
        }

        public override void DoWindowContents(Rect inRect)
        {
            DrawCloseButton(inRect);

            DrawSaturationValueSquare();
            DrawHueBar();

            WindowHelper.DrawBoxes(new[] {rectMain, rectHistoryMain});

            DrawColorHistoryButtons();
            DrawInputFieldLabels();
            DrawHexCodeInputField();
            DrawRGBInputValues();
            DrawControls();

            //Color Preview
            Widgets.DrawBoxSolid(rectColorInputBoxes[4].ContractedBy(5f), Color.black);
            Widgets.DrawBoxSolid(rectColorInputBoxes[4].ContractedBy(10f), SelectedColor);

            //Save color button
            if (Widgets.ButtonText(rectColorInputBoxes[5].ContractedBy(5f), "##SaveColor"))
            {
                SaveCurrentColor();
            }
        }

        private bool HistoryContains(Color color, out int index0, out int index1)
        {
            for (int i = 0; i < colorHistory.Length; i++)
            {
                for (int j = 0; j < colorHistory[i].Count; j++)
                {
                    if (colorHistory[i][j].Equals(color))
                    {
                        index0 = i;
                        index1 = j;
                        return true;
                    }
                }
            }

            index0 = -1;
            index1 = -1;
            return false;
        }

        private void DrawColorHistoryButtons()
        {
            for (int i = 0; i < colorHistory.Length; i++)
            {
                for (int j = 0; j < colorHistory[i].Count; j++)
                {
                    Rect tempMainRect = rectHistoryArray[i, j];

                    Widgets.DrawBoxSolid(tempMainRect, Color.white);
                    Widgets.DrawBoxSolid(tempMainRect.ContractedBy(1f), colorHistory[i][j]);
                    if (Widgets.ButtonInvisible(tempMainRect))
                    {
                        SelectedColor = colorHistory[i][j];
                        SaveCurrentColor();
                    }
                }
            }
        }

        private void DrawControls()
        {
            Color.RGBToHSV(SelectedColor, out float _, out float saturation, out float value);

            // Cross-hair
            Rect verticalLine = new Rect(0f, (int)(ColorComponentHeight - value * ColorComponentHeight - 2f), ColorComponentHeight, 3f);
            Rect horizontalLine = new Rect((int)(saturation * ColorComponentHeight - 2f), 0f, 3f, ColorComponentHeight);

            GUI.BeginGroup(rectSaturationValueSquare);

            GUI.color = Color.gray;
            Widgets.DrawBox(verticalLine);
            Widgets.DrawBox(horizontalLine);
            GUI.color = Color.white;

            Widgets.DrawBoxSolid(verticalLine.ContractedBy(1), Color.black);
            Widgets.DrawBoxSolid(horizontalLine.ContractedBy(1), Color.black);

            GUI.EndGroup();

            //HueLine
            Rect hueLine = new Rect(0f, (int)(rectHueBar.height - rectHueBar.height * hue - 2f), rectHueBar.width, 3);
            GUI.BeginGroup(rectHueBar);

            GUI.color = Color.gray;
            Widgets.DrawBox(hueLine);
            GUI.color = Color.white;

            Widgets.DrawBoxSolid(hueLine.ContractedBy(1), Color.black);

            GUI.EndGroup();
        }

        private void DrawHueBar()
        {
            GUI.DrawTexture(rectHueBar, hueBarTexture);

            if ((Mouse.IsOver(rectHueBar) || keepTrackingMouseHue) && Input.GetMouseButton(0) && !keepTrackingMouseSaturation)
            {
                keepTrackingMouseHue = true;
                Vector2 mousePositionInRect = Event.current.mousePosition - rectHueBar.position;

                mousePositionInRect.x = Mathf.Clamp(mousePositionInRect.x, 0f, rectHueBar.width);
                mousePositionInRect.y = Mathf.Clamp(mousePositionInRect.y, 0f, rectHueBar.height);

                Color.RGBToHSV(SelectedColor, out float _, out float saturation, out float value);
                hue = 1f - mousePositionInRect.y / rectHueBar.height;

                SelectedColor = Color.HSVToRGB(hue, saturation, value);
                RefreshSaturationTexture();
            }

            keepTrackingMouseHue = keepTrackingMouseHue && Input.GetMouseButton(0);
        }

        private void DrawSaturationValueSquare()
        {
            GUI.DrawTexture(rectSaturationValueSquare, texture);

            if ((Mouse.IsOver(rectSaturationValueSquare) || keepTrackingMouseSaturation) && Input.GetMouseButton(0) && !keepTrackingMouseHue)
            {
                keepTrackingMouseSaturation = true;
                Vector2 mousePositionInRect = Event.current.mousePosition - rectSaturationValueSquare.position;

                mousePositionInRect.x = Mathf.Clamp(mousePositionInRect.x, 0f, ColorComponentHeight);
                mousePositionInRect.y = Mathf.Clamp(mousePositionInRect.y, 0f, ColorComponentHeight);

                SelectedColor = Color.HSVToRGB(hue, mousePositionInRect.x / ColorComponentHeight, 1f - mousePositionInRect.y / ColorComponentHeight);
            }

            keepTrackingMouseSaturation = keepTrackingMouseSaturation && Input.GetMouseButton(0);
        }

        private void RefreshSaturationTexture()
        {
            if (hue == oldHue) return;

            oldHue = hue;
            Texture2D newTexture = new Texture2D(ColorComponentHeight, ColorComponentHeight)
            {
                wrapMode = TextureWrapMode.Clamp
            };

            Color[] colors = new Color[ColorComponentHeight * ColorComponentHeight];
            for (int x = 0; x < ColorComponentHeight; x++)
            {
                for (int y = 0; y < ColorComponentHeight; y++)
                {
                    colors[x + y * ColorComponentHeight] = Color.HSVToRGB(hue, (float)x / ColorComponentHeight, (float)y / ColorComponentHeight);
                }
            }

            newTexture.SetPixels(colors);
            newTexture.Apply();
            texture = newTexture;
        }

        /// <summary>
        ///     Draws the input field labels
        /// </summary>
        private void DrawInputFieldLabels()
        {
            Text.Anchor = TextAnchor.MiddleCenter;
            Text.Font = GameFont.Medium;
            Widgets.Label(rectColorInputBoxes[0], "##HexCode");
            Widgets.Label(rectColorInputBoxes[2], "##RGB");
            WindowHelper.ResetTextAndColor();
        }

        /// <summary>
        ///     Creates the <see cref="hexCode" /> value input field
        ///     Changes the <see cref="rectRGBValues" /> when a new value is inputted
        /// </summary>
        private void DrawHexCodeInputField()
        {
            if (!hexRx.IsMatch(hexCode)) //Mark the field red if there is an error
            {
                GUI.color = Color.red;
            }
            else if (hexChanged) //Only changes if the hex code is legal
            {
                int num = int.Parse(hexCode.Substring(1), NumberStyles.AllowHexSpecifier);

                float b = (num & 0xFF) / 255f;
                float g = ((num >> 8) & 0xFF) / 255f;
                float r = ((num >> 16) & 0xFF) / 255f;

                SelectedColor = new Color(r, g, b);

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
                    hexCode = $"{hexCode.Substring(0, 2)}{(hexCode.Length >= 3 ? hexCode.Substring(3) : string.Empty)}";
                }
            }
        }

        /// <summary>
        ///     Creates the RGB value input fields and stores the inputs inside <see cref="rectRGBValues" />
        ///     Changes the <see cref="hexCode" /> when new values are inputted
        /// </summary>
        private void DrawRGBInputValues()
        {
            bool rgbChanged = false;

            //Creates the RGB value inputs and handles them
            for (int i = 0; i < 3; i++)
            {
                int value = rectRGBValues[i];
                string colorBuffer = colorBuffers[i];

                Widgets.TextFieldNumeric(rectRGBInputBoxes[i].ContractedBy(5f), ref value, ref colorBuffer, 0f, 255f);

                rgbChanged = rgbChanged || value != rectRGBValues[i];

                rectRGBValues[i] = value;
                colorBuffers[i] = colorBuffer;
            }

            if (rgbChanged)
            {
                SelectedColor = new Color(rectRGBValues[0] / 255f, rectRGBValues[1] / 255f, rectRGBValues[2] / 255f);
            }
        }

        private void DrawCloseButton(Rect inRect)
        {
            if (Widgets.CloseButtonFor(inRect)) Close();
        }

        public override void Close(bool doCloseSound = true)
        {
            LastSelectedColor = SelectedColor;
            base.Close(doCloseSound);
        }
    }
}
