using System;
using System.Collections.Generic;
using Verse;

namespace Empire_Rewritten.Utils
{
    internal class FloatMenuOptionCreator
    {
        /// <summary>
        ///     Creates a <c>List</c> of <see cref="FloatMenuOption" />s of type <typeparamref name="T" /> that must be a
        ///     <see cref="Def" />
        ///     Each FloatMenuOption will trigger the provided action
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="defs"></param>
        /// <param name="floatMenuAction"></param>
        /// <returns></returns>
        public static List<FloatMenuOption> CreateFloatMenuOptions<T>(List<T> defs, Action<T> floatMenuAction) where T : Def
        {
            List<FloatMenuOption> floatMenuOptions = new List<FloatMenuOption>();

            foreach (T def in defs)
            {
                floatMenuOptions.Add(new FloatMenuOption(def.label, () => floatMenuAction(def)));
            }

            return floatMenuOptions;
        }
    }
}