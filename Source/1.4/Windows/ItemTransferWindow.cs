using Empire_Rewritten.Controllers;
using Empire_Rewritten.Settlements;
using Empire_Rewritten.Utils;
using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Empire_Rewritten.Windows
{
    public class ItemTransferWindow : Window
    {
        private static readonly Texture2D TradeArrow = ContentFinder<Texture2D>.Get("UI/Widgets/TradeArrow", true);

        private readonly Empire playerEmpire;

        private readonly Rect rectFull = new Rect(0f, 0f, 900f, 600f);
        private readonly Rect rectMain;

        private readonly Rect rectTop;
        private readonly Rect rectMid;
        private readonly Rect rectBot;

        private readonly Rect rectButtonApply;

        private readonly Rect rectStorageManagerListFull;
        private readonly Rect rectStorageManagerListTop;
        private readonly Rect rectStorageManagerListOuter;
        private readonly Rect rectStorageManagerListInner;
        private readonly Rect rectStorageItem;

        private readonly Rect rectItemTransferFull;
        private readonly Rect rectItemTransferTop;
        private readonly Rect rectItemTransferOuter;
        private readonly Rect rectItemTransferInner;
        private readonly Rect rectItem;

        private readonly Rect rectItemTransferTopBottom;
        private readonly Rect rectLabelDesc;
        private readonly Rect rectAmountOnMapDesc;
        private readonly Rect rectRest;
        private readonly Rect rectRestLeft;
        private readonly Rect rectRestRight;

        private Vector2 storageManagerScroll;
        private Vector2 itemTransferScroll;

        private readonly Dictionary<ThingDef, int> playerItems = new Dictionary<ThingDef, int>();
        private readonly Dictionary<ThingDef, int> combinedItems = new Dictionary<ThingDef, int>();
        private readonly Dictionary<ThingDef, int> transferAmounts = new Dictionary<ThingDef, int>();
        private readonly Dictionary<ThingDef, string> transferBuffer = new Dictionary<ThingDef, string>();
        private readonly Map playerMap;

        public ItemTransferWindow()
        {
            doCloseX = true;
            onlyOneOfTypeAllowed = true;
            preventCameraMotion = false;
            forcePause = true;

            if (Find.CurrentMap.IsPlayerHome)
            {
                playerMap = Find.CurrentMap;
            }
            else
            {
                playerMap = Find.AnyPlayerHomeMap;
            }

            playerEmpire = UpdateController.CurrentWorldInstance.FactionController.ReadOnlyFactionSettlementData.Find(x => !x.SettlementManager.IsAIPlayer).SettlementManager;

            GetMapItems();
            CombineItemDics();

            rectMain = rectFull.ContractedBy(25f);
            rectTop = rectMain.TopPartPixels(30f);
            rectBot = rectMain.BottomPartPixels(30f);
            rectButtonApply = new Rect(rectBot.x, rectBot.y + 5f, rectBot.width, rectBot.height - 10f);
            rectMid = new Rect(rectMain.x, rectMain.y + 30f, rectMain.width, rectMain.height - 30f * 2f);

            rectStorageManagerListFull = rectMid.LeftPartPixels(240);
            rectStorageManagerListTop = rectStorageManagerListFull.TopPartPixels(33f);
            rectStorageManagerListOuter = rectStorageManagerListFull.BottomPartPixels(rectStorageManagerListFull.height - rectStorageManagerListTop.height - 5f).MoveRect(new Vector2(0f, -5f));
            rectStorageManagerListInner = rectStorageManagerListOuter.GetInnerScrollRect(playerEmpire.StorageTracker.StoredThings.Count);
            rectStorageItem = rectStorageManagerListInner.TopPartPixels(29f);

            rectItemTransferFull = rectMid.RightPartPixels(rectMid.width - rectStorageManagerListOuter.width - 5f);
            rectItemTransferTop = rectItemTransferFull.TopPartPixels(33f);
            rectItemTransferOuter = rectItemTransferFull.BottomPartPixels(rectItemTransferFull.height - rectItemTransferTop.height - 5f).MoveRect(new Vector2(0f, -5f));
            rectItemTransferInner = rectItemTransferOuter.GetInnerScrollRect(combinedItems.Count * 29f);
            rectItem = rectItemTransferInner.TopPartPixels(29f);

            rectItemTransferTopBottom = rectItemTransferTop.BottomPartPixels(28f);
            rectLabelDesc = rectItemTransferTopBottom.LeftPartPixels(230f);
            rectAmountOnMapDesc = rectItemTransferTopBottom.RightPartPixels(rectItemTransferTopBottom.width - rectLabelDesc.width - 5f).LeftPartPixels(60f);
            rectRest = rectItemTransferTopBottom.RightPartPixels(rectItemTransferTopBottom.width - rectLabelDesc.width - 5f - rectAmountOnMapDesc.width - 5f);
            
            Rect tempRight = rectRest.RightPartPixels(100f);
            rectRestLeft = rectRest.LeftPartPixels(rectRest.width - tempRight.width);
            rectRestRight = new Rect(tempRight.x + 5f, tempRight.y, tempRight.width - 5f, tempRight.height);
        }

        protected override float Margin => 0f;

        public override Vector2 InitialSize => rectFull.size;

        private void GetMapItems()
        {
            List<Thing> mapItems = playerMap.listerThings.ThingsMatching(new ThingRequest() { group = ThingRequestGroup.HaulableEver });

            foreach (Thing item in mapItems)
            {
                if (!playerItems.TryAdd(item.def, item.stackCount))
                {
                    playerItems[item.def] += item.stackCount;
                }
            }
        }

        private void CombineItemDics()
        {
            foreach (KeyValuePair<ThingDef, int> kvp in playerItems)
            {
                if (!combinedItems.TryAdd(kvp.Key, kvp.Value))
                {
                    combinedItems[kvp.Key] += kvp.Value;
                }
            }

            foreach (KeyValuePair<ThingDef, int> kvp in playerEmpire.StorageTracker.StoredThings)
            {
                if (!combinedItems.TryAdd(kvp.Key, kvp.Value))
                {
                    combinedItems[kvp.Key] += kvp.Value;
                }
            }

            foreach (KeyValuePair<ThingDef, int> kvp in combinedItems)
            {
                transferAmounts.Add(kvp.Key, 0);
                transferBuffer.Add(kvp.Key, "0");
            }
        }

        public override void DoWindowContents(Rect inRect)
        {
            DrawTop();
            DrawBottom();
            DisplayStoredItems();
            DrawTransferTopPart();

            Widgets.BeginScrollView(rectItemTransferOuter, ref itemTransferScroll, rectItemTransferInner);

            //DO THIS
            int count = 0;
            foreach (KeyValuePair<ThingDef, int> kvp in combinedItems)
            {
                Rect itemRect = rectItem.MoveRect(new Vector2(0f, rectItem.height * count));
                Rect itemRectIcon = itemRect.LeftPartPixels(itemRect.height).ContractedBy(4f);
                Rect itemRectLabel = itemRect.MoveRect(new Vector2(itemRectIcon.width + 5f, 0f));
                itemRectLabel.xMax = rectLabelDesc.xMax;

                Rect itemAmountOnMap = itemRect.AlignXWith(rectAmountOnMapDesc);
                Rect itemRectTransferAmount = itemRect.AlignXWith(rectRestLeft);
                Rect itemFullyReduceAmountButton = itemRectTransferAmount.LeftPartPixels(itemRectTransferAmount.height);
                Rect itemFullyIncreaseAmountButton = itemRectTransferAmount.RightPartPixels(itemRectTransferAmount.height);
                Rect itemReduceAmountButton = itemRectTransferAmount.LeftPartPixels(itemRectTransferAmount.height).MoveRect(new Vector2(itemRectTransferAmount.height, 0));
                Rect itemIncreaseAmountButton = itemRectTransferAmount.RightPartPixels(itemRectTransferAmount.height).MoveRect(new Vector2(-itemRectTransferAmount.height, 0));
                Rect itemNumericFieldFull = new Rect(itemRectTransferAmount.x + itemRectTransferAmount.height * 2f, itemRectTransferAmount.y, itemRectTransferAmount.width - itemRectTransferAmount.height * 4f, itemRectTransferAmount.height);
                Rect itemNumericFieldTexture = new Rect(0f, 0f, TradeArrow.width, TradeArrow.height).CenteredOnXIn(itemNumericFieldFull).CenteredOnYIn(itemNumericFieldFull);
                Rect itemNumericFieldInput = new Rect(0f, 0f, 55f, 21f).CenteredOnXIn(itemNumericFieldFull).CenteredOnYIn(itemNumericFieldFull);
                Rect itemRectStorageAmount = itemRect.AlignXWith(rectRestRight);
                Rect itemInfoRect = itemRect.RightPartPixels(itemRect.height).ContractedBy(4f);
                itemRectStorageAmount.xMax = itemInfoRect.x - 5f;

                int modifier = GenUI.CurrentAdjustmentMultiplier();
                int itemTransferAmount = transferAmounts[kvp.Key];
                int amountOnMap = playerItems.ContainsKey(kvp.Key) ? playerItems[kvp.Key] : 0;
                int amountInStorage = playerEmpire.StorageTracker.StoredThings.ContainsKey(kvp.Key) ? playerEmpire.StorageTracker.StoredThings[kvp.Key] : 0;
                string itemTransferAmountBuffer = transferBuffer[kvp.Key];

                itemRect.DoRectHighlight(count % 2 == 1);

                Text.Anchor = TextAnchor.MiddleLeft;
                MouseoverSounds.DoRegion(itemRectIcon);
                Widgets.ThingIcon(itemRectIcon, kvp.Key);

                Widgets.DrawHighlightIfMouseover(itemRectLabel);
                MouseoverSounds.DoRegion(itemRectLabel);
                Widgets.Label(itemRectLabel.MoveRect(new Vector2(5f, 0f)), kvp.Key.LabelCap);
                TooltipHandler.TipRegion(itemRectLabel, kvp.Key.description);

                Text.Anchor = TextAnchor.MiddleRight;
                MouseoverSounds.DoRegion(itemAmountOnMap);
                Widgets.DrawHighlightIfMouseover(itemAmountOnMap);
                Widgets.Label(itemAmountOnMap, amountOnMap.ToString());
                TooltipHandler.TipRegion(itemAmountOnMap, "Empire_ITW_AmountOnMap".Translate());

                Text.Anchor = TextAnchor.MiddleRight;
                //MouseoverSounds.DoRegion(itemRectTransferAmount);
                //Widgets.DrawHighlightIfMouseover(itemRectTransferAmount);
                MouseoverSounds.DoRegion(itemNumericFieldInput);
                Widgets.TextFieldNumeric(itemNumericFieldInput, ref itemTransferAmount, ref itemTransferAmountBuffer, -amountInStorage, amountOnMap);

                bool amountIs0OrLess = itemTransferAmount <= 0;
                bool amountIs0OrMore = itemTransferAmount >= 0;
                if (amountOnMap + amountInStorage > 1)
                {
                    itemFullyReduceAmountButton.DrawButtonText(amountIs0OrLess ? "<<" : "0", () =>
                    {
                        itemTransferAmount = amountIs0OrLess ? -amountInStorage : 0;
                        SoundDefOf.Tick_High.PlayOneShotOnCamera();
                    }, itemTransferAmount == -amountInStorage);

                    itemFullyIncreaseAmountButton.DrawButtonText(amountIs0OrMore ? ">>" : "0", () =>
                    {
                        itemTransferAmount = amountIs0OrMore ? amountOnMap : 0;
                        SoundDefOf.Tick_Low.PlayOneShotOnCamera();
                    }, itemTransferAmount == amountOnMap);
                }
                else
                {
                    modifier = 1;
                    itemReduceAmountButton.x -= itemRectTransferAmount.height;
                    itemReduceAmountButton.width += itemRectTransferAmount.height;
                    itemIncreaseAmountButton.width += itemRectTransferAmount.height;
                }

                itemReduceAmountButton.DrawButtonText("<", () =>
                {
                    itemTransferAmount -= 1 * modifier;
                    SoundDefOf.Tick_High.PlayOneShotOnCamera();
                }, itemTransferAmount == -amountInStorage);

                itemIncreaseAmountButton.DrawButtonText(">", () =>
                {
                    Log.Message($"width: {TradeArrow.width}, height: {TradeArrow.height})");
                    itemTransferAmount += 1 * modifier;
                    SoundDefOf.Tick_Low.PlayOneShotOnCamera();
                }, itemTransferAmount == amountOnMap);

                if (itemTransferAmount != 0)
                {
                    if (amountIs0OrMore)
                    {
                        GUI.DrawTexture(itemNumericFieldTexture, TradeArrow);
                    }
                    else if (amountIs0OrLess)
                    {
                        GUI.DrawTexture(itemNumericFieldTexture.FlipHorizontal(), TradeArrow);
                    }
                }

                //TooltipHandler.TipRegion(itemRectTransferAmount, "Empire_ITW_AmountOfItemsTransferred".Translate());

                Text.Anchor = TextAnchor.MiddleRight;
                MouseoverSounds.DoRegion(itemRectStorageAmount);
                Widgets.DrawHighlightIfMouseover(itemRectStorageAmount);
                Widgets.Label(itemRectStorageAmount, amountInStorage.ToString());
                TooltipHandler.TipRegion(itemRectStorageAmount, "Empire_ITW_AmountStoredInStorage".Translate());

                Text.Anchor = TextAnchor.UpperLeft;
                Widgets.InfoCardButton(itemInfoRect, kvp.Key);

                //int moveAmount = Mathf.Clamp(itemTransferAmount, -amountInStorage, amountOnMap);

                transferAmounts[kvp.Key] = itemTransferAmount;
                transferBuffer[kvp.Key] = itemTransferAmount.ToString();
                count++;
            }

            Widgets.EndScrollView(); 
            Widgets.DrawBox(rectItemTransferOuter);
        }

        private void DrawTransferTopPart()
        {
            Widgets.DrawLightHighlight(rectItemTransferTopBottom);
            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(rectLabelDesc.MoveRect(new Vector2(5f, 0f)), "Empire_ITW_SelectItemsToTransfer".Translate());
            Text.Anchor = TextAnchor.UpperLeft;
        }

        private void DisplayStoredItems()
        {
            Widgets.DrawLightHighlight(rectStorageManagerListTop.BottomPartPixels(28f));
            Widgets.DrawBox(rectStorageManagerListOuter);

            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(rectStorageManagerListTop.MoveRect(new Vector2(5f, 0f)).BottomPartPixels(28f), "Empire_ITW_StoredItems".Translate());
            Widgets.BeginScrollView(rectStorageManagerListOuter, ref storageManagerScroll, rectStorageManagerListInner);

            int count = 0;
            foreach (KeyValuePair<ThingDef, int> kvp in playerEmpire.StorageTracker.StoredThings)
            {
                Rect itemRect = rectStorageItem.MoveRect(new Vector2(0f, rectStorageItem.height * count));
                Rect itemRectIcon = itemRect.LeftPartPixels(itemRect.height).ContractedBy(4f);
                Rect itemRectLabel = itemRect.MoveRect(new Vector2(itemRectIcon.width + 5f, 0f));

                itemRect.DoRectHighlight(count % 2 == 1);
                Widgets.ThingIcon(itemRectIcon, kvp.Key);
                Widgets.Label(itemRectLabel, $"{kvp.Key.LabelCap} {kvp.Value} ({(Rand.Value > 0.5 ? "+" : "-")} {Rand.Range(0, 100)})");
                Widgets.InfoCardButton(itemRect.RightPartPixels(itemRect.height).ContractedBy(4f), kvp.Key);

                count++;
            }

            Text.Anchor = TextAnchor.UpperLeft;
            Widgets.EndScrollView();
        }

        private void DrawTop()
        {
            Text.Font = GameFont.Medium;
            Widgets.Label(rectTop, "Empire_ITW_Title".Translate());
            Text.Font = GameFont.Small;

            Widgets.DrawLineHorizontal(rectTop.x, rectTop.yMax, rectTop.width);
        }

        private void DrawBottom()
        {
            Widgets.DrawLineHorizontal(rectBot.x, rectBot.y, rectBot.width);
            rectButtonApply.DrawButtonText("Empire_ITW_Apply".Translate(), ApplyAction);
            GUI.color = Color.white;
        }

        private void ApplyAction()
        {
            throw new NotImplementedException();
        }
    }
}
