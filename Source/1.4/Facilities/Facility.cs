using System.Collections.Generic;
using System.Linq;
using Empire_Rewritten.Resources;
using JetBrains.Annotations;
using RimWorld.Planet;
using Verse;

namespace Empire_Rewritten.Facilities
{
    public class Facility : IExposable, ILoadReferenceable
    {
        private readonly List<ResourceModifier> modifiers = new List<ResourceModifier>();
        private int amount;
        public FacilityDef def;
        private Settlement settlement;
        private bool shouldRecalculateModifiers = true;

        public Facility(FacilityDef def, Settlement settlement)
        {
            this.def = def;
            this.settlement = settlement;
            amount = 1;
            FacilityWorker = def.FacilityWorker;
        }

        [UsedImplicitly]
        public Facility() { }

        public FacilityWorker FacilityWorker { get; }

        public int Amount => amount;

        public int FacilitiesInstalled => amount;

        public List<ResourceModifier> ResourceModifiers
        {
            get
            {
                if (shouldRecalculateModifiers)
                {
                    Recalculate();
                    shouldRecalculateModifiers = false;
                }

                return modifiers;
            }
        }

        public void ExposeData()
        {
            Scribe_Defs.Look(ref def, "def");
            Scribe_Values.Look(ref amount, "amount");
            Scribe_References.Look(ref settlement, "settlement");
        }

        public string GetUniqueLoadID()
        {
            return $"Facility_{GetHashCode()}";
        }

        /// <summary>
        ///     Adds an amount of facilities to this <see cref="Facility" />'s settlement.
        /// </summary>
        /// <param name="amountToAdd">The <see cref="int">amount</see> of facilities to add</param>
        public void AddFacilities(int amountToAdd)
        {
            amount += amountToAdd;
            shouldRecalculateModifiers = true;
            FacilityWorker?.NotifyConstructed(this);
        }

        /// <summary>
        ///     Adds a single facility to this <see cref="Facility" />'s settlement.
        /// </summary>
        public void AddFacility()
        {
            AddFacilities(1);
        }

        /// <summary>
        ///     Removes an amount of facilities to this <see cref="Facility" />'s settlement.
        /// </summary>
        /// <param name="amountToRemove">The <see cref="int">amount</see> of facilities to remove</param>
        public void RemoveFacilities(int amountToRemove)
        {
            amount -= amountToRemove;
            if (amount < 0) amount = 0;

            FacilityWorker?.NotifyDestroyed(this);

            shouldRecalculateModifiers = true;
        }

        /// <summary>
        ///     Removes a single facility to this <see cref="Facility" />'s settlement.
        /// </summary>
        public void RemoveFacility()
        {
            RemoveFacilities(1);
        }

        /// <summary>
        ///     Recalculates the <see cref="ResourceModifiers" /> of this <see cref="Facility" />.
        /// </summary>
        private void Recalculate()
        {
            modifiers.Clear();

            IEnumerable<ResourceDef> defs = def.resourceMultipliers.Select(r => r.def).Concat(def.resourceOffsets.Select(r => r.def));

            foreach (ResourceDef resourceDef in defs)
            {
                Tile tile = Find.WorldGrid.tiles[settlement.Tile];
                ResourceModifier modifier = resourceDef.GetTileModifier(tile);

                ResourceChange multiplier = def.resourceMultipliers.FirstOrFallback(r => r.def == resourceDef);
                if (multiplier is null)
                {
                    modifier.multiplier *= amount;
                }
                else
                {
                    modifier.multiplier *= amount * multiplier.amount;
                }

                ResourceChange offset = def.resourceOffsets.FirstOrFallback(r => r.def == resourceDef);
                if (!(offset is null))
                {
                    modifier.offset += amount * offset.amount;
                }

                modifiers.Add(modifier);
            }
        }
    }
}