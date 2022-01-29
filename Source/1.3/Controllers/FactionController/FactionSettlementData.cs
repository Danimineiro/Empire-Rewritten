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
    /// This struct links a Faction and it's settlements through a SettlementManager 
    /// </summary>
    public struct FactionSettlementData : IExposable
    {
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

        public void ExposeData()
        {
            Scribe_References.Look(ref owner, "Owner");
            Scribe_References.Look(ref originalOwner, "originalOwner");
            Scribe_References.Look(ref settlementManager, "settlementManager");
        }

        public override string ToString()
        {
            return $"[FactionSettlementData] owner: {owner.Name}, originalOwner: {originalOwner.Name}, settlementManager: {settlementManager}";
        }
    }
}
