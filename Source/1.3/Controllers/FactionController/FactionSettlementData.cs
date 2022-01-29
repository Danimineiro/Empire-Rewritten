using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Empire_Rewritten
{
    /// <summary>
    /// This class links a Faction and it's settlements through a SettlementManager 
    /// </summary>
    public class FactionSettlementData : IExposable
    {
        public FactionSettlementData()
        {

        }
        /// <summary>
        /// Used for saving/loading
        /// </summary>
        public FactionSettlementData() { }

        /// <summary>
        /// Creates a new <c>FactionSettlementData</c> struct.
        /// Saves <paramref name="owner"/> into the <code>owner</code> and <code>originalOwner</code> field; saves <paramref name="settlementManager"/> into the <code>settlementManager field</code>
        /// Supposed to be called when a faction is created
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="settlementManager"></param>
        public FactionSettlementData(Faction owner, SettlementManager settlementManager)
        {
            this.owner = owner;
            this.originalOwner = owner; 
            this.settlementManager = settlementManager;
        }

        public Faction owner;
        private Faction originalOwner;
        private SettlementManager settlementManager;

        /// <summary>
        /// Returns the original Owner, shouldn't ever be changed
        /// </summary>
        public Faction OriginalOwner => originalOwner;

        /// <summary>
        /// Returns the SettlementManager, shouldn't ever be changed
        /// </summary>
        public SettlementManager SettlementManager => settlementManager;

        /// <summary>
        /// Creates all required FactionSettlementDatas
        /// </summary>
        /// <returns>the FactionSettlementDatas</returns>
        internal static List<FactionSettlementData> CreateFactionSettlementDatas()
        {
            List<FactionSettlementData> factionSettlementDatas = new List<FactionSettlementData>();
            foreach (Faction faction in Find.FactionManager.AllFactionsListForReading)
            {
                factionSettlementDatas.Add(new FactionSettlementData(faction, new SettlementManager()));
            }

            return factionSettlementDatas;
        }

        public void ExposeData()
        {
            Scribe_References.Look(ref owner, "Owner");
            Scribe_References.Look(ref originalOwner, "originalOwner");
            Scribe_Deep.Look(ref settlementManager, "settlementManager");
        }

        public override string ToString()
        {
            return $"[FactionSettlementData] owner: {owner.Name}, originalOwner: {originalOwner.Name}, settlementManager: {settlementManager}";
        }
    }
}
