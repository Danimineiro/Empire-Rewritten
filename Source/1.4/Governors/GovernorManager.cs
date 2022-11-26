using RimWorld.Planet;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Empire_Rewritten
{
    // Reminder: Hook governor creation into the PawnController once its implemented.
    // Reminder: Add governor stats and hook them into the settlement calculations.
    public class GovernorManager : IExposable
    {
        public GovernorHistoryDef history = null;
        public Pawn pawn = null;
        public String FullName => (pawn.Name as NameTriple).ToStringFull;

        private int daysSinceLastElection;
        private Settlement settlement;
        private bool isColonist = false;

        public GovernorManager (Settlement settlement)
        {
            this.settlement = settlement;
            this.daysSinceLastElection = 0;
            ReplaceGovernor();
        }

        public void ReplaceGovernor (Pawn pawn = null, bool appointed = false, GovernorHistoryDef def = null)
        {
            if (pawn == null)
            {
                PawnGenerationRequest req = MakeGenRequest();
                pawn = PawnGenerator.GeneratePawn(req);
            }
            if (def == null)
            {
                def = DefDatabase<GovernorHistoryDef>.AllDefsListForReading
                    .Where(find => find.appointed == appointed)
                    .RandomElement<GovernorHistoryDef>();
            }
            // To be replaced with proper faction pawn generation.
            // Add announcement for governor replacement
            Pawn oldgov = this.pawn;
            Faction playerfaction = Find.FactionManager.OfPlayer;
            if (oldgov != null)
            {
                if (this.isColonist)
                {
                    CaravanMaker.MakeCaravan(
                        new List<Pawn>() { oldgov },
                        playerfaction,
                        this.settlement.Tile,
                        true);
                }
                else
                {
                    oldgov.Discard(true);
                }
            }
            this.pawn = pawn;
            this.history = def;
            this.isColonist = appointed;
            this.daysSinceLastElection = 0;

        }

        public void ExposeData()
        {
            Scribe_Defs.Look(ref this.history, "History");
            Scribe_References.Look(ref this.pawn, "Pawn");
            Scribe_Values.Look(ref this.isColonist, "isColonist");
            Scribe_Values.Look(ref this.daysSinceLastElection, "daysSinceLastElection");
        }

        public PawnGenerationRequest MakeGenRequest ()
        {
            PawnGenerationRequest req = new PawnGenerationRequest(
                PawnKindDefOf.Colonist,
                this.settlement.Faction,
                PawnGenerationContext.NonPlayer,
                -1,
                false,
                false,
                false,
                false,
                true,
                0,
                true,
                true,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                0,
                0
                );
            return req;

        }

    }
}
