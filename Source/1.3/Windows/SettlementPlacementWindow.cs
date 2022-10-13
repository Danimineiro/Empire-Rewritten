using System.Collections.Generic;
using Empire_Rewritten.Controllers;
using Empire_Rewritten.Resources;
using Empire_Rewritten.Settlements;
using Empire_Rewritten.Territories;
using Empire_Rewritten.Utils;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace Empire_Rewritten.Windows
{
    public class SettlementPlacementWindow : Window
    {
        private readonly Dictionary<ResourceDef, ResourceModifier> tileMods = new Dictionary<ResourceDef, ResourceModifier>();
        private readonly Empire playerEmpire;

        private readonly Rect rectFull = new Rect(0f, 0f, 350f, 650f);
        private readonly Rect rectMain;
        private readonly Rect rectTop;
        private readonly Rect rectMid;
        private readonly Rect rectBot;
        private readonly Rect rectButtonApply;

        private readonly Rect rectCostLabel;
        private readonly Rect rectCostOuter;
        private readonly Rect rectCostInner;

        private readonly Rect rectResourceInfoLabel;
        private readonly Rect rectResourceInfoOuter;
        private readonly Rect rectResourceInfoInner;

        private int selectedWorldTile = -1;
        private Vector2 resourceScroll;
        private Vector2 costScroll;

        public SettlementPlacementWindow()
        {
            playerEmpire = Empire.PlayerEmpire;

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

            rectResourceInfoInner = rectResourceInfoOuter.GetInnerScrollRect(29f * DefDatabase<ResourceDef>.DefCount);

            rectCostLabel = rectMid.TopPartPixels(25f).MoveRect(new Vector2(0f, rectResourceInfoLabel.height + rectResourceInfoOuter.height + 5f));
            rectCostOuter = rectMid.TopPartPixels(29f * 4).MoveRect(new Vector2(0f, rectResourceInfoLabel.yMax * 2f));
            rectCostInner = rectCostOuter.GetInnerScrollRect(29f * Empire.SettlementCost.Count);

            CameraJumper.TryShowWorld();
        }

        protected override float Margin => 0f;

        public override Vector2 InitialSize => rectFull.size;

        protected override void SetInitialSizeAndPosition()
        {
            Vector2 initialSize = InitialSize;
            windowRect = new Rect(UI.screenWidth - initialSize.x - 30f, (UI.screenHeight - initialSize.y) / 2f, initialSize.x, initialSize.y);
            windowRect = windowRect.Rounded();
        }

        public override void DoWindowContents(Rect inRect)
        {
            GetTileData();

            DrawTop();
            DrawResourceInfo();
            DrawBottom();
            DrawCostList();
        }

        private void DrawCostList()
        {
            Widgets.Label(rectCostLabel, "Empire_SPW_ColonyCost".Translate());
            Widgets.DrawBox(rectCostOuter);

            Widgets.BeginScrollView(rectCostOuter, ref costScroll, rectCostInner);

            int count = 0;
            Rect rectFull = rectCostInner.TopPartPixels(29f);
            foreach (KeyValuePair<ThingDef, int> kvp in Empire.SettlementCost)
            {
                ThingDef def = kvp.Key;

                rectFull.DoRectHighlight(count % 2 == 1);
                Rect rectIcon = new Rect(rectFull.x + 5f, rectFull.y + 2f, rectFull.height - 4f, rectFull.height - 4f);
                Rect rectLabel = rectFull.MoveRect(new Vector2(rectFull.height + 5f, 0f));
                Rect rectCostLabel = rectFull.LeftPartPixels(rectFull.width - rectFull.height);
                Rect rectThingInfo = rectFull.RightPartPixels(rectFull.height).ContractedBy(4f);

                Widgets.ThingIcon(rectIcon, def);
                Widgets.Label(rectLabel, def.LabelCap);

                Text.Anchor = TextAnchor.MiddleRight;
                Widgets.Label(rectCostLabel, kvp.Value.ToString());
                Text.Anchor = TextAnchor.UpperLeft;

                Widgets.InfoCardButton(rectThingInfo, def);

                rectFull = rectFull.MoveRect(new Vector2(0f, 29f));
                count++;
            }

            Widgets.EndScrollView();
        }

        private void DrawResourceInfo()
        {
            Rect rectFull = rectResourceInfoInner.TopPartPixels(29f);
            Widgets.Label(rectResourceInfoLabel, "Empire_SPW_Modifiers".Translate());
            Widgets.DrawBox(rectResourceInfoOuter);
            Widgets.BeginScrollView(rectResourceInfoOuter, ref resourceScroll, rectResourceInfoInner);

            int count = 0;
            foreach (KeyValuePair<ResourceDef, ResourceModifier> kvp in tileMods)
            {
                ResourceDef def = kvp.Key;

                rectFull.DoRectHighlight(count % 2 == 1);
                Rect rectIcon = new Rect(rectFull.x + 5f, rectFull.y + 2f, rectFull.height - 4f, rectFull.height - 4f);
                Rect rectLabel = rectFull.MoveRect(new Vector2(rectFull.height + 5f, 0f));
                Rect rectModLabel = rectFull.LeftPartPixels(rectFull.width - rectFull.height);
                Rect rectThingInfo = rectFull.RightPartPixels(rectFull.height).ContractedBy(4f);

                GUI.DrawTexture(rectIcon, ContentFinder<Texture2D>.Get(def.iconData.texPath));
                Widgets.Label(rectLabel, def.LabelCap);

                Text.Anchor = TextAnchor.MiddleRight;
                Widgets.Label(rectModLabel, kvp.Value.TotalProduced().ToStringPercent());
                Text.Anchor = TextAnchor.UpperLeft;

                if (WindowHelper.InfoCardButtonWorker(rectThingInfo))
                {
                    Find.WindowStack.Add(new ResourceInfoWindow(def));
                }

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
            bool canPlace = CanPlaceHere(out List<string> reasons, playerEmpire);

            Widgets.DrawLineHorizontal(rectBot.x, rectBot.y, rectBot.width);
            TooltipHandler.TipRegion(rectButtonApply, reasons.Join(newString => newString, "\n"));

            if (!canPlace) GUI.color = new Color(1f, 0.4f, 0.4f);
            rectButtonApply.DrawButtonText("Empire_SPW_Apply".Translate(), ApplyAction);
            GUI.color = Color.white;
        }

        private void GetTileData()
        {
            int newTile = Find.WorldSelector.selectedTile;
            if (newTile == -1 || newTile == selectedWorldTile) return;
            selectedWorldTile = newTile;

            foreach (ResourceDef def in DefDatabase<ResourceDef>.AllDefsListForReading)
            {
                tileMods.SetOrAdd(def, def.GetTileModifier(Find.WorldGrid.tiles[selectedWorldTile]));
            }
        }

        public bool CanPlaceHere(out List<string> reasons, Empire playerEmpire)
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

            Territory territory = TerritoryManager.GetTerritoryManager.GetTerritory(Faction.OfPlayer);
            if (tile.WaterCovered)
            {
                reasons.Add("Empire_SPW_Water".Translate());
                flag = false;
            }

            if (!playerEmpire.StorageTracker.CanRemoveThingsFromStorage(Empire.SettlementCost))
            {
                reasons.Add("Empire_SPW_NoResources".Translate());
                flag = false;
            }

            if (!territory.HasTile(selectedWorldTile))
            {
                reasons.Add("Empire_SPW_TileNotInTerritory".Translate());
                flag = false;
            }

            if (Find.WorldObjects.AnySettlementBaseAtOrAdjacent(selectedWorldTile))
            {
                reasons.Add("FactionBaseAdjacent".Translate());
                flag = false;
            }

            return flag;
        }

        public void ApplyAction()
        {
            if (!CanPlaceHere(out List<string> reasons, playerEmpire))
            {
                Messages.Message(new Message((reasons.Count == 1) ? reasons[0] : "Empire_SPW_MultipleErrors".TranslateSimple(), MessageTypeDefOf.RejectInput));

                return;
            }

            playerEmpire.StorageTracker.TryRemoveThingsFromStorage(Empire.SettlementCost);
            playerEmpire.BuildNewSettlementOnTile(Find.WorldGrid[selectedWorldTile]);

            Close();
        }
    }
}
