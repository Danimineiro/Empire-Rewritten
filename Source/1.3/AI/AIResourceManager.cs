using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Empire_Rewritten.AI
{
    public class AIResourceManager : AIModule
    {
        private static List<ResourceDef> cachedDefs = new List<ResourceDef>();
        private AIPlayer parentPlayer;
        
        private int LowResourceDecider=30;

        public AIResourceManager(AIPlayer player) : base(player)
        {
        }

        private List<ResourceDef> FindAllResourceDefs
        {
            get
            {
                if (cachedDefs.NullOrEmpty())
                {
                    cachedDefs = DefDatabase<ResourceDef>.AllDefsListForReading;
                }
                return cachedDefs;
            }
        }

        


        /// <summary>
        /// Figure out what resources are "low" in production based on the amount being produced.
        /// LowResourceDecider changes the low value.
        /// </summary>
        /// <returns>Any resource below LowResourceDecider, or the lowest resource.</returns>
        public List<ResourceDef> FindLowResources()
        {
            List<ResourceDef> result = new List<ResourceDef>();
            Dictionary<ResourceDef, float> producedknown = new Dictionary<ResourceDef, float>();
            bool resourceBelowDecider = false;
           /* 
            Since producedknown is only useful if the AI produces the def
           the calculation checks against all ResourceDefs in the database
            */
            foreach(ResourceDef def in FindAllResourceDefs)
            {
                if (producedknown.ContainsKey(def))
                {
                    if (producedknown[def] <= LowResourceDecider)
                    {
                        result.Add(def);
                        resourceBelowDecider = true;
                    }
                }
                //If the def is not produced at all, add it!
                else
                {
                    result.Add(def);
                }
            }
            if (resourceBelowDecider)
            {
                /*
                 Find the lowest defs produced, and adds them to a list.
                This isn't perfect, however it should catch most things.
                 */
                float lowest=0;
                bool first = true;
                foreach(ResourceDef def in producedknown.Keys)
                {
                    if (producedknown[def] <lowest || (lowest == 0 && first))
                    {
                        first = false;
                        lowest = producedknown[def];
                        result.Add(def);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Find all resources produced.
        /// </summary>
        /// <returns></returns>
        public Dictionary<ResourceDef,float> AllResourcesProduced()
        {
           Dictionary<ResourceDef, ResourceModifier> modifiers=  parentPlayer.Manager.ResourceModifiersFromAllFacilities();
            Dictionary<ResourceDef, float> result = new Dictionary<ResourceDef, float>();
            foreach (ResourceDef def in modifiers.Keys)
            {
                ResourceModifier resourceModifier = modifiers[def];
                result.Add(def,resourceModifier.TotalProduced());
            }
            return result;
        }
    }
}
