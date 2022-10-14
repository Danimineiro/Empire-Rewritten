using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Empire_Rewritten.Controllers;
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
        private readonly Rect rectTopPart;
        private readonly Rect rectMiddlePart;
        private readonly Rect rectBottomPart;

        private string playerFactionName = Faction.OfPlayer.Name;

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
            rectTopPart = rectMain.TopPartPixels(30f);
            rectBottomPart = rectMain.BottomPartPixels(30f);
            rectMiddlePart = new Rect(rectMain.x, rectMain.y + 30f, rectMain.width, rectMain.height - 30f * 2f);

            rectLeft = rectMiddlePart.LeftPartPixels(330f);
            rectLeftNameLabel = rectLeft.TopPartPixels(30f);
            rectLeftNameInput = rectLeftNameLabel.MoveRect(new Vector2(0f, rectLeftNameLabel.height + 5f));
        }

        public override void DoWindowContents(Rect inRect)
        {
            Text.Font = GameFont.Medium;
            Widgets.Label(rectTopPart, "Empire_CPF_Title".Translate());
            Text.Font = GameFont.Small;

            Widgets.DrawLineHorizontal(rectTopPart.x, rectTopPart.yMax, rectTopPart.width);

            Widgets.Label(rectLeftNameLabel, "Empire_CPF_InputFactionName".Translate());
            playerFactionName = Widgets.TextField(rectLeftNameInput, playerFactionName, 25, CharacterCardUtility.ValidNameRegex);

            Widgets.DrawLineHorizontal(rectBottomPart.x, rectBottomPart.y, rectBottomPart.width);
            new Rect(rectBottomPart.x, rectBottomPart.y + 5f, rectBottomPart.width, rectBottomPart.height - 10f).DrawButtonText("Empire_CPF_Apply".Translate(), ApplyAction);
        }

        public void ApplyAction()
        {
            if (playerFactionName.NullOrEmpty() && Faction.OfPlayer!=null)
                return;

            Faction.OfPlayer.Name = playerFactionName;
            UpdateController updateController = UpdateController.CurrentWorldInstance;
            FactionController controller = updateController.FactionController;
            controller.CreatePlayer();

            Close();
        }
    }
}
