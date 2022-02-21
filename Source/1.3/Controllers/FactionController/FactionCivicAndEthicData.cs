using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Empire_Rewritten
{
    public class FactionCivicAndEthicData : IExposable
    {
        private readonly Faction faction;
        private readonly List<CivicDef> civics = new List<CivicDef>();
        private readonly List<EthicDef> ethics = new List<EthicDef>();

        public FactionCivicAndEthicData() { }

        public FactionCivicAndEthicData(Faction faction, IEnumerable<CivicDef> civics, IEnumerable<EthicDef> ethics)
        {
            this.faction = faction;
            this.civics = civics.ToList();
            this.ethics = ethics.ToList(); 
        }
        
        /// <summary>
        /// Provides the faction that this class saves data about
        /// </summary>
        public Faction Faction => faction;

        /// <summary>
        /// Provides the civics of this faction that this class saves data about
        /// </summary>
        public List<CivicDef> Civics => civics;

        /// <summary>
        /// Provides the ethics of this faction that this class saves data about
        /// </summary>
        public List<EthicDef> Ethics => ethics;

        public void ExposeData()
        {
            throw new NotImplementedException();
        }
    }
}
