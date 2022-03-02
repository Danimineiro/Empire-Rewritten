using System;
using System.Collections.Generic;
using System.Linq;
using Empire_Rewritten.Resources;
using JetBrains.Annotations;
using RimWorld.Planet;
using Verse;

namespace Empire_Rewritten.Facilities
{
    /// <summary>
    ///     This manages all the <see cref="Facility">Facilities</see> for a <see cref="Settlement" />.
    ///     It also manages the <see cref="ResourceModifier">ResourceModifiers</see> from each <see cref="Facility" />.
    /// </summary>
    public class FacilityManager : IExposable
    {
        private readonly List<Gizmo> gizmos = new List<Gizmo>();
        private List<ResourceModifier> cachedModifiers = new List<ResourceModifier>();
        private Dictionary<FacilityDef, Facility> installedFacilities = new Dictionary<FacilityDef, Facility>();
        private bool refreshGizmos = true;
        private bool refreshModifiers = true;
        private Settlement settlement;

        public FacilityManager(Settlement settlement)
        {
            this.settlement = settlement;
        }

        [UsedImplicitly]
        public FacilityManager()
        {
        }


        /// <summary>
        ///     All <see cref="ResourceModifier">ResourceModifiers</see> from installed <see cref="Facility">Facilities</see>.
        /// </summary>
        public IEnumerable<ResourceModifier> Modifiers
        {
            get
            {
                if (!refreshModifiers) return cachedModifiers;

                UpdateModiferCache();
                refreshModifiers = false;

                return cachedModifiers;
            }
        }

        /// <summary>
        ///     Installed <see cref="FacilityDef">FacilityDefs</see>
        /// </summary>
        public IEnumerable<FacilityDef> FacilityDefsInstalled => installedFacilities.Keys;


        public void ExposeData()
        {
            Scribe_Collections.Look(ref installedFacilities, "installedFacilities", LookMode.Deep);
            Scribe_Values.Look(ref settlement, "settlement");
        }

        /// <summary>
        ///     Gets the <see cref="Gizmo">Gizmos</see> for all active <see cref="Facility">Facilities</see> in this
        ///     <see cref="FacilityManager" />
        /// </summary>
        /// <returns>Every <see cref="Facility" />'s <see cref="Gizmo">Gizmos</see></returns>
        public IEnumerable<Gizmo> GetGizmos()
        {
            if (!refreshModifiers) return gizmos;
            refreshModifiers = false;

            gizmos.Clear();
            gizmos.AddRange(installedFacilities
                            .Keys.Where(facilityDef => facilityDef.facilityWorker != null)
                            .SelectMany(facilityDef =>
                                            ((FacilityWorker)Activator.CreateInstance(facilityDef.facilityWorker))
                                            .GetGizmos())
            );

            return gizmos;
        }

        /// <summary>
        ///     Refreshes the <see cref="ResourceModifier" /> cache.
        /// </summary>
        private void UpdateModiferCache()
        {
            var calculatedModifiers = new Dictionary<ResourceDef, ResourceModifier>();

            foreach (var modifier in installedFacilities.Values.SelectMany(facility => facility.ResourceModifiers))
            {
                if (calculatedModifiers.ContainsKey(modifier.def))
                {
                    var newModifier = calculatedModifiers[modifier.def].MergeWithModifier(modifier);
                    calculatedModifiers[modifier.def] = newModifier;
                }
                else
                {
                    calculatedModifiers.Add(modifier.def, modifier);
                }
            }

            cachedModifiers = calculatedModifiers.Values.ToList();
        }

        /// <summary>
        ///     Invalidates cached <see cref="Gizmo">Gizmos</see> and <see cref="ResourceModifier">ResourceModifiers</see>
        /// </summary>
        /// <param name="shouldRefreshGizmos">
        ///     Refresh <see cref="FacilityManager.gizmos" /> alongside
        ///     <see cref="FacilityManager.cachedModifiers" />
        /// </param>
        public void SetDataDirty(bool shouldRefreshGizmos = false)
        {
            refreshGizmos    = shouldRefreshGizmos;
            refreshModifiers = true;
        }


        /// <summary>
        ///     Adds a new <see cref="Facility" /> of a given <see cref="FacilityDef" /> to this <see cref="FacilityManager" />'s
        ///     <see cref="FacilityManager.settlement" />
        /// </summary>
        /// <param name="facilityDef">The <see cref="FacilityDef" /> to add</param>
        public void AddFacility(FacilityDef facilityDef)
        {
            if (installedFacilities.ContainsKey(facilityDef))
            {
                installedFacilities[facilityDef].AddFacility();
            }
            else
            {
                installedFacilities.Add(facilityDef, new Facility(facilityDef, settlement));
                SetDataDirty(true);
            }
        }

        /// <summary>
        ///     Removes a new <see cref="Facility" /> of a given <see cref="FacilityDef" /> from this
        ///     <see cref="FacilityManager" />'s <see cref="FacilityManager.settlement" />
        /// </summary>
        /// <param name="facilityDef">The <see cref="FacilityDef" /> to remove</param>
        public void RemoveFacility(FacilityDef facilityDef)
        {
            if (!installedFacilities.ContainsKey(facilityDef)) return;

            installedFacilities[facilityDef].RemoveFacility();
            if (installedFacilities[facilityDef].FacilitiesInstalled <= 0)
            {
                installedFacilities.Remove(facilityDef);
                SetDataDirty(true);
            }
        }

        /// <summary>
        ///     Checks whether a given <see cref="FacilityDef" /> can be built at this <see cref="FacilityManager" />
        /// </summary>
        /// <param name="facilityDef">The <see cref="FacilityDef" /> to check</param>
        /// <returns>Whether <paramref name="facilityDef" /> can be built here</returns>
        public bool CanBuildAt(FacilityDef facilityDef)
        {
            // TODO: Add proper logic here.
            return true;
        }

        /// <summary>
        ///     Checks whether this <see cref="FacilityManager" /> has a <see cref="Facility" /> of a given
        ///     <see cref="FacilityDef" />
        /// </summary>
        /// <param name="facilityDef">The <see cref="FacilityDef" /> to check for</param>
        /// <returns>Whether <paramref name="facilityDef" /> is installed here</returns>
        public bool HasFacility(FacilityDef facilityDef)
        {
            return installedFacilities.ContainsKey(facilityDef);
        }
    }
}