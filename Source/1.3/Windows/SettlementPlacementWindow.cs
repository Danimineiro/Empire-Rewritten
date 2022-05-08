using Empire_Rewritten.Controllers;
using Empire_Rewritten.Resources;
using Empire_Rewritten.Settlements;
using Empire_Rewritten.Territories;
using Empire_Rewritten.Utils;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            rectMid = new Rect(rectMain.x, rectMain.y + 30f, rectMain.width, rectMain.height - 30f * 2f);
            rectButtonApply = new Rect(rectBot.x, rectBot.y + 5f, rectBot.width, rectBot.height - 10f);

            rectCostLabel = rectMid.TopPartPixels(25f);
            rectCostOuter = rectMid.TopPartPixels(29f * 4 /**Default ResourceDef.Count**/).MoveRect(new Vector2(0f, rectCostLabel.height));

            rectCostInner = rectCostOuter.GetInnerScrollRect(29f * ResourceDef.AllResourceDefs.Count());

            CameraJumper.TryShowWorld();
        }

        public override void DoWindowContents(Rect inRect)
        {
            GetTileData();

            DrawTop();
            DrawMiddle();
            DrawBottom();
        }

        private void DrawMiddle()
        {
            Rect rectFull = rectCostInner.TopPartPixels(29f);
            Widgets.Label(rectCostLabel, "Empire_SPW_Modifiers".Translate());
            Widgets.DrawBox(rectCostOuter);
            Widgets.BeginScrollView(rectCostOuter, ref costScroll, rectCostInner);

            int count = 0;
            foreach(KeyValuePair<ResourceDef, ResourceModifier> kvp in tileMods)
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
            Widgets.DrawLineHorizontal(rectBot.x, rectBot.y, rectBot.width);
            rectButtonApply.DrawButtonText("Empire_SPW_Apply".Translate(), ApplyAction);
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

        public void ApplyAction()
        {
            //TODO: Check things inputs for validity here (name can't be empty)
            Territory playerTerritory = TerritoryManager.GetTerritoryManager.GetTerritory(Faction.OfPlayer);

            if (playerTerritory.Tiles.Contains(selectedWorldTile))
            {

                Empire playerEmpire = UpdateController.CurrentWorldInstance.FactionController.ReadOnlyFactionSettlementData.Find(x => !x.SettlementManager.IsAIPlayer).SettlementManager;
                if (playerEmpire.StorageTracker.CanRemoveThingsFromStorage(Empire.SettlementCost))
                {
                    playerEmpire.StorageTracker.TryRemoveThingsFromStorage(Empire.SettlementCost);
                    playerEmpire.BuildNewSettlementOnTile(Find.WorldGrid[selectedWorldTile]);
                }
            }
            Close();
        }
    }
}
