using Empire_Rewritten.Resources;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
namespace Empire_Rewritten.Facilities
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
        bool RefreshFacilityCount = true;
        List<Gizmo> gizmos = new List<Gizmo>();
        int stage = 1;
        int facilityCount = 0;
        public FacilityManager(Settlement settlement)
        {
            this.settlement = settlement;
        }

        public FacilityManager()
        {
          
        }

        public bool CanBuildNewFacilities()
        {
            return MaxFacilities > FacilityCount;
        }

        /// <summary>
        /// Get gizmos from all facilities in the settlement.
        /// </summary>
        public IEnumerable<Gizmo> GetGizmos()
        {
            if (RefreshModifiers)
            {
                gizmos.Clear();
                RefreshModifiers = false;
                foreach(Facility facility in installedFacilities.Values)
                {
                    gizmos.AddRange(facility.FacilityWorker.GetGizmos());
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
            Scribe_Values.Look(ref stage, "stage");
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
        /// Has a facility of facilityDef
        /// </summary>
        /// <param name="facilityDef"></param>
        /// <returns></returns>
        public bool HasFacility(FacilityDef facilityDef)
        {
            return installedFacilities.ContainsKey(facilityDef);
        }

        /// <summary>
        /// Installed facilitydefs
        /// </summary>
        public IEnumerable<FacilityDef> FacilityDefsInsalled
        {
            get
            {
                return installedFacilities.Keys;
            }
        }


        public bool IsFullyUpgraded
        {
            get
            {
                return stage>=10;
            }
        }

        public int MaxFacilities
        {
            get
            {
                return stage;
            }
        }

        public int FacilityCount
        {
            get
            {
                if (RefreshFacilityCount)
                {
                    int count = 0;
                    foreach(Facility facility in installedFacilities.Values)
                    {
                        count+=facility.Amount;
                    }
                    facilityCount=count;

                }
                return facilityCount;
            }
        }
    }
}
