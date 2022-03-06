﻿using System;
using System.Collections.Generic;
using System.Linq;
using Empire_Rewritten.AI;
using Empire_Rewritten.Borders;
using Empire_Rewritten.Controllers.CivicEthic;
using Empire_Rewritten.Player;
using Empire_Rewritten.Settlements;
using JetBrains.Annotations;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace Empire_Rewritten.Controllers
{
    /// <summary>
    ///     Manages <see cref="Faction">Factions</see> and their Settlements, Ethics, and Civics.
    ///     There will be one of these per <see cref="RimWorld.Planet.World" />
    /// </summary>
    public class FactionController : IExposable
    {
        private readonly Dictionary<Faction, AIPlayer> AIFactions = new Dictionary<Faction, AIPlayer>();
        private readonly List<FactionCivicAndEthicData> factionCivicAndEthicDataList = new List<FactionCivicAndEthicData>();
        private List<FactionSettlementData> factionSettlementDataList = new List<FactionSettlementData>();

        /// <summary>
        ///     Needed for loading
        /// </summary>
        [UsedImplicitly]
        public FactionController() { }

        /// <summary>
        ///     Creates a new <see cref="FactionController" />, telling it which <see cref="FactionSettlementData" /> it is
        ///     responsible for.
        /// </summary>
        /// <param name="factionSettlementDataList">
        ///     The <see cref="FactionSettlementData" /> instances that this
        ///     <see cref="FactionController" /> should maintain
        /// </param>
        public FactionController(List<FactionSettlementData> factionSettlementDataList)
        {
            ShouldRefreshBorders = true;
            BorderManager = new BorderManager();
            this.factionSettlementDataList = factionSettlementDataList;
        }

        public BorderManager BorderManager { get; } = new BorderManager();

        /// <summary>
        ///     Meant for things that cache the borders to check.
        /// </summary>
        public bool ShouldRefreshBorders { get; } = true;

        public void ExposeData()
        {
            Scribe_Collections.Look(ref factionSettlementDataList, "FactionSettlementDataList");
        }

        /// <param name="faction"></param>
        /// <returns>The <see cref="Empire" /> owned by a given <paramref name="faction" /></returns>
        public Empire GetOwnedSettlementManager(Faction faction)
        {
            foreach (FactionSettlementData factionSettlementData in factionSettlementDataList)
            {
                if (factionSettlementData.owner == faction)
                {
                    return factionSettlementData.SettlementManager;
                }
            }

            return null;
        }

        /// <summary>
        ///     Gets the AIPlayer of a faction.
        /// </summary>
        /// <param name="faction"></param>
        /// <returns></returns>
        public AIPlayer GetAIPlayer(Faction faction)
        {
            return AIFactions.ContainsKey(faction) ? AIFactions[faction] : null;
        }

        public void CreatePlayer()
        {
            Faction faction = Faction.OfPlayer;
            FactionSettlementData factionSettlementData = new FactionSettlementData(faction, new Empire(faction));
            factionSettlementDataList.Add(factionSettlementData);
            // NOTE: Why is this unused?
            UserPlayer player = new UserPlayer(faction);
            IEnumerable<WorldObject> settlements = Find.WorldObjects.Settlements.Where(x => x.Faction == faction);
            BorderManager.GetBorder(faction).SettlementClaimTiles((Settlement)settlements.First());
        }

        public void CreateNewAIPlayer(Faction faction)
        {
            AIPlayer aiPlayer = new AIPlayer(faction);
            AIFactions.Add(faction, aiPlayer);

            //Find preexisting settlements.
            List<Settlement> settlements = Find.WorldObjects.Settlements.Where(x => x.Faction == faction).ToList();
            if (settlements.Any())
            {
                if (settlements[0] is null)
                {
                    Log.Error("<color=orange>[Empire]</color> settlements has null-entry");
                }

                aiPlayer.Manager.AddSettlement(settlements[0]);
                settlements.RemoveAt(0);

                foreach (Settlement item in settlements)
                {
                    item.Destroy();
                }
            }

            Log.Message($"Created AI for: {faction.Name}");
        }

        /// <summary>
        ///     Adds a given <see cref="CivicDef" /> to a <see cref="Faction" />
        /// </summary>
        /// <param name="faction">The <see cref="Faction" /> to modify</param>
        /// <param name="civic">The <see cref="CivicDef" /> to add to <paramref name="faction" /></param>
        public void AddCivicToFaction(Faction faction, CivicDef civic)
        {
            GetOwnedCivicAndEthicData(faction).Civics.Add(civic);
            NotifyCivicsOrEthicsChanged(GetOwnedSettlementManager(faction));
        }

        /// <summary>
        ///     Adds multiple given <see cref="CivicDef">EthicDefs</see> to a <see cref="Faction" /> at once
        /// </summary>
        /// <param name="faction">The <see cref="Faction" /> to modify</param>
        /// <param name="civics">The <see cref="CivicDef">CivicDefs</see> to add to <paramref name="faction" /></param>
        public void AddCivicsToFaction(Faction faction, IEnumerable<CivicDef> civics)
        {
            GetOwnedCivicAndEthicData(faction).Civics.AddRange(civics);
            NotifyCivicsOrEthicsChanged(GetOwnedSettlementManager(faction));
        }

        /// <summary>
        ///     Adds a given <see cref="EthicDef" /> to a <see cref="Faction" />
        /// </summary>
        /// <param name="faction">The <see cref="Faction" /> to modify</param>
        /// <param name="ethic">The <see cref="EthicDef" /> to add to <paramref name="faction" /></param>
        public void AddEthicToFaction(Faction faction, EthicDef ethic)
        {
            GetOwnedCivicAndEthicData(faction).Ethics.Add(ethic);
            NotifyCivicsOrEthicsChanged(GetOwnedSettlementManager(faction));
        }

        /// <summary>
        ///     Adds multiple given <see cref="EthicDef">EthicDefs</see> to a <see cref="Faction" /> at once
        /// </summary>
        /// <param name="faction">The <see cref="Faction" /> to modify</param>
        /// <param name="ethics">The <see cref="EthicDef">EthicDefs</see> to add to <paramref name="faction" /></param>
        public void AddEthicsToFaction(Faction faction, IEnumerable<EthicDef> ethics)
        {
            GetOwnedCivicAndEthicData(faction).Ethics.AddRange(ethics);
            NotifyCivicsOrEthicsChanged(GetOwnedSettlementManager(faction));
        }

        public void NotifyCivicsOrEthicsChanged(Empire settlementManager)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Gets the <see cref="FactionCivicAndEthicData" /> of a given <see cref="Faction" />
        /// </summary>
        /// <param name="faction">The <see cref="Faction" /> to get the <see cref="FactionCivicAndEthicData" /> of</param>
        /// <returns>The <see cref="FactionCivicAndEthicData" /> linked to <paramref name="faction" /></returns>
        public FactionCivicAndEthicData GetOwnedCivicAndEthicData(Faction faction)
        {
            return factionCivicAndEthicDataList.FirstOrFallback(data => data.Faction == faction);
        }
    }
}