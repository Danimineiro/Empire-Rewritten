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

        private readonly Rect rectFlag;
        private readonly Rect rectDesignation;
        private readonly Rect rectExtraInfo;

        private readonly Rect rectBuildingsLabel;
        private readonly Rect rectBuildingsRect;
        private readonly Rect rectBuildingRect = new Rect(0f, 0f, 64f, 64f);

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

        private List<FacilityDef> installedFacilitesCached;
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
            forcePause = true;

            this.settlement = settlement;
            facilityManager = Empire.PlayerEmpire.GetFacilityManager(settlement);
            installedFacilitesCached = facilityManager.FacilityDefsInstalled.ToList();

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
        
            rectFlag = new Rect(rectMidTopLeft.x + CommonMargin, rectMidTopLeft.y + CommonMargin, FlagSize, FlagSize);
            rectDesignation = new Rect(rectMidTopLeft.x + rectFlag.width + CommonMargin * 2f, rectMidTopLeft.y + FlagSize * 0.5f, DesignationWidth, FlagSize * 0.5f);
            rectExtraInfo = new Rect( - CommonMargin, 0f, DesignationWidth * 1.5f, FlagSize * 0.5f).MoveRect(new Vector2(rectMidTopLeft.xMax - DesignationWidth * 1.5f, rectMidTopLeft.y + FlagSize * 0.5f));

            rectBuildingsLabel = new Rect(rectMidBottomLeftLeft.x + CommonMargin, rectMidBottomLeftLeft.y + CommonMargin, rectMidBottomLeftLeft.width - CommonMargin * 2f, FlagSize * 0.5f);
            rectBuildingsRect = new Rect(new Vector2(), buildingsRectSize)
            {
                center = rectMidBottomLeftLeft.center + new Vector2(0f, rectBuildingsLabel.height + CommonMargin)
            }.Rounded();
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

            Widgets.DrawBox(rectMidBottomRight);
            Widgets.Label(rectMidBottomRight, "rectMidBottomRight");

            Text.Anchor = TextAnchor.UpperLeft;

            DrawBottomPart();
        }

        private void DrawBottomLeftParts()
        {
            Widgets.DrawBox(rectMidBottomLeft);
            //Widgets.Label(rectMidBottomLeft, "rectMidBottomLeft");

            Widgets.DrawBox(rectMidBottomLeftLeft);
            //Widgets.Label(rectMidBottomLeftLeft, "rectMidBottomLeftLeft");
           
            DrawActionButtons();

            //Widgets.DrawBox(rectBuildingsLabel);
            Widgets.Label(rectBuildingsLabel, $"<b>{"Empire_SIW_BuildingsLabel".Translate()}</b>");

            Widgets.DrawBox(rectBuildingsRect);
            Vector2 moveVector = new Vector2();

            int facilityCount = facilityManager.FacilityCount;
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    int currentSpot = i * 6 + j; 
                    Rect buildingRect = new Rect(new Vector2(rectBuildingsRect.x + CommonMargin, rectBuildingsRect.y + CommonMargin), rectBuildingRect.size).MoveRect(moveVector);
                    moveVector += new Vector2(rectBuildingRect.width + CommonMargin, 0f);

                    Widgets.DrawBox(buildingRect);
                    Widgets.DrawHighlightIfMouseover(buildingRect);
                    
                    if (installedFacilitesCached.Count > currentSpot)
                    {
                        FacilityDef currentFacilityDef = installedFacilitesCached[currentSpot];
                        GUI.DrawTexture(buildingRect, ContentFinder<Texture2D>.Get(currentFacilityDef.iconData.texPath));

                        TextAnchor prev = Text.Anchor;
                        Text.Anchor = TextAnchor.LowerRight;
                        Widgets.Label(buildingRect.ContractedBy(5f), $"<b>{facilityManager.HasFacilityAmount(currentFacilityDef)}x</b>");
                        Text.Anchor = prev;

                        continue;
                    }

                    if (facilityManager.MaxFacilities - facilityCount - currentSpot < 0)
                    {
                        TooltipHandler.TipRegion(buildingRect, "Empire_SIW_SlotLocked".TranslateSimple());
                        Widgets.DrawBoxSolid(buildingRect, ColorLibrary.RedReadable - new Color(0f, 0f, 0f, 0.4f));
                        continue;
                    }

                    if (Widgets.ButtonInvisible(buildingRect))
                    {
                        List<FloatMenuOption> options = new List<FloatMenuOption>();
                        
                        foreach (FacilityDef facilityDef in DefDatabase<FacilityDef>.AllDefsListForReading)
                        {
                            options.Add(new FloatMenuOption(facilityDef.LabelCap, () =>
                            {
                                facilityManager.AddFacility(facilityDef);
                                RefreshCachedVariables();
                            }, ContentFinder<Texture2D>.Get(facilityDef.iconData.texPath), Color.white));
                        }

                        Find.WindowStack.Add(new FloatMenu(options));
                    }

                }

                moveVector += new Vector2(-(rectBuildingRect.width + CommonMargin) * 6f, rectBuildingRect.width + CommonMargin);
            }
        }

        private void RefreshCachedVariables()
        {
            installedFacilitesCached = facilityManager.FacilityDefsInstalled.ToList();
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
                Widgets.Label(rectTopContent, settlement.LabelCap);
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
        public static Gizmo GetConnectionGizmo(Settlement settlement)
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
