using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using RimWorld;
using Verse;

namespace Empire_Rewritten.Borders
{
    public class BorderManager : IExposable
    {
        private readonly Dictionary<Faction, int> borderIDs = new Dictionary<Faction, int>();
        private List<Border> borders = new List<Border>();

        public BorderManager()
        {
            GetBorderManager = this;
        }

        public List<Border> Borders => borders;

        public static BorderManager GetBorderManager { get; private set; }

        public void ExposeData()
        {
            Scribe_Collections.Look(ref borders, "borders");
        }

        public bool AnyFactionOwnsTile(int tile)
        {
            foreach (Border border in borders)
            {
                if (border.Tiles.Contains(tile))
                {
                    return true;
                }
            }

            return false;
        }

        public bool FactionOwnsTile([NotNull] Faction faction, int tile)
        {
            Border border = GetBorder(faction);
            return border != null && border.Tiles.Contains(tile);
        }

        public bool HasFactionRegistered([NotNull] Faction faction)
        {
            if (faction is null)
            {
                throw new ArgumentNullException(nameof(faction));
            }

            return borderIDs.ContainsKey(faction);
        }

        public Border GetBorder([NotNull] Faction faction)
        {
            if (faction is null)
            {
                throw new ArgumentNullException(nameof(faction));
            }

            if (HasFactionRegistered(faction))
            {
                return borders[borderIDs[faction]];
            }

            //Setup new faction border.
            Border newBorder = new Border(faction);
            borderIDs.Add(faction, borders.Count);
            borders.Add(newBorder);

            return newBorder;
        }
    }
}
