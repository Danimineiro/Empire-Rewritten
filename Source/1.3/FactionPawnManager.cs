using RimWorld;
using System.Collections.Generic;
using Verse;

namespace Empire_Rewritten
{
    public class FactionPawnManager : IExposable
    {
        private Dictionary<Faction, List<Pawn>> pawns = new Dictionary<Faction, List<Pawn>> ();
        
        /// <summary>
        /// Get all pawns for a faction.
        /// </summary>
        /// <param name="faction"></param>
        /// <returns></returns>
        public List<Pawn> PawnsForFaction(Faction faction)
        {
            List<Pawn> result = new List<Pawn> ();
            
            if(!pawns.ContainsKey(faction)||!pawns[faction].NullOrEmpty())
                return result;

            result=pawns[faction];

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
            
            if(!pawns.ContainsKey(faction))
                pawns.Add(faction, new List<Pawn>());
            pawns[faction].Add(result);
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
            Scribe_Collections.Look(ref pawns, "pawns", LookMode.Reference,LookMode.Reference);
        }
    }
}
