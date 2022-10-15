using System.Linq;
using JetBrains.Annotations;
using Verse;

namespace Empire_Rewritten.Resources.ResourceWorkers
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
    public class ResourceWorker_Rancher : ResourceWorker
    {
        public ResourceWorker_Rancher(ThingFilter filter) : base(filter) { }

        /// <summary>
        ///     Allows all animals marked as <see cref="RaceProperties.Roamer" /> to be produced through ranching
        /// </summary>
        /// <returns>Reference to the modified <see cref="ThingFilter" /></returns>
        public override ThingFilter PostModifyThingFilter()
        {
            foreach (PawnKindDef def in DefDatabase<PawnKindDef>.AllDefsListForReading.Where(def => def.RaceProps?.Roamer == true))
            {
                filter.SetAllow(def.race, true);
            }

            return filter;
        }
    }
}