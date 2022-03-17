using JetBrains.Annotations;
using UnityEngine;

namespace Empire_Rewritten.Windows
{
    public abstract class EmpireWindowTab
    {
        [NotNull] public readonly EmpireMainTabDef def;

        protected EmpireWindowTab([NotNull] EmpireMainTabDef def)
        {
            this.def = def;
        }

        public abstract void Draw(Rect inRect);
    }
}
