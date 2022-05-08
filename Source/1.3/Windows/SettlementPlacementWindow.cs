using Empire_Rewritten.Resources;
using Empire_Rewritten.Utils;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace Empire_Rewritten.Windows
{
    public class SettlementPlacementWindow : Window
    {
        private readonly Rect rectFull = new Rect(0f, 0f, 350f, 650f);
        private readonly Rect rectMain;
        private readonly Rect rectTop;
        private readonly Rect rectMid;
        private readonly Rect rectBot;
        private readonly Rect rectButtonApply;
        private readonly Rect rectResourceInfoLabel;
        private readonly Rect rectResourceInfoOuter;
        private readonly Rect rectResourceInfoInner;
        private readonly Rect rectCostLabel;
        private readonly Rect rectCostOuter;
        private readonly Rect rectCostInner;

        private Dictionary<ResourceDef, ResourceModifier> tileMods = new Dictionary<ResourceDef, ResourceModifier>();
        private Vector2 costScroll;
        private int selectedWorldTile = -1;

        protected override float Margin => 0f;

        public override Vector2 InitialSize => rectFull.size;

        protected override void SetInitialSizeAndPosition()
        {
            Vector2 initialSize = InitialSize;
            windowRect = new Rect(UI.screenWidth - initialSize.x - 30f, (UI.screenHeight - initialSize.y) / 2f, initialSize.x, initialSize.y);
            windowRect = windowRect.Rounded();
        }

        public SettlementPlacementWindow()
        {
            doCloseX = true;
            onlyOneOfTypeAllowed = true;
            preventCameraMotion = false;

            rectMain = rectFull.ContractedBy(25f);
            rectTop = rectMain.TopPartPixels(30f);
            rectBot = rectMain.BottomPartPixels(30f);
            rectMid = new Rect(rectMain.x, rectMain.y + 35f, rectMain.width, rectMain.height - 35f * 2f);
            rectButtonApply = new Rect(rectBot.x, rectBot.y + 5f, rectBot.width, rectBot.height - 10f);

            rectResourceInfoLabel = rectMid.TopPartPixels(25f);
            rectResourceInfoOuter = rectMid.TopPartPixels(29f * 4 /**Default ResourceDef.Count**/).MoveRect(new Vector2(0f, rectResourceInfoLabel.height));

            rectResourceInfoInner = rectResourceInfoOuter.GetInnerScrollRect(29f * ResourceDef.AllResourceDefs.Count());

            rectCostLabel = rectMid.TopPartPixels(25f).MoveRect(new Vector2(0f, rectResourceInfoLabel.height + rectResourceInfoOuter.height));
            rectCostOuter = rectMid.BottomPartPixels(rectMid.height - rectCostLabel.yMax);
            rectCostInner = rectCostOuter.GetInnerScrollRect(29f * 5);

            CameraJumper.TryShowWorld();
        }

        public override void DoWindowContents(Rect inRect)
        {
            GetTileData();

            DrawTop();
            DrawMiddle();
            DrawBottom();

            Widgets.DrawBox(rectCostLabel);
            Widgets.DrawBox(rectCostOuter);
        }

        private void DrawMiddle()
        {
            DrawResourceInfo();
        }

        private void DrawResourceInfo()
        {
            Rect rectFull = rectResourceInfoInner.TopPartPixels(29f);
            Widgets.Label(rectResourceInfoLabel, "Empire_SPW_Modifiers".Translate());
            Widgets.DrawBox(rectResourceInfoOuter);
            Widgets.BeginScrollView(rectResourceInfoOuter, ref costScroll, rectResourceInfoInner);

            int count = 0;
            foreach (KeyValuePair<ResourceDef, ResourceModifier> kvp in tileMods)
            {
                ResourceDef def = kvp.Key;

                rectFull.DoRectHighlight(count % 2 == 1);
                Rect rectIcon = new Rect(rectFull.x + 5f, rectFull.y + 2f, rectFull.height - 4f, rectFull.height - 4f);
                Rect rectLabel = rectFull.RightPartPixels(rectFull.width - rectIcon.xMax);
                Rect rectModLabel = rectFull.RightPartPixels(50f);

                GUI.DrawTexture(rectIcon, ContentFinder<Texture2D>.Get(def.iconData.texPath));
                Widgets.Label(rectLabel, def.LabelCap);
                Widgets.Label(rectModLabel, kvp.Value.multiplier.ToStringPercent());

                rectFull = rectFull.MoveRect(new Vector2(0f, 29f));
                count++;
            }

            Widgets.EndScrollView();
        }

        private void DrawTop()
        {
            Text.Font = GameFont.Medium;
            Widgets.Label(rectTop, "Empire_SPW_Title".Translate());
            Text.Font = GameFont.Small;

            Widgets.DrawLineHorizontal(rectTop.x, rectTop.yMax, rectTop.width);
        }

        private void DrawBottom()
        {
            bool canPlace = CanPlaceHere(out List<string> reasons);

            Widgets.DrawLineHorizontal(rectBot.x, rectBot.y, rectBot.width);
            TooltipHandler.TipRegion(rectButtonApply, reasons.Join((newString) => newString, "\n"));
            
            if (!canPlace) GUI.color = new Color(1f, 0.4f, 0.4f);
            rectButtonApply.DrawButtonText("Empire_SPW_Apply".Translate(), ApplyAction);
            GUI.color = Color.white;
        }

        private void GetTileData()
        {
            int newTile = Find.WorldSelector.selectedTile;
            if (newTile == -1 || newTile == selectedWorldTile) return;
            selectedWorldTile = newTile;

            foreach(ResourceDef def in ResourceDef.AllResourceDefs)
            {
                tileMods.SetOrAdd(def, def.GetTileModifier(Find.WorldGrid.tiles[selectedWorldTile]));
            }
        }

        public bool CanPlaceHere(out List<string> reasons)
        {
            bool flag = true;
            reasons = new List<string>();
            List<Tile> tiles = Find.WorldGrid.tiles;

            if (selectedWorldTile > tiles.Count || selectedWorldTile == -1)
            {
                reasons.Add("Empire_SPW_TileOutOfRange".Translate());
                return false; //Not just change the flag here because the next line would error
            }

            Tile tile = tiles[selectedWorldTile];

            if (tile.WaterCovered)
            {
                reasons.Add("Empire_SPW_Water".Translate());
                flag = false;
            }

            return flag;
        }

        public void ApplyAction()
        {
            if (!CanPlaceHere(out List<string> reasons))
            {
                Messages.Message(new Message(reasons.Join((newString) => newString, "\n"), MessageTypeDefOf.RejectInput));
                return;
            }
            //TODO: Check things inputs for validity here (name can't be empty)

            Close();
        }
    }
}
