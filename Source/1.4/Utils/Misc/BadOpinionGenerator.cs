using Empire_Rewritten.Settlements;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Empire_Rewritten
{
    /// <summary>
    /// This class generates blips to be shown at the bottom of the settlement screen. 
    /// </summary>
    public static class BadOpinionGenerator
    {
        //Language Key for Barks: Bark_Trait_TRAITDEFNAME_NUMBER, i.e. Bark_Bloodlust_0
        // Instead of a traitdef name, use Generic for generic barks.
        public static String GenerateBark (GovernorManager governor)
        {
            Pawn pawn = governor.pawn;
            List<string> possible = new List<string>();
            List<string> traits = pawn.story.traits.allTraits.Select(trait => trait.def.defName).ToList<string>();
            traits.Add("Generic");
            foreach (String trait in traits)
            {
                int counter = 0;
                string bark;
                string translated;
                // Strings that lack a translation just return the Key, instead. Here, we check
                // for equality to automatically include all new barks listed in the lang file.
                while (true)
                {
                    bark = "Empire_Bark_" + trait + "_" + counter;
                    if (!bark.CanTranslate())
                        break;
                    translated = bark.Translate();
                        possible.Add(bark.Translate());
                    if (counter > 99)
                        break;
                    counter++;
                }
            }

            return possible.RandomElement();
        }

    }



}
