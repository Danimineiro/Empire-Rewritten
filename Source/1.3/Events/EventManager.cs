﻿using Empire_Rewritten.Controllers;
using Empire_Rewritten.Settlements;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Empire_Rewritten.Events
{
    public class EventManager : IExposable
    {


        private static Random rand = new Random();
        private static List<EventDef> cachedEvents = new List<EventDef>();
        private static int lastDay;
        public EventManager()
        {
            UpdateController.CurrentWorldInstance.AddUpdateCall(DoRandomEventOnRandomEmpire,ShouldFireEvent);
            cachedEvents = DefDatabase<EventDef>.AllDefsListForReading;
        }

        public static bool ShouldFireEvent()
        {
            int passedDays = GenDate.DaysPassed;

            if (passedDays - lastDay >FactionController.daysPerTurn)
            {
                lastDay = GenDate.DaysPassed;
                return true;
            }
            return false;
        }

        public static void DoRandomEventOnRandomEmpire(FactionController controller)
        {
            Empire empire;
            empire = controller.ReadOnlyFactionSettlementData.RandomElement().SettlementManager;

            FireRandomEvent(empire);
        }
        public static void FireRandomEvent(Empire empire)
        {
            
            EventDef def = cachedEvents.Where(x=> !empire.IsAIPlayer || x.canAffectAI).RandomElement();

            EventWorker worker = empire.IsAIPlayer ? def.AIWorker : def.EventWorker;
            if(worker.Chance>rand.Next(0,100))
                worker.Event(empire);

        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref lastDay, nameof(lastDay));
        }
    }
}
