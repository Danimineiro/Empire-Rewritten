using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
namespace Empire_Rewritten { 
    public class StorageTracker : IExposable, ILoadReferenceable
    {
        private Dictionary<ThingDef, int> storedThings = new Dictionary<ThingDef, int>();

        public Dictionary<ThingDef,int> StoredThings { 
            get 
            { 
                return storedThings; 
            } 
        }

        public void AddThingsToStorage(ThingDef def, int count)
        {
            if (storedThings.ContainsKey(def))
            {
                storedThings[def] += count;
            }
            else
            {
                storedThings.Add(def, count);
            }
        }

        public bool RemoveThingsFromStorage(ThingDef def, int count)
        {
            if (CanRemoveThingsFromStorage(def, count))
            {
                storedThings[def] -= count;
                if (storedThings[def] <= 0)
                {
                    storedThings.Remove(def);
                }
                return true;
            }
            return false;
        }

        public bool CanRemoveThingsFromStorage(ThingDef def, int count)
        {
            return storedThings.ContainsKey(def) && storedThings[def] >=count;
        }

        public int GetCountOfThing(ThingDef def)
        {
            return storedThings.ContainsKey(def) ? storedThings[def] : 0;
        }
        
        public bool ContainsThing(ThingDef def)
        {
            return GetCountOfThing(def) > 0;
        }

        public void ExposeData()
        {
            Scribe_Collections.Look(ref storedThings, "storedThings",LookMode.Def,LookMode.Value);
        }

        public string GetUniqueLoadID()
        {
            return $"StorageTracker_{GetHashCode()}";
        }
    }
}
