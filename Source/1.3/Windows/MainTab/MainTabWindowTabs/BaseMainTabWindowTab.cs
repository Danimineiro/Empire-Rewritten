using JetBrains.Annotations;
using UnityEngine;

namespace Empire_Rewritten.Windows.MainTabWindowTabs
{
    public abstract class BaseMainTabWindowTab
    {
        [NotNull] public readonly MainTabWindowTabDef def;

        protected BaseMainTabWindowTab([NotNull] MainTabWindowTabDef def)
        {
            this.def = def;
        }

        public abstract void Draw(Rect inRect);
    }
}
