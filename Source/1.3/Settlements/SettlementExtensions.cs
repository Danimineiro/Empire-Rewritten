using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Empire_Rewritten
{
    public static class SettlementExtensions
    {
        public static IEnumerable<Gizmo> GetExtendedGizmos(this Settlement settlement)
        {
           UpdateController updateController = UpdateController.GetUpdateController;
          //  updateController.FactionController;
        }
    }
}
