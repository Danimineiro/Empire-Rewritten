using System.Collections.Generic;
using System.Linq;
using Empire_Rewritten.Settlements;
using JetBrains.Annotations;
using RimWorld;
using Verse;

namespace Empire_Rewritten.Controllers
{
    /// <summary>
    ///     Links a <see cref="Faction" /> and its <see cref="RimWorld.Planet.Settlement" /> through a
    ///     <see cref="SettlementManager" />
    /// </summary>
    public class FactionSettlementData : IExposable
    {
        private Faction originalOwner;

        public Faction owner;
        private Empire settlementManager;

        /// <summary>
        ///     Used for saving/loading
        /// </summary>
        [UsedImplicitly]
        public FactionSettlementData() { }

        /// <summary>
        ///     Supposed to be called when a <see cref="Faction" /> is created
        /// </summary>
        /// <param name="owner">The <see cref="Faction" /> that this <see cref="FactionSettlementData" /> belongs to</param>
        /// <param name="settlementManager">
        ///     The <see cref="SettlementManager" /> of this <see cref="FactionSettlementData" />
        /// </param>
        public FactionSettlementData(Faction owner, Empire settlementManager)
        {
            this.owner = owner;
            originalOwner = owner;
            this.settlementManager = settlementManager;
        }

        /// <summary>
        ///     The <see cref="Faction" /> that originally created this <see cref="FactionSettlementData" />
        ///     Should never change
        /// </summary>
        public Faction OriginalOwner => originalOwner;

        /// <summary>
        ///     Returns the SettlementManager, shouldn't ever be changed
        /// </summary>
        public Empire SettlementManager => settlementManager;

        public void ExposeData()
        {
            Scribe_References.Look(ref owner, "owner");
            Scribe_References.Look(ref originalOwner, "originalOwner");
            Scribe_Deep.Look(ref settlementManager, "settlementManager");
        }

        /// <summary>
        ///     Creates all required instances of <see cref="FactionSettlementData" />
        /// </summary>
        /// <returns>A <see cref="List{T}" /> of the newly created <see cref="FactionSettlementData" /> instances</returns>
        internal static List<FactionSettlementData> CreateFactionSettlementData()
        {
            return Find.FactionManager.AllFactionsListForReading.Select(faction => new FactionSettlementData(faction, new Empire(faction))).ToList();
        }

        public override string ToString()
        {
            return $"[FactionSettlementData] owner: {owner.Name}, originalOwner: {originalOwner.Name}, settlementManager: {settlementManager}";
        }
    }
}
