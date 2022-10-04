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

        private readonly Vector2 buildingsRectSize = new Vector2(419f, 143f);
        private readonly Settlement settlement;
        private readonly FacilityManager facilityManager;

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
        private Vector2 processDisplayRectSize;
        private Vector2 ScrollVec;
        private string tempName;
        private bool nameClicked = false;

        private static readonly List<SettlementAction> actions = new List<SettlementAction>
        {
            new SettlementAction()
            {
                Label = "Empire_SIW_AbandonSettlement".Translate(),
                Action = (settlement) =>
                {
                    settlement.Destroy();
                    Empire.PlayerEmpire.RemoveSettlement(settlement);
                    Find.WindowStack.WindowOfType<SettlementInfoWindow>().Close();
                }
            },

            new SettlementAction()
            {
                Label = "Empire_SIW_Upgrade".Translate(),
                Action = (settlement) =>
                {
                    settlement.GetFacilityManager().ModifyStageBy(1);
                }
            },

            new SettlementAction()
            {
                Label = "Test Add another option",
                Action = (settlement) =>
                {
                    Actions.Add(new SettlementAction()
                    {
                        Label = "Another option",
                        Action = (_) => { }
                    });
                }
            }
        };

        public SettlementInfoWindow(Settlement settlement)
        {
            doCloseX = true;
            onlyOneOfTypeAllowed = true;
            preventCameraMotion = false;
            //forcePause = true;

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

            rectMidBottomLeft = new Rect(rectMid.x, rectMid.y + MidTopPartHeight + CommonMargin, LeftPartWidth, rectMid.height - MidTopPartHeight - CommonMargin * 2f);
            rectMidBottomLeftLeft = rectMidBottomLeft.LeftPartPixels(buildingsRectSize.x);
            rectMidBottomLeftRight = rectMidBottomLeft.RightPartPixels(rectMidBottomLeft.width - buildingsRectSize.x).ContractedBy(CommonMargin);

            rectMidBottomRight = new Rect(rectMid.x + LeftPartWidth + CommonMargin, rectMid.y + MidTopPartHeight + CommonMargin, rectMid.width - LeftPartWidth - CommonMargin, rectMid.height - MidTopPartHeight - CommonMargin * 2f);
            rectMidBottomRightOuter = rectMidBottomRight.ContractedBy(CommonMargin);

            rectFlag = new Rect(rectMidTopLeft.x + CommonMargin, rectMidTopLeft.y + CommonMargin, FlagSize, FlagSize);
            rectDesignation = new Rect(rectMidTopLeft.x + rectFlag.width + CommonMargin * 2f, rectMidTopLeft.y + FlagSize * 0.5f, DesignationWidth, FlagSize * 0.5f);
            rectExtraInfo = new Rect( - CommonMargin, 0f, DesignationWidth * 1.5f, FlagSize * 0.5f).MoveRect(new Vector2(rectMidTopLeft.xMax - DesignationWidth * 1.5f, rectMidTopLeft.y + FlagSize * 0.5f));

            rectFacilitiesLabel = new Rect(rectMidBottomLeftLeft.x + CommonMargin, rectMidBottomLeftLeft.y + CommonMargin, rectMidBottomLeftLeft.width - CommonMargin * 2f, FlagSize * 0.5f);
            rectFacilitiesRect = new Rect(new Vector2(), buildingsRectSize)
            {
                center = rectMidBottomLeftLeft.center + new Vector2(0f, rectFacilitiesLabel.height + CommonMargin)
            }.Rounded();

            RecalculateScrollWidgets();
        }

        protected override float Margin => 0f;

        public override Vector2 InitialSize => rectFull.size;

        public static List<SettlementAction> Actions => actions;

        public override void DoWindowContents(Rect inRect)
        {
            WindowHelper.DrawLightCorneredHighlight(rectMidBottomLeftRight);

            DrawTopPart();

            Text.Anchor = TextAnchor.MiddleCenter;

            DrawBottomLeftParts();

            Widgets.DrawBox(rectMidTopLeft);
            Widgets.Label(rectMidTopLeft, "rectMidTopLeft");

            Widgets.DrawBox(rectFlag);
            Widgets.Label(rectFlag, "rectFlag");

            Widgets.DrawLightHighlight(rectDesignation);
            Widgets.DrawBox(rectDesignation);
            Widgets.Label(rectDesignation, "rectDesignation");

            Widgets.DrawLightHighlight(rectExtraInfo);
            Widgets.DrawBox(rectExtraInfo);
            Widgets.Label(rectExtraInfo, "rectExtraInfo");

            Widgets.DrawBox(rectMidTopRight);
            Widgets.Label(rectMidTopRight, "rectMidTopRight");
            
            DrawBottomRightPart();

            Text.Anchor = TextAnchor.UpperLeft;

            DrawBottomPart();
        }

        internal void RecalculateScrollWidgets()
        {
            float height = -CommonMargin + (processDisplayRectSize.y + CommonMargin) * facilityManager.Processes.Count;
            rectMidBottomRightInner = rectMidBottomRightOuter.GetInnerScrollRect(height);
            processDisplayRectSize = new Vector2(rectMidBottomRightInner.width, ProcessRectHeight);
        }

        private void DrawBottomRightPart()
        {
            TextAnchor prev = Text.Anchor;
            Widgets.DrawBox(rectMidBottomRight);
            Rect rectProcess = new Rect(rectMidBottomRightOuter.position, processDisplayRectSize);
            
            GUI.color = ColorLibrary.Grey - new Color(0f, 0f, 0f, 0.6f);
            GUI.DrawTexture(rectMidBottomRightOuter, ContentFinder<Texture2D>.Get("UI/Filler/ConstructingBig"), ScaleMode.ScaleToFit);
            GUI.color = Color.white;

            if (facilityManager.Processes.Count == 0)
            {
                Text.Anchor = TextAnchor.UpperCenter;
                Widgets.Label(rectMidBottomRightOuter, $"<color=grey><b>{"Empire_SIW_NoProcesses".Translate()}</b></color>");
                Text.Anchor = prev;
                return;
            }

            Widgets.BeginScrollView(rectMidBottomRightOuter, ref ScrollVec, rectMidBottomRightInner);

            for (int i = 0; i < facilityManager.Processes.Count; i++)
            {
                Rect rectProcessInner = rectProcess.ContractedBy(ProcessRectOutlineWidth);
                Rect iconRect = new Rect(rectProcessInner.x + 2f, rectProcessInner.y + 2f, 32f, 32f);
                Rect labelRect = new Rect(rectProcessInner.x + iconRect.width + CommonMargin, rectProcessInner.y, rectProcessInner.width - (iconRect.width + CommonMargin) * 2f, rectProcessInner.height);
                Rect progressBar = new Rect(iconRect.x, iconRect.yMax + 2f, rectProcessInner.width - 4f, rectProcessInner.height - (2f * 3f) - iconRect.height);

                Process curProcess = facilityManager.Processes[i];

                Widgets.DrawBoxSolidWithOutline(rectProcess, new Color(.15f, .15f, .15f, 1f), ColorLibrary.Grey, ProcessRectOutlineWidth);

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
            Widgets.DrawBox(rectMidBottomLeft);
            //Widgets.Label(rectMidBottomLeft, "rectMidBottomLeft");

            Widgets.DrawBox(rectMidBottomLeftLeft);
            //Widgets.Label(rectMidBottomLeftLeft, "rectMidBottomLeftLeft");
           
            DrawActionButtons();

            //Widgets.DrawBox(rectBuildingsLabel);
            Widgets.Label(rectFacilitiesLabel, $"<b>{"Empire_SIW_FacilitiesLabel".Translate()}</b>");

            Widgets.DrawBox(rectFacilitiesRect);
            Vector2 moveVector = new Vector2();

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    int currentSpot = i * 6 + j;
                    Rect buildingRect = new Rect(new Vector2(rectFacilitiesRect.x + CommonMargin, rectFacilitiesRect.y + CommonMargin), rectFacilityRect.size).MoveRect(moveVector);
                    moveVector += new Vector2(rectFacilityRect.width + CommonMargin, 0f);


                    bool flag = TryDrawBuilding(currentSpot, buildingRect);
                    flag = flag || TryBlockSpot(currentSpot, buildingRect);

                    if (!flag)
                    {
                        GUI.color = new Color(1f, 1f, 1f, 0.2f);
                        GUI.DrawTexture(buildingRect, ContentFinder<Texture2D>.Get("UI/Facilities/AddFacility"));
                        GUI.color = Color.white;

                        if (Widgets.ButtonInvisible(buildingRect))
                        {
                            Find.WindowStack.Add(new FloatMenu(GetFacilityOptions()));
                        }
                    }

                    Widgets.DrawBox(buildingRect);
                    Widgets.DrawLightHighlight(buildingRect);
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
                GUI.DrawTexture(buildingRect, ContentFinder<Texture2D>.Get("UI/Facilities/InConstruction"));

                Widgets.DrawBoxSolid(progress, ColorLibrary.BrightBlue - new Color(0f, 0f, 0f, 0.8f));
                GUI.color = ColorLibrary.BrightBlue;
                Widgets.DrawLineVertical(progress.xMax, buildingRect.y, buildingRect.height);
                GUI.color = Color.white;

                return true;
            }

            if (facilityManager.MaxFacilities <= currentSpot)
            {
                TooltipHandler.TipRegion(buildingRect, "Empire_SIW_SlotLocked".TranslateSimple());
                GUI.DrawTexture(buildingRect, ContentFinder<Texture2D>.Get("UI/Facilities/LockedBuildingSlot"));

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

                    options.AddRange(GetFacilityOptions(currentFacilityDef));
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
                Rect tempRect = new Rect(rectMidBottomLeftRight.x, rectMidBottomLeftRight.y, rectMidBottomLeftRight.width, ActionButtonHeight).MoveRect(new Vector2(0f, (ActionButtonHeight + CommonMargin) * i));

                //If there are too many options to be displayed, create a button that can display the remaining ones using a float menu
                if (i == 7 && Actions.Count > 8)
                {
                    if (Widgets.ButtonText(tempRect, "Empire_SIW_MoreActions".Translate()))
                    {
                        List<FloatMenuOption> options = new List<FloatMenuOption>();

                        for (int j = i; j < Actions.Count; j++)
                        {
                            options.Add(new FloatMenuOption(Actions[i].LabelCap, () => Actions[i].Action(settlement)));
                        }

                        Find.WindowStack.Add(new FloatMenu(options));
                    }

                    break;
                }

                if (Widgets.ButtonText(tempRect, Actions[i].Label))
                {
                    Actions[i].Action(settlement);
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
                Widgets.Label(rectTopContent, $"<b>{settlement.LabelCap}</b> | Level: {facilityManager.MaxFacilities}");
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
                icon = ContentFinder<Texture2D>.Get("UI/Icons/QuestionMark"),
                action = () => Find.WindowStack.Add(new SettlementInfoWindow(settlement))
            };
            
            return command_Action;
        }
    }
}
