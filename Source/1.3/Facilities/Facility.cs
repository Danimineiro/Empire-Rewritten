using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Empire_Rewritten
{
    public class Facility : IExposable, ILoadReferenceable
    {
        int amount;
        public FacilityDef def;

        private bool ShouldRecalculateModifiers = true;
        private List<ResourceModifier> modifiers = new List<ResourceModifier>();
        private Settlement settlement;

        public int FacilitiesInstalled
        {
            get
            {
                return amount;
            }
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref def, "def");
            Scribe_Values.Look(ref amount, "amount");
            Scribe_References.Look(ref settlement, "settlement");
        }

        public string GetUniqueLoadID()
        {
            return $"Facility_{GetHashCode()}";
        }

        public Facility(FacilityDef def,Settlement settlement)
        {
            this.def = def;
            this.settlement = settlement;
            amount=1;
        }

        public Facility()
        {

        }
        
        /// <summary>
        /// Add X facilities to a settlement.
        /// </summary>
        /// <param name="amountToAdd"></param>
        public void AddFacilities(int amountToAdd)
        {
            amount += amountToAdd;
            ShouldRecalculateModifiers = true;
        }

        /// <summary>
        /// Add a facility to a settlement.
        /// </summary>
        public void AddFacility()
        {
            AddFacilities(1);
        }

        /// <summary>
        /// Remove X facilities from a settlement.
        /// </summary>
        /// <param name="amountToRemove"></param>
        public void RemoveFacilities(int amountToRemove)
        {
            amount -= amountToRemove;
            if(amount < 0)
            {
                amount= 0;
            }
            ShouldRecalculateModifiers = true;
        }

        public void RemoveFacility()
        {
            RemoveFacilities(1);
        }

        public List<ResourceModifier> ResourceModifiers
        {
            get
            {
                if(ShouldRecalculateModifiers)
                {
                    Recalculate();
                    ShouldRecalculateModifiers = false;
                }
                return modifiers;
            }
        }

        private void Recalculate()
        {

            modifiers = new List<ResourceModifier>();
            foreach (ResourceDef resourceDef in def.resourceMultipliers.Keys)
            {
                Tile tile = Find.WorldGrid.tiles[settlement.Tile];
                ResourceModifier modifier = resourceDef.GetTileModifier(tile);
                modifier.multiplier *= (amount * def.resourceMultipliers[resourceDef]);
                modifiers.Add(modifier);

            }
        }

    }
}
