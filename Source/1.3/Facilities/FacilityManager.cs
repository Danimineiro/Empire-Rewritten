﻿using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Empire_Rewritten
{
    /// <summary>
    /// This manages all the facilities for a settlement.
    /// It also manages the resource modifiers from each facility.
    /// </summary>
    public class FacilityManager : IExposable
    {
        private Dictionary<FacilityDef,Facility> installedFacilities = new Dictionary<FacilityDef, Facility>();
        private Settlement settlement;


        public FacilityManager(Settlement settlement)
        {
            this.settlement = settlement;
        }

        public FacilityManager()
        {

        }

        /// <summary>
        /// Get all ResourceModifiers from installed facilities.
        /// </summary>
        public List<ResourceModifier> modifiers
        {
            get
            {
                Dictionary<ResourceDef, ResourceModifier> calculatedModifers = new Dictionary<ResourceDef, ResourceModifier>();
                foreach (Facility facility in installedFacilities.Values.ToList())
                {
                    foreach (ResourceModifier modifier in facility.ResourceModifiers)
                    {
                        if (calculatedModifers.ContainsKey(modifier.def))
                        {
                            ResourceModifier resourceModifier = calculatedModifers[modifier.def];
                            resourceModifier.offset += modifier.offset;
                            resourceModifier.multiplier *= modifier.multiplier;

                            calculatedModifers[modifier.def] = resourceModifier;
                        }
                        else
                        {
                            calculatedModifers.Add(modifier.def, modifier);
                        }
                    }
                }

                return calculatedModifers.Values.ToList();
            }
        }


        public void ExposeData()
        {
            Scribe_Collections.Look(ref installedFacilities, "installedFacilities", LookMode.Deep);
            Scribe_Values.Look(ref settlement, "settlement");
        }

        public void AddFacility(FacilityDef facilityDef)
        {
            if (installedFacilities.ContainsKey(facilityDef))
            {
                installedFacilities[facilityDef].AddFacility();
            }
            else
            {
                installedFacilities.Add(facilityDef, new Facility(facilityDef, settlement));
            }
          
        }

        public void RemoveFacility(FacilityDef facilityDef)
        {
            if (installedFacilities.ContainsKey(facilityDef))
            {
                installedFacilities[facilityDef].RemoveFacility();
                if (installedFacilities[facilityDef].FacilitiesInstalled <= 0)
                {
                    installedFacilities.Remove(facilityDef);
                }
            }
        }
    }
}