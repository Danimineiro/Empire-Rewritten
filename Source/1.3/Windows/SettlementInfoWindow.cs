using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Empire_Rewritten.Utils;
using UnityEngine;
using Verse;
using Empire_Rewritten.Settlements;
using RimWorld;
using Verse.Sound;
using Empire_Rewritten.Windows.Snippets;
using Empire_Rewritten.Facilities;
using Empire_Rewritten.Events.Processes;
using Empire_Rewritten.Windows.Textures;
using Empire_Rewritten.Resources;
using Empire_Rewritten.AI;

namespace Empire_Rewritten.Windows
{
    public class SettlementInfoWindow : Window
    {
        private readonly Rect rectFull = new Rect(0f, 0f, 900f, 620f);
        private readonly Rect rectMain;
        private readonly Rect rectTop;
        private readonly Rect rectTopContent;
        private readonly Rect rectMid;
        private readonly Rect rectBottom;
        private readonly Rect rectBottomContent;

        private readonly Rect rectMidTopLeft;
        private readonly Rect rectMidTopRight;
        private readonly Rect rectMidTopRightInner;
        private readonly Rect rectMidBottomLeft;
        private readonly Rect rectMidBottomLeftLeft;
        private readonly Rect rectMidBottomLeftRight;
        private readonly Rect rectMidBottomRight;
        private readonly Rect rectMidBottomRightOuter;

        private readonly Rect rectFlag;
        private readonly Rect rectDesignation;
        private readonly Rect rectExtraInfo;

        private readonly Rect rectFacilitiesLabel;
        private readonly Rect rectFacilitiesRect;
        private readonly Rect rectFacilityRect = new Rect(0f, 0f, 64f, 64f);

        private readonly Rect rectProduceSelectionMain;
        private readonly Rect rectProduceSelectionScrollOuter;

        private readonly Vector2 buildingsRectSize = new Vector2(419f, 143f);
        private readonly Settlement settlement;
        private readonly FacilityManager facilityManager;

        private readonly Color transparentGrey = ColorLibrary.Grey - new Color(0f, 0f, 0f, 0.6f);
        private readonly Color otherGrey = new Color(.15f, .15f, .15f, 1f);

        private readonly float resourceHeight;

        private const float CommonMargin = 5f;
        private const float UpperLowerHeight = 40f;
        private const float MidTopPartHeight = 145f;
        private const float LeftPartWidth = 600f;
        private const float ActionButtonHeight = 35f;

        private const float FlagSize = 64f;
        private const float DesignationWidth = 190f;
        private const float ApplyButtonWidth = 150f;
        private const float ProcessRectHeight = 56f;
        private const int ProcessRectOutlineWidth = 3;
        private Rect rectMidBottomRightInner;
        private Rect rectProduceSelectionScrollInner;

        private Vector2 processDisplayRectSize;
        private Vector2 processScroll;
        private Vector2 resourceScroll;
        private Vector2 produceScroll;

        private string tempName;
        private bool nameClicked = false;

        private static readonly List<SettlementAction> actions = new List<SettlementAction>
        {
            new SettlementAction()
            {
                Label = "Empire_SIW_AbandonSettlement".Translate(),
                Action = () =>
                {   
                    SettlementInfoWindow settlementInfoWindow = Find.WindowStack.WindowOfType<SettlementInfoWindow>();
                    Settlement settlement = settlementInfoWindow.settlement;

                    settlement.Destroy();
                    Empire.PlayerEmpire.RemoveSettlement(settlement);
                    settlementInfoWindow.Close();
                }
            },

            new SettlementAction()
            {
                Label = "Empire_SIW_Upgrade".Translate(),
                Action = () =>
                {
                    FacilityManager facilityManager = Find.WindowStack.WindowOfType<SettlementInfoWindow>().facilityManager;

                    if (facilityManager.Processes.Count(p => p is UpgradeProcess) + facilityManager.MaxFacilities + 1 > 12)
                    {
                        Messages.Message("Empire_SIW_CanNotUpgrade_MaxStage".Translate(), MessageTypeDefOf.RejectInput);
                        return;
                    }

                    UpgradeProcess process = new UpgradeProcess(30000, facilityManager);
                    facilityManager.Processes.Add(process);
                    facilityManager.NotifyProcessesChanged();
                }
            },

            new SettlementAction()
            {
                Label = "Test Add another option",
                Action = () =>
                {
                    Actions.Add(new SettlementAction()
                    {
                        Label = "Another option",
                        Action = () => { }
                    });
                }
            }
        };

        protected override float Margin => 0f;

        public override Vector2 InitialSize => rectFull.size;

        public static List<SettlementAction> Actions => actions;

        public SettlementInfoWindow(Settlement settlement)
        {
            onlyOneOfTypeAllowed = true;
            preventCameraMotion = false;
            closeOnAccept = false;
            draggable = true;

            this.settlement = settlement;
            facilityManager = Empire.PlayerEmpire.GetFacilityManager(settlement);

            rectMain = rectFull.ContractedBy(25f);
            rectTop = rectMain.TopPartPixels(UpperLowerHeight);
            rectTopContent = rectTop.TopPartPixels(UpperLowerHeight - CommonMargin);
            rectBottom = rectMain.BottomPartPixels(UpperLowerHeight);
            rectBottomContent = rectBottom.BottomPartPixels(UpperLowerHeight - CommonMargin);
            rectMid = new Rect(rectMain.x, rectMain.y + UpperLowerHeight + CommonMargin, rectMain.width, rectMain.height - (UpperLowerHeight + CommonMargin) * 2);

            rectMidTopLeft = new Rect(rectMid.x, rectMid.y, LeftPartWidth, MidTopPartHeight);
            rectMidTopRight = new Rect(rectMid.x + LeftPartWidth + CommonMargin, rectMid.y, rectMid.width - LeftPartWidth - CommonMargin, MidTopPartHeight);

            resourceHeight = Mathf.Round(rectMidTopRight.height / 4f);
            rectMidTopRightInner = rectMidTopRight.GetInnerScrollRect(resourceHeight * DefDatabase<ResourceDef>.DefCount);

            rectMidBottomLeft = new Rect(rectMid.x, rectMid.y + MidTopPartHeight + CommonMargin, LeftPartWidth, rectMid.height - MidTopPartHeight - CommonMargin * 2f);
            rectMidBottomLeftLeft = rectMidBottomLeft.LeftPartPixels(buildingsRectSize.x);
            rectMidBottomLeftRight = rectMidBottomLeft.RightPartPixels(rectMidBottomLeft.width - buildingsRectSize.x).ContractedBy(CommonMargin);

            rectMidBottomRight = new Rect(rectMid.x + LeftPartWidth + CommonMargin, rectMid.y + MidTopPartHeight + CommonMargin, rectMid.width - LeftPartWidth - CommonMargin, rectMid.height - MidTopPartHeight - CommonMargin * 2f);
            rectMidBottomRightOuter = rectMidBottomRight.ContractedBy(CommonMargin);

            rectFlag = new Rect(rectMidTopLeft.x + CommonMargin, rectMidTopLeft.y + CommonMargin, FlagSize, FlagSize);
            rectDesignation = new Rect(rectMidTopLeft.x + rectFlag.width + CommonMargin * 2f, rectMidTopLeft.y + FlagSize * 0.5f, DesignationWidth, FlagSize * 0.5f);
            rectExtraInfo = new Rect( - CommonMargin, 0f, DesignationWidth * 1.5f, FlagSize * 0.5f).MoveRect(new Vector2(rectMidTopLeft.xMax - DesignationWidth * 1.5f, rectMidTopLeft.y + FlagSize * 0.5f));

            rectFacilitiesLabel = new Rect(new Vector2(rectMidBottomLeftLeft.x, rectMidBottomLeftLeft.yMax - buildingsRectSize.y - 24f), buildingsRectSize).Rounded();
            rectFacilitiesRect = new Rect(new Vector2(rectMidBottomLeftLeft.x, rectMidBottomLeftLeft.yMax - buildingsRectSize.y), buildingsRectSize).Rounded();

            rectProduceSelectionMain = new Rect(rectMidBottomLeftLeft.x, rectMidBottomLeftLeft.y, buildingsRectSize.x, buildingsRectSize.y).Rounded();
            rectProduceSelectionScrollOuter = rectProduceSelectionMain.ContractedBy(CommonMargin);

            RecalculateScrollWidgets();
        }

        public override void DoWindowContents(Rect inRect)
        {
            if (Widgets.CloseButtonFor(rectMain)) Close();

            DrawTopPart();

            Text.Anchor = TextAnchor.MiddleCenter;

            DrawBottomLeftParts();

            GUI.color = Color.grey;
            Widgets.DrawBox(rectMidTopLeft, 2);
            GUI.color = Color.white;

            DrawFlag();
            DrawDesignation();

            Widgets.DrawLightHighlight(rectExtraInfo);
            Widgets.DrawBox(rectExtraInfo);
            Widgets.Label(rectExtraInfo, "rectExtraInfo");
            DrawMidTopRight();

            DrawBottomRightPart();

            Text.Anchor = TextAnchor.UpperLeft;

            DrawBottomPart();
        }

        private void DrawDesignation()
        {
            GUI.color = Color.grey;
            Widgets.DrawBox(rectDesignation, 2);
            GUI.color = Color.white;
            Widgets.DrawLightHighlight(rectDesignation);

            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(rectDesignation, $"<b>{facilityManager.Designation}</b>");
            Text.Anchor = TextAnchor.UpperLeft;
        }

        private void DrawFlag()
        {
            Text.Font = GameFont.Medium;
            Text.Anchor = TextAnchor.MiddleCenter;

            Widgets.DrawHighlight(rectFlag);
            Widgets.DrawShadowAround(rectFlag);

            GUI.color = Color.grey;
            Widgets.DrawBox(rectFlag, 3);
            GUI.color = Color.white;

            Widgets.Label(rectFlag, facilityManager.MaxFacilities.ToString());

            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
        }

        /// <summary>
        ///     Resource view
        /// </summary>
        private void DrawMidTopRight()
        {
            GUI.color = Color.grey;
            Widgets.DrawBox(rectMidTopRight, 2);
            GUI.color = Color.white;
            Text.Anchor = TextAnchor.MiddleLeft;

            Widgets.BeginScrollView(rectMidTopRight, ref resourceScroll, rectMidTopRightInner);
            int count = 0;

            foreach (ResourceModifier resourceModifier in facilityManager.Modifiers)
            {
                Text.Anchor = TextAnchor.MiddleLeft;
                Rect tempLineRect = rectMidTopRightInner.TopPartPixels(resourceHeight).MoveRect(new Vector2(0f, resourceHeight * count));
                Rect iconRect = tempLineRect.LeftPartPixels(tempLineRect.height).ContractedBy(4f);
                Rect thingInfoRect = tempLineRect.RightPartPixels(tempLineRect.height).ContractedBy(8f);
                Rect labelRect = new Rect(tempLineRect.x + tempLineRect.height + 2f, tempLineRect.y, tempLineRect.width - (tempLineRect.height + 2f) - thingInfoRect.width * 2f, tempLineRect.height);

                ResourceDef resource = resourceModifier.def;

                tempLineRect.DoRectHighlight(count % 2 == 0);
                GUI.DrawTexture(iconRect, ContentFinder<Texture2D>.Get(resource.iconData.texPath));
                Widgets.Label(labelRect, resource.LabelCap);

                Text.Anchor = TextAnchor.MiddleRight;
                Widgets.Label(labelRect, resourceModifier.TotalProduced().ToStringPercent());
                if (WindowHelper.InfoCardButtonWorker(thingInfoRect))
                {
                    Find.WindowStack.Add(new ResourceInfoWindow(resource));
                }
                count++;
            }

            Widgets.EndScrollView();
            Text.Anchor = TextAnchor.UpperLeft;
        }

        internal void RecalculateScrollWidgets()
        {
            float height = -CommonMargin + (processDisplayRectSize.y + CommonMargin) * facilityManager.Processes.Count;
            rectMidBottomRightInner = rectMidBottomRightOuter.GetInnerScrollRect(height);
            processDisplayRectSize = new Vector2(rectMidBottomRightInner.width, ProcessRectHeight);

            rectProduceSelectionScrollInner = rectProduceSelectionScrollOuter.GetInnerScrollRect((facilityManager.Produce.Count + 1) * (ActionButtonHeight + CommonMargin));
        }

        private void DrawBottomRightPart()
        {
            TextAnchor prev = Text.Anchor;
            Rect rectProcess = new Rect(rectMidBottomRightOuter.position, processDisplayRectSize);

            GUI.color = Color.grey;
            Widgets.DrawBox(rectMidBottomRight, 2);
            GUI.color = transparentGrey;
            GUI.DrawTexture(rectMidBottomRightOuter, Tex.InConstruction, ScaleMode.ScaleToFit);
            GUI.color = Color.white;

            if (facilityManager.Processes.Count == 0)
            {
                Text.Anchor = TextAnchor.UpperCenter;
                Widgets.Label(rectMidBottomRightOuter, $"<color=grey><b>{"Empire_SIW_NoProcesses".Translate()}</b></color>");
                Text.Anchor = prev;
                return;
            }

            Widgets.BeginScrollView(rectMidBottomRightOuter, ref processScroll, rectMidBottomRightInner);

            for (int i = 0; i < facilityManager.Processes.Count; i++)
            {
                Rect rectProcessInner = rectProcess.ContractedBy(ProcessRectOutlineWidth);
                Rect iconRect = new Rect(rectProcessInner.x + 2f, rectProcessInner.y + 2f, 32f, 32f);
                Rect labelRect = new Rect(rectProcessInner.x + iconRect.width + CommonMargin, rectProcessInner.y, rectProcessInner.width - (iconRect.width + CommonMargin) * 2f, rectProcessInner.height);
                Rect progressBar = new Rect(iconRect.x, iconRect.yMax + 2f, rectProcessInner.width - 4f, rectProcessInner.height - (2f * 3f) - iconRect.height);

                Process curProcess = facilityManager.Processes[i];

                Widgets.DrawBoxSolidWithOutline(rectProcess, otherGrey, Color.grey, ProcessRectOutlineWidth);

                GUI.DrawTexture(iconRect, curProcess.Icon);
                Text.Anchor = TextAnchor.UpperLeft;
                Widgets.Label(labelRect, $"{curProcess.LabelCap}\n{"Empire_SIW_ProcessProgress".Translate((curProcess.Duration - curProcess.WorkCompleted).ToStringTicksToPeriod())}");
                Widgets.FillableBar(progressBar, curProcess.Progress);
                
                if (Widgets.CloseButtonFor(rectProcessInner))
                {
                    facilityManager.CancelProcess(curProcess);
                    SoundDefOf.Click.PlayOneShotOnCamera();
                    i--;
                }

                TooltipHandler.TipRegion(rectProcess, curProcess.ToolTip);
                rectProcess = rectProcess.MoveRect(new Vector2(0f, processDisplayRectSize.y + CommonMargin));
            }

            Text.Anchor = prev;
            Widgets.EndScrollView();
        }

        private void DrawBottomLeftParts()
        {
            GUI.color = Color.grey;
            Widgets.DrawBox(rectMidBottomLeft, 2);
            Widgets.DrawBox(rectMidBottomLeftLeft, 2);
            Widgets.DrawBox(rectFacilitiesRect, 2);
            Widgets.DrawBox(rectProduceSelectionMain, 2);
            GUI.color = Color.white;

            DrawActionButtons();
            DrawFacilitySlots();
            DrawProduceSection();
        }

        private void DrawProduceSection()
        {
            Dictionary<ThingDef, int> tempDic = new Dictionary<ThingDef, int>();
            Rect rectProduceTemp = new Rect(rectProduceSelectionScrollInner.x, rectProduceSelectionScrollInner.y, rectProduceSelectionScrollInner.width, ActionButtonHeight);
            bool removeThings = false;
            bool amountChanged = false;

            Widgets.BeginScrollView(rectProduceSelectionScrollOuter, ref produceScroll, rectProduceSelectionScrollInner);

            foreach (ThingDef curThing in facilityManager.Produce.Keys)
            {
                DrawButtonBG(rectProduceTemp);

                string buffer = facilityManager.Produce[curThing].ToString();
                int tempAmount = facilityManager.Produce[curThing];

                Rect rectProduceTempInner = rectProduceTemp.ContractedBy(CommonMargin);
                Rect tempRectInfoIcon = rectProduceTempInner.RightPartPixels(rectProduceTempInner.height);
                Rect tempRectThingDefIcon = rectProduceTempInner.LeftPartPixels(rectProduceTempInner.height);

                Rect tempRectThingDefLabel = new Rect(rectProduceTempInner.x + tempRectThingDefIcon.width + CommonMargin, rectProduceTempInner.y, 150f, rectProduceTempInner.height);
                Rect tempRectThingDefAmount = new Rect(tempRectThingDefLabel.xMax + CommonMargin, rectProduceTempInner.y, rectProduceTempInner.width - tempRectThingDefLabel.width - tempRectThingDefIcon.width - tempRectInfoIcon.width - CommonMargin * 2f, rectProduceTempInner.height);

                Widgets.DefIcon(tempRectThingDefIcon, curThing);
                Widgets.Label(tempRectThingDefLabel, curThing.LabelCap);
                Widgets.IntEntry(tempRectThingDefAmount, ref tempAmount, ref buffer);
                Widgets.InfoCardButton(tempRectInfoIcon, curThing);

                tempAmount = Mathf.Clamp(tempAmount, 0, 100); //TODO: Think of maximum
                removeThings |= tempAmount == 0;
                amountChanged |= facilityManager.Produce[curThing] != tempAmount;

                tempDic.SetOrAdd(curThing, tempAmount);

                rectProduceTemp = rectProduceTemp.MoveRect(new Vector2(0f, ActionButtonHeight + CommonMargin));
            }

            if (amountChanged)
            {
                facilityManager.Produce.Clear();
                facilityManager.Produce.AddRange(tempDic);
                facilityManager.RefreshCachedDesignation();
            }

            if (removeThings)
            {
                facilityManager.Produce.RemoveAll(x => x.Value == 0);
                RecalculateScrollWidgets();
            }

            if (Widgets.ButtonInvisible(rectProduceTemp))
            {
                List<FloatMenuOption> tempOptions = new List<FloatMenuOption>();
                HashSet<ResourceDef> defsProduced = facilityManager.ProducedResourceDefsReadonly;
                HashSet<ThingDef> thingsProduced = new HashSet<ThingDef>();

                foreach (ResourceDef resourceDef in defsProduced)
                {
                    thingsProduced.AddRange(resourceDef.ResourcesCreated.AllowedThingDefs);
                }

                thingsProduced.RemoveWhere(thingDef => facilityManager.Produce.ContainsKey(thingDef));

                foreach (ThingDef thingDef in thingsProduced)
                {
                    tempOptions.Add(new FloatMenuOption(thingDef.LabelCap, () => AddProduceToManager(thingDef), thingDef));
                }

                Find.WindowStack.Add(new FloatMenu(tempOptions));
            }

            DrawButtonBG(rectProduceTemp);
            Widgets.DrawHighlightIfMouseover(rectProduceTemp);
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(rectProduceTemp, "<b>Add Produce</b>");
            Text.Anchor = TextAnchor.UpperLeft;

            Widgets.EndScrollView();
        }

        private void DrawButtonBG(Rect rectProduceTemp)
        {
            Widgets.DrawBoxSolid(rectProduceTemp, otherGrey);
            GUI.color = Color.grey;
            Widgets.DrawBox(rectProduceTemp, 2);
            GUI.color = Color.white;
        }

        private bool AddProduceToManager(ThingDef thing)
        {
            bool result = facilityManager.Produce.TryAdd(thing, 1);
            RecalculateScrollWidgets();
            facilityManager.RefreshCachedDesignation();
            return result;
        }

        private void DrawFacilitySlots()
        {
            Text.Anchor = TextAnchor.UpperCenter;

            Widgets.Label(rectFacilitiesLabel, $"<b>{"Empire_SIW_FacilitiesLabel".Translate()}</b>");

            Text.Anchor = TextAnchor.MiddleCenter;
            Vector2 moveVector = new Vector2();

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    int currentSpot = i * 6 + j;
                    Rect buildingRect = new Rect(new Vector2(rectFacilitiesRect.x + CommonMargin, rectFacilitiesRect.y + CommonMargin), rectFacilityRect.size).MoveRect(moveVector);
                    moveVector += new Vector2(rectFacilityRect.width + CommonMargin, 0f);

                    Widgets.DrawBoxSolid(buildingRect, otherGrey);
                    bool flag = TryDrawBuilding(currentSpot, buildingRect);
                    flag = flag || TryBlockSpot(currentSpot, buildingRect);

                    if (!flag)
                    {
                        GUI.color = new Color(1f, 1f, 1f, 0.2f);
                        GUI.DrawTexture(buildingRect, Tex.AddFacility);
                        GUI.color = Color.white;

                        if (Widgets.ButtonInvisible(buildingRect))
                        {
                            Find.WindowStack.Add(new FloatMenu(GetFacilityOptions()));
                        }
                    }

                    GUI.color = Color.grey;
                    Widgets.DrawBox(buildingRect, 2);
                    GUI.color = Color.white;

                    Widgets.DrawHighlightIfMouseover(buildingRect);
                }

                moveVector += new Vector2(-(rectFacilityRect.width + CommonMargin) * 6f, rectFacilityRect.width + CommonMargin);
            }
        }

        private bool TryBlockSpot(int currentSpot, Rect buildingRect)
        {
            if (facilityManager.GetProcessWithSlotID(currentSpot) is Process process)
            {
                TooltipHandler.TipRegion(buildingRect, "Empire_SIW_SlotInConstruction".TranslateSimple());

                float rightSide = buildingRect.width * process.Progress;
                Rect progress = new Rect(buildingRect.x, buildingRect.y, rightSide, buildingRect.height);
                GUI.DrawTexture(buildingRect, Tex.InConstruction);

                Widgets.DrawBoxSolid(progress, ColorLibrary.BrightBlue - new Color(0f, 0f, 0f, 0.8f));
                GUI.color = ColorLibrary.BrightBlue;
                Widgets.DrawLineVertical(progress.xMax, buildingRect.y, buildingRect.height);
                GUI.color = Color.white;

                return true;
            }

            if (facilityManager.MaxFacilities <= currentSpot)
            {
                TooltipHandler.TipRegion(buildingRect, "Empire_SIW_SlotLocked".TranslateSimple());
                GUI.DrawTexture(buildingRect, Tex.LockedBuildingSlot);

                return true;
            }

            return false;
        }

        private bool TryDrawBuilding(int currentSpot, Rect buildingRect)
        {
            if (facilityManager[currentSpot]?.def is FacilityDef currentFacilityDef)
            {
                GUI.DrawTexture(buildingRect, ContentFinder<Texture2D>.Get(currentFacilityDef.iconData.texPath));
                TooltipHandler.TipRegion(buildingRect, $"<b>{currentFacilityDef.LabelCap}</b>\n\n{currentFacilityDef.description}\n\n{"Empire_SIW_ClickToDeconstructOrChangeFacility".Translate()}");

                if (Widgets.ButtonInvisible(buildingRect))
                {
                    List<FloatMenuOption> options = new List<FloatMenuOption>()
                    {
                        new FloatMenuOption("Empire_SIW_DeconstructFacility".Translate(currentFacilityDef.LabelCap), () => facilityManager.RemoveFacility(currentFacilityDef)),
                    };

                    //options.AddRange(GetFacilityOptions(currentFacilityDef)); => TODO: Replace building dialog
                    Find.WindowStack.Add(new FloatMenu(options));
                }

                return true;
            }

            return false;
        }

        private List<FloatMenuOption> GetFacilityOptions(FacilityDef skipDef = null)
        {
            List<FloatMenuOption> options = new List<FloatMenuOption>();
            foreach (FacilityDef facilityDef in DefDatabase<FacilityDef>.AllDefsListForReading)
            {
                if (skipDef?.Equals(facilityDef) == true) continue;

                options.Add(new FloatMenuOption(facilityDef.LabelCap, () =>
                {
                    facilityManager.AddFacility(facilityDef);
                }, ContentFinder<Texture2D>.Get(facilityDef.iconData.texPath), Color.white));
            }
            return options;
        }

        private void DrawActionButtons()
        {
            //Create buttons that display any special option that can be taken in this settlement
            for (int i = 0; i < Math.Min(Actions.Count, 8); i++)
            {
                Rect tempRect = new Rect(rectMidBottomLeftRight.x - 1f, rectMidBottomLeftRight.y, rectMidBottomLeftRight.width, ActionButtonHeight).MoveRect(new Vector2(0f, (ActionButtonHeight + CommonMargin) * i));

                //If there are too many options to be displayed, create a button that can display the remaining ones using a float menu
                if (i == 7 && Actions.Count > 8)
                {
                    if (Widgets.ButtonText(tempRect, "Empire_SIW_MoreActions".Translate()))
                    {
                        List<FloatMenuOption> options = new List<FloatMenuOption>();

                        for (int j = i; j < Actions.Count; j++)
                        {
                            options.Add(new FloatMenuOption(Actions[i].LabelCap, () => Actions[i].Action()));
                        }

                        Find.WindowStack.Add(new FloatMenu(options));
                    }

                    break;
                }

                if (Widgets.ButtonText(tempRect, Actions[i].Label))
                {
                    Actions[i].Action();
                    SoundDefOf.Click.PlayOneShotOnCamera();
                }
            }
        }

        private void DrawBottomPart()
        {
            Widgets.DrawLineHorizontal(rectBottom.x, rectBottom.y, rectBottom.width);
            Widgets.ButtonText(rectBottomContent, "idk space for buttons or something? may not needed");
        }

        private void DrawTopPart()
        {
            Text.Font = GameFont.Medium;

            if (!nameClicked)
            {
                Widgets.Label(rectTopContent, $"<b>{settlement.LabelCap}</b>");
                if (Widgets.ButtonInvisible(rectTopContent, false))
                {
                    nameClicked = true;
                    tempName = settlement.LabelCap;
                    SoundDefOf.TabOpen.PlayOneShotOnCamera();
                }
            }
            else
            {
                tempName = Widgets.TextField(rectTopContent.LeftPartPixels(rectTopContent.width - ApplyButtonWidth - CommonMargin), tempName, 100, CharacterCardUtility.ValidNameRegex);

                Text.Font = GameFont.Small;
                if (Widgets.ButtonText(rectTopContent.RightPartPixels(ApplyButtonWidth), "Empire_SIW_Apply".Translate()))
                {
                    nameClicked = false;
                    settlement.Name = tempName;
                    SoundDefOf.Click.PlayOneShotOnCamera();
                }
            }

            Text.Font = GameFont.Small;

            Widgets.DrawLineHorizontal(rectTop.x, rectTop.yMax, rectTop.width);
        }

        /// <summary>
        ///     The gizmo you see when you click on the settlement of a player empire
        /// </summary>
        public static Gizmo OpenOverviewGizmo(Settlement settlement)
        {
            Command_Action command_Action = new Command_Action()
            {
                defaultLabel = "Empire_SIW_OpenOverviewLabel".Translate(),
                defaultDesc = "Empire_SIW_OpenOverviewDesc".Translate(),
                icon = Tex.QuestionMark,
                action = () => Find.WindowStack.Add(new SettlementInfoWindow(settlement))
            };
            
            return command_Action;
        }
    }
}
