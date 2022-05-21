using Empire_Rewritten.Controllers;
using Empire_Rewritten.Settlements;
using JetBrains.Annotations;
using RimWorld;
using UnityEngine;
using Verse;
#if DEBUG
using System.Linq;
using System.Reflection;
#endif

namespace Empire_Rewritten.Windows.MainTabWindowTabs
{
    [UsedImplicitly]
    public class SettlementTab : BaseMainTabWindowTab
    {
        private readonly Rect rectMain = EmpireMainTabWindow.RectTabSpace;

        private Vector2 scrollPosition;

        public SettlementTab([NotNull] MainTabWindowTabDef def) : base(def) 
        {
            
        }

        public override void Draw(Rect inRect)
        {
            
        }
    }
}
