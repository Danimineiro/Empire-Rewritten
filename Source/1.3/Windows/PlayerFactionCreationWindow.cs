using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Empire_Rewritten.Utils;
using RimWorld;
using UnityEngine;
using Verse;

namespace Empire_Rewritten.Windows
{
    public class PlayerFactionCreationWindow : Window
    {

        private readonly Rect rectFull = new Rect(0f, 0f, 1000f, 600f);
        private readonly Rect rectMain;
        private readonly Rect rectLeft;
        private readonly Rect rectLeftNameLabel;
        private readonly Rect rectLeftNameInput;

        private string playerFactionName = "";

        public override Vector2 InitialSize => rectFull.size;

        protected override float Margin => 0f;

        public PlayerFactionCreationWindow()
        {
            doCloseX = true;
            closeOnClickedOutside = true;
            forcePause = true;
            preventCameraMotion = true;
            onlyOneOfTypeAllowed = true;

            rectMain = rectFull.ContractedBy(25f);
            rectLeft = rectMain.LeftPartPixels(330f);
            rectLeftNameLabel = rectLeft.TopPartPixels(30f);
            rectLeftNameInput = rectLeftNameLabel.MoveRect(new Vector2(0f, rectLeftNameLabel.height + 5f));
        }

        public override void DoWindowContents(Rect inRect)
        {
            Widgets.Label(rectLeftNameLabel, "##InputFactionName");
            playerFactionName = Widgets.TextField(rectLeftNameInput, playerFactionName, 25, CharacterCardUtility.ValidNameRegex);
            Widgets.DrawBox(rectMain);
        }
    }
}
