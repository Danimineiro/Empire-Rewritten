using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Empire_Rewritten.Borders;
using Empire_Rewritten.AI;
using RimWorld.Planet;

namespace Empire_Rewritten
{
    public class FactionController : IExposable
    {
        private List<FactionSettlementData> factionSettlementDataList = new List<FactionSettlementData>();
        private readonly List<FactionCivicAndEthicData> factionCivicAndEthicDataList = new List<FactionCivicAndEthicData>();
        private Dictionary<Faction, AIPlayer> AIFactions = new Dictionary<Faction, AIPlayer>();
        private BorderManager borderManager = new BorderManager();
        private bool shouldRefreshBorders = true;

        public BorderManager BorderManager
        {
            get
            {
                return borderManager;
            }
        }
        /// <summary>
        /// Meant for things that cache the borders to check.
        /// </summary>
        public bool ShouldRefreshBorders
        {
            get
            {
                return shouldRefreshBorders;
            }
        }
        /// <summary>
        /// Needed for loading
        /// </summary>
        public FactionController() { }

        /// <summary>
        /// Creates a new FactionController using a List of <c>FactionSettlementData</c> structs
        /// </summary>
        /// <param name="factionSettlementDataList"></param>
        public FactionController(List<FactionSettlementData> factionSettlementDataList)
        {
            shouldRefreshBorders = true;
            borderManager= new BorderManager();
            this.factionSettlementDataList = factionSettlementDataList;
        }

        /// <param name="faction"></param>
        /// <returns>The <c>SettlementManager</c> owned by a given <paramref name="faction"/></returns>
        public SettlementManager GetOwnedSettlementManager(Faction faction)
        {
            foreach (FactionSettlementData factionSettlementData in factionSettlementDataList)
            {
                if (factionSettlementData.owner == faction) return factionSettlementData.SettlementManager;
            }

            return null;
        }

        /// <summary>
        /// Gets the AIPlayer of a faction.
        /// </summary>
        /// <param name="faction"></param>
        /// <returns></returns>
        public AIPlayer GetAIPlayer(Faction faction)
        {
            return AIFactions.ContainsKey(faction) ? AIFactions[faction] : null;
        }

        public void CreateNewAIPlayer(Faction faction)
        {
            AIPlayer aIPlayer = new AIPlayer(faction);
            AIFactions.Add(faction,aIPlayer);

            //Find preexisting settlements.
            IEnumerable<WorldObject> settlements = Find.WorldObjects.AllWorldObjects.Where(x => x is Settlement s && s.Faction == faction);
            if (settlements.Count() > 0) {
                List<WorldObject> list = settlements.ToList();
                aIPlayer.Manager.AddSettlement(list[0] as Settlement);
                list.RemoveAt(0);

                foreach(WorldObject item in list)
                {
                    item.Destroy();
                }
            }
            Log.Message($"Created AI for: {faction.Name}");
        }

        /// <summary>
        /// Adds one <paramref name="civic"/> to a <paramref name="faction"/>
        /// </summary>
        /// <param name="faction"></param>
        /// <param name="civic"></param>
        public void AddCivicToFaction(Faction faction, CivicDef civic)
        {
            GetOwnedCivicAndEthicData(faction).Civics.Add(civic);
            NotifyCivicsOrEthicsChanged(GetOwnedSettlementManager(faction));
        }

        /// <summary>
        /// Adds multiple <paramref name="civics"/> to a <paramref name="faction"/>
        /// </summary>
        /// <param name="faction"></param>
        /// <param name="civics"></param>
        public void AddCivicsToFaction(Faction faction, IEnumerable<CivicDef> civics)
        {
            foreach (CivicDef civic in civics)
            {
                GetOwnedCivicAndEthicData(faction).Civics.Add(civic);
            }

            NotifyCivicsOrEthicsChanged(GetOwnedSettlementManager(faction));
        }

        /// <summary>
        /// Adds a singel <paramref name="ethic"/> to a <paramref name="faction"/>
        /// </summary>
        /// <param name="faction"></param>
        /// <param name="ethic"></param>
        public void AddEthicToFaction(Faction faction, EthicDef ethic)
        {
            GetOwnedCivicAndEthicData(faction).Ethics.Add(ethic);
            NotifyCivicsOrEthicsChanged(GetOwnedSettlementManager(faction));
        }

        /// <summary>
        /// Adds multiple <paramref name="ethics"/> to a <paramref name="faction"/>
        /// </summary>
        /// <param name="faction"></param>
        /// <param name="ethics"></param>
        public void AddEthicsToFaction(Faction faction, IEnumerable<EthicDef> ethics)
        {
            foreach (EthicDef ethic in ethics)
            {
                GetOwnedCivicAndEthicData(faction).Ethics.Add(ethic);
            }

            NotifyCivicsOrEthicsChanged(GetOwnedSettlementManager(faction));
        }

        public void NotifyCivicsOrEthicsChanged(SettlementManager settlementManager)
        {
            throw new NotImplementedException();
        }

        /// <param name="faction"></param>
        /// <returns>The <c>FactionCivicAndEthicData</c>s linked to a given <paramref name="faction"/></returns>
        public FactionCivicAndEthicData GetOwnedCivicAndEthicData(Faction faction)
        {
            foreach (FactionCivicAndEthicData factionCivicAndEthicData in factionCivicAndEthicDataList)
            {
                if (factionCivicAndEthicData.Faction == faction) return factionCivicAndEthicData;
            }

            return null;
        }

        public void ExposeData()
        {
            Scribe_Collections.Look(ref factionSettlementDataList, "FactionSettlementDataList");
            Scribe_Deep.Look(ref borderManager, "borderManager");
        }
    }
}
