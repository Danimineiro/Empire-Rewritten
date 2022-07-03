using RimWorld.Planet;
using System;
using Verse;

namespace Empire_Rewritten.Windows.Snippets
{
    public class SettlementAction
    {
        private string label;
        private Action<Settlement> action;
        private string labelCapCached;

        public Action<Settlement> Action
        {
            get => action;
            set => action = value;
        }

        public string Label
        {
            get => label;
            set => label = value;
        }

        public string LabelCap
        {
            get => labelCapCached ?? (labelCapCached = label.CapitalizeFirst());
            set => label = value;
        }
    }
}
