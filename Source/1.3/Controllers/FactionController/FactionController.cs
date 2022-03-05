using System;
using System.Collections.Generic;
using Empire_Rewritten.Controllers.CivicEthic;
using Empire_Rewritten.Settlements;
using JetBrains.Annotations;
using RimWorld;
using Verse;

namespace Empire_Rewritten.Controllers
{
    /// <summary>
    ///     Manages <see cref="Faction">Factions</see> and their Settlements, Ethics, and Civics.
    ///     There will be one of these per <see cref="RimWorld.Planet.World" />
    /// </summary>
    public class FactionController : IExposable
    {
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
            this.factionSettlementDataList = factionSettlementDataList;
        }

        public void ExposeData()
        {
            Scribe_Collections.Look(ref factionSettlementDataList, "FactionSettlementDataList");
        }

        /// <param name="faction"></param>
        /// <returns>The <c>SettlementManager</c> owned by a given <paramref name="faction" /></returns>
        public SettlementManager GetOwnedSettlementManager(Faction faction)
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
        ///     Adds a given <see cref="EthicDef" /> to a <see cref="Faction" />
        /// </summary>
        /// <param name="faction">The <see cref="Faction" /> to modify</param>
        /// <param name="civic">The <see cref="CivicDef" /> to add to <paramref name="faction" /></param>
        public void AddCivicToFaction(Faction faction, CivicDef civic)
        {
            GetOwnedCivicAndEthicData(faction).Civics.Add(civic);
            NotifyCivicsOrEthicsChanged(GetOwnedSettlementManager(faction));
        }

        /// <summary>
        ///     Adds multiple given <see cref="EthicDef">EthicDefs</see> to a <see cref="Faction" /> at once
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

        public void NotifyCivicsOrEthicsChanged(SettlementManager settlementManager)
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