using System;
using System.Collections.Generic;
using System.Linq;
using Empire_Rewritten.Events.Processes;
using Empire_Rewritten.Resources;
using Empire_Rewritten.Utils;
using JetBrains.Annotations;
using RimWorld;
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
        private HashSet<ResourceDef> cachedProducedResources;
        private List<Process> processes = new List<Process>();
        private Dictionary<ThingDef, int> produce = new Dictionary<ThingDef, int>();
        private Settlement settlement;

        private string cachedDesignation = "Empire_FM_Undefined".Translate();
        private bool refreshModifiers = true;
        private bool refreshGizmos = true;
        private int facilityCount;
        private int stage = 1;
        private int id = -1;

        public Dictionary<ThingDef, int> Produce => produce;

        public HashSet<ResourceDef> ProducedResourceDefsReadonly
        {
            get
            {
                if (cachedProducedResources is null)
                {
                    RefreshCaches();
                }
                return cachedProducedResources;
            }
        }

        /// <summary>
        ///     Refreshes the internal list <see cref="cachedProducedResources"/>
        /// </summary>
        public void RefreshCaches()
        {
            cachedProducedResources = new HashSet<ResourceDef>();
            cachedProducedResources.AddRange(DefDatabase<ResourceDef>.AllDefsListForReading.Where(x => !x.isFacilityResource));
            foreach (Facility facility in installedFacilities.Values)
            {
                cachedProducedResources.AddRange(facility.def.ProducedResources);
            }

            RefreshCachedDesignation();
        }

        /// <summary>
        ///     Refreshes the settlements <see cref="cachedDesignation"/> 
        /// </summary>
        public void RefreshCachedDesignation()
        {
            ResourceDef[] tempList = new ResourceDef[cachedProducedResources.Count];
            cachedProducedResources.CopyTo(tempList);
            tempList.InsertionSort((resource0, resource1) => AmountOfProduceForResourceDef(resource1) - AmountOfProduceForResourceDef(resource0));

            cachedDesignation = $"{tempList[0].LabelCap} {"Empire_FM_Settlement".Translate()}";
        }

        /// <summary>
        ///     Calculates the amount of <see cref="ThingDef"/>s a <see cref="ResourceDef"/> is producing 
        /// </summary>
        /// <param name="def">the given <see cref="ResourceDef"/></param>
        /// <returns>The amount</returns>
        private int AmountOfProduceForResourceDef(ResourceDef def) => produce.Sum(kvp => (def.ResourcesCreated.AllowedThingDefs.Contains(kvp.Key) ? 1 : 0) * kvp.Value);

        /// <summary>
        ///     Used for Loading/Saving, creates the reference ID used for <see cref="ILoadReferenceable"/>
        /// </summary>
        private static int NextID
        {
            get
            {
                lastID++;
                return lastID;
            }
        }

        /// <summary>
        ///     Creates a new <see cref="FacilityManager"/> that manages the given <see cref="Settlement"/> <paramref name="settlement"/>
        /// </summary>
        /// <param name="settlement"></param>
        public FacilityManager(Settlement settlement) : base()
        {
            this.settlement = settlement;
            id = NextID;
        }

        /// <summary>
        ///     Used for Loading/Saving
        /// </summary>
        [UsedImplicitly]
        public FacilityManager() => lastID = Math.Max(lastID, id);

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
            int pos = processes.FirstIndexOf((p) => p is IProcessSlotID slotIDProcess && slotIDProcess.SlotID == slotID);
            if (pos == -1) return null;
            return processes[pos];
        }

        /// <summary>
        ///     Ends a process early
        /// </summary>
        /// <param name="process"></param>
        public void CancelProcess(Process process)
        {
            if (process == null) return;

            process.Cancel();
            int index = processes.IndexOf(process);
            processes.Remove(process);

            for (int j = index; j < processes.Count; j++)
            {
                if (processes[j] is IProcessSlotID slotIDProcess)
                {
                    slotIDProcess.SlotID--;
                }
            }

            NotifyProcessesChanged();
        }

        /// <summary>
        ///     This function notifies this <see cref="FacilityManager"/> that it's List of <see cref="Process"/> (<see cref="Processes"/>) has changed and requires action
        /// </summary>
        public void NotifyProcessesChanged()
        {
            processes.RemoveAll((p) => p.Progress >= 1f);
            if (processes.Count > 0)
            {
                processes[0].Suspended = false;
            }

            if (Find.WindowStack.IsOpen<Windows.SettlementInfoWindow>())
            {
                Find.WindowStack.WindowOfType<Windows.SettlementInfoWindow>().RecalculateScrollWidgets();
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

        /// <summary>
        ///     Returns true if the <see cref="FacilityManager"/> has reached its final stage
        /// </summary>
        public bool IsFullyUpgraded => stage >= 12;

        /// <summary>
        ///     Returns the amount of <see cref="Facilities"/> that can be build in this <see cref="FacilityManager"/>
        /// </summary>
        public int MaxFacilities => stage;

        /// <summary>
        ///     Upgrades this <see cref="FacilityManager"/> by the given <paramref name="amount"/>, can also be used to downgrade. Is limited to a minimum of 1 and maximum of 12
        /// </summary>
        /// <param name="amount"></param>
        public void ModifyStageBy(int amount)
        {
            stage = Mathf.Clamp(stage + amount, 1, 12);
        }

        /// <summary>
        ///     Returns the first open SlotID
        /// </summary>
        public int FirstOpenSlotID
        {
            get
            {
                if (!CanBuildNewFacilities) return -1;
                return FacilityCount + NumberOfFacilitiesBeingBuild;
            }
        }

        /// <summary>
        ///     Counts the number of <see cref="FacilityBuildProcess"/>es inside the <see cref="Processes"/> list
        /// </summary>
        public int NumberOfFacilitiesBeingBuild => processes.Count(p => p is FacilityBuildProcess);

        /// <summary>
        ///     Counts the amount of installed <see cref="Facilities"/>
        /// </summary>
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

        /// <summary>
        ///     Determines if new <see cref="Facilities"/> can be build at this location
        /// </summary>
        public bool CanBuildNewFacilities => (FacilityCount + NumberOfFacilitiesBeingBuild) < MaxFacilities;

        /// <summary>
        ///     Returns the internal <see cref="RimWorld.Planet.Settlement"/>
        /// </summary>
        public Settlement Settlement => settlement;

        /// <summary>
        ///     Exposes the internal list of <see cref="Process"/>es
        /// </summary>
        public List<Process> Processes => processes;

        /// <summary>
        ///     The string that describes what this settlement focuses on
        /// </summary>
        public string Designation => cachedDesignation;

        public void ExposeData()
        {
            Scribe_Collections.Look(ref installedFacilities, nameof(installedFacilities), LookMode.Def, LookMode.Deep);
            Scribe_Collections.Look(ref processes, nameof(processes), LookMode.Deep);
            Scribe_Collections.Look(ref produce, nameof(produce), LookMode.Def, LookMode.Value);

            Scribe_References.Look(ref settlement, nameof(settlement));

            Scribe_Values.Look(ref cachedDesignation, nameof(cachedDesignation));
            Scribe_Values.Look(ref stage, nameof(stage));
            Scribe_Values.Look(ref id, nameof(id));
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
                IEnumerable<Gizmo> facilityGizmos = facility.FacilityWorker?.GetGizmos();

                if (!facilityGizmos.EnumerableNullOrEmpty())
                {
                    gizmos.AddRange(facility.FacilityWorker.GetGizmos());
                }
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
                    AddOrSetResourceModifier(ref calculatedModifiers, modifier);
                }
            }

            foreach (ResourceDef def in DefDatabase<ResourceDef>.AllDefsListForReading)
            {
                AddOrSetResourceModifier(ref calculatedModifiers, def.GetTileModifier(Find.WorldGrid.tiles[settlement.Tile]));
            }

            cachedModifiers = calculatedModifiers.Values.ToList();
        }

        private void AddOrSetResourceModifier(ref Dictionary<ResourceDef, ResourceModifier> calculatedModifiers, ResourceModifier modifier)
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

            processes.Add(new FacilityBuildProcess(facilityDef.LabelCap, "Empire_FM_BuildingToolTip".Translate(facilityDef.LabelCap), DebugSettings.godMode ? 0 : facilityDef.buildDuration, facilityDef.iconData.texPath, this, facilityDef, FirstOpenSlotID));
            NotifyProcessesChanged();
            RefreshCaches();
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
                if (process is IProcessSlotID slotIDProcess)
                {
                    slotIDProcess.SlotID--;
                }
            }

            if (!installedFacilities.ContainsKey(facilityDef)) return;

            installedFacilities[facilityDef].RemoveFacility();
            if (installedFacilities[facilityDef].FacilitiesInstalled <= 0)
            {
                installedFacilities.Remove(facilityDef);
                SetDataDirty(true);
            }

            RefreshCaches();
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