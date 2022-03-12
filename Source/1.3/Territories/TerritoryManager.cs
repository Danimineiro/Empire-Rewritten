using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using RimWorld;
using Verse;

namespace Empire_Rewritten.Territories
{
    public class TerritoryManager : IExposable
    {
        private readonly Dictionary<Faction, int> territoryIDs = new Dictionary<Faction, int>();
        private List<Territory> territories = new List<Territory>();

        public TerritoryManager()
        {
            GetTerritoryManager = this;
        }

        public List<Territory> Territories => territories;

        public static TerritoryManager GetTerritoryManager { get; private set; }

        public void ExposeData()
        {
            Scribe_Collections.Look(ref territories, "territories");
        }

        public bool AnyFactionOwnsTile(int tile)
        {
            foreach (Territory territory in territories)
            {
                if (territory.Tiles.Contains(tile))
                {
                    return true;
                }
            }

            return false;
        }

        public bool FactionOwnsTile([NotNull] Faction faction, int tile)
        {
            Territory territory = GetTerritory(faction);
            return territory != null && territory.Tiles.Contains(tile);
        }

        public bool HasFactionRegistered([NotNull] Faction faction)
        {
            if (faction is null)
            {
                throw new ArgumentNullException(nameof(faction));
            }

            return territoryIDs.ContainsKey(faction);
        }

        public Territory GetTerritory([NotNull] Faction faction)
        {
            if (faction is null)
            {
                throw new ArgumentNullException(nameof(faction));
            }

            if (HasFactionRegistered(faction))
            {
                return territories[territoryIDs[faction]];
            }

            // Set up new faction territory.
            Territory newTerritory = new Territory(faction);
            territoryIDs.Add(faction, territories.Count);
            territories.Add(newTerritory);

            return newTerritory;
        }
    }
}
