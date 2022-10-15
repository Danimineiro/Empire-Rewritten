using Verse;

namespace Empire_Rewritten.Resources
{
    /// <summary>
    ///     Base <see cref="ResourceWorker" />. Should be extended on.
    /// </summary>
    public class ResourceWorker
    {
        /// <summary>
        ///     A <see cref="ThingFilter" /> representing which <see cref="Thing">Things</see> are allowed to be produced.
        /// </summary>
        protected readonly ThingFilter filter;

        public ResourceWorker(ThingFilter filter)
        {
            this.filter = filter;
        }

        /// <summary>
        ///     Allows you to modify <see cref="ResourceWorker.filter" /> additionally after this
        ///     <see cref="ResourceWorker">ResourceWorker's</see> <see cref="ResourceDef" /> has been initialized.
        /// </summary>
        /// <returns>A reference to <see cref="ResourceWorker.filter" /></returns>
        public virtual ThingFilter PostModifyThingFilter()
        {
            return filter;
        }
    }
}