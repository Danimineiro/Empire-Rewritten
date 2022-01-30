using RimWorld.Planet;
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
        private List<ResourceModifier> cachedModifiers = new List<ResourceModifier>();
        bool RefreshModifiers = true;
        bool RefreshGizmos = true;
        List<Gizmo> gizmos = new List<Gizmo>();

        public FacilityManager(Settlement settlement)
        {
            this.settlement = settlement;
        }

        public FacilityManager()
        {
          
        }

        /// <summary>
        /// Get gizmos from all facilities in the settlement.
        /// </summary>
        public IEnumerable<Gizmo> GetGizmos()
        {
            if (RefreshModifiers)
            {
                RefreshModifiers = false;
                foreach(FacilityDef facilityDef in installedFacilities.Keys.ToList())
                {
                    if(facilityDef.facilityWorker!= null )
                    {
                        FacilityWorker worker = (FacilityWorker)Activator.CreateInstance(facilityDef.facilityWorker);
                        List<Gizmo> newGizmos = (List<Gizmo>)worker.GetGizmos();
                        foreach(Gizmo gizmo in newGizmos)
                        {
                            gizmos.Add(gizmo);
                        }
                    }
                }
            }
            return gizmos;
        }

        /// <summary>
        /// Update the ResourceModifierCache when called.
        /// </summary>
        private void UpdateModiferCache()
        {
            Dictionary<ResourceDef, ResourceModifier> calculatedModifers = new Dictionary<ResourceDef, ResourceModifier>();
            foreach (Facility facility in installedFacilities.Values.ToList())
            {
                foreach (ResourceModifier modifier in facility.ResourceModifiers)
                {
                    if (calculatedModifers.ContainsKey(modifier.def))
                    {
                        ResourceModifier newModifier = calculatedModifers[modifier.def].MergeWithModifier(modifier);
                        calculatedModifers[modifier.def] = newModifier;
                    }
                    else
                    {
                        calculatedModifers.Add(modifier.def, modifier);
                    }
                }
            }
            cachedModifiers = calculatedModifers.Values.ToList();
        }


        /// <summary>
        /// Get all ResourceModifiers from installed facilities.
        /// </summary>
        public List<ResourceModifier> modifiers
        {
            get
            {
                if (RefreshModifiers)
                {
                    UpdateModiferCache();
                    RefreshModifiers = false;
                }
                return cachedModifiers;
            }
        }


        public void ExposeData()
        {
            Scribe_Collections.Look(ref installedFacilities, "installedFacilities", LookMode.Deep);
            Scribe_Values.Look(ref settlement, "settlement");
        }

        /// <summary>
        /// Set gizmos and resourcemodifiers to be refreshed when called for.
        /// </summary>
        /// <param name="refreshGizmos">Refresh gizmos alongside resourcemodifiers</param>
        public void SetDataDirty(bool refreshGizmos = false)
        {
            RefreshGizmos = refreshGizmos;
            RefreshModifiers = true;
        }
        
        
        
        
        /// <summary>
        /// Used for building a new facility on the settlement.
        /// </summary>
        /// <param name="facilityDef"></param>
        public void AddFacility(FacilityDef facilityDef)
        {
            bool facilityChange = false;
            if (installedFacilities.ContainsKey(facilityDef))
            {
                installedFacilities[facilityDef].AddFacility();
            }
            else
            {
                facilityChange = true;
                installedFacilities.Add(facilityDef, new Facility(facilityDef, settlement));
            }
            SetDataDirty(facilityChange);
          
        }

        public void RemoveFacility(FacilityDef facilityDef)
        {
            bool facilityChange = false;
            if (installedFacilities.ContainsKey(facilityDef))
            {
                installedFacilities[facilityDef].RemoveFacility();
                if (installedFacilities[facilityDef].FacilitiesInstalled <= 0)
                {
                    facilityChange=true;
                    installedFacilities.Remove(facilityDef);
                }
                SetDataDirty(facilityChange);
            }
        }
       
        /// <summary>
        /// Can this be built here?
        /// </summary>
        /// <returns></returns>
        public bool CanBuildAt(FacilityDef facilityDef)
        {
            return true;
        }
    }
}
