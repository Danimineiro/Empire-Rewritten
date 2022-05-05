using RimWorld;
using System.Collections.Generic;
using Verse;

namespace Empire_Rewritten
{
    public struct FactionPawns : IExposable
    {
        public Faction faction;
        public List<Pawn> pawns;
        public void ExposeData()
        {
            Scribe_References.Look(ref faction, nameof(faction));
            Scribe_Collections.Look(ref pawns, nameof(pawns), LookMode.Reference);
        }

        public bool IsForFaction(Faction faction) => faction == this.faction;

        public bool HasPawn(Pawn pawn)=>pawns.Contains(pawn);
    }
    public class FactionPawnManager : IExposable
    {
        private List<FactionPawns> FactionPawns = new List<FactionPawns>();

        /// <summary>
        /// Check if we have the faction stored.
        /// </summary>
        /// <param name="faction"></param>
        /// <returns></returns>
        public bool HasFaction(Faction faction) => FactionPawns.Any(fp => fp.IsForFaction(faction));

        /// <summary>
        /// Get all pawns for a faction.
        /// </summary>
        /// <param name="faction"></param>
        /// <returns></returns>
        public List<Pawn> PawnsForFaction(Faction faction)
        {
            List<Pawn> result = new List<Pawn> ();
            
            if(HasFaction(faction))
                return result;

            result=FactionPawns.Find(fp=>fp.faction==faction).pawns;

            return result;
        }
    
        /// <summary>
        /// Generate a pawn for a faction
        /// 
        /// </summary>
        /// <param name="faction">Target faction</param>
        public void GeneratePawnForFaction(Faction faction)
        {
            PawnGenerationRequest generationRequest = new PawnGenerationRequest(faction.RandomPawnKind(), mustBeCapableOfViolence: true, faction: faction, fixedIdeo: faction.ideos.PrimaryIdeo);
            Pawn result = PawnGenerator.GeneratePawn(generationRequest);

            FactionPawns factionPawns;


            if (!HasFaction(faction))
            {
                factionPawns = new FactionPawns()
                {
                    faction = faction,
                    pawns = new List<Pawn>()
                };

            }
            else
            {
                factionPawns=FactionPawns.Find(fp=>fp.faction==faction);
            }

            factionPawns.pawns.Add(result);

            //Replace at index
            int index = FactionPawns.IndexOf(factionPawns);
            FactionPawns.RemoveAt(index);
            FactionPawns.Insert(index, factionPawns);
        }
      
        /// <summary>
        /// Generate multiple pawns for a faction
        /// </summary>
        /// <param name="faction">Target faction</param>
        /// <param name="amount">Amount of pawns to generate</param>
        public void GeneratePawnsForFaction(Faction faction, int amount)
        {
            for(int a =0; a < amount; a++)
            {
                GeneratePawnForFaction (faction);
            }
        }

        public void ExposeData()
        {
            Scribe_Collections.Look(ref FactionPawns, nameof(FactionPawns),LookMode.Deep);
        }
    }
}
