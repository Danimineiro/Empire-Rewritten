using System;
using System.Collections.Generic;
using System.Linq;
using Empire_Rewritten.Controllers.CivicEthic;
using JetBrains.Annotations;
using RimWorld;
using Verse;

namespace Empire_Rewritten.Controllers
{
    public class FactionCivicAndEthicData : IExposable
    {
        /// <summary>
        ///     For saving/loading
        /// </summary>
        [UsedImplicitly]
        public FactionCivicAndEthicData() { }

        public FactionCivicAndEthicData(Faction faction, IEnumerable<CivicDef> civics, IEnumerable<EthicDef> ethics)
        {
            Faction = faction;
            Civics = civics.ToList();
            Ethics = ethics.ToList();
        }

        /// <summary>
        ///     The <see cref="Faction" /> that this <see cref="FactionCivicAndEthicData" /> saves data about
        /// </summary>
        public Faction Faction { get; }

        /// <summary>
        ///     The <see cref="CivicDef">CivicDefs</see> of <see cref="FactionCivicAndEthicData" />'s
        ///     <see cref="FactionCivicAndEthicData.Faction" />
        /// </summary>
        public List<CivicDef> Civics { get; } = new List<CivicDef>();

        /// <summary>
        ///     The <see cref="EthicDef">EthicDefs</see> of <see cref="FactionCivicAndEthicData" />'s
        ///     <see cref="FactionCivicAndEthicData.Faction" />
        /// </summary>
        public List<EthicDef> Ethics { get; } = new List<EthicDef>();

        public void ExposeData()
        {
            throw new NotImplementedException();
        }
    }
}