using Verse;

namespace Empire_Rewritten
{
    public class ResourceWorker
    {
        protected readonly ThingFilter filter = null;

        public ResourceWorker(ThingFilter filter) => this.filter = filter;

        public virtual ThingFilter PostModifyThingFilter()
        {
            return filter;
        }
    }
}
