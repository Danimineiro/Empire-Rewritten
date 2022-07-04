using System.Collections.Generic;
using System.Linq;
using Empire_Rewritten.Events.Processes;
using Empire_Rewritten.Resources;
using Empire_Rewritten.Utils;
using JetBrains.Annotations;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace Empire_Rewritten.Facilities
{
    /// <summary>
    ///     This manages all the <see cref="Facility">Facilities</see> for a <see cref="Settlement" />.
    ///     It also manages the <see cref="ResourceModifier">ResourceModifiers</see> from each <see cref="Facility" />.
    /// </summary>
    public class FacilityManager : IExposable, ILoadReferenceable
    {
        private static int lastID = -1;

        private readonly List<Gizmo> gizmos = new List<Gizmo>();
        private readonly bool refreshFacilityCount = true;

        private Dictionary<FacilityDef, Facility> installedFacilities = new Dictionary<FacilityDef, Facility>();
        private List<ResourceModifier> cachedModifiers = new List<ResourceModifier>();
        private List<Process> processes = new List<Process>();
        private Settlement settlement;

        private bool refreshModifiers = true;
        private bool refreshGizmos = true;
        private int facilityCount;
        private int stage = 1;
        private int id;

        private static int NextID
        {
            get
            {
                lastID++;
                return lastID;
            }
        }

        public FacilityManager(Settlement settlement) : base()
        {
            this.settlement = settlement;
        }

        [UsedImplicitly]
        public FacilityManager() 
        {
            id = NextID;
        }

        /// <summary>
        ///     Looks through all facilities to return the <see cref="Facility"/> that has something at the given position 
        /// </summary>
        /// <param name="index"></param>
        /// <returns>a <see cref="Facility"/> if something is found, <c>null</c> otherwise</returns>
        public Facility this[int index]
        {
            get
            {
                int pos = 0;

                foreach (KeyValuePair<FacilityDef, Facility> kvp in installedFacilities)
                {
                    Facility facility = kvp.Value;
                    pos += facility.Amount;
                    if (index < pos) return facility;
                }

                return null;
            }
        }

        /// <summary>
        ///     Looks through all facilities to return the <see cref="Facility"/> with the given <paramref name="facilityDef"/>
        /// </summary>
        /// <param name="index"></param>
        /// <returns>a <see cref="Facility"/> if something is found, <c>null</c> otherwise</returns>
        public Facility this[FacilityDef facilityDef]
        {
            get => installedFacilities.TryGetValue(facilityDef);
            set => installedFacilities.TryAdd(facilityDef, value);
        }

        /// <param name="slotID"></param>
        /// <returns>The <see cref="FacilityBuildProcess"/> as <see cref="Process"/> that builds in the given <paramref name="slotID"/></returns>
        public Process GetProcessWithSlotID(int slotID)
        {
            int pos = processes.FirstIndexOf((p) => p is FacilityBuildProcess buildProcess && buildProcess.SlotID == slotID);
            if(pos == -1) return null;
            return processes[pos];
        }

        public void NotifyProcessesChanged()
        {
            processes.RemoveAll((p) => p.Progress >= 1f);
            if (processes.Count > 0)
            {
                processes[0].Suspended = false;
            }
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

        public bool IsFullyUpgraded => stage >= 12;

        public int MaxFacilities => stage;

        public void ModifyStageBy(int amount)
        {
            stage = Mathf.Clamp(stage + amount, 1, 12);
        }

        public int FirstOpenSlotID
        {
            get
            {
                if (!CanBuildNewFacilities) return -1;
                return FacilityCount + NumberOfFacilitiesBeingBuild;
            }
        }

        public int NumberOfFacilitiesBeingBuild => processes.Sum((p) => p is FacilityBuildProcess ? 1 : 0);

        public int FacilityCount
        {
            get
            {
                if (refreshFacilityCount)
                {
                    facilityCount = installedFacilities.Sum((kvp) => kvp.Value.Amount);
                }

                return facilityCount;
            }
        }

        public bool CanBuildNewFacilities => (FacilityCount + NumberOfFacilitiesBeingBuild) < MaxFacilities;

        public Settlement Settlement { get => settlement; set => settlement = value; }

        public void ExposeData()
        {
            Scribe_Collections.Look(ref installedFacilities, nameof(installedFacilities), LookMode.Deep, LookMode.Deep);
            Scribe_Collections.Look(ref processes, nameof(processes), LookMode.Deep);
            Scribe_References.Look(ref settlement, nameof(settlement));
            Scribe_Values.Look(ref stage, nameof(stage));
        }

        /// <summary>
        ///     Get gizmos from all facilities in the settlement.
        /// </summary>
        public IEnumerable<Gizmo> GetGizmos()
        {
            if (!refreshGizmos) return gizmos;

            gizmos.Clear();
            refreshGizmos = false;
            foreach (Facility facility in installedFacilities.Values)
            {
                gizmos.AddRange(facility.FacilityWorker.GetGizmos());
            }

            return gizmos;
        }

        /// <summary>
        ///     Refreshes the <see cref="FacilityManager.cachedModifiers" />.
        /// </summary>
        private void UpdateModiferCache()
        {
            Dictionary<ResourceDef, ResourceModifier> calculatedModifiers = new Dictionary<ResourceDef, ResourceModifier>();

            foreach (Facility facility in installedFacilities.Values)
            {
                foreach (ResourceModifier modifier in facility.ResourceModifiers)
                {
                    if (calculatedModifiers.ContainsKey(modifier.def))
                    {
                        ResourceModifier newModifier = calculatedModifiers[modifier.def].MergeWithModifier(modifier);
                        calculatedModifiers[modifier.def] = newModifier;
                    }
                    else
                    {
                        calculatedModifiers.Add(modifier.def, modifier);
                    }
                }
            }

            cachedModifiers = calculatedModifiers.Values.ToList();
        }

        /// <summary>
        ///     Invalidates cached <see cref="Gizmo">Gizmos</see> and <see cref="ResourceModifier">ResourceModifiers</see>
        /// </summary>
        /// <param name="shouldRefreshGizmos">
        ///     Whether to refresh <see cref="FacilityManager.gizmos" /> alongside <see cref="FacilityManager.cachedModifiers" />
        /// </param>
        public void SetDataDirty(bool shouldRefreshGizmos = false)
        {
            refreshGizmos = shouldRefreshGizmos;
            refreshModifiers = true;
        }

        /// <summary>
        ///     Adds a new <see cref="Facility" /> of a given <see cref="FacilityDef" /> to this <see cref="FacilityManager" />'s
        ///     <see cref="FacilityManager.settlement" />
        /// </summary>
        /// <param name="facilityDef">The <see cref="FacilityDef" /> to add</param>
        public void AddFacility(FacilityDef facilityDef)
        {
            if (FirstOpenSlotID == -1)
            {
                $"Tried to add Facility {facilityDef.LabelCap} to settlement manager of {settlement.LabelCap}, but FirstOpenSlotID is -1 (Can't build more facilities!)".ErrorOnce();
                return;
            }

            processes.Add(new FacilityBuildProcess("Empire_FM_BuildingLabel".Translate(facilityDef.LabelCap), "Empire_FM_BuildingToolTip".Translate(), facilityDef.buildDuration, this, facilityDef, FirstOpenSlotID));
            NotifyProcessesChanged();
            //if (installedFacilities.ContainsKey(facilityDef))
            //{
            //    installedFacilities[facilityDef].AddFacility();
            //}
            //else
            //{
            //    installedFacilities.Add(facilityDef, new Facility(facilityDef, settlement));
            //    SetDataDirty(true);
            //}
        }

        /// <summary>
        ///     Removes a new <see cref="Facility" /> of a given <see cref="FacilityDef" /> from this
        ///     <see cref="FacilityManager" />'s <see cref="FacilityManager.settlement" />
        /// </summary>
        /// <param name="facilityDef">The <see cref="FacilityDef" /> to remove</param>
        public void RemoveFacility(FacilityDef facilityDef)
        {
            foreach (Process process in processes)
            {
                if (process is FacilityBuildProcess buildProcess)
                {
                    buildProcess.SlotID--;
                }
            }

            if (!installedFacilities.ContainsKey(facilityDef)) return;

            installedFacilities[facilityDef].RemoveFacility();
            if (installedFacilities[facilityDef].FacilitiesInstalled <= 0)
            {
                installedFacilities.Remove(facilityDef);
                SetDataDirty(true);
            }
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

        /// <summary>
        ///     Checks whether this <see cref="FacilityManager" /> has a <see cref="Facility" /> of a given
        ///     <see cref="FacilityDef" />
        /// </summary>
        /// <param name="facilityDef">The <see cref="FacilityDef" /> to check for</param>
        /// <returns>How many of <paramref name="facilityDef"/> are installed in this <see cref="FacilityManager"/></returns>
        public int HasFacilityAmount(FacilityDef facilityDef)
        {
            if (installedFacilities.TryGetValue(facilityDef, out Facility facility)) return facility.Amount;
            return 0;
        }

        public string GetUniqueLoadID()
        {
            return $"{nameof(FacilityManager)}{id}";
        }
    }
}