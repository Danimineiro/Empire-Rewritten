using Empire_Rewritten.Resources;
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

        private FacilityWorker worker;

        public FacilityWorker FacilityWorker
        {
            get
            {
                return worker;
            }
        }
        public int Amount
        {
            get
            {
                return amount;
            }
        }
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

            if (def.facilityWorker != null)
            {
                worker = (FacilityWorker)Activator.CreateInstance(def.facilityWorker);
                worker.facilityDef = def;
            }
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
            List<ResourceDef> defs = new List<ResourceDef>();
            foreach(ResourceChange change in def.resourceMultipliers)
            {
                defs.Add(change.def);
            }
            foreach (ResourceChange change in def.resourceOffsets)
            {
                defs.Add(change.def);
            }

            foreach (ResourceDef resourceDef in defs)
            {
                Tile tile = Find.WorldGrid.tiles[settlement.Tile];
                ResourceModifier modifier = resourceDef.GetTileModifier(tile);
                modifier.multiplier *= (amount *(def.resourceOffsets.Any(x => x.def == resourceDef) ? def.resourceMultipliers.Find(x=>x.def==resourceDef).amount : 1));
                modifier.offset += (amount * (def.resourceOffsets.Any(x=>x.def==resourceDef) ? def.resourceOffsets.Find(x => x.def == resourceDef).amount : 0));
                modifiers.Add(modifier);

            }
        }



    }
}
