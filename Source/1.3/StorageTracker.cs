using System.Collections.Generic;
using Empire_Rewritten.Resources;
using Empire_Rewritten.Utils;
using Verse;

namespace Empire_Rewritten
{
    public class StorageTracker : IExposable, ILoadReferenceable
    {
        // For AI use
        private Dictionary<ThingDef, int> storedThings = new Dictionary<ThingDef, int>();

        public Dictionary<ResourceDef, int> ApproximateResources { get; } = new Dictionary<ResourceDef, int>();

        public Dictionary<ThingDef, int> StoredThings => storedThings;

        public void ExposeData()
        {
            Scribe_Collections.Look(ref storedThings, "storedThings", LookMode.Def, LookMode.Value);
        }

        public string GetUniqueLoadID()
        {
            return $"StorageTracker_{GetHashCode()}";
        }

        /// <summary>
        ///     Adds a given <see cref="int">amount</see> of a given <see cref="ThingDef" /> to this <see cref="StorageTracker" />
        /// </summary>
        /// <param name="def">The <see cref="ThingDef" /> to add</param>
        /// <param name="count">The <see cref="int">amount</see> of <paramref name="def" /> to add</param>
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



        public bool TryRemoveThingsFromStorage(Dictionary<ThingDef, int> things)
        {
            bool hasRemovedThings = true;
            foreach(KeyValuePair<ThingDef,int> kvp in things)
            {
               hasRemovedThings &= TryRemoveThingsFromStorage(kvp.Key, kvp.Value);
            }
            return hasRemovedThings;
        }

        /// <summary>
        ///     Tries to remove a given <see cref="int">amount</see> of a given <see cref="ThingDef" /> from this
        ///     <see cref="StorageTracker" />
        /// </summary>
        /// <param name="def">The <see cref="ThingDef" /> to try and remove</param>
        /// <param name="count">The <see cref="int">amount</see> of <paramref name="def" /> to try and remove</param>
        /// <returns>Whether the given <paramref name="count" /> of <paramref name="def" /> could successfully be removed</returns>
        public bool TryRemoveThingsFromStorage(ThingDef def, int count)
        {
            if (!CanRemoveThingsFromStorage(def, count)) return false;

            int newCount = storedThings[def] -= count;
            if (newCount <= 0)
            {
                if (newCount < 0)
                {
                    Logger.Warn($"We had negative of {def.defName} after {nameof(TryRemoveThingsFromStorage)}, this shouldn't happen");
                }

                storedThings.Remove(def);
            }

            return true;
        }

        /// <summary>
        /// Check if we can remove a set of <see cref="ThingDef"/> from the <see cref="StorageTracker"/>.
        /// </summary>
        /// <param name="thingsAndAmount"></param>
        /// <returns></returns>
        public bool CanRemoveThingsFromStorage(Dictionary<ThingDef, int> thingsAndAmount)
        {
            bool canRemove = true;
            foreach(KeyValuePair<ThingDef, int> kvp in thingsAndAmount)
            {
               canRemove &= CanRemoveThingsFromStorage(kvp.Key, kvp.Value);
            }
            return canRemove;
        }

        /// <summary>
        ///     Checks whether you can remove a given <see cref="int">amount</see> of a given <see cref="ThingDef" /> from this
        ///     <see cref="StorageTracker" />
        /// </summary>
        /// <param name="def">The <see cref="ThingDef" /> to check for</param>
        /// <param name="count">The <see cref="int">amount</see> of <paramref name="def" /> to check for</param>
        /// <returns>
        ///     <c>true</c> if there's more than <paramref name="count" /> of <paramref name="def" /> contained in this
        ///     <see cref="StorageTracker" />, false otherwise
        /// </returns>
        public bool CanRemoveThingsFromStorage(ThingDef def, int count)
        {
            return GetCountOfThing(def) >= count;
        }

        /// <summary>
        ///     Checks how much a given <see cref="ThingDef" /> is contained in this <see cref="StorageTracker" />
        /// </summary>
        /// <param name="def">The <see cref="ThingDef" /> to look for</param>
        /// <returns>How many of <paramref name="def" /> are contained in this <see cref="StorageTracker" />. 0 if not contained</returns>
        public int GetCountOfThing(ThingDef def)
        {
            return storedThings.ContainsKey(def) ? storedThings[def] : 0;
        }

        /// <summary>
        ///     Checks whether a given <see cref="ThingDef" /> is contained in this <see cref="StorageTracker" />
        /// </summary>
        /// <param name="def">The <see cref="ThingDef" /> to look for</param>
        /// <returns>Whether <paramref name="def" /> is contained in this <see cref="StorageTracker" /></returns>
        public bool ContainsThing(ThingDef def)
        {
            return storedThings.ContainsKey(def);
        }
    }
}